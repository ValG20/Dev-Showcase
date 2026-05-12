using APIEditorialUCR.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sistema_Editorial.Model;
using Sistema_Editorial.Model.Valeria.Reportes;

namespace APIEditorialUCR.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ReporteController : Controller
    {
        private readonly DbContextEditorial _context;

        public ReporteController(DbContextEditorial context)
        {
            _context = context;
        }

        [HttpGet("Lista")]
        public List<Reporte> Lista()
        {
            return _context.Reportes
                .Include(r => r.DetallesReporte)
                .ToList();
        }

        [HttpGet("Buscar")]
        public IActionResult Buscar(int id)
        {
            var temp = _context.Reportes
                .Include(r => r.DetallesReporte)
                .FirstOrDefault(x => x.id_reporte == id);

            if (temp == null)
                return NotFound($"No existe un reporte con el ID {id}.");

            return Ok(temp);
        }

        [HttpPost("Guardar")]
        public string Guardar(Reporte temp)
        {
            string msj = "Reporte guardado correctamente.";

            try
            {
                _context.Reportes.Add(temp);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                msj = ex.InnerException?.Message ?? ex.Message;
            }

            return msj;
        }

        [HttpPut("Actualizar")]
        public string Actualizar(Reporte temp)
        {
            string msj = "Actualizando reporte...";

            try
            {
                var obj = _context.Reportes.FirstOrDefault(r => r.id_reporte == temp.id_reporte);

                if (obj == null)
                {
                    msj = "No existe el reporte.";
                }
                else
                {
                    obj.tipo_reporte = temp.tipo_reporte;
                    obj.titulo = temp.titulo;
                    obj.descripcion = temp.descripcion;
                    obj.usuario_generador = temp.usuario_generador;
                    obj.fecha_generacion = temp.fecha_generacion;
                    obj.ruta_archivo = temp.ruta_archivo;
                    obj.formato = temp.formato;
                    obj.periodo_inicio = temp.periodo_inicio;
                    obj.periodo_fin = temp.periodo_fin;
                    obj.estado = temp.estado;

                    _context.Reportes.Update(obj);
                    _context.SaveChanges();

                    msj = "Reporte actualizado correctamente.";
                }
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
            string msj = "Eliminando reporte...";

            try
            {
                var temp = _context.Reportes
                    .Include(r => r.DetallesReporte)
                    .FirstOrDefault(r => r.id_reporte == id);

                if (temp == null)
                {
                    msj = "No existe el reporte.";
                }
                else
                {
                    // Primero eliminar los detalles si existen
                    if (temp.DetallesReporte != null && temp.DetallesReporte.Any())
                    {
                        _context.Detalles_Reportes.RemoveRange(temp.DetallesReporte);
                    }

                    _context.Reportes.Remove(temp);
                    _context.SaveChanges();
                    msj = "Reporte y sus detalles eliminados correctamente.";
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
