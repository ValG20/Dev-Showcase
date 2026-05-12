using APIEditorialUCR.Model;
using Microsoft.AspNetCore.Mvc;
using Sistema_Editorial.Model;

namespace APIEditorialUCR.Controllers.Valeria
{
    [ApiController]
    [Route("[controller]")]
    public class ManuscritoController : Controller
    {
        private readonly DbContextEditorial _context = null;

        public ManuscritoController(DbContextEditorial pContext)
        {
            _context = pContext;
        }

        [HttpGet("Lista")]
        public List<Manuscrito> Lista()
        {
            List<Manuscrito> temp = _context.Manuscritos.ToList();
            return temp;
        }

        [HttpGet("Buscar")]
        public IActionResult Buscar(int id)
        {
            var temp = _context.Manuscritos.FirstOrDefault(x => x.id_manuscrito == id);

            if (temp == null)
            {
                return NotFound($"No existe un manuscrito con el identificador {id}.");
            }

            return Ok(temp);
        }

        [HttpPost("Guardar")]
        public string Guardar(Manuscrito temp)
        {
            string msj = "Manuscrito guardado correctamente.";

            try
            {
                _context.Manuscritos.Add(temp);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                msj = ex.InnerException?.Message ?? ex.Message;
            }

            return msj;
        }

        [HttpDelete("Eliminar")]
        public string Eliminar(int idManuscrito)
        {
            string msj = "Eliminando manuscrito...";

            try
            {
                var temp = _context.Manuscritos.FirstOrDefault(r => r.id_manuscrito == idManuscrito);

                if (temp == null)
                {
                    msj = "No existe el manuscrito.";
                }
                else
                {
                    _context.Manuscritos.Remove(temp);
                    _context.SaveChanges();
                    msj = "Manuscrito eliminado correctamente.";
                }
            }
            catch (Exception ex)
            {
                msj = ex.InnerException?.Message ?? ex.Message;
            }

            return msj;
        }

        [HttpPut("Actualizar")]
        public string Actualizar(Manuscrito temp)
        {
            string msj = "Actualizando manuscrito...";

            try
            {
                var objManuscrito = _context.Manuscritos.FirstOrDefault(r => r.id_manuscrito == temp.id_manuscrito);

                if (objManuscrito == null)
                {
                    msj = "No existe el manuscrito.";
                }
                else
                {
                    objManuscrito.id_propuesta = temp.id_propuesta;
                    objManuscrito.id_autor = temp.id_autor;
                    objManuscrito.titulo_trabajo = temp.titulo_trabajo;
                    objManuscrito.fecha_envio = temp.fecha_envio;
                    objManuscrito.estado = temp.estado;
                    objManuscrito.observaciones_generales = temp.observaciones_generales;

                    _context.Manuscritos.Update(objManuscrito);
                    _context.SaveChanges();

                    msj = "Manuscrito actualizado correctamente.";
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
