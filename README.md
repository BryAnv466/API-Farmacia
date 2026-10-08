# API Farmacia (.NET 8 + PostgreSQL) — solo lectura

Todos los endpoints son GET.

| Recurso | Rutas |
|---|---|
| Categorías | GET /api/categorias, GET /api/categorias/{id} (incluye sus productos) |
| Productos (principal) | GET /api/productos?buscar=&categoriaId=&requiereReceta= , GET /api/productos/{id} |
| Clientes | GET /api/clientes, GET /api/clientes/{id} |
| Pedidos (+ detalle) | GET /api/pedidos?clienteId=&estado= , GET /api/pedidos/{id} |
| Promociones | GET /api/promociones?vigentes=true , GET /api/promociones/{id} |

Swagger en `/swagger`. Los datos de ejemplo se cargan automáticamente al iniciar.

## Ejecutar local
1. Tener PostgreSQL y ajustar `ConnectionStrings:Default` en appsettings.json
2. `dotnet run`

## Desplegar en Render
1. Sube la carpeta a un repositorio de GitHub.
2. Render > New > PostgreSQL (plan Free). Copia la *Internal Database URL*.
3. Render > New > Web Service > conecta el repo (Runtime: Docker).
4. En Environment agrega `DATABASE_URL` = la Internal Database URL.
5. Deploy. Tu API quedará en https://tu-servicio.onrender.com/swagger
