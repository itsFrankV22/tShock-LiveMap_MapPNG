using Terraria;
using TShockAPI;
using TerrariaApi.Server;
using tShock_LiveMap;
using tShock_LiveMap.Export;
using tShock_LiveMap.Extras;
using tShock_LiveMap.REST;
using tShock_LiveMap.WebSockets;

namespace WorldMapExporter
{
    [ApiVersion(2, 1)]
    public class WorldMapExporterPlugin : TerrariaPlugin
    {
        public override string Author => "FrankV22";
        public override string Description => "MAP Exporter .png";
        public override string Name => "LiveMap_WorldMapExporter";
        public override Version Version => new Version(1, 3, 0);

        private static System.Timers.Timer mapTimer;
        private Config config;

        public WorldMapExporterPlugin(Main game) : base(game) { }

        public override void Initialize()
        {
            try
            {
                config = Config.Load();

                ServerApi.Hooks.GamePostInitialize.Register(this, OnPostInitialize);
                Commands.ChatCommands.Add(new Command("map.export", GenMap, "genmap", "mapgen"));

                mapTimer = new System.Timers.Timer(config.AutoSaveMinutes * 60 * 1000);
                mapTimer.Elapsed += (s, e) => GenMapAuto();
                mapTimer.AutoReset = true;
                mapTimer.Enabled = true;

                Console.WriteLine($"\x1b[106;30;1m {Name} {Version} by {Author} \x1b[0m");
            }
            catch (Exception ex)
            {
                _ = Telemetry.Report(ex);
                TShock.Log.ConsoleError($"[{Name}] Error during plugin initialization: {ex.Message}");
            }
        }

        private async void OnPostInitialize(EventArgs e)
        {
            try
            {
                RESTapi.Register();
                Telemetry.Start(this);
                MapSocketServer.Start(config.WebSocketPort);
                await CheckUpdates.CheckUpdateVerbose(this);
            }
            catch (Exception ex)
            {
                await Telemetry.Report(ex);
                TShock.Log.ConsoleError($"[{Name}] Error OnPostInitialize: {ex.Message}");
            }
        }

        private void GenMap(CommandArgs args)
        {
            TSPlayer plr = args.Player;
            try
            {
                string outputPath = MapExporter.GenerateFullMap();
                plr.SendSuccessMessage($"Map Exported: {outputPath}");
            }
            catch (Exception ex)
            {
                _ = Telemetry.Report(ex);
                plr.SendErrorMessage($"Error Exporting MAP: {ex.Message}");
            }
        }

        private void GenMapAuto()
        {
            try
            {
                MapExporter.GenerateFullMap();
            }
            catch (Exception ex)
            {
                _ = Telemetry.Report(ex);
                TShock.Log.ConsoleError($"[{Name}] Error in automatic map export: {ex.Message}");
            }
        }
    }
}