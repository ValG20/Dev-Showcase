using APIEditorialUCR.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sistema_Editorial.Model;

namespace APIEditorialUCR.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DisenoController : Controller
    {
        private readonly DbContextEditorial _context = null;

        public DisenoController(DbContextEditorial pContext)
        {
            _context = pContext;
        }

        [HttpGet("Lista")]
        public List<Diseno> Lista()
        {
            List<Diseno> temp = _context.Disenos.ToList();

            return temp;
        }

        [HttpGet("Buscar")]
        public IActionResult Buscar(int id)
        {
            var temp = _context.Disenos.FirstOrDefault(x => x.id_diseno == id);

            if (temp == null)
            {
                return NotFound($"No existe un diseño con el identificador {id}.");
            }

            return Ok(temp);
        }

        [HttpPost("Guardar")]
        public string Guardar(Diseno temp)
        {
            string msj = "Archivo  guardado correctamente.";

            try
            {
                _context.Disenos.Add(temp);

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
                var temp = _context.Disenos.FirstOrDefault(r => r.id_diseno == idArchivo);

                if (temp == null)
                {
                    msj = "No existe el archivo.";
                }
                else
                {

                    _context.Disenos.Remove(temp);
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
        public string Actualizar(Diseno temp)
        {
            string msj = "Actualizando diseño...";

            try
            {
                var objDiseno = _context.Disenos.FirstOrDefault(r => r.id_diseno == temp.id_diseno);

                if (objDiseno == null)
                {
                    msj = "No existe el diseño.";
                }
                else
                {
                    objDiseno.id_usuario = temp.id_usuario;
                    objDiseno.id_propuesta = temp.id_propuesta;
                    objDiseno.fecha_envio = temp.fecha_envio;
                    objDiseno.descripcion = temp.descripcion;
                    objDiseno.estado = temp.estado;
                    objDiseno.observaciones = temp.observaciones;

                    _context.Disenos.Update(objDiseno);
                    _context.SaveChanges();

                    msj = "Diseño actualizado correctamente.";
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
