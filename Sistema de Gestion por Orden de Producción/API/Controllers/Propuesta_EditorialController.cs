using APIEditorialUCR.Model;
using Microsoft.AspNetCore.Mvc;
using Sistema_Editorial.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace APIEditorialUCR.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class Propuesta_EditorialController : Controller
    {
        private readonly DbContextEditorial _context = null;

        public Propuesta_EditorialController(DbContextEditorial pContext)
        {
            _context = pContext;
        }

      
        [HttpGet("Lista")]
        public List<Propuesta_Editorial> Lista()
        {
            List<Propuesta_Editorial> temp = _context.Propuesta_Editorial.ToList();
            return temp;
        }

       
        [HttpGet("Buscar")]
        public IActionResult Buscar(int id)
        {
            var temp = _context.Propuesta_Editorial.FirstOrDefault(x => x.id_propuesta == id);

            if (temp == null)
            {
                return NotFound($"No existe una propuesta editorial con el ID {id}.");
            }

            return Ok(temp);
        }

        
        [HttpPost("Guardar")]
        public IActionResult Guardar([FromBody] Propuesta_Editorial temp)
        {
            try
            {
                // Generar ID manual
                int nuevoId = 1;
                if (_context.Propuesta_Editorial.Any())
                    nuevoId = _context.Propuesta_Editorial.Max(p => p.id_propuesta) + 1;
                temp.id_propuesta = nuevoId;

                temp.fecha_creacion = DateTime.Now;

                _context.Propuesta_Editorial.Add(temp);
                _context.SaveChanges();

                // Devuelve solo el ID generado
                return Ok(temp.id_propuesta);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = ex.InnerException?.Message ?? ex.Message });
            }
        }

     
        [HttpDelete("Eliminar")]
        public string Eliminar(int idPropuesta)
        {
            string msj = "Eliminando propuesta editorial...";

            try
            {
                var temp = _context.Propuesta_Editorial.FirstOrDefault(r => r.id_propuesta == idPropuesta);

                if (temp == null)
                {
                    msj = "No existe la propuesta editorial.";
                }
                else
                {
                    _context.Propuesta_Editorial.Remove(temp);
                    _context.SaveChanges();
                    msj = "Propuesta editorial eliminada correctamente.";
                }
            }
            catch (Exception ex)
            {
                msj = ex.InnerException?.Message ?? ex.Message;
            }

            return msj;
        }


        [HttpPut("Actualizar")]
        public string Actualizar([FromBody] Propuesta_Editorial temp)
        {
            string msj = "Actualizando propuesta editorial...";

            try
            {
                var objPropuesta = _context.Propuesta_Editorial.FirstOrDefault(r => r.id_propuesta == temp.id_propuesta);

                if (objPropuesta == null)
                {
                    msj = "No existe la propuesta editorial.";
                }
                else
                {
                    // === Mapeo de propiedades basado en el modelo proporcionado ===
                    objPropuesta.fecha_creacion = temp.fecha_creacion;
                    objPropuesta.titulo_obra = temp.titulo_obra;
                    objPropuesta.subtitulo = temp.subtitulo;
                    objPropuesta.serie_coleccion = temp.serie_coleccion;
                    objPropuesta.publico_meta = temp.publico_meta;
                    objPropuesta.palabras_claves = temp.palabras_claves;
                    // =============================================================

                    _context.Propuesta_Editorial.Update(objPropuesta);
                    _context.SaveChanges();

                    msj = "Propuesta editorial actualizada correctamente.";
                }
            }
            catch (Exception ex)
            {
                msj = ex.InnerException?.Message ?? ex.Message;
            }

            return msj;
        }

        [HttpPost("GuardarDevolver")]
        public IActionResult GuardarDevolver([FromBody] Propuesta_Editorial temp)
        {
            try
            {
                // Generar ID manual
                int nuevoId = 1;
                if (_context.Propuesta_Editorial.Any())
                    nuevoId = _context.Propuesta_Editorial.Max(p => p.id_propuesta) + 1;
                temp.id_propuesta = nuevoId;

                temp.fecha_creacion = DateTime.Now;

                _context.Propuesta_Editorial.Add(temp);
                _context.SaveChanges();

                // Devuelve solo el ID generado
                return Ok(temp);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = ex.InnerException?.Message ?? ex.Message });
            }
        }

    }
}