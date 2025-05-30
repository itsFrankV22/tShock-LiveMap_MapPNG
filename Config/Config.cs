using Newtonsoft.Json;

namespace tShock_LiveMap
{
    public class Config
    {
        // Minutes between automatic saves
        public int AutoSaveMinutes { get; set; } = 5;
        // Port where the WebSocket server will listen
        public int WebSocketPort { get; set; } = 8585;

        // Path to the config file
        private static readonly string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tshock", "livemap", "config.json");

        // Loads config from disk or creates a default config if not present
        public static Config Load()
        {
            try
            {
                if (!File.Exists(configPath))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(configPath));
                    var defaultConfig = new Config();
                    File.WriteAllText(configPath, JsonConvert.SerializeObject(defaultConfig, Formatting.Indented));
                    return defaultConfig;
                }

                string json = File.ReadAllText(configPath);
                return JsonConvert.DeserializeObject<Config>(json) ?? new Config();
            }
            catch
            {
                // Return default config on error
                return new Config();
            }
        }
    }
}