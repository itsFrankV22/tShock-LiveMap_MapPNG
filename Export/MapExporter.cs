using System.Drawing;
using TShockAPI;
using Terraria;
using tShock_LiveMap.Colors;
using tShock_LiveMap.Extras;
using System.Runtime.InteropServices;

namespace tShock_LiveMap.Export
{
    public static class MapExporter
    {
        /// <summary>
        /// Exporta el mapa completo a un PNG en la carpeta /Map/worldMap.png
        /// </summary>
        public static string GenerateFullMap()
        {
            // Check if not Windows or GDI+ is not available
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                if (!IsGdiPlusAvailable())
                {
                    TShock.Log.ConsoleError("[LiveMap_WorldMapExporter] Automatic full map export is NOT yet available on Linux/Mac unless GDI+ is installed (libgdiplus). Work is in progress.");
                    throw new PlatformNotSupportedException("Automatic full map export is not yet available on Linux/Mac unless GDI+ is installed (libgdiplus).");
                }
            }

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
                        ITile itile = Main.tile[x, y];
                        Tile tile = itile as Tile;
                        if (tile != null)
                            bmp.SetPixel(x, y, GetTileColor(tile, x, y));
                    }
                }
                bmp.Save(outputPath, System.Drawing.Imaging.ImageFormat.Png);
            }
            return outputPath;
        }

        /// <summary>
        /// Devuelve los bytes PNG de un chunk de tamaño chunkSize*chunkSize.
        /// </summary>
        public static byte[] GenerateChunkData(int chunkX, int chunkY, int chunkSize = 64)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                if (!IsGdiPlusAvailable())
                {
                    TShock.Log.ConsoleError("[LiveMap_WorldMapExporter] Chunk export is NOT yet available on Linux/Mac unless GDI+ is installed (libgdiplus). Work is in progress.");
                    throw new PlatformNotSupportedException("Chunk export is not yet available on Linux/Mac unless GDI+ is installed (libgdiplus).");
                }
            }

            int startX = chunkX * chunkSize;
            int startY = chunkY * chunkSize;

            int endX = Math.Min(startX + chunkSize, Main.maxTilesX);
            int endY = Math.Min(startY + chunkSize, Main.maxTilesY);

            int width = endX - startX;
            int height = endY - startY;

            using (Bitmap bmp = new Bitmap(width, height))
            {
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        int worldX = startX + x;
                        int worldY = startY + y;
                        ITile itile = Main.tile[worldX, worldY];
                        Tile tile = itile as Tile;

                        if (tile != null)
                        {
                            Color color = GetTileColor(tile, worldX, worldY);
                            bmp.SetPixel(x, y, color);
                        }
                    }
                }

                using MemoryStream ms = new MemoryStream();
                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                return ms.ToArray();
            }
        }

        /// <summary>
        /// Check if GDI+ is available by attempting to construct a Bitmap.
        /// </summary>
        private static bool IsGdiPlusAvailable()
        {
            try
            {
                using (var bmp = new Bitmap(1, 1))
                {
                    // Try to access pixel
                    bmp.SetPixel(0, 0, Color.Black);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Lógica de color de tile, compatible con MapColors y MapColorHelper.
        /// </summary>
        public static Color GetTileColor(Tile tile, int x, int y)
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
                _ = Telemetry.Report(ex);
                return Color.Magenta;
            }
        }
    }
}