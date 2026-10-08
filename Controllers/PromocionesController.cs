using FarmaciaApi.Data;
using FarmaciaApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FarmaciaApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PromocionesController(AppDbContext db) : ControllerBase
{
    // GET api/promociones?vigentes=true  -> solo las activas y dentro de fecha
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Promocion>>> GetAll(bool? vigentes)
    {
        var q = db.Promociones.Include(p => p.Producto).AsNoTracking().AsQueryable();
        if (vigentes == true)
        {
            var hoy = DateOnly.FromDateTime(DateTime.UtcNow);
            q = q.Where(p => p.Activa && p.FechaInicio <= hoy && p.FechaFin >= hoy);
        }
        return await q.OrderBy(p => p.FechaFin).ToListAsync();
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Promocion>> Get(int id)
    {
        var p = await db.Promociones.Include(x => x.Producto).AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
        return p is null ? NotFound() : p;
    }
}
