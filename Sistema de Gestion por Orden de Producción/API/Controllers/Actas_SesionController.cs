using APIEditorialUCR.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sistema_Editorial.Model;
using System;

namespace APIEditorialUCR.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class Actas_SesionController : ControllerBase
    {
        private readonly DbContextEditorial _context;

        public Actas_SesionController(DbContextEditorial context)
        {
            _context = context;
        }

       
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Actas_Sesion>>> GetActas_Sesiones()
        {
            return await _context.Actas_Sesion.ToListAsync();
        }

   
        [HttpGet("{id}")]
        public async Task<ActionResult<Actas_Sesion>> GetActa_Sesion(int id)
        {
            var actaSesion = await _context.Actas_Sesion.FindAsync(id);

            if (actaSesion == null)
                return NotFound();

            return actaSesion;
        }



        [HttpPost("Guardar")]
        public string Guardar(Actas_Sesion temp)
        {
            string msj = "Acta Sesión guardado correctamente.";

            try
            {
                // Solo agregar, sin asignar ID manual
                _context.Actas_Sesion.Add(temp);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                msj = ex.InnerException?.Message ?? ex.Message;
            }

            return msj;
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> PutActa_Sesion(int id, Actas_Sesion actaSesion)
        {
            if (id != actaSesion.id_acta)
                return BadRequest("El ID del cuerpo no coincide con el de la URL.");

            _context.Entry(actaSesion).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Actas_Sesion.Any(e => e.id_acta == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

     
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteActa_Sesion(int id)
        {
            var actaSesion = await _context.Actas_Sesion.FindAsync(id);
            if (actaSesion == null)
                return NotFound();

            _context.Actas_Sesion.Remove(actaSesion);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
