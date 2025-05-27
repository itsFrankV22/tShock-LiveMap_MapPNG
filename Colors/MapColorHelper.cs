using System.Drawing;
using Terraria;

namespace tShock_LiveMap.Colors
{
    public static class MapColorHelper
    {
        public static int SpaceLayer => 0; // Empieza desde arriba, y=0
        public static int SkyLayer => (int)(Main.worldSurface * 0.45); // Capa cielo (aprox)
        public static int SurfaceLayer => (int)Main.worldSurface; // Superficie real
        public static int UndergroundLayer => (int)Main.rockLayer; // Cueva
        public static int HellLayer => Main.maxTilesY - 200; // Inframundo

        public static Color GetHeightGradient(int y)
        {
            int maxY = Main.maxTilesY - 1;

            // Espacio exterior
            if (y <= SkyLayer)
            {
                float t = InverseLerp(SpaceLayer, SkyLayer, y);
                // De azul oscuro a celeste tenue
                return LerpColor(Color.FromArgb(45, 56, 120), Color.FromArgb(130, 190, 255), t);
            }
            // Cielo alto
            else if (y <= SurfaceLayer)
            {
                float t = InverseLerp(SkyLayer, SurfaceLayer, y);
                // De celeste a verde claro (cielo a césped)
                return LerpColor(Color.FromArgb(130, 190, 255), Color.FromArgb(110, 214, 97), t);
            }
            // Superficie/interior (césped a tierra)
            else if (y <= UndergroundLayer)
            {
                float t = InverseLerp(SurfaceLayer, UndergroundLayer, y);
                // De verde claro a marrón claro
                return LerpColor(Color.FromArgb(110, 214, 97), Color.FromArgb(151, 107, 75), t);
            }
            // Subsuelo/cueva
            else if (y < HellLayer)
            {
                float t = InverseLerp(UndergroundLayer, HellLayer, y);
                // De marrón claro a gris oscuro
                return LerpColor(Color.FromArgb(151, 107, 75), Color.FromArgb(80, 80, 100), t);
            }
            // Inframundo
            else
            {
                float t = InverseLerp(HellLayer, maxY, y);
                // De gris oscuro a rojo volcánico
                return LerpColor(Color.FromArgb(80, 80, 100), Color.FromArgb(170, 50, 35), t);
            }
        }

        public static Color GetLiquidColor(ITile tile)
        {
            if (tile.liquid == 0) return Color.Transparent;

            switch (tile.liquidType())
            {
                case 0: // Agua
                    return Color.FromArgb(120, 65, 125, 255); // Azul agua translúcido
                case 1: // Lava
                    return Color.FromArgb(170, 255, 80, 24); // Lava naranja translúcido
                case 2: // Miel
                    return Color.FromArgb(180, 220, 180, 70); // Miel dorada translúcida
                case 3: // Shimmer (Terraria 1.4.4+)
                    return Color.FromArgb(160, 170, 100, 255); // Shimmer: lila-azulado translúcido

                default:
                    return Color.Transparent;
            }
        }

        // Interpolación lineal de colores (lerp)
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

        // Lerp auxiliar
        public static float InverseLerp(float a, float b, float v)
        {
            if (a == b) return 0f;
            return Math.Clamp((v - a) / (b - a), 0f, 1f);
        }
    }
}