using EditorialUCR.Models;
using EditorialUCR.Models.Api;
using EditorialUCR.Models.Central;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace EditorialUCR.Controllers
{
    public class ProductosController : Controller
    {
        private ApiEditorial client = null;
        private HttpClient api = null;

        public ProductosController()
        {
            client = new ApiEditorial();
            api = client.IniciarApi();
        }


        public async Task<IActionResult> Catalogo()
        {
            List<Producto> catalogos = new List<Producto>();

            try
            {
                HttpResponseMessage response = await api.GetAsync("Productos/Lista");

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    catalogos = JsonConvert.DeserializeObject<List<Producto>>(result);
                }

                // Si no hay datos, mostramos mensaje
                if (catalogos == null || catalogos.Count == 0)
                {
                    ViewBag.Mensaje = "No hay pedidos registrados actualmente.";
                    catalogos = new List<Producto>();
                }
            }
            catch (Exception ex)
            {
                ViewBag.Mensaje = $"Error al obtener los productos: {ex.InnerException?.Message ?? ex.Message}";
            }

            // Enviamos la lista (aunque esté vacía) a la vista Productos.cshtml
            return View("Catalogo", catalogos);
        }

    }
}
