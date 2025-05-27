# tShock-LiveMap_MapPNG

¿Español? [READMEes.md](READMEes.md).

Generate a PNG map of your Terraria world using tShock – a simple and direct visualization of your server world.

## What does this plugin do?

- Generates a PNG image of your current server world map.
- The `/genmap` command creates the file at `/Map/worldMap.png`.
- The map visualizes tiles, walls, and liquids using colors based on their type, paint, and height.
- Supports manual (command) and automatic generation every 5 minutes.
- Includes automatic update checking and optional telemetry.

## How does it work?

EXAMPLES:  
> Has background gradient  
![worldMap](https://github.com/user-attachments/assets/6e5b5598-5813-45c3-99c8-d5b187888ad1)

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

## Installation (latest tShock version)

1. **Download the compiled `.dll`** from [Releases](https://github.com/itsFrankV22/tShock-LiveMap_MapPNG/releases) or build it yourself.
2. **Place the `.dll`** in your tShock server’s `/ServerPlugins` folder.
3. **Restart the server**.
4. Use the `/genmap` command in the server chat (requires the `map.export` permission).

## Permissions

- The `/genmap` command requires the `map.export` permission.

## Example usage

```bash
/genmap
```
The map will be saved as `/Map/worldMap.png` in the server root.

## Relevant code

- [Main.cs](https://github.com/itsFrankV22/tShock-LiveMap_MapPNG/blob/main/Main.cs): Main logic for generation and commands.
- [CheckUpdates.cs](https://github.com/itsFrankV22/tShock-LiveMap_MapPNG/blob/main/CheckUpdates.cs): Update checking.
- [Telemetry.cs](https://github.com/itsFrankV22/tShock-LiveMap_MapPNG/blob/main/Telemetry.cs): Telemetry and automatic reporting.

## Contributing

The code is in development. Contributions to improve color dictionaries and add new features are welcome!

---

Simple. Minimalist. Instantly visualize your Terraria world with a single command.

---

**Author:** FrankV22  
**Repo:** https://github.com/itsFrankV22/tShock-LiveMap_MapPNG

---
