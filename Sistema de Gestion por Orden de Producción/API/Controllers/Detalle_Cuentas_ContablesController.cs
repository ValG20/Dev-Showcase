using APIEditorialUCR.Model;
using APIEditorialUCR.Models;
using Microsoft.AspNetCore.Mvc;
using Sistema_Editorial.Model;

namespace APIEditorialUCR.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class Detalle_Cuentas_ContablesController : Controller
    {
        private readonly DbContextEditorial _context = null;

        public Detalle_Cuentas_ContablesController(DbContextEditorial pContext)
        {
            _context = pContext;
        }

        [HttpGet("Lista")]
        public List<Detalle_Cuentas_Contables> Lista()
        {
            List<Detalle_Cuentas_Contables> temp = _context.Detalle_Cuentas_Contables.ToList();
            return temp;
        }

        [HttpGet("Detalles")]
        public ActionResult<Detalle_Cuentas_Contables> Detalles(int id)
        {
            try
            {
                var detalle = _context.Detalle_Cuentas_Contables
                    .FirstOrDefault(d => d.id_detalle == id);

                if (detalle == null)
                {
                    return NotFound($"No se encontró el detalle con ID {id}.");
                }

                return Ok(detalle);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpPost("Guardar")]
        public string Guardar(Detalle_Cuentas_Contables temp)
        {
            string msj = "Detalle guardado correctamente.";

            try
            {
                int nuevoId = 1;
                if (_context.Detalle_Cuentas_Contables.Any())
                    nuevoId = _context.Detalle_Cuentas_Contables.Max(d => d.id_detalle) + 1; 
                temp.id_detalle = nuevoId;

                _context.Detalle_Cuentas_Contables.Add(temp);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                msj = ex.InnerException?.Message ?? ex.Message;
            }

            return msj;
        }


        [HttpDelete("Eliminar")]
        public string Eliminar(int id)
        {
            string msj = "Eliminando detalle...";

            try
            {
                var temp = _context.Detalle_Cuentas_Contables
                    .FirstOrDefault(d => d.id_detalle == id);

                if (temp == null)
                {
                    msj = "No existe el detalle.";
                }
                else
                {
                    _context.Detalle_Cuentas_Contables.Remove(temp);
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                msj = ex.InnerException?.Message ?? ex.Message;
            }
            return msj;
        }

        [HttpPut("Actualizar")]
        public string Actualizar(Detalle_Cuentas_Contables temp)
        {
            string msj = "Actualizando detalle...";

            try
            {
                var objDetalle = _context.Detalle_Cuentas_Contables
                    .FirstOrDefault(d => d.id_detalle == temp.id_detalle);

                if (objDetalle == null)
                {
                    msj = "No existe el detalle.";
                }
                else
                {
                    objDetalle.id_pedido = temp.id_pedido;
                    objDetalle.id_cuenta = temp.id_cuenta;
                    objDetalle.costo_directo = temp.costo_directo;
                    objDetalle.costo_indirecto = temp.costo_indirecto;
                    objDetalle.libro_digital = temp.libro_digital;
                    objDetalle.otros = temp.otros;

                    _context.Detalle_Cuentas_Contables.Update(objDetalle);
                    _context.SaveChanges();

                    msj = "Detalle actualizado correctamente.";
                }
            }
            catch (Exception ex)
            {
                msj = ex.InnerException?.Message ?? ex.Message;
            }

            return msj;
        }
    }
}
