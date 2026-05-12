using AppWebEditorial.Models;
using EditorialUCR.Models;
using EditorialUCR.Models.Api;
using Microsoft.AspNetCore.Mvc;

namespace EditorialUCR.Controllers
{
    public class Acta_PropuestaController : Controller
    {
        private AppWebEditorial.Models.ApiEditorial _client = null;
        private HttpClient _api = null;

        public Acta_PropuestaController()
        {
            _client = new AppWebEditorial.Models.ApiEditorial();
            _api = _client.IniciarApi();
        }

        [HttpGet]
        public IActionResult Crear(int id_acta)
        {
            ViewBag.id_acta = id_acta;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Crear(
      Acta_Propuesta propuesta,
      IFormFile dictamen,
      IFormFile observaciones_comite,
            int id_acta
  )
        {
           
            propuesta.id_acta = id_acta;
            if (dictamen == null || dictamen.Length == 0)
            {
                ModelState.AddModelError("dictamen", "Debe subir el dictamen");
                return View(propuesta);
            }

            if (observaciones_comite == null || observaciones_comite.Length == 0)
            {
                ModelState.AddModelError("observaciones_comite", "Debe subir las observaciones.");
                return View(propuesta);
            }

            using (var ms = new MemoryStream())
            {
                await dictamen.CopyToAsync(ms);
                propuesta.dictamen = Convert.ToBase64String(ms.ToArray());
            }

            using (var ms = new MemoryStream())
            {
                await observaciones_comite.CopyToAsync(ms);
                propuesta.observaciones_comite = Convert.ToBase64String(ms.ToArray());
            }

            propuesta.id_propuesta = 1;

            // ======= ENVIAR A LA API =======
            var agregar = await _api.PostAsJsonAsync("api/Acta_Propuesta", propuesta);

            if (agregar.IsSuccessStatusCode)
            {
                TempData["Success"] = "La propuesta se ha creado correctamente.";
                return RedirectToAction("PedidosPendientes","Pedidos");
            }

            var errorMsg = await agregar.Content.ReadAsStringAsync();
            TempData["Error"] = "Error al guardar la propuesta. " + errorMsg;

            return View(propuesta);
        }



    }
}
