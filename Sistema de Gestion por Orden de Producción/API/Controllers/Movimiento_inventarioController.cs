using APIEditorialUCR.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sistema_Editorial.Model;

namespace APIEditorialUCR.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class Movimiento_inventarioController : Controller
    {
        private readonly DbContextEditorial _context = null;

        public Movimiento_inventarioController(DbContextEditorial pContext)
        {
            _context = pContext;
        }

        [HttpGet("Lista")]
        public List<Movimiento_inventario> Lista()
        {
            List<Movimiento_inventario> temp = _context.Movimientos_inventarios.ToList();

            return temp;
        }

        [HttpGet("Buscar")]
        public IActionResult Buscar(int id)
        {
            var temp = _context.Movimientos_inventarios.FirstOrDefault(x => x.id_movimiento == id);

            if (temp == null)
            {
                return NotFound($"No existe un movimientos de inventarios con el identificador {id}.");
            }

            return Ok(temp);
        }

        [HttpPost("Guardar")]
        public string Guardar(Movimiento_inventario temp)
        {
            string msj = "Movimiento inventario guardado correctamente.";

            try
            {
                _context.Movimientos_inventarios.Add(temp);

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
            string msj = "Movimiento inventario impresion...";

            try
            {
                var temp = _context.Movimientos_inventarios.FirstOrDefault(r => r.id_movimiento == id);

                if (temp == null)
                {
                    msj = "No existe el Movimiento inventario.";
                }
                else
                {

                    _context.Movimientos_inventarios.Remove(temp);
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
        public string Actualizar(Movimiento_inventario temp)
        {
            string msj = "Actualizando movimiento de inventario...";

            try
            {
                var objMovimiento = _context.Movimientos_inventarios.FirstOrDefault(r => r.id_movimiento == temp.id_movimiento);

                if (objMovimiento == null)
                {
                    msj = "No existe el movimiento de inventario.";
                }
                else
                {
                    objMovimiento.id_producto = temp.id_producto;
                    objMovimiento.tipo_movimiento = temp.tipo_movimiento;
                    objMovimiento.cantidad = temp.cantidad;
                    objMovimiento.usuario_responsable = temp.usuario_responsable;
                    objMovimiento.fecha_movimiento = temp.fecha_movimiento;
                    objMovimiento.observaciones = temp.observaciones;

                    _context.Movimientos_inventarios.Update(objMovimiento);
                    _context.SaveChanges();

                    msj = "Movimiento de inventario actualizado correctamente.";
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
