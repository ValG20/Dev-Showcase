using APIEditorialUCR.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sistema_Editorial.Model;
namespace APIEditorialUCR.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class Acta_PropuestaController : ControllerBase
    {
        private readonly DbContextEditorial _context;

        public Acta_PropuestaController(DbContextEditorial context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Acta_Propuesta>>> GetActas_Propuestas()
        {
            try
            {
                var actasPropuestas = await _context.Acta_Propuesta.ToListAsync();

                if (actasPropuestas == null || !actasPropuestas.Any())
                {
                    return NotFound(new { mensaje = "No se encontraron actas propuestas." });
                }

                return Ok(actasPropuestas);
            }
            catch (Exception ex)
            {
                // Puedes loguear el error si tienes un logger
                return StatusCode(500, new { mensaje = "Ocurrió un error al obtener los registros.", detalle = ex.Message });
            }
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<Acta_Propuesta>> GetActa_Propuesta(int id)
        {
            var actaPropuesta = await _context.Acta_Propuesta.FindAsync(id);

            if (actaPropuesta == null)
                return NotFound();

            return actaPropuesta;
        }


        [HttpPost]
        public async Task<ActionResult<Acta_Propuesta>> PostActa_Propuesta(Acta_Propuesta actaPropuesta)
        {
            try
            {
                // Obtener el máximo ID actual y sumarle 1
                int maxId = 0;
                if (await _context.Acta_Propuesta.AnyAsync())
                {
                    maxId = await _context.Acta_Propuesta.MaxAsync(a => a.id_acta_propuesta);
                }
                actaPropuesta.id_acta_propuesta = maxId + 1;

                _context.Acta_Propuesta.Add(actaPropuesta);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetActa_Propuesta),
                    new { id = actaPropuesta.id_acta_propuesta }, actaPropuesta);
            }
            catch (Exception ex)
            {
                return BadRequest("Error al generar el ID manual: " + ex.Message);
            }
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> PutActa_Propuesta(int id, Acta_Propuesta actaPropuesta)
        {
            if (id != actaPropuesta.id_acta_propuesta)
                return BadRequest("El ID del cuerpo no coincide con el de la URL.");

            _context.Entry(actaPropuesta).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Acta_Propuesta.Any(e => e.id_acta_propuesta == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteActa_Propuesta(int id)
        {
            var actaPropuesta = await _context.Acta_Propuesta   .FindAsync(id);
            if (actaPropuesta == null)
                return NotFound();

            _context.Acta_Propuesta.Remove(actaPropuesta);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
