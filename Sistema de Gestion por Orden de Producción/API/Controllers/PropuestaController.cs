using APIEditorialUCR.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sistema_Editorial.Model;
using System;

namespace APIEditorialUCR.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PropuestaController : ControllerBase
    {
        private readonly DbContextEditorial _context;

        public PropuestaController(DbContextEditorial context)
        {
            _context = context;
        }

       
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Propuesta>>> GetPropuestas()
        {
            return await _context.Propuestas.ToListAsync();
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<Propuesta>> GetPropuesta(int id)
        {
            var propuesta = await _context.Propuestas.FindAsync(id);
            if (propuesta == null)
                return NotFound();
            return propuesta;
        }

        [HttpPost]
        public async Task<ActionResult<Propuesta>> PostPropuesta(Propuesta propuesta)
        {
            if (propuesta == null)
                return BadRequest(new { mensaje = "El cuerpo de la solicitud de Propuesta no puede estar vacío." });

            try
            {
                int nuevoId = 1;
                if (_context.Propuestas.Any())
                    nuevoId = _context.Propuestas.Max(p => p.id_propuesta) + 1;

                propuesta.id_propuesta = nuevoId;
                propuesta.fecha_envio = DateTime.Now;
                propuesta.estado = "enviado";

                _context.Propuestas.Add(propuesta);
                await _context.SaveChangesAsync();

                // SOLO devuelve la ID
                return Ok(propuesta.id_propuesta);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    mensaje = "Error al guardar la Propuesta.",
                    detalle = ex.InnerException?.Message ?? ex.Message
                });
            }
        }




        [HttpPut("{id}")]
        public async Task<IActionResult> PutPropuesta(int id, Propuesta propuesta)
        {
            if (id != propuesta.id_propuesta)
                return BadRequest();

            _context.Entry(propuesta).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

 
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePropuesta(int id)
        {
            var propuesta = await _context.Propuestas.FindAsync(id);
            if (propuesta == null)
                return NotFound();

            _context.Propuestas.Remove(propuesta);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }


}
