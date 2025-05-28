using Newtonsoft.Json;
using TerrariaApi.Server;
using TShockAPI;
using ItemDecoration; // Import telemetry for error reporting

namespace CheckUpdates
{
    /// <summary>
    /// Provides update checking utilities for the plugin, including GitHub release version checking.
    /// All exceptions are reported to telemetry and logged to the console.
    /// </summary>
    public class CheckUpdates
    {
        /// <summary>
        /// Requests the latest release version from GitHub.
        /// Returns the version as a Version object, or null if error.
        /// Any errors are reported to telemetry and logged.
        /// </summary>
        public static async Task<Version?> RequestLatestVersion()
        {
            string url = "https://api.github.com/repos/itsFrankV22/tShock-LiveMap_MapPNG/releases/latest";

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.UserAgent.TryParseAdd("request"); // Set a user agent header

                try
                {
                    // Get the response from GitHub API
                    var response = await client.GetStringAsync(url);
                    dynamic? latestRelease = JsonConvert.DeserializeObject<dynamic>(response);

                    if (latestRelease == null) return null;

                    // Extract the version tag
                    string tag = latestRelease.name;
                    tag = tag.Trim('v');
                    string[] nums = tag.Split('.');

                    Version version = new Version(int.Parse(nums[0]),
                                                  int.Parse(nums[1]),
                                                  int.Parse(nums[2])
                                                  );
                    return version;
                }
                catch (Exception ex)
                {
                    // Report and log any error while fetching or parsing the version
                    _ = Telemetry.Report(ex);
                    TShock.Log.ConsoleError($"[ LIVEMAP ] Error checking for updates: {ex.Message}");
                }
            }

            return null;
        }

        /// <summary>
        /// Checks if the currently installed plugin version is up to date with the latest GitHub release.
        /// Any errors are reported to telemetry.
        /// </summary>
        public static async Task<bool> IsUpToDate(TerrariaPlugin plugin)
        {
            try
            {
                Version? latestVersion = await RequestLatestVersion();
                Version curVersion = plugin.Version;

                return latestVersion != null && curVersion >= latestVersion;
            }
            catch (Exception ex)
            {
                // Report and log any error while comparing versions
                _ = Telemetry.Report(ex);
                TShock.Log.ConsoleError($"[ LIVEMAP ] Error in IsUpToDate: {ex.Message}");
                return true; // Assume up-to-date on error, to avoid spam
            }
        }

        /// <summary>
        /// Checks for updates and logs a detailed status message to the console.
        /// All exceptions are reported to telemetry and logged to the console.
        /// </summary>
        public static async Task CheckUpdateVerbose(TerrariaPlugin? plugin)
        {
            if (plugin == null) return;

            TShock.Log.ConsoleInfo($"[ LIVEMAP ] Checking for updates...");

            try
            {
                Version? latestVersion = await RequestLatestVersion(); // latest version from GitHub
                Version currentVersion = plugin.Version;               // currently installed version

                if (latestVersion == null)
                {
                    TShock.Log.ConsoleWarn("[ LIVEMAP ] Could not determine the latest version.");
                    return;
                }

                bool isUpToDate = currentVersion >= latestVersion;

                if (isUpToDate)
                {
                    TShock.Log.ConsoleInfo($"[ LIVEMAP ] Plugin is up to date! (v{currentVersion})");
                }
                else
                {
                    TShock.Log.ConsoleError("**********************************************");
                    TShock.Log.ConsoleError("[ LIVEMAP ] Plugin is NOT up to date!");
                    TShock.Log.ConsoleInfo($"      INSTALLED: v{currentVersion}");
                    TShock.Log.ConsoleInfo($"      AVAILABLE: v{latestVersion}");
                    TShock.Log.ConsoleError("**********************************************");
                }
            }
            catch (Exception ex)
            {
                // Report and log any error while checking updates
                _ = Telemetry.Report(ex);
                TShock.Log.ConsoleError($"[ LIVEMAP ] Error in CheckUpdateVerbose: {ex.Message}");
            }
        }
    }
}