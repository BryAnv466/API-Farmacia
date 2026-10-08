using FarmaciaApi.Data;
using FarmaciaApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FarmaciaApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientesController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Cliente>>> GetAll() =>
        await db.Clientes.AsNoTracking().OrderBy(c => c.Apellido).ToListAsync();

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Cliente>> Get(int id)
    {
        var c = await db.Clientes.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        return c is null ? NotFound() : c;
    }
}
