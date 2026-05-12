using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APIEditorialUCR.Model; 
using Sistema_Editorial.Model; 
namespace APIEditorialUCR.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DireccionAutorController : ControllerBase
    {
        private readonly DbContextEditorial _context;

        public DireccionAutorController(DbContextEditorial context)
        {
            _context = context;
        }

     
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Direccion_Autor>>> ListarDirecciones()
        {
            return await _context.Direccion_Autor.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Direccion_Autor>> ObtenerDireccion(int id)
        {
            var direccion = await _context.Direccion_Autor.FindAsync(id);
            if (direccion == null) return NotFound();
            return direccion;
        }

        
        [HttpGet("Autor/{idAutor}")]
        public async Task<ActionResult<IEnumerable<Direccion_Autor>>> DireccionesPorAutor(int idAutor)
        {
            var direcciones = await _context.Direccion_Autor
                .Where(d => d.id_autor == idAutor)
                .ToListAsync();
            return direcciones;
        }

       
        [HttpPost]
        public async Task<ActionResult<Direccion_Autor>> Crear(Direccion_Autor direccion)
        {
            _context.Direccion_Autor.Add(direccion);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(ObtenerDireccion), new { id = direccion.id_autor }, direccion);
        }

        
        [HttpPut("{id}")]
        public async Task<IActionResult> Actualizar(int id, Direccion_Autor direccionActualizada)
        {
            var direccion = await _context.Direccion_Autor.FindAsync(id);
            if (direccion == null) return NotFound();

            direccion.direccion = direccionActualizada.direccion;
            direccion.id_autor = direccionActualizada.id_autor;

            await _context.SaveChangesAsync();
            return NoContent();
        }

       
        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var direccion = await _context.Direccion_Autor.FindAsync(id);
            if (direccion == null) return NotFound();

            _context.Direccion_Autor.Remove(direccion);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
