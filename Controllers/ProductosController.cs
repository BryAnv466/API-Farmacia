using FarmaciaApi.Data;
using FarmaciaApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FarmaciaApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductosController(AppDbContext db) : ControllerBase
{
    // GET api/productos?buscar=para&categoriaId=1&requiereReceta=false
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Producto>>> GetAll(
        string? buscar, int? categoriaId, bool? requiereReceta)
    {
        var q = db.Productos.Include(p => p.Categoria).AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(buscar))
            q = q.Where(p => EF.Functions.ILike(p.Nombre, $"%{buscar}%"));
        if (categoriaId.HasValue) q = q.Where(p => p.CategoriaId == categoriaId);
        if (requiereReceta.HasValue) q = q.Where(p => p.RequiereReceta == requiereReceta);
        return await q.OrderBy(p => p.Nombre).ToListAsync();
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Producto>> Get(int id)
    {
        var p = await db.Productos.Include(x => x.Categoria).AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
        return p is null ? NotFound() : p;
    }
}
