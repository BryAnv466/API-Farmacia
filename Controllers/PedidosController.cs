using FarmaciaApi.Data;
using FarmaciaApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FarmaciaApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PedidosController(AppDbContext db) : ControllerBase
{
    // GET api/pedidos?clienteId=1&estado=Pagado
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Pedido>>> GetAll(int? clienteId, string? estado)
    {
        var q = db.Pedidos.Include(p => p.Cliente)
            .Include(p => p.Detalles).ThenInclude(d => d.Producto)
            .AsNoTracking().AsQueryable();
        if (clienteId.HasValue) q = q.Where(p => p.ClienteId == clienteId);
        if (!string.IsNullOrWhiteSpace(estado)) q = q.Where(p => p.Estado == estado);
        return await q.OrderByDescending(p => p.Fecha).ToListAsync();
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Pedido>> Get(int id)
    {
        var p = await db.Pedidos.Include(x => x.Cliente)
            .Include(x => x.Detalles).ThenInclude(d => d.Producto)
            .AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        return p is null ? NotFound() : p;
    }
}
