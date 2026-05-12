using APIBitacoras.Model;
using APIEditorialUCR.Model;
using Microsoft.AspNetCore.Mvc;
using Sistema_Editorial.Model;

namespace APIEditorialUCR.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductosController : Controller
    {
        private readonly DbContextEditorial _context = null;


        public ProductosController(DbContextEditorial pContext)
        {
            _context = pContext;
        }

        [HttpGet("Lista")]
        public List<Producto> Lista()
        {

            List<Producto> temp = _context.Productos.ToList();

            return temp;
        }

        [HttpPost("Guardar")]
        public string Guardar(Producto temp)
        {
            string msj = "Catalogo guardado correctamente.";
            int maxId = 0;

            try
            {
                List<Producto> lista = _context.Productos.ToList();

                if (lista != null && lista.Count > 0)
                {
                    maxId = lista.Max(p => p.Id);
                }

                temp.Id = maxId + 1;

                _context.Productos.Add(temp);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                msj = $"Error al guardar: {ex.InnerException?.Message ?? ex.Message}";
            }

            return msj;
        }


        [HttpGet("BuscarEnCatalogo")]
        public ActionResult<Producto> BuscarEnCatalogo(int id)
        {
            Producto temp = _context.Productos.FirstOrDefault(x => x.Id == id);

            if (temp == null)
            {
                return NotFound(new { Message = "No existe ningun producto de catalogo con ese ID." });
            }

            return Ok(temp);
        }



    }
}
