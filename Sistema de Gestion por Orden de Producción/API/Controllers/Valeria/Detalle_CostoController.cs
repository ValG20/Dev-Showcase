using APIEditorialUCR.Model;
using Microsoft.AspNetCore.Mvc;
using Sistema_Editorial.Model;

namespace APIEditorialUCR.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class Detalle_CostoController : Controller
    {
        private readonly DbContextEditorial _context;

        public Detalle_CostoController(DbContextEditorial context)
        {
            _context = context;
        }

        [HttpGet("Lista")]
        public List<Detalle_Costo> Lista()
        {
            return _context.Detalles_Costos.ToList();
        }

        [HttpGet("Buscar")]
        public IActionResult Buscar(int id)
        {
            var temp = _context.Detalles_Costos.FirstOrDefault(x => x.id_detalle == id);

            if (temp == null)
                return NotFound($"No existe un detalle de costo con el ID {id}.");

            return Ok(temp);
        }

        [HttpPost("Guardar")]
        public string Guardar(Detalle_Costo temp)
        {
            string msj = "Detalle de costo guardado correctamente.";

            try
            {
                _context.Detalles_Costos.Add(temp);
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
            string msj = "Eliminando detalle de costo...";

            try
            {
                var temp = _context.Detalles_Costos.FirstOrDefault(r => r.id_detalle == id);

                if (temp == null)
                {
                    msj = "No existe el detalle de costo.";
                }
                else
                {
                    _context.Detalles_Costos.Remove(temp);
                    _context.SaveChanges();
                    msj = "Detalle de costo eliminado correctamente.";
                }
            }
            catch (Exception ex)
            {
                msj = ex.InnerException?.Message ?? ex.Message;
            }

            return msj;
        }

        [HttpPut("Actualizar")]
        public string Actualizar(Detalle_Costo temp)
        {
            string msj = "Actualizando detalle de costo...";

            try
            {
                var obj = _context.Detalles_Costos.FirstOrDefault(r => r.id_detalle == temp.id_detalle);

                if (obj == null)
                {
                    msj = "No existe el detalle de costo.";
                }
                else
                {
                    obj.id_presupuesto = temp.id_presupuesto;
                    obj.tipo_concepto = temp.tipo_concepto;
                    obj.descripcion = temp.descripcion;
                    obj.cantidad = temp.cantidad;
                    obj.costo_unitario = temp.costo_unitario;
                    obj.subtotal = temp.subtotal;

                    _context.Detalles_Costos.Update(obj);
                    _context.SaveChanges();

                    msj = "Detalle de costo actualizado correctamente.";
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
