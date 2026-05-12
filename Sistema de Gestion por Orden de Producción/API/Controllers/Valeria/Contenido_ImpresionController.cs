using Microsoft.AspNetCore.Mvc;
using Sistema_Editorial.Model;
using Sistema_Editorial.Model.Valeria.Impresion;
using Microsoft.EntityFrameworkCore;
using APIEditorialUCR.Model;

namespace APIEditorialUCR.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class Contenido_ImpresionController : Controller
    {
        private readonly DbContextEditorial _context;

        public Contenido_ImpresionController(DbContextEditorial context)
        {
            _context = context;
        }

        [HttpGet("Lista")]
        public List<Contenido_Impresion> Lista()
        {
            return _context.Contenido_Impresion
                .Include(c => c.id_pedido)
                .ToList();
        }

        [HttpGet("Buscar")]
        public IActionResult Buscar(int id)
        {
            var temp = _context.Contenido_Impresion
                .Include(c => c.id_pedido)
                .FirstOrDefault(x => x.id_contenido == id);

            if (temp == null)
                return NotFound($"No existe un contenido de impresión con el ID {id}.");

            return Ok(temp);
        }

        [HttpPost("Guardar")]
        public string Guardar(Contenido_Impresion temp)
        {
            string msj = "Contenido de impresión guardado correctamente.";

            try
            {
                int nuevoId = 1;
                if (_context.Contenido_Impresion.Any())
                    nuevoId = _context.Contenido_Impresion.Max(c => c.id_contenido) + 1;
                temp.id_contenido = nuevoId;

                _context.Contenido_Impresion.Add(temp);
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
            string msj = "Eliminando contenido de impresión...";

            try
            {
                var temp = _context.Contenido_Impresion.FirstOrDefault(r => r.id_contenido == id);

                if (temp == null)
                {
                    msj = "No existe el contenido de impresión.";
                }
                else
                {
                    _context.Contenido_Impresion.Remove(temp);
                    _context.SaveChanges();
                    msj = "Contenido de impresión eliminado correctamente.";
                }
            }
            catch (Exception ex)
            {
                msj = ex.InnerException?.Message ?? ex.Message;
            }

            return msj;
        }

        [HttpPut("Actualizar")]
        public string Actualizar(Contenido_Impresion temp)
        {
            string msj = "Actualizando contenido de impresión...";

            try
            {
                var obj = _context.Contenido_Impresion.FirstOrDefault(r => r.id_contenido == temp.id_contenido);

                if (obj == null)
                {
                    msj = "No existe el contenido de impresión.";
                }
                else
                {
                    obj.id_pedido = temp.id_pedido;
                    obj.tipo_impresion = temp.tipo_impresion;
                    obj.tipo_papel = temp.tipo_papel;
                    obj.tamano_corte = temp.tamano_corte;
                    obj.salen_tc = temp.salen_tc;

                    _context.Contenido_Impresion.Update(obj);
                    _context.SaveChanges();

                    msj = "Contenido de impresión actualizado correctamente.";
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
