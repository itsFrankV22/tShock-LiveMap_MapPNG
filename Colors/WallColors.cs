using System.Drawing;
using Terraria.ID;
using TShockAPI;

namespace tShock_LiveMap.Colors
{
    public static class WallColors
    {
        public static readonly Dictionary<int, Color> Colors = BuildColors();

        private static Dictionary<int, Color> BuildColors()
        {
            var dict = new Dictionary<int, Color>();

            // Helper to add safely
            void Add(int id, Color color, string comment)
            {
                if (!dict.TryAdd(id, color))
                {
                    // Duplicated key, log to console with type, id, and name
                    string name = GetWallName(id);
                    TShock.Log.ConsoleInfo($"[WorldMapExporter] Duplicate WALL id={id} ({name}) ({comment}) ignored.");
                }
            }

            // ---- Walls ----
            Add(WallID.None, Color.Black, "No wall");
            Add(WallID.Stone, Color.FromArgb(89, 85, 82), "Stone Wall");
            Add(WallID.Dirt, Color.FromArgb(106, 85, 54), "Dirt Wall");
            Add(WallID.EbonstoneUnsafe, Color.FromArgb(53, 44, 68), "Ebonstone Wall");
            Add(WallID.CrimstoneUnsafe, Color.FromArgb(117, 32, 57), "Crimstone Wall");
            Add(WallID.Sandstone, Color.FromArgb(187, 168, 88), "Sandstone Wall");
            Add(WallID.HardenedSand, Color.FromArgb(196, 181, 127), "Hardened Sand Wall");
            Add(WallID.SnowWallUnsafe, Color.FromArgb(184, 226, 254), "Snow Wall");
            Add(WallID.IceUnsafe, Color.FromArgb(131, 194, 235), "Ice Wall");
            Add(WallID.GrassUnsafe, Color.FromArgb(50, 193, 65), "Grass Wall");
            Add(WallID.CorruptGrassUnsafe, Color.FromArgb(85, 85, 168), "Corrupt Grass Wall");
            Add(WallID.CrimsonGrassUnsafe, Color.FromArgb(166, 53, 92), "Crimson Grass Wall");
            Add(WallID.JungleUnsafe, Color.FromArgb(42, 170, 49), "Jungle Wall");
            Add(WallID.FlowerUnsafe, Color.FromArgb(89, 207, 72), "Flower Wall");
            Add(WallID.Cave6Unsafe, Color.FromArgb(112, 74, 56), "Cave Wall (Brown)");
            Add(WallID.Cave7Unsafe, Color.FromArgb(88, 61, 46), "Cave Wall (Purple)");
            Add(WallID.Cave8Unsafe, Color.FromArgb(162, 175, 180), "Cave Wall (Pearlstone)");

            Add(WallID.Wood, Color.FromArgb(99, 50, 30), "Wood Wall");
            Add(WallID.BorealWood, Color.FromArgb(107, 132, 139), "Boreal Wood Wall");
            Add(WallID.PalmWood, Color.FromArgb(233, 232, 177), "Palm Wood Wall");
            Add(WallID.Ebonwood, Color.FromArgb(88, 61, 46), "Ebonwood Wall");
            Add(WallID.Shadewood, Color.FromArgb(110, 90, 100), "Shadewood Wall");
            Add(WallID.Pearlwood, Color.FromArgb(170, 152, 185), "Pearlwood Wall");
            Add(WallID.SpookyWood, Color.FromArgb(89, 53, 36), "Spooky Wood Wall");
            Add(WallID.LivingWood, Color.FromArgb(151, 107, 75), "Living Wood Wall");

            Add(WallID.BlueDungeonSlabUnsafe, Color.FromArgb(43, 96, 222), "Blue Dungeon Slab Wall");
            Add(WallID.GreenDungeonSlabUnsafe, Color.FromArgb(68, 157, 61), "Green Dungeon Slab Wall");
            Add(WallID.PinkDungeonSlabUnsafe, Color.FromArgb(203, 61, 64), "Pink Dungeon Slab Wall");
            Add(WallID.BlueDungeonTileUnsafe, Color.FromArgb(43, 96, 222), "Blue Dungeon Tile Wall");
            Add(WallID.GreenDungeonTileUnsafe, Color.FromArgb(68, 157, 61), "Green Dungeon Tile Wall");
            Add(WallID.PinkDungeonTileUnsafe, Color.FromArgb(203, 61, 64), "Pink Dungeon Tile Wall");
            Add(WallID.BlueDungeonUnsafe, Color.FromArgb(43, 96, 222), "Blue Dungeon Wall");
            Add(WallID.GreenDungeonUnsafe, Color.FromArgb(68, 157, 61), "Green Dungeon Wall");
            Add(WallID.PinkDungeonUnsafe, Color.FromArgb(203, 61, 64), "Pink Dungeon Wall");

            Add(WallID.ObsidianBrick, Color.FromArgb(97, 68, 210), "Obsidian Brick Wall");
            Add(WallID.HellstoneBrick, Color.FromArgb(222, 96, 62), "Hellstone Brick Wall");

            Add(WallID.MarbleUnsafe, Color.FromArgb(200, 200, 210), "Marble Wall");
            Add(WallID.GraniteUnsafe, Color.FromArgb(60, 44, 82), "Granite Wall");

            Add(WallID.HiveUnsafe, Color.FromArgb(217, 128, 59), "Hive Wall");
            Add(WallID.LihzahrdBrickUnsafe, Color.FromArgb(178, 144, 64), "Lihzahrd Brick Wall");

            // ... add more as needed!

            return dict;
        }

        private static string GetWallName(int id)
        {
            foreach (var field in typeof(WallID).GetFields())
            {
                if (field.FieldType == typeof(int) && (int)field.GetValue(null) == id)
                    return field.Name;
            }
            return id.ToString();
        }
    }
}