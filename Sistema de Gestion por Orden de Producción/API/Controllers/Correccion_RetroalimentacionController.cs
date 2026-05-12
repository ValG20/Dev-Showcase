using APIEditorialUCR.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sistema_Editorial.Model;
using System;

namespace APIEditorialUCR.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class Correccion_RetroalimentacionController : ControllerBase
    {
        private readonly DbContextEditorial _context;

        public Correccion_RetroalimentacionController(DbContextEditorial context)
        {
            _context = context;
        }

       
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Correccion_Retroalimentacion>>> GetCorrecciones_Retroalimentaciones()
        {
            return await _context.Correccion_Retroalimentaciones.ToListAsync();
        }

       
        [HttpGet("{id}")]
        public async Task<ActionResult<Correccion_Retroalimentacion>> GetCorreccion_Retroalimentacion(int id)
        {
            var correccion = await _context.Correccion_Retroalimentaciones.FindAsync(id);

            if (correccion == null)
                return NotFound();

            return correccion;
        }

       
        [HttpPost]
        public async Task<ActionResult<Correccion_Retroalimentacion>> PostCorreccion_Retroalimentacion(Correccion_Retroalimentacion correccion)
        {
            _context.Correccion_Retroalimentaciones.Add(correccion);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCorreccion_Retroalimentacion), new { id = correccion.id_correccion }, correccion);
        }

       
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCorreccion_Retroalimentacion(int id, Correccion_Retroalimentacion correccion)
        {
            if (id != correccion.id_correccion)
                return BadRequest("El ID del cuerpo no coincide con el de la URL.");

            _context.Entry(correccion).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Correccion_Retroalimentaciones.Any(e => e.id_correccion == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCorreccion_Retroalimentacion(int id)
        {
            var correccion = await _context.Correccion_Retroalimentaciones.FindAsync(id);
            if (correccion == null)
                return NotFound();

            _context.Correccion_Retroalimentaciones.Remove(correccion);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
