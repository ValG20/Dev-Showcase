using Microsoft.AspNetCore.Mvc;
using Sistema_Editorial.Model;
using Sistema_EditorialS.Model.Valeria.Reportes;
using Microsoft.EntityFrameworkCore;
using APIEditorialUCR.Model;

namespace APIEditorialUCR.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class Detalle_ReporteController : Controller
    {
        private readonly DbContextEditorial _context;

        public Detalle_ReporteController(DbContextEditorial context)
        {
            _context = context;
        }

        [HttpGet("Lista")]
        public List<Detalle_Reporte> Lista()
        {
            return _context.Detalles_Reportes
                .Include(d => d.Reporte)
                .ToList();
        }

        [HttpGet("Buscar")]
        public IActionResult Buscar(int id)
        {
            var temp = _context.Detalles_Reportes
                .Include(d => d.Reporte)
                .FirstOrDefault(x => x.id_detalle == id);

            if (temp == null)
                return NotFound($"No existe un detalle de reporte con el ID {id}.");

            return Ok(temp);
        }

        [HttpPost("Guardar")]
        public string Guardar(Detalle_Reporte temp)
        {
            string msj = "Detalle de reporte guardado correctamente.";

            try
            {
                _context.Detalles_Reportes.Add(temp);
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
            string msj = "Eliminando detalle de reporte...";

            try
            {
                var temp = _context.Detalles_Reportes.FirstOrDefault(r => r.id_detalle == id);

                if (temp == null)
                {
                    msj = "No existe el detalle de reporte.";
                }
                else
                {
                    _context.Detalles_Reportes.Remove(temp);
                    _context.SaveChanges();
                    msj = "Detalle de reporte eliminado correctamente.";
                }
            }
            catch (Exception ex)
            {
                msj = ex.InnerException?.Message ?? ex.Message;
            }

            return msj;
        }

        [HttpPut("Actualizar")]
        public string Actualizar(Detalle_Reporte temp)
        {
            string msj = "Actualizando detalle de reporte...";

            try
            {
                var obj = _context.Detalles_Reportes.FirstOrDefault(r => r.id_detalle == temp.id_detalle);

                if (obj == null)
                {
                    msj = "No existe el detalle de reporte.";
                }
                else
                {
                    obj.id_reporte = temp.id_reporte;
                    obj.id_referencia = temp.id_referencia;
                    obj.tipo_referencia = temp.tipo_referencia;

                    _context.Detalles_Reportes.Update(obj);
                    _context.SaveChanges();

                    msj = "Detalle de reporte actualizado correctamente.";
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
