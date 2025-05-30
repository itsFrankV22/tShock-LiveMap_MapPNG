# tShock-LiveMap_MapPNG

¿Español? [READMEes.md](READMEes.md).

Generate a PNG map of your Terraria world using tShock – a simple and direct visualization of your server world.

## What does this plugin do?

- Generates a PNG image of your current server world map.
- The `/genmap` command creates the file at `/Map/worldMap.png`.
- The map visualizes tiles, walls, and liquids using colors based on their type, paint, and height.
- Supports manual (command) and automatic generation every 5 minutes (configurable).
- **NEW:** Adds a WebSocket server for real-time map updates and chunk streaming.
- Includes automatic update checking, optional telemetry, and REST API endpoints to expose the map as base64.
- Configuration file at `tshock/livemap/config.json`.

## How does it work?

### Main command

- `/genmap`: Generates the PNG map and notifies the user with the file path.

### Generation logic

1. Gets world size (`Main.maxTilesX`, `Main.maxTilesY`).
2. Creates a new Bitmap with those dimensions.
3. Iterates through every tile:
   - If there is a tile, gets its color by type and paint.
   - If there is a wall, gets its color.
   - If there is liquid, uses a special color.
   - Otherwise, applies a height-based gradient.
4. Saves the image as `worldMap.png` in the `/Map` folder.

### Automation

- By default, every 5 minutes (configurable in `tshock/livemap/config.json`), the plugin automatically generates the map.

### Other features

- **WebSocket support:** Real-time map streaming at `ws://<host>:<port>/map/ws/`. See `browserConsoleTest.md` for a frontend example.
- **CheckUpdates.cs:** On startup, checks for new versions and logs to console.
- **Telemetry.cs:** Sends basic server/plugin info to an external endpoint for statistics and error reporting (optional, see code to adapt/disable).
- **RESTapi.cs:** Exposes REST endpoints:
  - `/map/base64`: Returns the current map PNG encoded in base64 (requires `tshock.rest.map` permission).
  - `/map/report`: Allows manual error reporting to telemetry.

## Installation (latest tShock version)

1. **Download the compiled `.dll`** from [Releases](https://github.com/itsFrankV22/tShock-LiveMap_MapPNG/releases) or build it yourself.
2. **Place the `.dll`** in your tShock `/ServerPlugins` folder.
3. **Restart the server**.
4. Use the `/genmap` command in your server chat (requires `map.export` permission).

## Permissions

- The `/genmap` command requires the `map.export` permission.
- The `/map/base64` REST endpoint requires `tshock.rest.map`.
- The `/map/report` endpoint requires `tshock.rest.admin`.

## Example usage

```bash
/genmap
```
The map will be saved as `/Map/worldMap.png` in your server root.

## WebSocket Example

See [browserConsoleTest.md](browserConsoleTest.md) for a JavaScript client that draws the map in real time in a browser.

## Relevant code

- [Main.cs](https://github.com/itsFrankV22/tShock-LiveMap_MapPNG/blob/main/Main.cs): Main plugin logic, initialization.
- [Export/MapExporter.cs](https://github.com/itsFrankV22/tShock-LiveMap_MapPNG/blob/main/Export/MapExporter.cs): Map generation and exporting logic.
- [Colors/MapColors.cs](https://github.com/itsFrankV22/tShock-LiveMap_MapPNG/blob/main/Colors/MapColors.cs): Tile and wall color logic.
- [Colors/MapColorHelper.cs](https://github.com/itsFrankV22/tShock-LiveMap_MapPNG/blob/main/Colors/MapColorHelper.cs): Color gradients, liquid colors.
- [Extras/CheckUpdates.cs](https://github.com/itsFrankV22/tShock-LiveMap_MapPNG/blob/main/Extras/CheckUpdates.cs): Update checks.
- [Extras/Telemetry.cs](https://github.com/itsFrankV22/tShock-LiveMap_MapPNG/blob/main/Extras/Telemetry.cs): Telemetry and error reporting.
- [REST/RESTapi.cs](https://github.com/itsFrankV22/tShock-LiveMap_MapPNG/blob/main/REST/RESTapi.cs): REST API for map and telemetry endpoints.
- [WebSocket/MapWebSocket.cs](https://github.com/itsFrankV22/tShock-LiveMap_MapPNG/blob/main/WebSocket/MapWebSocket.cs): WebSocket server and logic.
- [Config/Config.cs](https://github.com/itsFrankV22/tShock-LiveMap_MapPNG/blob/main/Config/Config.cs): Configuration loading.

## Contributing

The code is under development. Contributions are welcome to improve color dictionaries, add new features, or frontend viewers!

---

Simple. Minimalist. Direct: visualize your Terraria world with a single command, or stream it live!

---

**Author:** FrankV22  
**Repository:** https://github.com/itsFrankV22/tShock-LiveMap_MapPNG

---