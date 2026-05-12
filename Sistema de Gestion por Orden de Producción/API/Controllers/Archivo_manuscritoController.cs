using APIEditorialUCR.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sistema_Editorial.Model;

namespace APIEditorialUCR.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class Archivo_manuscritoController : Controller
    {
        private readonly DbContextEditorial _context = null;

        public Archivo_manuscritoController(DbContextEditorial pContext)
        {
            _context = pContext;
        }

        [HttpGet("Lista")]
        public List<Archivo_manuscrito> Lista()
        {
            List<Archivo_manuscrito> temp = _context.Archivo_Manuscrito.ToList();

            return temp;
        }

        [HttpGet("Buscar")]
        public IActionResult Buscar(int id)
        {
            var temp = _context.Archivo_Manuscrito.FirstOrDefault(x => x.id_archivo == id);

            if (temp == null)
            {
                return NotFound($"No existe un archivo manuscrito con el identificador {id}.");
            }

            return Ok(temp);
        }

        [HttpPost("Guardar")]
        public string Guardar(Archivo_manuscrito temp)
        {
            string msj = "Archivo guardado correctamente.";

            try
            {
                _context.Archivo_Manuscrito.Add(temp);

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
                var temp = _context.Archivo_Manuscrito.FirstOrDefault(r => r.id_archivo == idArchivo);

                if (temp == null)
                {
                    msj = "No existe el archivo.";
                }
                else
                {

                    _context.Archivo_Manuscrito.Remove(temp);
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
        public string Actualizar(Archivo_manuscrito temp)
        {
            string msj = "Actualizando archivo de manuscrito...";

            try
            {
                var objArchivo = _context.Archivo_Manuscrito.FirstOrDefault(r => r.id_archivo == temp.id_archivo);

                if (objArchivo == null)
                {
                    msj = "No existe el archivo de manuscrito.";
                }
                else
                {
                    objArchivo.id_manuscrito = temp.id_manuscrito;
                    objArchivo.nombre_archivo = temp.nombre_archivo;
                    objArchivo.tipo_archivo = temp.tipo_archivo;
                    objArchivo.ruta_almacenamiento = temp.ruta_almacenamiento;
                    objArchivo.fecha_subida = temp.fecha_subida;

                    _context.Archivo_Manuscrito.Update(objArchivo);
                    _context.SaveChanges();

                    msj = "Archivo de manuscrito actualizado correctamente.";
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
