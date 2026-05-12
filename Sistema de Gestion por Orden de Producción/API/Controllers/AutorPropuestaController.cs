using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APIEditorialUCR.Model;
using Microsoft.AspNetCore.Http.HttpResults;   

namespace APIEditorialUCR.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Autor_PropuestaController : ControllerBase
    {
        private readonly DbContextEditorial _context;

        public Autor_PropuestaController(DbContextEditorial context)
        {
            _context = context;
        }

       
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Autor_Propuesta>>> GetAutorPropuestas()
        {
            return await _context.Autor_Propuesta.ToListAsync();
        }

        
        [HttpGet("{id}")]
        public async Task<ActionResult<Autor_Propuesta>> GetAutorPropuesta(int id)
        {
            var entidad = await _context.Autor_Propuesta.FindAsync(id);

            if (entidad == null)
                return NotFound();

            return entidad;
        }

       
        [HttpPost]
        public async Task<ActionResult<int>> CreateAutorPropuesta(Autor_Propuesta autorPropuesta)
        {
            // Validaciones básicas
            if (autorPropuesta.id_propuesta <= 0 || autorPropuesta.id_autor <= 0)
                return BadRequest("id_propuesta e id_autor deben ser válidos.");

            if (string.IsNullOrWhiteSpace(autorPropuesta.funcion_en_obra))
                return BadRequest("La función en la obra es obligatoria.");

            // Generar ID manual (como en AutorController)
            int nuevoId = 1;
            if (_context.Autor_Propuesta.Any())
                nuevoId = _context.Autor_Propuesta.Max(a => a.id_autor_propuesta) + 1;

            autorPropuesta.id_autor_propuesta = nuevoId;

            _context.Autor_Propuesta.Add(autorPropuesta);
            await _context.SaveChangesAsync();

            // Devuelve solo el ID generado
            return Ok(autorPropuesta.id_autor_propuesta);
        }

      
        [HttpPost("devolverObjeto")]
        public async Task<ActionResult<Autor_Propuesta>> CreateAutorPropuestaObjeto(Autor_Propuesta autorPropuesta)
        {
            if (autorPropuesta.id_propuesta <= 0 || autorPropuesta.id_autor <= 0)
                return BadRequest("id_propuesta e id_autor deben ser válidos.");

            if (string.IsNullOrWhiteSpace(autorPropuesta.funcion_en_obra))
                return BadRequest("La función en la obra es obligatoria.");

            int nuevoId = 1;
            if (_context.Autor_Propuesta.Any())
                nuevoId = _context.Autor_Propuesta.Max(a => a.id_autor_propuesta) + 1;

            autorPropuesta.id_autor_propuesta = nuevoId;


            _context.Autor_Propuesta.Add(autorPropuesta);
            await _context.SaveChangesAsync();

            // Devuelve el objeto completo
            return Ok(autorPropuesta);

        }

        
        [HttpPut("{id}")]
        public async Task<ActionResult<int>> UpdateAutorPropuesta(int id, Autor_Propuesta cambio)
        {
            var entidad = await _context.Autor_Propuesta.FindAsync(id);

            if (entidad == null)
                return NotFound();

            entidad.id_propuesta = cambio.id_propuesta;
            entidad.id_autor = cambio.id_autor;
            entidad.funcion_en_obra = cambio.funcion_en_obra;

            await _context.SaveChangesAsync();

            return Ok(entidad.id_autor_propuesta);
        }

        
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAutorPropuesta(int id)
        {
            var entidad = await _context.Autor_Propuesta.FindAsync(id);

            if (entidad == null)
                return NotFound();

            _context.Autor_Propuesta.Remove(entidad);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}