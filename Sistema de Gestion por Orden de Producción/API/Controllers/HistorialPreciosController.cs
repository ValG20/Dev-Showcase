using APIBitacoras.Model;
using APIEditorialUCR.Model;
using Microsoft.AspNetCore.Mvc;
using Sistema_Editorial.Model;

namespace APIBitacoras.Controllers
{
    [ApiController]
    [Route("[controller]")]

    public class HistorialPreciosController : ControllerBase
    {
       
        private readonly DbContextEditorial _context = null;

       
        public HistorialPreciosController(DbContextEditorial pContext)
        {
            _context = pContext; 
        }

        
        [HttpGet("List")]
        public List<Historial_Precios> List()
        {
            List<Historial_Precios> temp = _context.Historial_Precios.ToList();
            return temp;
        }

        
        [HttpPost("Save")]
        public string Save(Historial_Precios temp)
        {
            string msj = "Historial de precio guardado...";
            try
            {
                _context.Historial_Precios.Add(temp);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                msj = ex.InnerException?.Message ?? ex.Message;
            }
            return msj;
        }

        
        [HttpGet("SearchById")]
        public ActionResult<Historial_Precios> SearchById(int id_historial)
        {
            Historial_Precios temp = _context.Historial_Precios.FirstOrDefault(x => x.id_historial == id_historial);

            if (temp == null)
            {
                return NotFound(new { Message = "No existe ningún historial de precios con ese ID." });
            }

            return Ok(temp);
        }

        
        [HttpDelete("Delete")]
        public async Task<string> Delete(int id_historial)
        {
            string msj = "";
            try
            {
                var temp = _context.Historial_Precios.FirstOrDefault(u => u.id_historial == id_historial);
                if (temp == null)
                {
                    msj = $"No existe el historial de precios con ID {id_historial}";
                }
                else
                {
                    _context.Historial_Precios.Remove(temp);
                    await _context.SaveChangesAsync();
                    msj = $"Historial de precios con ID {id_historial} eliminado correctamente.";
                }
            }
            catch (Exception ex)
            {
                msj = ex.InnerException?.Message ?? ex.Message;
            }
            return msj;
        }

        
        [HttpPut("Update")]
        public async Task<string> Update(Historial_Precios pHistorial)
        {
            string msj = $"Actualizando historial de precios {pHistorial.id_historial}";

            try
            {
                var temp = _context.Historial_Precios.FirstOrDefault(t => t.id_historial == pHistorial.id_historial);

                if (temp != null)
                {
                    temp.id_producto = pHistorial.id_producto;
                    temp.usuario_modificador = pHistorial.usuario_modificador;
                    temp.fecha_modificacion = pHistorial.fecha_modificacion;
                    temp.precio_anterior = pHistorial.precio_anterior;
                    temp.precio_nuevo = pHistorial.precio_nuevo;

                    _context.Historial_Precios.Update(temp);
                    await _context.SaveChangesAsync();

                    msj = $"El historial de precios con ID {temp.id_historial} fue actualizado correctamente.";
                }
                else
                {
                    msj = $"No existe un historial de precios con el ID {pHistorial.id_historial}.";
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
