using APIEditorialUCR.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sistema_Editorial.Model;
using System;

namespace APIEditorialUCR.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class Especificaciones_TecnicasController : ControllerBase
    {
        private readonly DbContextEditorial _context;

        public Especificaciones_TecnicasController(DbContextEditorial context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Especificaciones_Tecnicas>>> GetEspecificaciones_Tecnicas()
        {
            return await _context.Especificaciones_Tecnicas.ToListAsync();
        }

        
        [HttpGet("{id}")]
        public async Task<ActionResult<Especificaciones_Tecnicas>> GetEspecificacion_Tecnica(int id)
        {
            var especificacion = await _context.Especificaciones_Tecnicas.FindAsync(id);

            if (especificacion == null)
                return NotFound();

            return especificacion;
        }

       
        [HttpPost]
        public async Task<ActionResult<int>> PostEspecificacion_Tecnica(Especificaciones_Tecnicas especificacion)
        {
           
            int nuevoId = 1;
            if (_context.Especificaciones_Tecnicas.Any())
                nuevoId = _context.Especificaciones_Tecnicas.Max(e => e.id_tecnica) + 1;
            especificacion.id_tecnica = nuevoId;

            _context.Especificaciones_Tecnicas.Add(especificacion);
            await _context.SaveChangesAsync();

           
            return Ok(especificacion.id_tecnica);
        }



       
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEspecificacion_Tecnica(int id, Especificaciones_Tecnicas especificacion)
        {
            if (id != especificacion.id_tecnica)
                return BadRequest("El ID del cuerpo no coincide con el de la URL.");

            _context.Entry(especificacion).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Especificaciones_Tecnicas.Any(e => e.id_tecnica == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEspecificacion_Tecnica(int id)
        {
            var especificacion = await _context.Especificaciones_Tecnicas.FindAsync(id);
            if (especificacion == null)
                return NotFound();

            _context.Especificaciones_Tecnicas.Remove(especificacion);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
