using APIBitacoras.Model;
using APIEditorialUCR.Model;
using Microsoft.AspNetCore.Mvc;
using Sistema_Editorial.Model;

namespace APIBitacoras.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PresupuestosController : ControllerBase
    {
       
        private readonly DbContextEditorial _context = null;

       
        public PresupuestosController(DbContextEditorial pContext)
        {
            _context = pContext; 
        }

        
        [HttpGet("List")]
        public List<Presupuestos> List()
        {
            List<Presupuestos> temp = _context.Presupuestos.ToList();
            return temp;
        }

        
        [HttpPost("Save")]
        public string Save(Presupuestos temp)
        {
            string msj = "Presupuesto guardado...";
            try
            {
                _context.Presupuestos.Add(temp);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                msj = ex.InnerException?.Message ?? ex.Message;
            }
            return msj;
        }

       
        [HttpGet("SearchById")]
        public ActionResult<Presupuestos> SearchById(int id_presupuesto)
        {
            Presupuestos temp = _context.Presupuestos.FirstOrDefault(x => x.id_presupuesto == id_presupuesto);

            if (temp == null)
            {
                return NotFound(new { Message = "No existe ningún presupuesto con ese ID." });
            }

            return Ok(temp);
        }

        
        [HttpDelete("Delete")]
        public async Task<string> Delete(int id_presupuesto)
        {
            string msj = "";
            try
            {
                var temp = _context.Presupuestos.FirstOrDefault(u => u.id_presupuesto == id_presupuesto);
                if (temp == null)
                {
                    msj = $"No existe el presupuesto con ID {id_presupuesto}";
                }
                else
                {
                    _context.Presupuestos.Remove(temp);
                    await _context.SaveChangesAsync();
                    msj = $"Presupuesto con ID {id_presupuesto} eliminado correctamente.";
                }
            }
            catch (Exception ex)
            {
                msj = ex.InnerException?.Message ?? ex.Message;
            }
            return msj;
        }

        
        [HttpPut("Update")]
        public async Task<string> Update(Presupuestos pPresupuesto)
        {
            string msj = $"Actualizando presupuesto {pPresupuesto.id_presupuesto}";

            try
            {
                var temp = _context.Presupuestos.FirstOrDefault(t => t.id_presupuesto == pPresupuesto.id_presupuesto);

                if (temp != null)
                {
                    temp.id_pedido = pPresupuesto.id_pedido;
                    temp.fecha_generacion = pPresupuesto.fecha_generacion;
                    temp.total_materiales = pPresupuesto.total_materiales;
                    temp.total_mano_obra = pPresupuesto.total_mano_obra;
                    temp.total_general = pPresupuesto.total_general;
                    temp.generado_por = pPresupuesto.generado_por;
                    temp.aprobado_por = pPresupuesto.aprobado_por;
                    temp.estado = pPresupuesto.estado;
                    temp.observaciones = pPresupuesto.observaciones;

                    _context.Presupuestos.Update(temp);
                    await _context.SaveChangesAsync();

                    msj = $"El presupuesto con ID {temp.id_presupuesto} fue actualizado correctamente.";
                }
                else
                {
                    msj = $"No existe un presupuesto con el ID {pPresupuesto.id_presupuesto}.";
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
