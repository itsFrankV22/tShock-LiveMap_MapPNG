using ItemDecoration;
using System.Drawing;
using Terraria;
using TerrariaApi.Server;
using tShock_LiveMap.Colors;
using TShockAPI;

namespace WorldMapExporter
{
    [ApiVersion(2, 1)]
    public class WorldMapExporterPlugin : TerrariaPlugin
    {
        public override string Author => "FrankV22";
        public override string Description => "MAP Exporter .png";
        public override string Name => "LiveMap_WorldMapExporter";
        public override Version Version => new Version(1, 1, 0);

        private static System.Timers.Timer mapTimer;

        public WorldMapExporterPlugin(Main game) : base(game)
        {
        }

        public override void Initialize()
        {
            ServerApi.Hooks.GamePostInitialize.Register(this, OnPostInitialize);

            Commands.ChatCommands.Add(new Command("map.export", GenMap, "genmap"));

            mapTimer = new System.Timers.Timer(300000);
            mapTimer.Elapsed += (s, e) => GenMapAuto();
            mapTimer.AutoReset = true;
            mapTimer.Enabled = true;

            Console.WriteLine($"\x1b[106;30;1m {Name} {Version} by {Author} \x1b[0m");
        }

        private async void OnPostInitialize(EventArgs e)
        {
            try
            {
                Telemetry.Start(this);

                await CheckUpdates.CheckUpdates.CheckUpdateVerbose(this);
            }
            catch (Exception ex)
            {
                await Telemetry.Report(ex);
                TShock.Log.ConsoleError($"[ {Name} ] Error OnPostInitialize: {ex.Message}");
            }
        }

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
                plr.SendErrorMessage($"Error Exporting MAP: {ex.Message}");
            }
        }

        private void GenMapAuto()
        {
            try
            {
                GenerateMap();
            }
            catch {  }
        }

        private string GenerateMap()
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
        private Color TileToColor(Tile tile, int x, int y)
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
    }
}