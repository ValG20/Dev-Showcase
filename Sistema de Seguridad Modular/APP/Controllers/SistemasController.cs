using AppSeguridad.Models;
using AppWebSeguridad.Models;
using AppWebSeguridad.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace AppWebSeguridad.Controllers
{
    public class SistemasController : Controller
    {
        private readonly ApiSeguridad _client;
        private readonly HttpClient _api;

        public SistemasController()
        {
            _client = new ApiSeguridad();
            _api = _client.IniciarApi();
        }

        //Metodo para guardar en Bitacora
        private async Task<bool> GuardarBitacoraSimpleAsync(string accion, string detalle, string rutaPantalla)
        {
            try
            {
                int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");
                if (idUsuario == null || string.IsNullOrWhiteSpace(rutaPantalla))
                    return false;

                // Obtener idPantalla desde la ruta

                var respPantalla = await _api.GetAsync($"Pantallas/SearchByRuta?idSistema={8080}&ruta={Uri.EscapeDataString(rutaPantalla)}");

                if (!respPantalla.IsSuccessStatusCode)
                    return false;

                var pantallaJson = await respPantalla.Content.ReadAsStringAsync();
                var pantalla = JsonConvert.DeserializeObject<Pantalla>(pantallaJson);

                if (pantalla == null) return false;

                var bitacora = new
                {
                    idUsuario = idUsuario.Value,
                    idSistema = 8080,
                    idPantalla = pantalla.idPantalla,
                    fecha = DateTime.Now,
                    accion = accion,
                    detalle = detalle
                };

                var content = new StringContent(JsonConvert.SerializeObject(bitacora), Encoding.UTF8, "application/json");
                var response = await _api.PostAsync("Bitacoras/Save", content);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al registrar bitácora: {ex.Message}");
                return false;
            }
        }

        [Authorize] 
        [HttpGet]
        public async Task<IActionResult> List()
        {
            if (!await TienePermisoAsync("/Sistemas/List", 8080))
            {
                return RedirectToAction("AccessDenied", "Usuarios");
            }

            try
            {
                // Obtener permisos de la sesión
                var permisosJson = HttpContext.Session.GetString("Permisos");
                var permisos = JsonConvert.DeserializeObject<List<PermisoVista>>(permisosJson);


                var idPantallaResponse = await _api.GetAsync($"Pantallas/SearchByRuta?idSistema=8080&ruta={Uri.EscapeDataString("/Sistemas/List")}");

                if (idPantallaResponse.IsSuccessStatusCode)
                {
                    var pantalla = await idPantallaResponse.Content.ReadFromJsonAsync<Pantalla>();
                    int idPantallaRoles = pantalla.idPantalla;

                    var permisoPantalla = permisos.FirstOrDefault(p => p.IdPantalla == idPantallaRoles);

                    ViewBag.PuedeInsertar = permisoPantalla?.PuedeInsertar ?? false;
                    ViewBag.PuedeModificar = permisoPantalla?.PuedeModificar ?? false;
                    ViewBag.PuedeBorrar = permisoPantalla?.PuedeBorrar ?? false;
                    ViewBag.PuedeConsultar = permisoPantalla?.PuedeConsultar ?? false;
                }

                // Llamada a la API para obtener la lista
                HttpResponseMessage response = await _api.GetAsync("Sistema/List");

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    var listado = JsonConvert.DeserializeObject<List<Sistema>>(result);

                    return View(listado);
                }

                TempData["ErrorMessage"] = "Error al obtener la lista de sistemas";
                return View(new List<Sistema>());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en List: {ex.Message}");
                TempData["ErrorMessage"] = "Error interno al obtener la lista";
                return View(new List<Sistema>());
            }
        }

        [Authorize] // Aquí ya no hace falta poner Roles porque ya el token define los permisos
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            if (!await TienePermisoAsync("/Sistemas/Create", 8080))
            {
                return RedirectToAction("AccessDenied", "Usuarios");
            }
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Sistema temp)
        {
            if (!await TienePermisoAsync("/Sistemas/Create", 8080))
            {
                return RedirectToAction("AccessDenied", "Usuarios");
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(temp);
                }

                // Aquí haces la llamada al API externo
                var resultado = await _api.PostAsJsonAsync("Sistema/Save", temp);

                if (resultado.IsSuccessStatusCode)
                {
                    await GuardarBitacoraSimpleAsync("Agregar", $"Se Agrego el Sistema '{temp.nombre}'", "/Sistemas/Create");

                    TempData["SuccessMessage"] = "Sistema creado correctamente";
                    return RedirectToAction("List");
                }

                string respuesta = await resultado.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, $"Error: {respuesta}");
                return View(temp);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en Create: {ex.Message}");
                TempData["ErrorMessage"] = "Error interno al crear el sistema";
                return View(temp);
            }
        }


        [Authorize] // Aquí ya no hace falta poner Roles porque ya el token define los permisos
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            if (!await TienePermisoAsync("/Sistemas/Details", 8080))
            {
                return RedirectToAction("AccessDenied", "Usuarios");
            }
            try
            {
                HttpResponseMessage response = await _api.GetAsync($"Sistema/SearchID?id={id}");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var sistem = JsonConvert.DeserializeObject<Sistema>(json);
                    return View(sistem);
                }

                TempData["ErrorMessage"] = $"No se encontró ningún sistema con el id {id}";
                return RedirectToAction("List");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en Details: {ex.Message}");
                TempData["ErrorMessage"] = "Error interno al obtener detalles";
                return RedirectToAction("List");
            }
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (!await TienePermisoAsync("/Sistemas/Edit", 8080))
            {
                return RedirectToAction("AccessDenied", "Usuarios");
            }
            try
            {
                HttpResponseMessage response = await _api.GetAsync($"Sistema/SearchID?id={id}");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var sistem = JsonConvert.DeserializeObject<Sistema>(json);
                    return View(sistem);
                }

                TempData["ErrorMessage"] = $"No se encontró ningún sistema con el id {id}";
                return RedirectToAction("List");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en Edit (GET): {ex.Message}");
                TempData["ErrorMessage"] = "Error interno al cargar la edición";
                return RedirectToAction("List");
            }
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Sistema temp)
        {
            if (!await TienePermisoAsync("/Sistemas/Edit", 8080))
            {
                return RedirectToAction("AccessDenied", "Usuarios");
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(temp);
                }

                HttpResponseMessage response = await _api.PutAsJsonAsync("Sistema/Update", temp);

                if (response.IsSuccessStatusCode)
                {

                    await GuardarBitacoraSimpleAsync("Editar", $"Se Edito el Sistema '{temp.nombre}'", "/Sistemas/Edit");

                    TempData["SuccessMessage"] = "Sistema creado correctamente";
                    return RedirectToAction("List");
                }

                string error = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, $"Error al actualizar: {error}");
                return View(temp);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en Edit (POST): {ex.Message}");
                TempData["ErrorMessage"] = "Error interno al editar el sistema";
                return View(temp);
            }
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            if (!await TienePermisoAsync("/Sistemas/Delete", 8080))
            {
                return RedirectToAction("AccessDenied", "Usuarios");
            }
            try
            {
                HttpResponseMessage response = await _api.GetAsync($"Sistema/SearchID?id={id}");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var sistema = JsonConvert.DeserializeObject<Sistema>(json);
                    return View(sistema);
                }

                TempData["ErrorMessage"] = $"No se encontró ningún sistema con el id {id}";
                return RedirectToAction("List");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en Delete (GET): {ex.Message}");
                TempData["ErrorMessage"] = "Error interno al cargar la eliminación";
                return RedirectToAction("List");
            }
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int idSistema)
        {
            if (!await TienePermisoAsync("/Sistemas/Delete", 8080))
            {
                return RedirectToAction("AccessDenied", "Usuarios");
            }

            try
            {
                // Obtener el sistema para bitácora
                var responseGet = await _api.GetAsync($"Sistema/SearchID?id={idSistema}");
                if (!responseGet.IsSuccessStatusCode)
                {
                    TempData["ErrorMessage"] = "El sistema no existe o ya fue eliminado";
                    return RedirectToAction("List");
                }

                var json = await responseGet.Content.ReadAsStringAsync();
                var sistema = JsonConvert.DeserializeObject<Sistema>(json);

                // Eliminar el sistema
                var responseDelete = await _api.DeleteAsync($"Sistema/Delete?pIdSistema={idSistema}");
                if (responseDelete.IsSuccessStatusCode)
                {
                    await GuardarBitacoraSimpleAsync("Eliminar", $"Se eliminó el Sistema '{sistema.nombre}'", "/Sistemas/Delete");
                    TempData["SuccessMessage"] = "Sistema eliminado correctamente";
                }
                else
                {
                    TempData["ErrorMessage"] = "Ocurrió un error al eliminar el sistema";
                }

                return RedirectToAction("List");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en DeleteConfirmed: {ex.Message}");
                TempData["ErrorMessage"] = "Error interno al eliminar el sistema";
                return RedirectToAction("List");
            }
        }


        private async Task<bool> TienePermisoAsync(string rutaPantalla, int idSistema)
        {
            var token = HttpContext.Session.GetString("JWToken");
            if (string.IsNullOrEmpty(token)) return false;

            _api.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Codificar la ruta para que sea segura en la URL
            var encodedRuta = Uri.EscapeDataString(rutaPantalla);
            var response = await _api.GetAsync($"Pantallas/SearchByRuta?idSistema={idSistema}&ruta={encodedRuta}");

            if (!response.IsSuccessStatusCode)
                return false;

            var jsonPantalla = await response.Content.ReadAsStringAsync();
            var pantalla = JsonConvert.DeserializeObject<Pantalla>(jsonPantalla);

            if (pantalla == null || pantalla.idPantalla == 0)
                return false;

            var permisosJson = HttpContext.Session.GetString("Permisos");
            if (string.IsNullOrEmpty(permisosJson)) return false;

            var permisos = JsonConvert.DeserializeObject<List<PermisoVista>>(permisosJson);

            var permiso = permisos.FirstOrDefault(p =>
                p.IdPantalla == pantalla.idPantalla && p.PuedeConsultar);

            return permiso != null;
        }


    }//Fin class 
}
