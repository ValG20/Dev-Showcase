using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APIEditorialUCR.Model;

namespace APIEditorialUCR.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TelefonoAutorController : ControllerBase
    {
        private readonly DbContextEditorial _context;

        public TelefonoAutorController(DbContextEditorial context)
        {
            _context = context;
        }

       
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Telefono_Autor>>> ListarTelefonos()
        {
            return await _context.Telefono_Autor.ToListAsync();
        }

      
        [HttpGet("{id}")]
        public async Task<ActionResult<Telefono_Autor>> ObtenerTelefono(int id)
        {
            var telefono = await _context.Telefono_Autor.FindAsync(id);
            if (telefono == null) return NotFound();
            return telefono;
        }

     
        [HttpGet("Autor/{idAutor}")]
        public async Task<ActionResult<IEnumerable<Telefono_Autor>>> TelefonosPorAutor(int idAutor)
        {
            var telefonos = await _context.Telefono_Autor
                .Where(t => t.id_autor == idAutor)
                .ToListAsync();
            return telefonos;
        }

      
        [HttpPost]
        public async Task<ActionResult<Telefono_Autor>> Crear(Telefono_Autor telefono)
        {
            _context.Telefono_Autor.Add(telefono);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(ObtenerTelefono), new { id = telefono.id_autor }, telefono);
        }

     
        [HttpPut("{id}")]
        public async Task<IActionResult> Actualizar(int id, Telefono_Autor telefonoActualizado)
        {
            var telefono = await _context.Telefono_Autor.FindAsync(id);
            if (telefono == null) return NotFound();

            telefono.telefono = telefonoActualizado.telefono;
            telefono.id_autor = telefonoActualizado.id_autor;

            await _context.SaveChangesAsync();
            return NoContent();
        }

      
        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var telefono = await _context.Telefono_Autor.FindAsync(id);
            if (telefono == null) return NotFound();

            _context.Telefono_Autor.Remove(telefono);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
