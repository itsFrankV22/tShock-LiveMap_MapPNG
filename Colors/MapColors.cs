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

        // Loads color definitions from the embedded XML resource.
        private static void LoadColorsFromEmbeddedXml()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "tShock_LiveMap.Resources.MapColors.xml";

            using Stream stream = assembly.GetManifestResourceStream(resourceName)
                ?? throw new Exception("Could not load MapColors.xml resource");

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

        // Parses a #AARRGGBB color string to a Color instance.
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

        // Returns the private paint field from a classic Tile using reflection.
        public static byte GetPaint(Tile tile)
        {
            var paintField = typeof(Tile).GetField("paint", BindingFlags.Instance | BindingFlags.NonPublic);
            return paintField != null ? (byte)paintField.GetValue(tile) : (byte)0;
        }

        // Returns the private wallPaint field from a classic Tile using reflection.
        public static byte GetWallPaint(Tile tile)
        {
            var wallPaintField = typeof(Tile).GetField("wallPaint", BindingFlags.Instance | BindingFlags.NonPublic);
            return wallPaintField != null ? (byte)wallPaintField.GetValue(tile) : (byte)0;
        }

        // Returns the paint property from an ITile (for modern sources).
        public static byte GetPaint(ITile tile)
        {
            if (tile == null) return 0;
            try
            {
                var type = tile.GetType();
                if (type == null) return 0;
                var prop = type.GetProperty("paint", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (prop == null) return 0;
                var value = prop.GetValue(tile);
                if (value is byte b) return b;
                if (value is int i) return (byte)i;
            }
            catch { }
            return 0;
        }

        // Returns the wallPaint property from an ITile (for modern sources).
        public static byte GetWallPaint(ITile tile)
        {
            if (tile == null) return 0;
            var prop = tile.GetType().GetProperty("wallPaint", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (prop != null)
            {
                object value = prop.GetValue(tile);
                if (value is byte b)
                    return b;
                if (value is int i)
                    return (byte)i;
            }
            return 0;
        }

        // Returns the color defined for a given tile id and paint, or a debug fallback color.
        public static Color GetTileColor(int id, int paint)
        {
            if (TileColors.TryGetValue((id, paint), out var color))
                return color;
            // Fallback: debug color for tiles with undefined color.
            return Color.Magenta;
        }

        // Returns the color defined for a given wall id and paint, or a debug fallback color.
        public static Color GetWallColor(int id, int paint)
        {
            if (WallColors.TryGetValue((id, paint), out var color))
                return color;
            // Fallback: debug color for walls with undefined color.
            return Color.Cyan;
        }

        // Helper for ITile: gets color for a tile, using reflection to get type and paint.
        public static Color GetTileColor(ITile tile)
        {
            if (tile == null) return Color.Transparent;
            bool active = false;
            try
            {
                // Try to get active() method or IsActive property.
                var method = tile.GetType().GetMethod("active");
                if (method != null)
                    active = (bool)method.Invoke(tile, null);
                else
                    active = (tile.GetType().GetProperty("IsActive")?.GetValue(tile) as bool?) ?? false;
            }
            catch { active = false; }

            if (!active)
                return Color.Transparent;

            ushort type = 0;
            try
            {
                var prop = tile.GetType().GetProperty("type");
                if (prop != null)
                {
                    object value = prop.GetValue(tile);
                    if (value is ushort us)
                        type = us;
                    else if (value is int i)
                        type = (ushort)i;
                    else if (value is byte b)
                        type = b;
                }
            }
            catch { type = 0; }

            var paint = GetPaint(tile);
            return GetTileColor(type, paint);
        }

        // Helper for ITile: gets color for a wall, using reflection to get wall id and paint.
        public static Color GetWallColor(ITile tile)
        {
            if (tile == null) return Color.Transparent;
            ushort wall = 0;
            try
            {
                var prop = tile.GetType().GetProperty("wall");
                if (prop != null)
                {
                    object value = prop.GetValue(tile);
                    if (value is ushort us)
                        wall = us;
                    else if (value is int i)
                        wall = (ushort)i;
                    else if (value is byte b)
                        wall = b;
                }
            }
            catch { wall = 0; }

            if (wall == 0) return Color.Transparent;

            var paint = GetWallPaint(tile);
            return GetWallColor(wall, paint);
        }
    }
}