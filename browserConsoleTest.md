```ConsoleBrowser
// Executing (about:blank)
(() => {
    let CHUNK_SIZE = 16, TILE_WIDTH = 16 * 52, TILE_HEIGHT = 16 * 15;
    let canvas = document.createElement('canvas');
    let IP = 'ws://localhost:8585/map/ws/'
    canvas.width = TILE_WIDTH; canvas.height = TILE_HEIGHT;
    canvas.style.border = "1px solid black";
    document.body.appendChild(canvas);
    let ctx = canvas.getContext('2d');
    let tileColorData = Array.from({length: TILE_HEIGHT}, () => Array(TILE_WIDTH).fill('#000000'));
    let initializedCanvas = false;

    function drawChunk(chunk) {
        let {chunkX, chunkY, width, height, colors, mapWidth, mapHeight} = chunk;
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

    window.redibujarMapa = function() {
        for (let py = 0; py < TILE_HEIGHT; py++) {
            for (let px = 0; px < TILE_WIDTH; px++) {
                ctx.fillStyle = tileColorData[py][px];
                ctx.fillRect(px, py, 1, 1);
            }
        }
    };

    let ws = new WebSocket(IP);
    ws.onopen = () => console.log("WebSocket conectado");
    ws.onclose = () => console.log("WebSocket cerrado");
    ws.onerror = err => console.error("WebSocket error", err);
    ws.onmessage = evt => {
        try {
            let data = JSON.parse(evt.data);
            if (data.type === "chunk_update") {
                if (data.width && data.width !== CHUNK_SIZE) CHUNK_SIZE = data.width;
                drawChunk(data);
            }
        } catch (e) { console.error("Error al procesar mensaje", e, evt.data); }
    };
    console.log("¡Canvas y WebSocket listos! Esperando chunks...");
})();
```