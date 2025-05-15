using Microsoft.AspNetCore.Mvc;
using ms_productos_service.Data;
using ms_productos_service.Models;

namespace ms_productos_service.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductosController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProductosController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Producto>> Get()
    {
        return _context.Productos.ToList();
    }
[HttpGet("{id}")]
    public ActionResult<Producto> Get(int id)
    {
        var producto = _context.Productos.Find(id);

        if (producto == null)
        {
            return NotFound();
        }

        return Ok(producto);
    }
}