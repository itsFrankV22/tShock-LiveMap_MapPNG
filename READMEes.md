# tShock-LiveMap_MapPNG

English? [README.md](README.md).

Genera un mapa en PNG de tu mundo Terraria usando tShock: una visualización sencilla y directa del mundo de tu servidor.

## ¿Qué hace este plugin?

- Permite generar una imagen PNG del mapa del mundo actual del servidor.
- El comando `/genmap` crea el archivo en `/Map/worldMap.png`.
- El mapa muestra tiles, muros y líquidos usando colores según tipo, pintura y altura.
- Soporta generación manual (comando) y automática cada 5 minutos (configurable).
- **NUEVO:** Añade un servidor WebSocket para ver el mapa en tiempo real y recibir chunks.
- Incluye comprobación automática de actualizaciones, telemetría opcional y REST API para exponer el mapa en base64.
- Archivo de configuración en `tshock/livemap/config.json`.

## ¿Cómo funciona?

### Comando principal

- `/genmap`: Genera el mapa PNG y avisa al usuario con la ruta del archivo.

### Lógica de generación

1. Obtiene el tamaño del mundo (`Main.maxTilesX`, `Main.maxTilesY`).
2. Crea un nuevo Bitmap con esas dimensiones.
3. Recorre cada tile del mundo:
   - Si hay tile, obtiene su color según tipo y pintura.
   - Si hay pared, obtiene su color.
   - Si hay líquido, usa color especial.
   - Si no, aplica degradado según la altura.
4. Guarda la imagen como `worldMap.png` en la carpeta `/Map`.

### Automatización

- Por defecto, cada 5 minutos (configurable en `tshock/livemap/config.json`), el plugin genera el mapa automáticamente.

### Otras funciones

- **WebSocket:** Mapa en tiempo real en `ws://<host>:<puerto>/map/ws/`. Ver `browserConsoleTest.md` para ejemplo de frontend.
- **CheckUpdates.cs:** Al iniciar verifica si hay versiones nuevas y avisa en la consola.
- **Telemetry.cs:** Envía información básica del servidor/plugin a un endpoint externo para estadísticas y reporte de errores (opcional, revisa el código para adaptar/desactivar).
- **RESTapi.cs:** Expone endpoints REST:
  - `/map/base64`: Devuelve el mapa PNG actual codificado en base64 (requiere permiso `tshock.rest.map`).
  - `/map/report`: Permite reportar errores manualmente a la telemetría.

## Instalación (última versión de tShock)

1. **Descarga el `.dll` compilado** desde [Releases](https://github.com/itsFrankV22/tShock-LiveMap_MapPNG/releases) o compílalo tú mismo.
2. **Coloca el `.dll`** en la carpeta `/ServerPlugins` de tu servidor tShock.
3. **Reinicia el servidor**.
4. Usa el comando `/genmap` en el chat del servidor (requiere permiso `map.export`).

## Permisos

- El comando `/genmap` requiere permiso `map.export`.
- El endpoint REST `/map/base64` requiere permiso `tshock.rest.map`.
- El endpoint `/map/report` requiere permiso `tshock.rest.admin`.

## Ejemplo de uso

```bash
/genmap
```
El mapa se guardará como `/Map/worldMap.png` en la raíz del servidor.

## Ejemplo WebSocket

Ver [browserConsoleTest.md](browserConsoleTest.md) para un cliente JavaScript que dibuja el mapa en tiempo real en el navegador.

## Código relevante

- [Main.cs](https://github.com/itsFrankV22/tShock-LiveMap_MapPNG/blob/main/Main.cs): Lógica principal del plugin e inicialización.
- [Export/MapExporter.cs](https://github.com/itsFrankV22/tShock-LiveMap_MapPNG/blob/main/Export/MapExporter.cs): Lógica de generación y exportación del mapa.
- [Colors/MapColors.cs](https://github.com/itsFrankV22/tShock-LiveMap_MapPNG/blob/main/Colors/MapColors.cs): Lógica de colores de tiles y muros.
- [Colors/MapColorHelper.cs](https://github.com/itsFrankV22/tShock-LiveMap_MapPNG/blob/main/Colors/MapColorHelper.cs): Gradientes, colores de líquidos.
- [Extras/CheckUpdates.cs](https://github.com/itsFrankV22/tShock-LiveMap_MapPNG/blob/main/Extras/CheckUpdates.cs): Comprobación de actualizaciones.
- [Extras/Telemetry.cs](https://github.com/itsFrankV22/tShock-LiveMap_MapPNG/blob/main/Extras/Telemetry.cs): Telemetría y reportes.
- [REST/RESTapi.cs](https://github.com/itsFrankV22/tShock-LiveMap_MapPNG/blob/main/REST/RESTapi.cs): API REST para mapa y telemetría.
- [WebSocket/MapWebSocket.cs](https://github.com/itsFrankV22/tShock-LiveMap_MapPNG/blob/main/WebSocket/MapWebSocket.cs): Servidor WebSocket y lógica.
- [Config/Config.cs](https://github.com/itsFrankV22/tShock-LiveMap_MapPNG/blob/main/Config/Config.cs): Carga de configuración.

## Aportaciones

El código está en desarrollo. ¡Se aceptan contribuciones para mejorar los diccionarios de colores, nuevas funciones o visores frontend!

---

Simple. Minimalista. Directo para visualizar tu mundo de Terraria con un solo comando... ¡o en streaming!

---

**Autor:** FrankV22  
**Repositorio:** https://github.com/itsFrankV22/tShock-LiveMap_MapPNG

---