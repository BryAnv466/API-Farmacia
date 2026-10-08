using FarmaciaApi.Data;
using FarmaciaApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FarmaciaApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriasController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Categoria>>> GetAll() =>
        await db.Categorias.AsNoTracking().OrderBy(c => c.Nombre).ToListAsync();

    // Devuelve la categoría con sus productos
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Categoria>> Get(int id)
    {
        var c = await db.Categorias.AsNoTracking().Include(x => x.Productos)
            .FirstOrDefaultAsync(x => x.Id == id);
        return c is null ? NotFound() : c;
    }
}
