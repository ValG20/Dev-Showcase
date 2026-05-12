using APIEditorialUCR.Model;
using Microsoft.AspNetCore.Mvc;
using Sistema_Editorial.Model;

namespace APIEditorialUCR.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class Archivo_adjuntoController : Controller
    {
        private readonly DbContextEditorial _context = null;

        public Archivo_adjuntoController(DbContextEditorial pContext)
        {
            _context = pContext;
        }

        [HttpGet("Lista")]
        public List<Archivo_adjunto> Lista()
        {
            List<Archivo_adjunto> temp = _context.Archivos_Adjuntos_Propuesta.ToList();

            return temp;
        }

        [HttpGet("Buscar")]
        public IActionResult Buscar(int id)
        {
            var temp = _context.Servicio_Impresion.FirstOrDefault(x => x.id_servicio == id);

            if (temp == null)
            {
                return NotFound($"No existe un archivo adjunto con el {id}.");
            }

            return Ok(temp);
        }


        [HttpPost("Guardar")]
        public string Guardar(Archivo_adjunto temp)
        {
            string msj = "Archivo  guardado correctamente.";

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

        [HttpDelete("Eliminar")]
        public string Eliminar(int idArchivo)
        {
            string msj = "Eliminando archivo...";

            try
            {
                var temp = _context.Archivos_Adjuntos_Propuesta.FirstOrDefault(r => r.id_archivo == idArchivo);

                if (temp == null)
                {
                    msj = "No existe el archivo.";
                }
                else
                {

                    _context.Archivos_Adjuntos_Propuesta.Remove(temp);
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
        public string Actualizar(Archivo_adjunto temp)
        {
            string msj = "Actualizando archivo adjunto...";

            try
            {
                var objArchivo = _context.Archivos_Adjuntos_Propuesta.FirstOrDefault(r => r.id_archivo == temp.id_archivo);

                if (objArchivo == null)
                {
                    msj = "No existe el archivo adjunto.";
                }
                else
                {
                    objArchivo.id_propuesta = temp.id_propuesta;
                    objArchivo.nombre_archivo = temp.nombre_archivo;
                    objArchivo.tipo_archivo = temp.tipo_archivo;
                    objArchivo.ruta_almacenamiento = temp.ruta_almacenamiento;

                    _context.Archivos_Adjuntos_Propuesta.Update(objArchivo);
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

    }


}
