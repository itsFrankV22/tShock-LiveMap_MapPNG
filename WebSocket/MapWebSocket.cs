using Newtonsoft.Json;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using Terraria;
using tShock_LiveMap.Export;
using tShock_LiveMap.Extras;
using TShockAPI;

namespace tShock_LiveMap.WebSockets
{
    public static class MapSocketServer
    {
        private static HttpListener listener;
        private static List<WebSocket> clients = new List<WebSocket>();
        private static System.Timers.Timer wsTimer;
        private static CancellationTokenSource cancelSource = new CancellationTokenSource();

        private const int ChunkSize = 16;
        private const int ViewRadius = 1;

        // Map cache: (chunkX, chunkY) -> array of hex color strings
        private static readonly Dictionary<(int, int), string[]> MapChunks = new();
        private static int MaxChunkX => Main.maxTilesX / ChunkSize;
        private static int MaxChunkY => Main.maxTilesY / ChunkSize;

        // Start the WebSocket server and initialize the map cache/timer
        public static void Start(int port)
        {
            try
            {
                listener = new HttpListener();
                listener.Prefixes.Clear();
                listener.Prefixes.Add($"http://*:{port}/map/ws/");
                listener.Start();
                Task.Run(() => AcceptClientsAsync(cancelSource.Token));
                TShock.Log.ConsoleInfo($"[MapSocket] WebSocket started on /map/ws at port {port}");

                InitializeMapCache();

                wsTimer = new System.Timers.Timer(250);
                wsTimer.Elapsed += (s, e) => UpdateAllChunks();
                wsTimer.AutoReset = true;
                wsTimer.Enabled = true;
            }
            catch (Exception ex)
            {
                _ = Telemetry.Report(ex);
                TShock.Log.ConsoleError($"[MapSocket] Error starting WebSocket: {ex.Message}");
            }
        }

        // Stop WebSocket server and cleanup
        public static void Stop()
        {
            cancelSource?.Cancel();
            listener?.Stop();
            wsTimer?.Stop();

            lock (clients)
            {
                foreach (var ws in clients)
                {
                    ws?.Abort();
                }
                clients.Clear();
            }
        }

        /// <summary>
        /// Initializes or refreshes the whole map cache in memory.
        /// </summary>
        public static void InitializeMapCache()
        {
            lock (MapChunks)
            {
                MapChunks.Clear();
                for (int chunkX = 0; chunkX < MaxChunkX; chunkX++)
                    for (int chunkY = 0; chunkY < MaxChunkY; chunkY++)
                    {
                        MapChunks[(chunkX, chunkY)] = GetChunkColors(chunkX, chunkY);
                    }
                TShock.Log.ConsoleInfo($"[MapSocket] Map cache initialized ({MaxChunkX * MaxChunkY} chunks)");
            }
        }

        // Accept WebSocket clients asynchronously
        private static async Task AcceptClientsAsync(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                try
                {
                    var context = await listener.GetContextAsync();
                    if (context.Request.IsWebSocketRequest)
                    {
                        var socketContext = await context.AcceptWebSocketAsync(null);
                        var socket = socketContext.WebSocket;

                        // Send the complete map to the client before adding to updates
                        _ = Task.Run(() => HandleNewClient(socket));
                    }
                    else
                    {
                        context.Response.StatusCode = 400;
                        context.Response.Close();
                    }
                }
                catch (Exception ex)
                {
                    _ = Telemetry.Report(ex);
                }
            }
        }

        /// <summary>
        /// Sends the full map to a new client, then adds it to the updates list.
        /// </summary>
        private static async Task HandleNewClient(WebSocket socket)
        {
            try
            {
                // 1. Send the complete map from cache, in batches
                var allChunks = new List<KeyValuePair<(int, int), string[]>>();
                lock (MapChunks)
                    allChunks.AddRange(MapChunks);

                int batchSize = 64; // Tune this value for performance
                for (int i = 0; i < allChunks.Count; i += batchSize)
                {
                    var batch = allChunks.Skip(i).Take(batchSize);
                    foreach (var kv in batch)
                    {
                        var payload = new
                        {
                            type = "chunk_update",
                            chunkX = kv.Key.Item1,
                            chunkY = kv.Key.Item2,
                            width = ChunkSize,
                            height = ChunkSize,
                            colors = kv.Value,
                            mapWidth = Main.maxTilesX,
                            mapHeight = Main.maxTilesY
                        };
                        var json = JsonConvert.SerializeObject(payload);
                        var bytes = Encoding.UTF8.GetBytes(json);
                        if (socket.State == WebSocketState.Open)
                            await socket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
                    }
                    // Small delay to avoid saturating network/browser
                    await Task.Delay(15);
                    if (socket.State != WebSocketState.Open)
                        break;
                }

                // 2. Add client to update list
                lock (clients)
                    clients.Add(socket);

                // 3. Listen for connection close
                var buffer = new byte[1024];
                while (socket.State == WebSocketState.Open)
                {
                    var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    if (result.MessageType == WebSocketMessageType.Close)
                        break;
                }
            }
            catch { }
            finally
            {
                lock (clients)
                    clients.Remove(socket);
                socket?.Abort();
            }
        }

