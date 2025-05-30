using TShockAPI;
using Rests;
using tShock_LiveMap.Extras;

namespace tShock_LiveMap.REST
{
    /// <summary>
    /// Provides REST API endpoints for the WorldMapExporter plugin.
    /// </summary>
    public static class RESTapi
    {
        /// <summary>
        /// Registers the REST API endpoint for retrieving the world map as a base64 string.
        /// Call this method from your plugin's Initialize().
        /// </summary>
        public static void Register()
        {
            // Register a new REST endpoint at /map/base64.
            // Only users with the "tshock.rest.admin" permission can access it.
            TShock.RestApi.Register(new SecureRestCommand(
                "/map/base64",
                GetMapBase64,
                "tshock.rest.map"
            ));

            // Register the REST endpoint for reporting errors (Telemetry)
            TShock.RestApi.Register(new SecureRestCommand(
                "/map/report",
                ReportError,
                "tshock.rest.admin"
            ));
        }

        /// <summary>
        /// Handler for the "/map/base64" REST endpoint.
        /// Returns the world map as a base64 string if it exists.
        /// </summary>
        /// <param name="args">REST request arguments provided by TShock.</param>
        /// <returns>
        /// An anonymous object with:
        ///   - success (bool): true if the map was found, false otherwise.
        ///   - map (string): base64-encoded PNG image (if successful).
        ///   - error (string): error message (if failed).
        /// </returns>
        private static object GetMapBase64(RestRequestArgs args)
        {
            try
            {
                // Build the path to the expected map image (worldMap.png)
                string mapPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Map", "worldMap.png");

                // If the file doesn't exist, return an error response.
                if (!File.Exists(mapPath))
                {
                    return new { success = false, error = "Map image not found. try /genmap" };
                }

                // Read the image file as bytes and convert it to a base64 string.
                byte[] imageBytes = File.ReadAllBytes(mapPath);
                string base64 = Convert.ToBase64String(imageBytes);

                // Return the base64 string in a successful response.
                return new { success = true, map = base64 };
            }
            catch (Exception ex)
            {
                // Report error to telemetry and return an error response.
                // Fire and forget telemetry report.
                _ = Telemetry.Report(ex);
                return new { success = false, error = ex.Message };
            }
        }

        /// <summary>
        /// Handler for the "/map/report" REST endpoint.
        /// Allows manual error reporting via HTTP POST or GET, for admin/debugging.
        /// Accepts error message and stack trace as parameters.
        /// </summary>
        /// <param name="args">REST request arguments provided by TShock.</param>
        /// <returns>
        /// An anonymous object with:
        ///   - success (bool): true if the report was sent, false otherwise.
        ///   - error (string): error message (if failed).
        /// </returns>
        private static object ReportError(RestRequestArgs args)
        {
            try
            {
                // Read "message" and "stack" from REST parameters, defaulting if missing
                string message = args.Parameters["message"] ?? "Manual report from REST";
                string stack = args.Parameters["stack"] ?? string.Empty;

                // Build a synthetic exception using both message and stack (if provided)
                var ex = new Exception(message + (string.IsNullOrEmpty(stack) ? "" : $"\nStack:\n{stack}"));

                // Fire-and-forget: immediately report this to telemetry
                _ = Telemetry.Report(ex);

                // REST response: always indicate success if no exceptions thrown
                return new { success = true, info = "Error report sent to telemetry." };
            }
            catch (Exception ex)
            {
                // If any error occurs in the above, try to report it to telemetry, too
                _ = Telemetry.Report(ex);
                // Return error info to the client
                return new { success = false, error = ex.Message };
            }
        }
    }
}