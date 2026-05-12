using APIEditorialUCR.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sistema_Editorial.Model;

namespace APIEditorialUCR.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class Archivo_AdjuntoController : Controller
    {
        private readonly DbContextEditorial _context;

        public Archivo_AdjuntoController(DbContextEditorial context)
        {
            _context = context;
        }

        // Listar todos los archivos adjuntos
        [HttpGet("Lista")]
        public List<Archivo_adjunto> Lista()
        {
            return _context.Archivos_Adjuntos_Propuesta.ToList();
        }

        // Buscar un archivo por ID
        [HttpGet("Buscar")]
        public IActionResult Buscar(int id)
        {
            var temp = _context.Archivos_Adjuntos_Propuesta
                .FirstOrDefault(a => a.id_archivo == id);

            if (temp == null)
                return NotFound($"No existe un archivo adjunto con el ID {id}.");

            return Ok(temp);
        }

        // Guardar nuevo archivo adjunto
        [HttpPost("Guardar")]
        public string Guardar(Archivo_adjunto temp)
        {
            string msj = "Archivo adjunto guardado correctamente.";

            try
            {
                _context.Archivos_Adjuntos_Propuesta.Add(temp);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                msj = ex.InnerException?.Message ?? ex.Message;
            }

            return msj;
        }

        // Actualizar archivo adjunto existente
        [HttpPut("Actualizar")]
        public string Actualizar(Archivo_adjunto temp)
        {
            string msj = "Actualizando archivo adjunto...";

            try
            {
                var obj = _context.Archivos_Adjuntos_Propuesta
                    .FirstOrDefault(a => a.id_archivo == temp.id_archivo);

                if (obj == null)
                {
                    msj = "No existe el archivo adjunto.";
                }
                else
                {
                    obj.id_propuesta = temp.id_propuesta;
                    obj.nombre_archivo = temp.nombre_archivo;
                    obj.tipo_archivo = temp.tipo_archivo;
                    obj.ruta_almacenamiento = temp.ruta_almacenamiento;

                    _context.Archivos_Adjuntos_Propuesta.Update(obj);
                    _context.SaveChanges();
                    msj = "Archivo adjunto actualizado correctamente.";
                }
            }
            catch (Exception ex)
            {
                msj = ex.InnerException?.Message ?? ex.Message;
            }

            return msj;
        }

        // Eliminar archivo adjunto
        [HttpDelete("Eliminar")]
        public string Eliminar(int id)
        {
            string msj = "Eliminando archivo adjunto...";

            try
            {
                var temp = _context.Archivos_Adjuntos_Propuesta
                    .FirstOrDefault(a => a.id_archivo == id);

                if (temp == null)
                {
                    msj = "No existe el archivo adjunto.";
                }
                else
                {
                    _context.Archivos_Adjuntos_Propuesta.Remove(temp);
                    _context.SaveChanges();
                    msj = "Archivo adjunto eliminado correctamente.";
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
