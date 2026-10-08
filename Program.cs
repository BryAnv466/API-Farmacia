var builder = WebApplication.CreateBuilder(args);

// CORS: permite que la página web consuma la API (solo GET)
builder.Services.AddCors(o => o.AddDefaultPolicy(p => p.AllowAnyOrigin().AllowAnyHeader().WithMethods("GET")));

var app = builder.Build();
app.UseCors();

// ============================================================
//  DATOS (en memoria)
// ============================================================
var categorias = new List<Categoria>
{
    new(1, "Analgésicos", "Alivio del dolor y la fiebre"),
    new(2, "Antibióticos", "Requieren receta médica"),
    new(3, "Vitaminas y suplementos", "Complementos nutricionales"),
    new(4, "Cuidado personal", "Higiene y cuidado diario"),
    new(5, "Antialérgicos", "Control de alergias y rinitis"),
};

var productos = new List<Producto>
{
    new(1, "Paracetamol 500 mg x 100", "Analgésico y antipirético", 12.50m, 200, false, new DateOnly(2028, 3, 31), null, 1),
    new(2, "Ibuprofeno 400 mg x 30", "Antiinflamatorio", 9.90m, 150, false, new DateOnly(2028, 6, 30), null, 1),
    new(3, "Amoxicilina 500 mg x 21", "Antibiótico de amplio espectro", 24.00m, 80, true, new DateOnly(2027, 11, 30), null, 2),
    new(4, "Vitamina C 1000 mg x 30", "Refuerzo del sistema inmune", 18.00m, 120, false, new DateOnly(2028, 1, 31), null, 3),
    new(5, "Alcohol en gel 250 ml", "Desinfectante de manos", 7.50m, 300, false, new DateOnly(2028, 9, 30), null, 4),
    new(6, "Loratadina 10 mg x 10", "Antihistamínico para alergias", 6.80m, 90, false, new DateOnly(2028, 2, 28), null, 5),
    new(7, "Protector solar FPS 50", "Protección solar facial y corporal", 35.00m, 60, false, new DateOnly(2028, 12, 31), null, 4),
};

var clientes = new List<Cliente>
{
    new(1, "María", "Quispe", "12345678", "987654321", "maria@example.com"),
    new(2, "Luis", "Mendoza", "23456789", "976543210", "luis@example.com"),
    new(3, "Ana", "Torres", "34567890", "965432109", "ana@example.com"),
};

var promociones = new List<Promocion>
{
    new(1, "10% en Vitamina C", "Promoción de temporada", 10, new DateOnly(2026, 1, 1), new DateOnly(2026, 12, 31), true, 4),
    new(2, "15% en Alcohol en gel", "Cuidemos nuestra salud", 15, new DateOnly(2026, 1, 1), new DateOnly(2026, 12, 31), true, 5),
    new(3, "5% de descuento en toda la tienda", "Promoción general de aniversario", 5, new DateOnly(2026, 10, 1), new DateOnly(2026, 10, 31), true, null),
};

var pedidos = new List<Pedido>
{
    new(1, new DateTime(2026, 9, 15, 15, 30, 0, DateTimeKind.Utc), "Entregado", 41.20m, 1),
    new(2, new DateTime(2026, 9, 28, 18, 10, 0, DateTimeKind.Utc), "Pagado", 46.50m, 2),
    new(3, new DateTime(2026, 10, 5, 14, 45, 0, DateTimeKind.Utc), "Pendiente", 19.80m, 3),
};

var detalles = new List<DetallePedido>
{
    new(1, 1, 1, 2, 12.50m, 0),
    new(2, 1, 4, 1, 18.00m, 10),
    new(3, 2, 3, 1, 24.00m, 0),
    new(4, 2, 5, 3, 7.50m, 0),
    new(5, 3, 2, 2, 9.90m, 0),
};

// ============================================================
//  Funciones auxiliares: arman cada objeto con sus relaciones
// ============================================================
Producto ConCategoria(Producto p) =>
    p with { Categoria = categorias.FirstOrDefault(c => c.Id == p.CategoriaId) };

Promocion ConProducto(Promocion p) =>
    p with { Producto = productos.FirstOrDefault(x => x.Id == p.ProductoId) };

Pedido ConDetalle(Pedido p) => p with
{
    Cliente = clientes.FirstOrDefault(c => c.Id == p.ClienteId),
    Detalles = detalles.Where(d => d.PedidoId == p.Id)
        .Select(d => d with { Producto = productos.FirstOrDefault(x => x.Id == d.ProductoId) })
        .ToList()
};

