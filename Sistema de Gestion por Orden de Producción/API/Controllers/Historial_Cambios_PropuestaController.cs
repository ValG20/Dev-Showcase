using APIEditorialUCR.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sistema_Editorial.Model;
using System;

namespace APIEditorialUCR.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class Historial_Cambios_PropuestaController : ControllerBase
    {
        private readonly DbContextEditorial _context;

        public Historial_Cambios_PropuestaController(DbContextEditorial context)
        {
            _context = context;
        }

        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Historial_Cambios_Propuesta>>> GetHistorial_Cambios_Propuesta()
        {
            return await _context.Historial_Cambios_Propuesta.ToListAsync();
        }

       
        [HttpGet("{id}")]
        public async Task<ActionResult<Historial_Cambios_Propuesta>> GetHistorial_Cambio(int id)
        {
            var historial = await _context.Historial_Cambios_Propuesta.FindAsync(id);

            if (historial == null)
                return NotFound();

            return historial;
        }

        
        [HttpPost]
        public async Task<ActionResult<Historial_Cambios_Propuesta>> PostHistorial_Cambio(Historial_Cambios_Propuesta historial)
        {
            _context.Historial_Cambios_Propuesta.Add(historial);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetHistorial_Cambio), new { id = historial.id_historial }, historial);
        }

       
        [HttpPut("{id}")]
        public async Task<IActionResult> PutHistorial_Cambio(int id, Historial_Cambios_Propuesta historial)
        {
            if (id != historial.id_historial)
                return BadRequest("El ID del cuerpo no coincide con el de la URL.");

            _context.Entry(historial).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Historial_Cambios_Propuesta.Any(e => e.id_historial == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHistorial_Cambio(int id)
        {
            var historial = await _context.Historial_Cambios_Propuesta.FindAsync(id);
            if (historial == null)
                return NotFound();

            _context.Historial_Cambios_Propuesta.Remove(historial);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
