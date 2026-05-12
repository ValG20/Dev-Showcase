using Microsoft.AspNetCore.Mvc;
using Sistema_Editorial.Model;
using Sistema_Editorial.Model.Valeria.Impresion;
using Microsoft.EntityFrameworkCore;
using APIEditorialUCR.Model;

namespace APIEditorialUCR.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class Acabado_FinalController : Controller
    {
        private readonly DbContextEditorial _context;

        public Acabado_FinalController(DbContextEditorial context)
        {
            _context = context;
        }

        [HttpGet("Lista")]
        public List<Acabado_Final> Lista()
        {
            return _context.Acabados_Finales
                .Include(a => a.id_pedido)
                .ToList();
        }

        [HttpGet("Buscar")]
        public IActionResult Buscar(int id)
        {
            var temp = _context.Acabados_Finales
                .Include(a => a.id_pedido)
                .FirstOrDefault(x => x.id_acabado == id);

            if (temp == null)
                return NotFound($"No existe un acabado final con el ID {id}.");

            return Ok(temp);
        }

        [HttpPost("Guardar")]
        public string Guardar(Acabado_Final temp)
        {
            string msj = "Acabado final guardado correctamente.";

            try
            {
                int nuevoId = 1;
                if (_context.Acabados_Finales.Any())
                    nuevoId = _context.Acabados_Finales.Max(a => a.id_acabado) + 1;
                temp.id_acabado = nuevoId;

                _context.Acabados_Finales.Add(temp);
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
            string msj = "Eliminando acabado final...";

            try
            {
                var temp = _context.Acabados_Finales.FirstOrDefault(r => r.id_acabado == id);

                if (temp == null)
                {
                    msj = "No existe el acabado final.";
                }
                else
                {
                    _context.Acabados_Finales.Remove(temp);
                    _context.SaveChanges();
                    msj = "Acabado final eliminado correctamente.";
                }
            }
            catch (Exception ex)
            {
                msj = ex.InnerException?.Message ?? ex.Message;
            }

            return msj;
        }

        [HttpPut("Actualizar")]
        public string Actualizar(Acabado_Final temp)
        {
            string msj = "Actualizando acabado final...";

            try
            {
                var obj = _context.Acabados_Finales.FirstOrDefault(r => r.id_acabado == temp.id_acabado);

                if (obj == null)
                {
                    msj = "No existe el acabado final.";
                }
                else
                {
                    obj.id_pedido = temp.id_pedido;
                    obj.cant_copias = temp.cant_copias;
                    obj.tipos = temp.tipos;
                    obj.otro = temp.otro;
                    obj.tecnologia_impresion_especial = temp.tecnologia_impresion_especial;

                    _context.Acabados_Finales.Update(obj);
                    _context.SaveChanges();

                    msj = "Acabado final actualizado correctamente.";
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
