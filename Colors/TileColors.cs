using System.Drawing;
using Terraria.ID;
using TShockAPI;

namespace tShock_LiveMap.Colors
{
    public static class TileColors
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
                    string name = GetTileName(id);
                    TShock.Log.ConsoleInfo($"[WorldMapExporter] Duplicate TILE id={id} ({name}) ({comment}) ignored.");
                }
            }

            // --- SOIL BLOCKS ---
            Add(TileID.Dirt, Color.FromArgb(151, 107, 75), "Dirt Block");
            Add(TileID.Mud, Color.FromArgb(87, 85, 86), "Mud Block");
            Add(TileID.Sand, Color.FromArgb(224, 201, 92), "Sand Block");
            Add(TileID.Ebonsand, Color.FromArgb(53, 44, 68), "Ebonsand Block");
            Add(TileID.Pearlsand, Color.FromArgb(234, 219, 185), "Pearlsand Block");
            Add(TileID.Crimsand, Color.FromArgb(227, 98, 137), "Crimsand Block");
            Add(TileID.SnowBlock, Color.FromArgb(255, 255, 255), "Snow Block");
            Add(TileID.HardenedSand, Color.FromArgb(210, 192, 127), "Hardened Sand Block");
            Add(TileID.Silt, Color.FromArgb(98, 95, 167), "Silt Block");
            Add(TileID.Ash, Color.FromArgb(178, 175, 142), "Ash Block");
            Add(TileID.ClayBlock, Color.FromArgb(190, 134, 74), "Clay Block");
            Add(TileID.Slush, Color.FromArgb(141, 181, 203), "Slush Block");

            // --- STONE BLOCKS ---
            Add(TileID.Stone, Color.FromArgb(128, 128, 128), "Stone Block");
            Add(TileID.Ebonstone, Color.FromArgb(88, 61, 46), "Ebonstone Block");
            Add(TileID.Crimstone, Color.FromArgb(200, 44, 44), "Crimstone Block");
            Add(TileID.Pearlstone, Color.FromArgb(162, 175, 180), "Pearlstone Block");
            Add(TileID.Obsidian, Color.FromArgb(97, 68, 210), "Obsidian");
            Add(TileID.Marble, Color.FromArgb(200, 200, 210), "Marble Block");
            Add(TileID.Granite, Color.FromArgb(60, 44, 82), "Granite Block");
            Add(TileID.DesertFossil, Color.FromArgb(202, 174, 110), "Desert Fossil");

            // --- GRASS BLOCKS ---
            Add(TileID.Grass, Color.FromArgb(28, 216, 94), "Grass Block");
            Add(TileID.CorruptGrass, Color.FromArgb(141, 137, 223), "Corrupt Grass");
            Add(TileID.JungleGrass, Color.FromArgb(59, 194, 54), "Jungle Grass");
            Add(TileID.HallowedGrass, Color.FromArgb(121, 185, 255), "Hallowed Grass");
            Add(TileID.MushroomGrass, Color.FromArgb(29, 43, 83), "Mushroom Grass");
            Add(TileID.AshGrass, Color.FromArgb(238, 190, 143), "Ash Grass");
            Add(TileID.GolfGrass, Color.FromArgb(16, 173, 110), "Golf Grass");

            // --- MOSS ---
            Add(TileID.BlueMoss, Color.FromArgb(0, 128, 255), "Blue Moss");
            Add(TileID.GreenMoss, Color.FromArgb(0, 255, 0), "Green Moss");
            Add(TileID.PurpleMoss, Color.FromArgb(128, 0, 255), "Purple Moss");
            Add(TileID.RedMoss, Color.FromArgb(255, 0, 0), "Red Moss");
            Add(TileID.BrownMoss, Color.FromArgb(128, 64, 0), "Brown Moss");
            Add(TileID.LavaMoss, Color.FromArgb(255, 128, 0), "Lava Moss");

            // --- ORES ---
            Add(TileID.Copper, Color.FromArgb(198, 124, 78), "Copper Ore");
            Add(TileID.Tin, Color.FromArgb(182, 175, 130), "Tin Ore");
            Add(TileID.Iron, Color.FromArgb(152, 171, 172), "Iron Ore");
            Add(TileID.Lead, Color.FromArgb(112, 112, 130), "Lead Ore");
            Add(TileID.Silver, Color.FromArgb(236, 230, 192), "Silver Ore");
            Add(TileID.Tungsten, Color.FromArgb(182, 221, 204), "Tungsten Ore");
            Add(TileID.Gold, Color.FromArgb(246, 194, 73), "Gold Ore");
            Add(TileID.Platinum, Color.FromArgb(185, 221, 236), "Platinum Ore");
            Add(TileID.Cobalt, Color.FromArgb(0, 71, 171), "Cobalt Ore");
            Add(TileID.Palladium, Color.FromArgb(255, 128, 80), "Palladium Ore");
            Add(TileID.Mythril, Color.FromArgb(0, 255, 200), "Mythril Ore");
            Add(TileID.Orichalcum, Color.FromArgb(255, 0, 220), "Orichalcum Ore");
            Add(TileID.Adamantite, Color.FromArgb(255, 0, 0), "Adamantite Ore");
            Add(TileID.Titanium, Color.FromArgb(140, 140, 150), "Titanium Ore");
            Add(TileID.Crimtane, Color.FromArgb(211, 17, 17), "Crimtane Ore");
            Add(TileID.Demonite, Color.FromArgb(61, 61, 122), "Demonite Ore");
            Add(TileID.Chlorophyte, Color.FromArgb(0, 255, 0), "Chlorophyte Ore");
            Add(TileID.Hellstone, Color.FromArgb(255, 140, 21), "Hellstone");

            // --- GEMS ---
            Add(TileID.Amethyst, Color.FromArgb(200, 0, 200), "Amethyst");
            Add(TileID.Topaz, Color.FromArgb(255, 220, 0), "Topaz");
            Add(TileID.Sapphire, Color.FromArgb(0, 140, 255), "Sapphire");
            Add(TileID.Emerald, Color.FromArgb(0, 255, 0), "Emerald");
            Add(TileID.Ruby, Color.FromArgb(255, 0, 0), "Ruby");
            Add(TileID.Diamond, Color.FromArgb(185, 242, 255), "Diamond");
            Add(TileID.AmberStoneBlock, Color.FromArgb(255, 186, 0), "Amber");

            // --- SPECIAL BLOCKS ---
            Add(TileID.IceBlock, Color.FromArgb(180, 210, 255), "Ice Block");
            Add(TileID.HoneyBlock, Color.FromArgb(252, 180, 70), "Honey Block");
            Add(TileID.Cloud, Color.FromArgb(200, 200, 200), "Cloud Block");
            Add(TileID.RainCloud, Color.FromArgb(160, 160, 160), "Rain Cloud Block");
            Add(TileID.CrispyHoneyBlock, Color.FromArgb(217, 128, 59), "Crispy Honey Block");
            Add(TileID.PumpkinBlock, Color.FromArgb(255, 156, 12), "Pumpkin Block");
            Add(TileID.HayBlock, Color.FromArgb(216, 200, 94), "Hay Block");
            Add(TileID.Cactus, Color.FromArgb(95, 200, 82), "Cactus Block");
            Add(TileID.BambooBlock, Color.FromArgb(190, 186, 117), "Bamboo Block");
            Add(TileID.LivingMahogany, Color.FromArgb(170, 110, 50), "Living Mahogany");
            Add(TileID.LivingMahoganyLeaves, Color.FromArgb(136, 179, 59), "Living Mahogany Leaves");

            // --- STRUCTURES & BRICKS ---
            Add(TileID.Trees, Color.FromArgb(254, 121, 2), "Tree");
            Add(TileID.LihzahrdBrick, Color.FromArgb(178, 144, 64), "Lihzahrd Brick");
            Add(TileID.LihzahrdAltar, Color.FromArgb(255, 187, 0), "Lihzahrd Altar");
            Add(TileID.HellstoneBrick, Color.FromArgb(222, 96, 62), "Hellstone Brick");
            Add(TileID.BlueDungeonBrick, Color.FromArgb(43, 96, 222), "Blue Dungeon Brick");
            Add(TileID.GreenDungeonBrick, Color.FromArgb(68, 157, 61), "Green Dungeon Brick");
            Add(TileID.PinkDungeonBrick, Color.FromArgb(203, 61, 64), "Pink Dungeon Brick");
            Add(TileID.GrayBrick, Color.FromArgb(144, 148, 144), "Gray Brick");
            Add(TileID.RedBrick, Color.FromArgb(211, 74, 74), "Red Brick");
            Add(TileID.Sandstone, Color.FromArgb(220, 200, 120), "Sandstone Block");
            Add(TileID.Ebonwood, Color.FromArgb(88, 61, 46), "Ebonwood");
            Add(TileID.Shadewood, Color.FromArgb(110, 90, 100), "Shadewood");
            Add(TileID.PalmWood, Color.FromArgb(233, 232, 177), "Palm Wood");
            Add(TileID.BorealWood, Color.FromArgb(107, 132, 139), "Boreal Wood");
            Add(TileID.RichMahogany, Color.FromArgb(131, 79, 49), "Rich Mahogany");
            Add(TileID.Pearlwood, Color.FromArgb(170, 152, 185), "Pearlwood");
            Add(TileID.DynastyWood, Color.FromArgb(139, 117, 100), "Dynasty Wood");
            Add(TileID.SpookyWood, Color.FromArgb(89, 53, 36), "Spooky Wood");
            Add(TileID.Meteorite, Color.FromArgb(131, 65, 152), "Meteorite");
            Add(TileID.MeteoriteBrick, Color.FromArgb(124, 50, 139), "Meteorite Brick");

            // --- LIGHT SOURCES ---
            Add(TileID.Torches, Color.FromArgb(253, 221, 3), "Torch");
            Add(TileID.Campfire, Color.FromArgb(253, 253, 3), "Campfire");
            Add(TileID.CrystalBall, Color.FromArgb(182, 255, 255), "Crystal Ball");

            // --- LIQUIDS ---
            Add(TileID.WaterDrip, Color.FromArgb(64, 104, 208), "Water Drip");
            Add(TileID.LavaDrip, Color.FromArgb(253, 32, 3), "Lava Drip");
            Add(TileID.HoneyDrip, Color.FromArgb(252, 180, 70), "Honey Drip");

            // --- DECORATION / OTHERS ---
            Add(TileID.Books, Color.FromArgb(180, 180, 210), "Books");
            Add(TileID.Cobweb, Color.FromArgb(240, 240, 240), "Cobweb");
            Add(TileID.Pots, Color.FromArgb(200, 160, 110), "Pots");
            Add(TileID.Crystals, Color.FromArgb(150, 255, 255), "Crystals");
            Add(TileID.Plants, Color.FromArgb(28, 216, 94), "Plants");
            Add(TileID.Plants2, Color.FromArgb(121, 185, 255), "Plants2");
            Add(TileID.Coral, Color.FromArgb(255, 85, 155), "Coral");
            Add(TileID.Seaweed, Color.FromArgb(43, 96, 222), "Seaweed");
            Add(TileID.Vines, Color.FromArgb(59, 194, 54), "Vines");

            return dict;
        }

        private static string GetTileName(int id)
        {
            foreach (var field in typeof(TileID).GetFields())
            {
                if (field.FieldType == typeof(int) && (int)field.GetValue(null) == id)
                    return field.Name;
            }
            return id.ToString();
        }
    }
}