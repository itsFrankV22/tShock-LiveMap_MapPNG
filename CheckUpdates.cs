using Newtonsoft.Json;
using TerrariaApi.Server;
using TShockAPI;

namespace CheckUpdates
{
    public class CheckUpdates
    {
        public static async Task<Version?> RequestLatestVersion()
        {
            string url = "https://api.github.com/repos/itsFrankV22/tShock-LiveMap_MapPNG/releases/latest";

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.UserAgent.TryParseAdd("request"); // Set a user agent header

                try
                {
                    var response = await client.GetStringAsync(url);
                    dynamic? latestRelease = JsonConvert.DeserializeObject<dynamic>(response);

                    if (latestRelease == null) return null;

                    string tag = latestRelease.name;

                    tag = tag.Trim('v');
                    string[] nums = tag.Split('.');

                    Version version = new Version(int.Parse(nums[0]),
                                                  int.Parse(nums[1]),
                                                  int.Parse(nums[2])
                                                  );
                    return version;
                }
                catch
                {
                    TShock.Log.ConsoleInfo($"[ LIVEMAP ] Error");
                }
            }

            return null;
        }

        public static async Task<bool> IsUpToDate(TerrariaPlugin plugin)
        {
            Version? latestVersion = await RequestLatestVersion();
            Version curVersion = plugin.Version;

            return latestVersion != null && curVersion >= latestVersion;
        }

        public static async Task CheckUpdateVerbose(TerrariaPlugin? plugin)
        {
            if (plugin == null) return;

            TShock.Log.ConsoleInfo($"[ LIVEMAP ] Checking for updates...");

            Version? latestVersion = await RequestLatestVersion(); // última versión en GitHub
            Version currentVersion = plugin.Version;               // versión local instalada

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
    }
}