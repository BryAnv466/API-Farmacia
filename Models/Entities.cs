namespace FarmaciaApi.Models;

public class Categoria
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public string? Descripcion { get; set; }
    public List<Producto> Productos { get; set; } = new();
}

public class Producto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public string? Descripcion { get; set; }
    public decimal Precio { get; set; }
    public int Stock { get; set; }
    public bool RequiereReceta { get; set; }
    public DateOnly? FechaVencimiento { get; set; }
    public string? ImagenUrl { get; set; }
    public int CategoriaId { get; set; }
    public Categoria? Categoria { get; set; }
}

public class Cliente
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public string Apellido { get; set; } = "";
    public string? Dni { get; set; }
    public string? Telefono { get; set; }
    public string? Email { get; set; }
}

public class Pedido
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; }
    public string Estado { get; set; } = "Pendiente"; // Pendiente, Pagado, Entregado, Cancelado
    public decimal Total { get; set; }
    public int ClienteId { get; set; }
    public Cliente? Cliente { get; set; }
    public List<DetallePedido> Detalles { get; set; } = new();
}

public class DetallePedido
{
    public int Id { get; set; }
    public int PedidoId { get; set; }
    public int ProductoId { get; set; }
    public Producto? Producto { get; set; }
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal DescuentoAplicado { get; set; }
}

public class Promocion
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public string? Descripcion { get; set; }
    public decimal PorcentajeDescuento { get; set; }
    public DateOnly FechaInicio { get; set; }
    public DateOnly FechaFin { get; set; }
    public bool Activa { get; set; } = true;
    public int? ProductoId { get; set; }   // null = promoción general
    public Producto? Producto { get; set; }
}
