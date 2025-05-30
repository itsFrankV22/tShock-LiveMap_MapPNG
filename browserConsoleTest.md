# WebSocket Client Example to Visualize the Real-Time Map

This JavaScript script lets you display your Terraria server map in real time, using the WebSocket provided by the plugin. You can run it directly from your browser console or use it as a starting point for your own web viewer.

## How to use this script?

1. Make sure your server is running and the WebSocket is enabled (default port: 8585).
2. Open any modern browser and press F12 to open the developer console.
3. Copy and paste the script below into the console and run it.
4. You should see a canvas drawing the map and connection logs in your console.

**You can change the `IP` variable to match your server's IP/port.**

---

```js
// --- LiveMap WebSocket Client Demo ---
// Author: FrankV22

(() => {
    // Set your WebSocket host/port here:
    let IP = 'ws://localhost:8585/map/ws/';

    // Chunk size (the server may adjust this, autodetected)
    let CHUNK_SIZE = 16;
    let TILE_WIDTH = 16 * 52, TILE_HEIGHT = 16 * 15; // Default values (server will send the real ones)

    // Create canvas and add to document
    let canvas = document.createElement('canvas');
    canvas.width = TILE_WIDTH; canvas.height = TILE_HEIGHT;
    canvas.style.border = "1px solid black";
    document.body.appendChild(canvas);

    // Context and color buffer
    let ctx = canvas.getContext('2d');
    let tileColorData = Array.from({length: TILE_HEIGHT}, () => Array(TILE_WIDTH).fill('#000000'));
    let initializedCanvas = false;

    function drawChunk(chunk) {
        let {chunkX, chunkY, width, height, colors, mapWidth, mapHeight} = chunk;
        // If we get new map sizes, adapt canvas and buffer
        if (!initializedCanvas && mapWidth && mapHeight) {
            TILE_WIDTH = mapWidth; TILE_HEIGHT = mapHeight;
            canvas.width = TILE_WIDTH; canvas.height = TILE_HEIGHT;
            tileColorData = Array.from({length: TILE_HEIGHT}, () => Array(TILE_WIDTH).fill('#000000'));
            initializedCanvas = true;
        }
        for (let y = 0; y < height; y++) {
            for (let x = 0; x < width; x++) {
                let color = colors[y * width + x];
                let px = chunkX * CHUNK_SIZE + x, py = chunkY * CHUNK_SIZE + y;
                if (px < TILE_WIDTH && py < TILE_HEIGHT) {
                    tileColorData[py][px] = color;
                    ctx.fillStyle = color;
                    ctx.fillRect(px, py, 1, 1);
                }
            }
        }
    }

    // Global function to redraw the full map from buffer (for testing)
    window.redrawMap = function() {
        for (let py = 0; py < TILE_HEIGHT; py++) {
            for (let px = 0; px < TILE_WIDTH; px++) {
                ctx.fillStyle = tileColorData[py][px];
                ctx.fillRect(px, py, 1, 1);
            }
        }
    };

    // Auto-reconnect WebSocket logic
    let ws;
    function connectWS() {
        ws = new WebSocket(IP);
        ws.onopen = () => console.log("WebSocket connected to " + IP);
        ws.onclose = () => {
            console.warn("WebSocket closed, reconnecting in 5s...");
            setTimeout(connectWS, 5000);
        };
        ws.onerror = err => console.error("WebSocket error", err);
        ws.onmessage = evt => {
            try {
                let data = JSON.parse(evt.data);
                if (data.type === "chunk_update") {
                    if (data.width && data.width !== CHUNK_SIZE) CHUNK_SIZE = data.width;
                    drawChunk(data);
                }
            } catch (e) {
                console.error("Error processing message", e, evt.data);
            }
        };
    }

    connectWS();
    console.log("Canvas and WebSocket ready! Waiting for chunks...");
})();
```

---

### Notes

- You can adapt the canvas for zoom, panning, overlays, etc.
- To clear/redraw the map from buffer, run in console: `redrawMap();`
- To use a different host, just change the line `let IP = ...` to your server's host and port.

---

Want a more advanced web map viewer? Fork and contribute!