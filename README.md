# API Farmacia (.NET 8) — solo lectura

Todo el código está en `Program.cs`. Los datos viven en memoria (sin base de datos).
Todos los endpoints son GET.

| Recurso | Rutas |
|---|---|
| Categorías | GET /api/categorias, GET /api/categorias/{id} (incluye sus productos) |
| Productos | GET /api/productos?buscar=&categoriaId=&requiereReceta= , GET /api/productos/{id} |
| Clientes | GET /api/clientes, GET /api/clientes/{id} |
| Pedidos (+ detalle) | GET /api/pedidos?clienteId=&estado= , GET /api/pedidos/{id} |
| Promociones | GET /api/promociones?vigentes=true , GET /api/promociones/{id} |

## Ejecutar local
```
dotnet run
```

## Desplegar en Render
1. Sube la carpeta a un repositorio de GitHub.
2. Render > New > Web Service > conecta el repo (Runtime: Docker).
3. Deploy. Ya no se necesita base de datos ni variables de entorno.
