# tShock-LiveMap_MapPNG

¿Español? [READMEes.md](READMEes.md).

Generate a PNG map of your Terraria world using tShock – a simple and direct visualization of your server world.

## What does this plugin do?

- Generates a PNG image of your current server world map.
- The `/genmap` command creates the file at `/Map/worldMap.png`.
- The map visualizes tiles, walls, and liquids using colors based on their type, paint, and height.
- Supports manual (command) and automatic generation every 5 minutes.
- Includes automatic update checking, optional telemetry, and a new REST API endpoint to expose the map as base64.

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

- Every 5 minutes, the plugin automatically generates the map (you can modify the interval in the code).

### Other features

- **CheckUpdates.cs**: On startup, checks for new versions and logs to console.
- **Telemetry.cs**: Sends basic server/plugin info to an external endpoint for statistics and error reporting.
  - You can review the code to adapt or disable telemetry if you want.
- **RESTapi.cs**: Exposes REST endpoints:
  - `/map/base64`: Returns the current map PNG encoded in base64 (requires `tshock.rest.map` permission).
  - `/map/report`: Allows manual error reporting to telemetry.

## Installation (latest tShock version)

1. **Download the compiled `.dll** from [Releases](https://github.com/itsFrankV22/tShock-LiveMap_MapPNG/releases) or build it yourself.
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

## Relevant code

- [Main.cs](https://github.com/itsFrankV22/tShock-LiveMap_MapPNG/blob/main/Main.cs): Main generation and command logic.
- [CheckUpdates.cs](https://github.com/itsFrankV22/tShock-LiveMap_MapPNG/blob/main/CheckUpdates.cs): Update checks.
- [Telemetry.cs](https://github.com/itsFrankV22/tShock-LiveMap_MapPNG/blob/main/Telemetry.cs): Telemetry and error reporting.
- [RESTapi.cs](https://github.com/itsFrankV22/tShock-LiveMap_MapPNG/blob/main/RESTapi.cs): REST API for map and telemetry endpoints.

## Contributing

The code is under development. Contributions are welcome to improve color dictionaries and add new features!

---

Simple. Minimalist. Direct: visualize your Terraria world with a single command.

---

**Author:** FrankV22  
**Repository:** https://github.com/itsFrankV22/tShock-LiveMap_MapPNG

---