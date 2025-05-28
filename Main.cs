using ItemDecoration;
using System.Drawing;
using Terraria;
using TerrariaApi.Server;
using tShock_LiveMap.Colors;
using TShockAPI;

namespace WorldMapExporter
{
    /// <summary>
    /// Main plugin class for exporting the Terraria world map as a PNG image.
    /// Includes telemetry error reporting for all critical operations.
    /// </summary>
    [ApiVersion(2, 1)]
    public class WorldMapExporterPlugin : TerrariaPlugin
    {
        public override string Author => "FrankV22";
        public override string Description => "MAP Exporter .png";
        public override string Name => "LiveMap_WorldMapExporter";
        public override Version Version => new Version(1, 1, 0);

        private static System.Timers.Timer mapTimer;

        /// <summary>
        /// Plugin constructor, required for TerrariaPlugin.
        /// </summary>
        public WorldMapExporterPlugin(Main game) : base(game)
        {
        }

        /// <summary>
        /// Initializes the plugin: registers hooks, commands, timers, endpoints, and logs the startup.
        /// </summary>
        public override void Initialize()
        {
            try
            {
                // Register event for when the server has finished initializing.
                ServerApi.Hooks.GamePostInitialize.Register(this, OnPostInitialize);

                // Add the manual map export command.
                Commands.ChatCommands.Add(new Command("map.export", GenMap, "genmap"));

                // Setup a timer to export the map automatically every 5 minutes.
                mapTimer = new System.Timers.Timer(300000);
                mapTimer.Elapsed += (s, e) => GenMapAuto();
                mapTimer.AutoReset = true;
                mapTimer.Enabled = true;

                // Register REST API endpoints.
                RESTapi.Register();

                // Log plugin startup to console.
                Console.WriteLine($"\x1b[106;30;1m {Name} {Version} by {Author} \x1b[0m");
            }
            catch (Exception ex)
            {
                // Report initialization errors to telemetry and log to console.
                _ = Telemetry.Report(ex);
                TShock.Log.ConsoleError($"[{Name}] Error during plugin initialization: {ex.Message}");
            }
        }

        /// <summary>
        /// Called after the server has fully initialized.
        /// Starts telemetry and checks for plugin updates.
        /// Telemetry errors are reported and logged.
        /// </summary>
        private async void OnPostInitialize(EventArgs e)
        {
            try
            {
                // Start telemetry reporting.
                Telemetry.Start(this);

                // Check for updates (async).
                await CheckUpdates.CheckUpdates.CheckUpdateVerbose(this);
            }
            catch (Exception ex)
            {
                // Report and log errors from telemetry or update checking.
                await Telemetry.Report(ex);
                TShock.Log.ConsoleError($"[{Name}] Error OnPostInitialize: {ex.Message}");
            }
        }

        /// <summary>
        /// Command handler for "/genmap".
        /// Generates and saves the world map as a PNG image.
        /// Reports all errors to telemetry and informs the player.
        /// </summary>
        private void GenMap(CommandArgs args)
        {
            TSPlayer plr = args.Player;
            try
            {
                string outputPath = GenerateMap();
                plr.SendSuccessMessage($"Map Exported: {outputPath}");
            }
            catch (Exception ex)
            {
                // Report error to telemetry and notify the player.
                _ = Telemetry.Report(ex);
                plr.SendErrorMessage($"Error Exporting MAP: {ex.Message}");
            }
        }

        /// <summary>
        /// Called by the timer to export the map automatically.
        /// Reports errors to telemetry.
        /// </summary>
        private void GenMapAuto()
        {
            try
            {
                GenerateMap();
            }
            catch (Exception ex)
            {
                // Report any error during auto map export.
                _ = Telemetry.Report(ex);
                TShock.Log.ConsoleError($"[{Name}] Error in automatic map export: {ex.Message}");
            }
        }

        /// <summary>
        /// Generates a PNG image of the current Terraria world map, pixel by pixel.
        /// Returns the full file path of the saved image.
        /// All errors are propagated up to be handled by the caller (for telemetry reporting).
        /// </summary>
        private string GenerateMap()
        {
            try
            {
                string mapPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Map");
                if (!Directory.Exists(mapPath))
                    Directory.CreateDirectory(mapPath);

                string outputPath = Path.Combine(mapPath, "worldMap.png");
                int width = Main.maxTilesX;
                int height = Main.maxTilesY;

                using (Bitmap bmp = new Bitmap(width, height))
                {
                    for (int x = 0; x < width; x++)
                    {
                        for (int y = 0; y < height; y++)
                        {
                            Tile tile = Main.tile[x, y] as Terraria.Tile;
                            if (tile != null)
                                bmp.SetPixel(x, y, TileToColor(tile, x, y));
                        }
                    }
                    bmp.Save(outputPath, System.Drawing.Imaging.ImageFormat.Png);
                }
                return outputPath;
            }
            catch (Exception ex)
            {
                // Report map generation errors to telemetry and rethrow so caller can handle/log as desired.
                _ = Telemetry.Report(ex);
                throw;
            }
        }

        /// <summary>
        /// Determines the appropriate color for a tile at the given world coordinates.
        /// Uses tile paint, wall paint, liquid, and height gradient.
        /// </summary>
        private Color TileToColor(Tile tile, int x, int y)
        {
            try
            {
                byte tilePaint = MapColors.GetPaint(tile);
                byte wallPaint = MapColors.GetWallPaint(tile);

                if (tile.active())
                {
                    var color = MapColors.GetTileColor(tile.type, tilePaint);
                    if (color.A == 0 && tilePaint != 0)
                        color = MapColors.GetTileColor(tile.type, 0);

                    if (color.A > 0)
                        return color;
                }

                if (tile.wall > 0)
                {
                    var wallColor = MapColors.GetWallColor(tile.wall, wallPaint);
                    if (wallColor.A == 0 && wallPaint != 0)
                        wallColor = MapColors.GetWallColor(tile.wall, 0);

                    if (wallColor.A > 0)
                        return wallColor;
                }

                var liquidColor = MapColorHelper.GetLiquidColor(tile);
                if (liquidColor.A > 0)
                    return liquidColor;

                return MapColorHelper.GetHeightGradient(y);
            }
            catch (Exception ex)
            {
                // Report errors in color calculation to telemetry, fallback to magenta as error indicator.
                _ = Telemetry.Report(ex);
                return Color.Magenta;
            }
        }
    }
}