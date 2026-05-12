using APIEditorialUCR.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sistema_Editorial.Model;
using System;

namespace APIEditorialUCR.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PortadaController : ControllerBase
    {
        private readonly DbContextEditorial _context;

        public PortadaController(DbContextEditorial context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<Portada>>> GetPortadas()
        {
            return await _context.Portadas.ToListAsync();
        }

        
        [HttpGet("{id}")]
        public async Task<ActionResult<Portada>> GetPortada(int id)
        {
            var portada = await _context.Portadas.FindAsync(id);

            if (portada == null)
                return NotFound();

            return portada;
        }

        
        [HttpPost]
        public async Task<ActionResult<Portada>> PostPortada(Portada portada)
        {
            int nuevoId = 1;
            if (_context.Portadas.Any())
                nuevoId = _context.Portadas.Max(p => p.id_portada) + 1;
            portada.id_portada = nuevoId;

            _context.Portadas.Add(portada);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPortada), new { id = portada.id_portada }, portada);
        }

       
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPortada(int id, Portada portada)
        {
            if (id != portada.id_portada)
                return BadRequest("El ID del cuerpo no coincide con el de la URL.");

            _context.Entry(portada).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Portadas.Any(e => e.id_portada == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePortada(int id)
        {
            var portada = await _context.Portadas.FindAsync(id);
            if (portada == null)
                return NotFound();

            _context.Portadas.Remove(portada);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
