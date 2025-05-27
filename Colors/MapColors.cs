using System.Drawing;
using System.Reflection;
using System.Xml.Linq;
using Terraria;

namespace tShock_LiveMap.Colors
{
    public static class MapColors
    {
        public static Dictionary<(int id, int paint), Color> TileColors { get; private set; }
            = new Dictionary<(int, int), Color>();

        public static Dictionary<(int id, int paint), Color> WallColors { get; private set; }
            = new Dictionary<(int, int), Color>();

        static MapColors()
        {
            LoadColorsFromEmbeddedXml();
        }

        private static void LoadColorsFromEmbeddedXml()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "tShock_LiveMap.Resources.MapColors.xml";

            using Stream stream = assembly.GetManifestResourceStream(resourceName)
                ?? throw new Exception("No se pudo cargar el recurso MapColors.xml");

            XDocument xmlDoc = XDocument.Load(stream);

            foreach (var tileElem in xmlDoc.Root.Element("Tiles")!.Elements("Tile"))
            {
                int id = int.Parse(tileElem.Attribute("Id")?.Value ?? "-1");
                int paint = int.Parse(tileElem.Attribute("Paint")?.Value ?? "0");
                string colorStr = tileElem.Attribute("Color")?.Value ?? "#00000000";

                Color color = ParseColor(colorStr);
                TileColors[(id, paint)] = color;
            }

            foreach (var wallElem in xmlDoc.Root.Element("Walls")?.Elements("Wall") ?? Enumerable.Empty<XElement>())
            {
                int id = int.Parse(wallElem.Attribute("Id")?.Value ?? "-1");
                int paint = int.Parse(wallElem.Attribute("Paint")?.Value ?? "0");
                string colorStr = wallElem.Attribute("Color")?.Value ?? "#00000000";

                Color color = ParseColor(colorStr);
                WallColors[(id, paint)] = color;
            }
        }

        private static Color ParseColor(string colorString)
        {
            if (string.IsNullOrEmpty(colorString) || !colorString.StartsWith("#") || colorString.Length != 9)
                return Color.Transparent;

            byte a = Convert.ToByte(colorString.Substring(1, 2), 16);
            byte r = Convert.ToByte(colorString.Substring(3, 2), 16);
            byte g = Convert.ToByte(colorString.Substring(5, 2), 16);
            byte b = Convert.ToByte(colorString.Substring(7, 2), 16);

            return Color.FromArgb(a, r, g, b);
        }

        public static byte GetPaint(Tile tile)
        {
            var paintField = typeof(Tile).GetField("paint", BindingFlags.Instance | BindingFlags.NonPublic);
            return paintField != null ? (byte)paintField.GetValue(tile) : (byte)0;
        }

        public static byte GetWallPaint(Tile tile)
        {
            var wallPaintField = typeof(Tile).GetField("wallPaint", BindingFlags.Instance | BindingFlags.NonPublic);
            return wallPaintField != null ? (byte)wallPaintField.GetValue(tile) : (byte)0;
        }

        public static Color GetTileColor(int id, int paint)
        {
            if (TileColors.TryGetValue((id, paint), out var color))
                return color;
            return Color.Transparent;
        }

        public static Color GetWallColor(int id, int paint)
        {
            if (WallColors.TryGetValue((id, paint), out var color))
                return color;
            return Color.Transparent;
        }
    }
}
