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
        public override Version Version => new Version(1, 0, 0);

        private static Dictionary<int, Color> tileColors = TileColors.Colors;
        private static Dictionary<int, Color> wallColors = WallColors.Colors;

        private static System.Timers.Timer mapTimer;

        public WorldMapExporterPlugin(Main game) : base(game)
        {
        }

        public override void Initialize()
        {
            Commands.ChatCommands.Add(new Command("map.export", GenMap, "genmap"));

            mapTimer = new System.Timers.Timer(300000);
            mapTimer.Elapsed += (s, e) => GenMapAuto();
            mapTimer.AutoReset = true;
            mapTimer.Enabled = true;
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
                        ITile tile = Main.tile[x, y];
                        bmp.SetPixel(x, y, TileToColor(tile, x, y));
                    }
                }
                bmp.Save(outputPath, System.Drawing.Imaging.ImageFormat.Png);
            }
            return outputPath;
        }

        private Color TileToColor(ITile tile, int x, int y)
        {
            if (tile.active() && tileColors.TryGetValue(tile.type, out Color color))
                return color;

            if (tile.wall > 0 && wallColors.TryGetValue(tile.wall, out Color wallColor))
                return wallColor;

            // Si tiene líquido, usa color de líquido
            var liquidColor = MapColorHelper.GetLiquidColor(tile);
            if (liquidColor.A > 0)
                return liquidColor;

            // Si tile no tiene color y no es pared ni líquido, haz:
            return MapColorHelper.GetHeightGradient(y);
        }
    }

}