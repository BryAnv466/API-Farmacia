using FarmaciaApi.Models;
using Microsoft.EntityFrameworkCore;

namespace FarmaciaApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<Producto> Productos => Set<Producto>();
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Pedido> Pedidos => Set<Pedido>();
    public DbSet<DetallePedido> DetallesPedido => Set<DetallePedido>();
    public DbSet<Promocion> Promociones => Set<Promocion>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<Producto>().Property(p => p.Precio).HasPrecision(10, 2);
        b.Entity<Pedido>().Property(p => p.Total).HasPrecision(10, 2);
        b.Entity<DetallePedido>().Property(p => p.PrecioUnitario).HasPrecision(10, 2);
        b.Entity<DetallePedido>().Property(p => p.DescuentoAplicado).HasPrecision(5, 2);
        b.Entity<Promocion>().Property(p => p.PorcentajeDescuento).HasPrecision(5, 2);

        b.Entity<Producto>().HasOne(p => p.Categoria).WithMany(c => c.Productos)
            .HasForeignKey(p => p.CategoriaId).OnDelete(DeleteBehavior.Restrict);
        b.Entity<Pedido>().HasOne(p => p.Cliente).WithMany()
            .HasForeignKey(p => p.ClienteId).OnDelete(DeleteBehavior.Restrict);
        b.Entity<DetallePedido>().HasOne<Pedido>().WithMany(p => p.Detalles)
            .HasForeignKey(d => d.PedidoId).OnDelete(DeleteBehavior.Cascade);
        b.Entity<DetallePedido>().HasOne(d => d.Producto).WithMany()
            .HasForeignKey(d => d.ProductoId).OnDelete(DeleteBehavior.Restrict);
        b.Entity<Promocion>().HasOne(p => p.Producto).WithMany()
            .HasForeignKey(p => p.ProductoId).OnDelete(DeleteBehavior.SetNull);

        // ----- Datos iniciales -----
        b.Entity<Categoria>().HasData(
            new Categoria { Id = 1, Nombre = "Analgésicos", Descripcion = "Alivio del dolor y la fiebre" },
            new Categoria { Id = 2, Nombre = "Antibióticos", Descripcion = "Requieren receta médica" },
            new Categoria { Id = 3, Nombre = "Vitaminas y suplementos", Descripcion = "Complementos nutricionales" },
            new Categoria { Id = 4, Nombre = "Cuidado personal", Descripcion = "Higiene y cuidado diario" },
            new Categoria { Id = 5, Nombre = "Antialérgicos", Descripcion = "Control de alergias y rinitis" });

        b.Entity<Producto>().HasData(
            new Producto { Id = 1, Nombre = "Paracetamol 500 mg x 100", Descripcion = "Analgésico y antipirético", Precio = 12.50m, Stock = 200, RequiereReceta = false, FechaVencimiento = new DateOnly(2028, 3, 31), CategoriaId = 1 },
            new Producto { Id = 2, Nombre = "Ibuprofeno 400 mg x 30", Descripcion = "Antiinflamatorio", Precio = 9.90m, Stock = 150, RequiereReceta = false, FechaVencimiento = new DateOnly(2028, 6, 30), CategoriaId = 1 },
            new Producto { Id = 3, Nombre = "Amoxicilina 500 mg x 21", Descripcion = "Antibiótico de amplio espectro", Precio = 24.00m, Stock = 80, RequiereReceta = true, FechaVencimiento = new DateOnly(2027, 11, 30), CategoriaId = 2 },
            new Producto { Id = 4, Nombre = "Vitamina C 1000 mg x 30", Descripcion = "Refuerzo del sistema inmune", Precio = 18.00m, Stock = 120, RequiereReceta = false, FechaVencimiento = new DateOnly(2028, 1, 31), CategoriaId = 3 },
            new Producto { Id = 5, Nombre = "Alcohol en gel 250 ml", Descripcion = "Desinfectante de manos", Precio = 7.50m, Stock = 300, RequiereReceta = false, FechaVencimiento = new DateOnly(2028, 9, 30), CategoriaId = 4 },
            new Producto { Id = 6, Nombre = "Loratadina 10 mg x 10", Descripcion = "Antihistamínico para alergias", Precio = 6.80m, Stock = 90, RequiereReceta = false, FechaVencimiento = new DateOnly(2028, 2, 28), CategoriaId = 5 },
            new Producto { Id = 7, Nombre = "Protector solar FPS 50", Descripcion = "Protección solar facial y corporal", Precio = 35.00m, Stock = 60, RequiereReceta = false, FechaVencimiento = new DateOnly(2028, 12, 31), CategoriaId = 4 });

        b.Entity<Cliente>().HasData(
            new Cliente { Id = 1, Nombre = "María", Apellido = "Quispe", Dni = "12345678", Telefono = "987654321", Email = "maria@example.com" },
            new Cliente { Id = 2, Nombre = "Luis", Apellido = "Mendoza", Dni = "23456789", Telefono = "976543210", Email = "luis@example.com" },
            new Cliente { Id = 3, Nombre = "Ana", Apellido = "Torres", Dni = "34567890", Telefono = "965432109", Email = "ana@example.com" });

        b.Entity<Promocion>().HasData(
            new Promocion { Id = 1, Nombre = "10% en Vitamina C", Descripcion = "Promoción de temporada", PorcentajeDescuento = 10, FechaInicio = new DateOnly(2026, 1, 1), FechaFin = new DateOnly(2026, 12, 31), Activa = true, ProductoId = 4 },
            new Promocion { Id = 2, Nombre = "15% en Alcohol en gel", Descripcion = "Cuidemos nuestra salud", PorcentajeDescuento = 15, FechaInicio = new DateOnly(2026, 1, 1), FechaFin = new DateOnly(2026, 12, 31), Activa = true, ProductoId = 5 },
            new Promocion { Id = 3, Nombre = "5% de descuento en toda la tienda", Descripcion = "Promoción general de aniversario", PorcentajeDescuento = 5, FechaInicio = new DateOnly(2026, 10, 1), FechaFin = new DateOnly(2026, 10, 31), Activa = true, ProductoId = null });

        b.Entity<Pedido>().HasData(
            new Pedido { Id = 1, ClienteId = 1, Fecha = new DateTime(2026, 9, 15, 15, 30, 0, DateTimeKind.Utc), Estado = "Entregado", Total = 41.20m },
            new Pedido { Id = 2, ClienteId = 2, Fecha = new DateTime(2026, 9, 28, 18, 10, 0, DateTimeKind.Utc), Estado = "Pagado", Total = 46.50m },
            new Pedido { Id = 3, ClienteId = 3, Fecha = new DateTime(2026, 10, 5, 14, 45, 0, DateTimeKind.Utc), Estado = "Pendiente", Total = 19.80m });

        b.Entity<DetallePedido>().HasData(
            new DetallePedido { Id = 1, PedidoId = 1, ProductoId = 1, Cantidad = 2, PrecioUnitario = 12.50m, DescuentoAplicado = 0 },
            new DetallePedido { Id = 2, PedidoId = 1, ProductoId = 4, Cantidad = 1, PrecioUnitario = 18.00m, DescuentoAplicado = 10 },
            new DetallePedido { Id = 3, PedidoId = 2, ProductoId = 3, Cantidad = 1, PrecioUnitario = 24.00m, DescuentoAplicado = 0 },
            new DetallePedido { Id = 4, PedidoId = 2, ProductoId = 5, Cantidad = 3, PrecioUnitario = 7.50m, DescuentoAplicado = 0 },
            new DetallePedido { Id = 5, PedidoId = 3, ProductoId = 2, Cantidad = 2, PrecioUnitario = 9.90m, DescuentoAplicado = 0 });
    }
}
