
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APIBitacoras.Model;          
using Sistema_Editorial.Model;
using APIEditorialUCR.Model;     

namespace APIEditorialUCR.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AutorController : ControllerBase
    {
        private readonly DbContextEditorial _context;

        public AutorController(DbContextEditorial context)
        {
            _context = context;
        }

      
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Autor>>> GetAutores()
        {
            return await _context.Autores.ToListAsync();
        }

      
        [HttpGet("{id}")]
        public async Task<ActionResult<Autor>> GetAutor(int id)
        {
            var autor = await _context.Autores.FindAsync(id);

            if (autor == null)
                return NotFound();

            return autor;
        }

       
        [HttpPost]
        public async Task<ActionResult<int>> CreateAutor(Autor autor)
        {
            if (!autor.correo_electronico.Contains("@"))
                return BadRequest("El correo electrónico no es válido.");

            // Generar ID manual
            int nuevoId = 1;
            if (_context.Autores.Any())
                nuevoId = _context.Autores.Max(a => a.id_autor) + 1;
            autor.id_autor = nuevoId;

            autor.fecha_actualizacion = DateTime.Now;

            _context.Autores.Add(autor);
            await _context.SaveChangesAsync();

            return Ok(autor.id_autor); 
        }



       
        [HttpPut("{id}")]
        public async Task<ActionResult<int>> UpdateAutor(int id, Autor cambio)
        {
            var autor = await _context.Autores.FindAsync(id);

            if (autor == null)
                return NotFound();

            autor.nombre_apellidos = cambio.nombre_apellidos;
            autor.nacionalidad = cambio.nacionalidad;
            autor.documento_identidad = cambio.documento_identidad;
            autor.estado_civil = cambio.estado_civil;
            autor.profesion = cambio.profesion;
            autor.correo_electronico = cambio.correo_electronico;

            autor.fecha_actualizacion = DateTime.Now;

            await _context.SaveChangesAsync();

            // Devuelve el mismo id actualizado
            return Ok(autor.id_autor);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAutor(int id)
        {
            var autor = await _context.Autores.FindAsync(id);

            if (autor == null)
                return NotFound();

            _context.Autores.Remove(autor);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("Devolver")]
        public async Task<ActionResult<int>> CreateAutorDevolver(Autor autor)
        {
            if (!autor.correo_electronico.Contains("@"))
                return BadRequest("El correo electrónico no es válido.");

            // Generar ID manual
            int nuevoId = 1;
            if (_context.Autores.Any())
                nuevoId = _context.Autores.Max(a => a.id_autor) + 1;
            autor.id_autor = nuevoId;

            autor.fecha_actualizacion = DateTime.Now;

            _context.Autores.Add(autor);
            await _context.SaveChangesAsync();

            return Ok(autor); // Devuelve el ID generado
        }

    }
}

