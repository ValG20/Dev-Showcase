using AppWebEditorial.Models;
using EditorialUCR.Models;
using EditorialUCR.Models.Api;
using Microsoft.AspNetCore.Mvc;

namespace EditorialUCR.Controllers
{
    public class Actas_SesionController : Controller
    {
        private AppWebEditorial.Models.ApiEditorial _client = null;
        private HttpClient _api = null;

        public Actas_SesionController()
        {
            _client = new AppWebEditorial.Models.ApiEditorial();
            _api = _client.IniciarApi();
        }

        [HttpGet]
        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Crear(Actas_Sesion propuesta)
        {
            try
            {
                // Obtener la lista completa de actas desde la API
                var response = await _api.GetAsync("api/Actas_Sesion");

                int ultimoId = 1;

                if (response.IsSuccessStatusCode)
                {
                    var listaJson = await response.Content.ReadAsStringAsync();
                    var lista = System.Text.Json.JsonSerializer.Deserialize<List<Actas_Sesion>>(listaJson);

                    if (lista != null && lista.Count > 0)
                    {
                        ultimoId = lista.Max(a => a.id_acta);
                    }
                }

                // Asignar ID manual (último + 1)
                int idAsignar = ultimoId + 1;
                propuesta.id_acta = idAsignar;

                // Guardar la propuesta
                var agregar = await _api.PostAsJsonAsync("api/Actas_Sesion/Guardar", propuesta);

                if (agregar.IsSuccessStatusCode)
                {
                    TempData["Success"] = "La propuesta se ha creado correctamente.";
                    return RedirectToAction("Crear", "Acta_Propuesta", new { id_acta = idAsignar });
                }
                else
                {
                    var errorMsg = await agregar.Content.ReadAsStringAsync();
                    TempData["Error"] = "Error al guardar la propuesta. " + errorMsg;
                    return View(propuesta);
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Ocurrió un error: " + ex.Message;
                return View(propuesta);
            }
        }


    }
}