// ============================================================
//  ENDPOINTS (todos GET)
// ============================================================
app.MapGet("/", () => Results.Ok(new
{
    api = "API Farmacia (solo lectura)",
    endpoints = new[]
    {
        "/api/categorias", "/api/productos", "/api/clientes", "/api/pedidos", "/api/promociones"
    }
}));

// ----- Categorías -----
app.MapGet("/api/categorias", () => categorias.OrderBy(c => c.Nombre));

// Devuelve la categoría con sus productos
app.MapGet("/api/categorias/{id:int}", (int id) =>
    categorias.FirstOrDefault(x => x.Id == id) is { } c
        ? Results.Ok(c with { Productos = productos.Where(p => p.CategoriaId == id).ToList() })
        : Results.NotFound());

// ----- Productos -----
// GET /api/productos?buscar=para&categoriaId=1&requiereReceta=false
app.MapGet("/api/productos", (string? buscar, int? categoriaId, bool? requiereReceta) =>
{
    var q = productos.AsEnumerable();
    if (!string.IsNullOrWhiteSpace(buscar))
        q = q.Where(p => p.Nombre.Contains(buscar, StringComparison.OrdinalIgnoreCase));
    if (categoriaId.HasValue) q = q.Where(p => p.CategoriaId == categoriaId);
    if (requiereReceta.HasValue) q = q.Where(p => p.RequiereReceta == requiereReceta);
    return q.OrderBy(p => p.Nombre).Select(ConCategoria);
});

app.MapGet("/api/productos/{id:int}", (int id) =>
    productos.FirstOrDefault(x => x.Id == id) is { } p
        ? Results.Ok(ConCategoria(p))
        : Results.NotFound());

// ----- Clientes -----
app.MapGet("/api/clientes", () => clientes.OrderBy(c => c.Apellido));

app.MapGet("/api/clientes/{id:int}", (int id) =>
    clientes.FirstOrDefault(x => x.Id == id) is { } c ? Results.Ok(c) : Results.NotFound());

// ----- Pedidos -----
// GET /api/pedidos?clienteId=1&estado=Pagado
app.MapGet("/api/pedidos", (int? clienteId, string? estado) =>
{
    var q = pedidos.AsEnumerable();
    if (clienteId.HasValue) q = q.Where(p => p.ClienteId == clienteId);
    if (!string.IsNullOrWhiteSpace(estado))
        q = q.Where(p => p.Estado.Equals(estado, StringComparison.OrdinalIgnoreCase));
    return q.OrderByDescending(p => p.Fecha).Select(ConDetalle);
});

app.MapGet("/api/pedidos/{id:int}", (int id) =>
    pedidos.FirstOrDefault(x => x.Id == id) is { } p ? Results.Ok(ConDetalle(p)) : Results.NotFound());

// ----- Promociones -----
// GET /api/promociones?vigentes=true  -> solo las activas y dentro de fecha
app.MapGet("/api/promociones", (bool? vigentes) =>
{
    var q = promociones.AsEnumerable();
    if (vigentes == true)
    {
        var hoy = DateOnly.FromDateTime(DateTime.UtcNow);
        q = q.Where(p => p.Activa && p.FechaInicio <= hoy && p.FechaFin >= hoy);
    }
    return q.OrderBy(p => p.FechaFin).Select(ConProducto);
});

app.MapGet("/api/promociones/{id:int}", (int id) =>
    promociones.FirstOrDefault(x => x.Id == id) is { } p ? Results.Ok(ConProducto(p)) : Results.NotFound());

app.Run();

// ============================================================
//  MODELOS
// ============================================================
record Categoria(int Id, string Nombre, string? Descripcion)
{
    public List<Producto>? Productos { get; init; }
}

record Producto(int Id, string Nombre, string? Descripcion, decimal Precio, int Stock,
                bool RequiereReceta, DateOnly? FechaVencimiento, string? ImagenUrl, int CategoriaId)
{
    public Categoria? Categoria { get; init; }
}

record Cliente(int Id, string Nombre, string Apellido, string? Dni, string? Telefono, string? Email);

record Pedido(int Id, DateTime Fecha, string Estado, decimal Total, int ClienteId)
{
    public Cliente? Cliente { get; init; }
    public List<DetallePedido> Detalles { get; init; } = new();
}

record DetallePedido(int Id, int PedidoId, int ProductoId, int Cantidad,
                     decimal PrecioUnitario, decimal DescuentoAplicado)
{
    public Producto? Producto { get; init; }
}

record Promocion(int Id, string Nombre, string? Descripcion, decimal PorcentajeDescuento,
                 DateOnly FechaInicio, DateOnly FechaFin, bool Activa, int? ProductoId)
{
    public Producto? Producto { get; init; }
}