        /// <summary>
        /// Sends updates of nearby chunks to each active player.
        /// </summary>
        private static void UpdateAllChunks()
        {
            try
            {
                var alreadySent = new HashSet<(int, int)>();

                foreach (TSPlayer player in TShock.Players)
                {
                    if (player == null || !player.Active) continue;

                    int tileX = (int)(player.X / 16f);
                    int tileY = (int)(player.Y / 16f);

                    int chunkX = tileX / ChunkSize;
                    int chunkY = tileY / ChunkSize;

                    for (int dx = -ViewRadius; dx <= ViewRadius; dx++)
                    {
                        for (int dy = -ViewRadius; dy <= ViewRadius; dy++)
                        {
                            int cx = chunkX + dx;
                            int cy = chunkY + dy;

                            if (cx < 0 || cy < 0 ||
                                cx >= MaxChunkX ||
                                cy >= MaxChunkY)
                                continue;

                            if (alreadySent.Add((cx, cy)))
                            {
                                MapChunks[(cx, cy)] = GetChunkColors(cx, cy);
                                BroadcastChunkUpdate(cx, cy);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _ = Telemetry.Report(ex);
                TShock.Log.ConsoleError($"[MapSocket] Error in UpdateAllChunks: {ex.Message}");
            }
        }

        /// <summary>
        /// Sends chunk information to all connected WebSocket clients.
        /// </summary>
        public static void BroadcastChunkUpdate(int chunkX, int chunkY)
        {
            string[] chunkData;
            lock (MapChunks)
            {
                if (!MapChunks.TryGetValue((chunkX, chunkY), out chunkData))
                    chunkData = GetChunkColors(chunkX, chunkY); // fallback (should not happen)
            }

            var payload = new
            {
                type = "chunk_update",
                chunkX,
                chunkY,
                width = ChunkSize,
                height = ChunkSize,
                colors = chunkData
            };

            var json = JsonConvert.SerializeObject(payload);
            var bytes = Encoding.UTF8.GetBytes(json);

            lock (clients)
            {
                foreach (var ws in clients.ToList())
                {
                    if (ws.State == WebSocketState.Open)
                    {
                        _ = ws.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
                    }
                }
            }
        }

        /// <summary>
        /// Returns an array of hex color strings for tiles in the given chunk.
        /// Uses your color helpers (make sure they accept ITile).
        /// </summary>
        private static string[] GetChunkColors(int chunkX, int chunkY)
        {
            var colors = new string[ChunkSize * ChunkSize];
            int startX = chunkX * ChunkSize;
            int startY = chunkY * ChunkSize;

            for (int y = 0; y < ChunkSize; y++)
            {
                for (int x = 0; x < ChunkSize; x++)
                {
                    int worldX = startX + x;
                    int worldY = startY + y;

                    if (worldX < 0 || worldX >= Main.maxTilesX || worldY < 0 || worldY >= Main.maxTilesY)
                    {
                        colors[y * ChunkSize + x] = "#000000";
                        continue;
                    }

                    ITile itile = Main.tile[worldX, worldY];
                    Tile tile = itile as Tile;
                    if (tile != null)
                    {
                        System.Drawing.Color sysColor = MapExporter.GetTileColor(tile, worldX, worldY);
                        colors[y * ChunkSize + x] = $"#{sysColor.R:X2}{sysColor.G:X2}{sysColor.B:X2}";
                    }
                    else
                    {
                        colors[y * ChunkSize + x] = "#000000";
                    }
                }
            }
            return colors;
        }
    }
}