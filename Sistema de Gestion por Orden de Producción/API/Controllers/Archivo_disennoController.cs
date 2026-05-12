using APIEditorialUCR.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sistema_Editorial.Model;

namespace APIEditorialUCR.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class Archivo_disennoController : Controller
    {
        private readonly DbContextEditorial _context = null;

        public Archivo_disennoController(DbContextEditorial pContext)
        {
            _context = pContext;
        }

        [HttpGet("Lista")]
        public List<Archivo_disenno> Lista()
        {
            List<Archivo_disenno> temp = _context.Archivos_Disenos.ToList();

            return temp;
        }

        [HttpGet("Buscar")]
        public IActionResult Buscar(int id)
        {
            var temp = _context.Archivos_Disenos.FirstOrDefault(x => x.id_archivo == id);

            if (temp == null)
            {
                return NotFound($"No existe un archivo de diseño con el identificador {id}.");
            }

            return Ok(temp);
        }


        [HttpPost("Guardar")]
        public string Guardar(Archivo_disenno temp)
        {
            string msj = "Archivo  guardado correctamente.";

            try
            {
                _context.Archivos_Disenos.Add(temp);

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
                var temp = _context.Archivos_Disenos.FirstOrDefault(r => r.id_archivo == idArchivo);

                if (temp == null)
                {
                    msj = "No existe el archivo.";
                }
                else
                {

                    _context.Archivos_Disenos.Remove(temp);
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
        public string Actualizar(Archivo_disenno temp)
        {
            string msj = "Actualizando archivo de diseño...";

            try
            {
                var objArchivo = _context.Archivos_Disenos.FirstOrDefault(r => r.id_archivo == temp.id_archivo);

                if (objArchivo == null)
                {
                    msj = "No existe el archivo de diseño.";
                }
                else
                {
                    objArchivo.id_diseno = temp.id_diseno;
                    objArchivo.nombre_archivo = temp.nombre_archivo;
                    objArchivo.tipo_archivo = temp.tipo_archivo;
                    objArchivo.ruta_almacenamiento = temp.ruta_almacenamiento;
                    objArchivo.fecha_subida = temp.fecha_subida;

                    _context.Archivos_Disenos.Update(objArchivo);
                    _context.SaveChanges();

                    msj = "Archivo de diseño actualizado correctamente.";
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
