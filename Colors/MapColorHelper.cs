using System.Drawing;
using Terraria;

namespace tShock_LiveMap.Colors
{
    public static class MapColorHelper
    {
        // World layer constants (World Y coordinate ranges)
        public static int SpaceLayer => 0; // Top of the world (y=0)
        public static int SkyLayer => (int)(Main.worldSurface * 0.45); // Sky layer
        public static int SurfaceLayer => (int)Main.worldSurface; // Actual surface
        public static int UndergroundLayer => (int)Main.rockLayer; // Cave layer
        public static int HellLayer => Main.maxTilesY - 200; // Underworld

        /// <summary>
        /// Returns a color gradient based on height (y coordinate).
        /// </summary>
        public static Color GetHeightGradient(int y)
        {
            int maxY = Main.maxTilesY - 1;

            // Space
            if (y <= SkyLayer)
            {
                float t = InverseLerp(SpaceLayer, SkyLayer, y);
                // Dark blue to light sky blue
                return LerpColor(Color.FromArgb(45, 56, 120), Color.FromArgb(130, 190, 255), t);
            }
            // High Sky
            else if (y <= SurfaceLayer)
            {
                float t = InverseLerp(SkyLayer, SurfaceLayer, y);
                // Sky blue to light green (sky to grass)
                return LerpColor(Color.FromArgb(130, 190, 255), Color.FromArgb(110, 214, 97), t);
            }
            // Surface/Interior (grass to dirt)
            else if (y <= UndergroundLayer)
            {
                float t = InverseLerp(SurfaceLayer, UndergroundLayer, y);
                // Light green to light brown
                return LerpColor(Color.FromArgb(110, 214, 97), Color.FromArgb(151, 107, 75), t);
            }
            // Underground/Cave
            else if (y < HellLayer)
            {
                float t = InverseLerp(UndergroundLayer, HellLayer, y);
                // Light brown to dark gray
                return LerpColor(Color.FromArgb(151, 107, 75), Color.FromArgb(80, 80, 100), t);
            }
            // Underworld
            else
            {
                float t = InverseLerp(HellLayer, maxY, y);
                // Dark gray to volcanic red
                return LerpColor(Color.FromArgb(80, 80, 100), Color.FromArgb(170, 50, 35), t);
            }
        }

        /// <summary>
        /// Returns a color for liquid tiles.
        /// </summary>
        public static Color GetLiquidColor(ITile tile)
        {
            if (tile.liquid == 0) return Color.Transparent;

            switch (tile.liquidType())
            {
                case 0: // Water
                    return Color.FromArgb(120, 65, 125, 255); // Translucent blue water
                case 1: // Lava
                    return Color.FromArgb(170, 255, 80, 24); // Translucent orange lava
                case 2: // Honey
                    return Color.FromArgb(180, 220, 180, 70); // Translucent honey
                case 3: // Shimmer (Terraria 1.4.4+)
                    return Color.FromArgb(160, 170, 100, 255); // Translucent blue-purple shimmer
                default:
                    return Color.Transparent;
            }
        }

        /// <summary>
        /// Linear color interpolation (lerp) between c1 and c2.
        /// </summary>
        public static Color LerpColor(Color c1, Color c2, float t)
        {
            t = Math.Clamp(t, 0f, 1f);
            return Color.FromArgb(
                255,
                (int)(c1.R + (c2.R - c1.R) * t),
                (int)(c1.G + (c2.G - c1.G) * t),
                (int)(c1.B + (c2.B - c1.B) * t)
            );
        }

        /// <summary>
        /// Returns the normalized t value for v between a and b.
        /// </summary>
        public static float InverseLerp(float a, float b, float v)
        {
            if (a == b) return 0f;
            return Math.Clamp((v - a) / (b - a), 0f, 1f);
        }
    }
}