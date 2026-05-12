using Microsoft.AspNetCore.Mvc;
using Sistema_Editorial.Model;
using Sistema_Editorial.Model.Valeria.Impresion;
using Microsoft.EntityFrameworkCore;
using APIEditorialUCR.Model;

namespace APIEditorialUCR.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class Ejemplar_CostoController : Controller
    {
        private readonly DbContextEditorial _context;

        public Ejemplar_CostoController(DbContextEditorial context)
        {
            _context = context;
        }

        [HttpGet("Lista")]
        public List<Ejemplar_Costo> Lista()
        {
            return _context.Ejemplares_Costos
                .Include(e => e.id_pedido)
                .ToList();
        }

        [HttpGet("Buscar")]
        public IActionResult Buscar(int id)
        {
            var temp = _context.Ejemplares_Costos
                .Include(e => e.id_pedido)
                .FirstOrDefault(x => x.id_ejemplar == id);

            if (temp == null)
                return NotFound($"No existe un ejemplar con el ID {id}.");

            return Ok(temp);
        }

        [HttpPost("Guardar")]
        public string Guardar(Ejemplar_Costo temp)
        {
            string msj = "Ejemplar guardado correctamente.";

            try
            {
                int nuevoId = 1;
                if (_context.Ejemplares_Costos.Any())
                    nuevoId = _context.Ejemplares_Costos.Max(e => e.id_ejemplar) + 1; // Ajustar según tu campo PK
                temp.id_ejemplar = nuevoId;

                _context.Ejemplares_Costos.Add(temp);
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
            string msj = "Eliminando ejemplar...";

            try
            {
                var temp = _context.Ejemplares_Costos.FirstOrDefault(r => r.id_ejemplar == id);

                if (temp == null)
                {
                    msj = "No existe el ejemplar.";
                }
                else
                {
                    _context.Ejemplares_Costos.Remove(temp);
                    _context.SaveChanges();
                    msj = "Ejemplar eliminado correctamente.";
                }
            }
            catch (Exception ex)
            {
                msj = ex.InnerException?.Message ?? ex.Message;
            }

            return msj;
        }

        [HttpPut("Actualizar")]
        public string Actualizar(Ejemplar_Costo temp)
        {
            string msj = "Actualizando ejemplar...";

            try
            {
                var obj = _context.Ejemplares_Costos.FirstOrDefault(r => r.id_ejemplar == temp.id_ejemplar);

                if (obj == null)
                {
                    msj = "No existe el ejemplar.";
                }
                else
                {
                    obj.id_pedido = temp.id_pedido;
                    obj.cantidad = temp.cantidad;
                    obj.costo = temp.costo;

                    _context.Ejemplares_Costos.Update(obj);
                    _context.SaveChanges();

                    msj = "Ejemplar actualizado correctamente.";
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
