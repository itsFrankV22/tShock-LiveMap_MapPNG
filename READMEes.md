# tShock-LiveMap_MapPNG

English? [README.md](README.md).

Genera un mapa en PNG de tu mundo Terraria usando tShock: una visualización sencilla y directa del mundo de tu servidor.

## ¿Qué hace este plugin?

- Permite generar una imagen PNG del mapa del mundo actual del servidor.
- El comando `/genmap` crea el archivo en `/Map/worldMap.png`.
- El mapa muestra tiles, muros y líquidos usando colores según tipo, pintura y altura.
- Soporta generación manual (comando) y automática cada 5 minutos.
- Incluye comprobación automática de actualizaciones y telemetría opcional.

## ¿Cómo funciona?

EJEMPLOS:
> Tiene degradado en el Fondo  
![worldMap](https://github.com/user-attachments/assets/6e5b5598-5813-45c3-99c8-d5b187888ad1)

### Comando principal

- `/genmap`: Genera el mapa PNG y avisa al usuario con la ruta del archivo.

### Lógica de generación

1. Obtiene el tamaño del mundo (`Main.maxTilesX`, `Main.maxTilesY`).
2. Crea un nuevo Bitmap con esas dimensiones.
3. Recorre cada tile del mundo:
   - Si hay tile, obtiene su color según tipo y pintura.
   - Si hay pared, obtiene su color.
   - Si hay líquido, usa color especial.
   - Si no, aplica un degradado según la altura.
4. Guarda la imagen como `worldMap.png` en la carpeta `/Map`.

### Automatización

- Cada 5 minutos el plugin genera el mapa automáticamente (puedes modificar el intervalo en el código).

### Otras funciones

- **CheckUpdates.cs**: Al iniciar verifica si hay versiones nuevas y avisa en la consola.
- **Telemetry.cs**: Envía información básica del servidor/plugin a un endpoint externo para estadísticas y reporte de errores.
  - Puedes revisar el código para adaptar o desactivar la telemetría si lo deseas.

## Instalación (última versión de tShock)

1. **Descarga el `.dll` compilado** desde [Releases](https://github.com/itsFrankV22/tShock-LiveMap_MapPNG/releases) o compílalo tú mismo.
2. **Coloca el `.dll`** en la carpeta `/ServerPlugins` de tu servidor tShock.
3. **Reinicia el servidor**.
4. Usa el comando `/genmap` en el chat del servidor (requiere permiso `map.export`).

## Permisos

- El comando `/genmap` requiere permiso `map.export`.

## Ejemplo de uso

```bash
/genmap
```
El mapa se guardará como `/Map/worldMap.png` en la raíz del servidor.

## Código relevante

- [Main.cs](https://github.com/itsFrankV22/tShock-LiveMap_MapPNG/blob/main/Main.cs): Lógica principal de generación y comandos.
- [CheckUpdates.cs](https://github.com/itsFrankV22/tShock-LiveMap_MapPNG/blob/main/CheckUpdates.cs): Verificación de actualizaciones.
- [Telemetry.cs](https://github.com/itsFrankV22/tShock-LiveMap_MapPNG/blob/main/Telemetry.cs): Telemetría y reportes automáticos.

## Aportaciones

El código está en desarrollo. ¡Se aceptan contribuciones para mejorar los diccionarios de colores y nuevas funciones!

---

Simple. Minimalista. Directo para visualizar tu mundo de Terraria con un solo comando.

---

**Autor:** FrankV22  
**Repositorio:** https://github.com/itsFrankV22/tShock-LiveMap_MapPNG

---
