using AppSeguridad.Models;
using AppWebSeguridad.Models;
using AppWebSeguridad.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace AppWebSeguridad.Controllers
{
    public class PantallasController : Controller
    {
        private ApiSeguridad _client = null;

        private HttpClient _api = null;

        public PantallasController()
        {
            _client = new ApiSeguridad();
            _api = _client.IniciarApi();
        }

        [Authorize]
[HttpGet]
public async Task<IActionResult> List(int? idSistema, string ruta)
{
    if (!await TienePermisoAsync("/Pantallas/List", 8080))
    {
        return RedirectToAction("AccessDenied", "Usuarios");
    }

    try
    {
        // Obtener permisos de la sesión
        var permisosJson = HttpContext.Session.GetString("Permisos");
        var permisos = JsonConvert.DeserializeObject<List<PermisoVista>>(permisosJson);

        var idPantallaResponse = await _api.GetAsync($"Pantallas/SearchByRuta?idSistema={8080}&ruta={Uri.EscapeDataString("/Pantallas/List")}");
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

        // Obtener sistemas
        var sistemas = await ObtenerSistemas();

        // Añadir opción "Todos"
        var sistemasConVacio = new List<Sistema> { new Sistema { idSistema = 0, nombre = "-- Todos --" } };
        sistemasConVacio.AddRange(sistemas);

        ViewBag.Sistemas = new SelectList(sistemasConVacio, "idSistema", "nombre", idSistema ?? 0);

        // Obtener listado completo de pantallas
        List<Pantalla> listado = new List<Pantalla>();
        var response = await _api.GetAsync("Pantallas/List");
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadAsStringAsync();
            listado = JsonConvert.DeserializeObject<List<Pantalla>>(result);

            // Filtrar por sistema si aplica
            if (idSistema.HasValue && idSistema.Value != 0)
            {
                listado = listado.Where(p => p.idSistema == idSistema.Value).ToList();
            }

            // Filtrar por ruta si aplica
            if (!string.IsNullOrEmpty(ruta))
            {
                listado = listado.Where(p => p.Ruta == ruta).ToList();
            }
        }

        // Filtrar rutas disponibles para el combo, según sistema seleccionado
        List<string> rutasDisponibles = new List<string>();
        if (idSistema.HasValue && idSistema.Value != 0)
        {
            rutasDisponibles = listado.Select(p => p.Ruta)
                                     .Where(r => !string.IsNullOrEmpty(r))
                                     .Distinct()
                                     .ToList();
        }

        ViewBag.Rutas = new SelectList(rutasDisponibles, ruta ?? "");

        // Diccionario para mostrar nombres de sistemas en tabla
        ViewBag.SistemasDic = sistemas.ToDictionary(s => s.idSistema, s => s.nombre);

        // Guardar valores seleccionados para mantenerlos en la vista
        ViewBag.SelectedSistema = idSistema ?? 0;
        ViewBag.SelectedRuta = ruta ?? "";

        return View(listado);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error en List (Pantallas): {ex.Message}");
        TempData["ErrorMessage"] = "Error interno al obtener la lista de pantallas";
        return View(new List<Pantalla>());
    }
}

        // Método auxiliar para obtener sistemas
        private async Task<List<Sistema>> ObtenerSistemas()
        {
            var response = await _api.GetAsync("Sistema/List");
            if (!response.IsSuccessStatusCode)
                return new List<Sistema>();

            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<Sistema>>(json);
        }



        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Agregar()
        {
            if (!await TienePermisoAsync("/Pantallas/Agregar", 8080))
            {
                return RedirectToAction("AccessDenied", "Usuarios");
            }

            // Cargar lista de sistemas
            var sistemasResponse = await _api.GetAsync("Sistema/List");
            if (sistemasResponse.IsSuccessStatusCode)
            {
                var sistemasJson = await sistemasResponse.Content.ReadAsStringAsync();
                var sistemas = JsonConvert.DeserializeObject<List<Sistema>>(sistemasJson);
                ViewBag.Sistemas = new SelectList(sistemas, "idSistema", "nombre");
            }
            else
            {
                ViewBag.Sistemas = new SelectList(Enumerable.Empty<SelectListItem>());
            }

            return View();
        }


        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Agregar([Bind] Pantalla temp)
        {
            if (!await TienePermisoAsync("/Pantallas/Agregar", 8080))
            {
                return RedirectToAction("AccessDenied", "Usuarios");
            }
            HttpResponseMessage responseList = await _api.GetAsync("Pantallas/List");

            if (responseList.IsSuccessStatusCode)
            {
                var json = await responseList.Content.ReadAsStringAsync();
                var PantallaExistente = JsonConvert.DeserializeObject<List<Pantalla>>(json);

                // Verificar si ya existe una pantalla con el mismo idPantalla Y idSistema
                bool Existe = PantallaExistente.Any(p => p.idPantalla == temp.idPantalla && p.idSistema == temp.idSistema);

                if (Existe)
                {
                    ModelState.AddModelError("idPantalla", "Ya existe una Pantalla con este ID en el sistema seleccionado.");
                    return View(temp); // Retorna la vista con el error si ya existe
                }

                // Paso 3: Crear el cliente si no hay duplicados
                var jsonContent = new StringContent(JsonConvert.SerializeObject(temp), Encoding.UTF8, "application/json");
                HttpResponseMessage response = await _api.PostAsync("Pantallas/Save", jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    

                    //sistema
                    var Sistemaresponse = await _api.GetAsync($"Sistema/SearchID?id={temp.idSistema}");
                    var sistemaJson = await Sistemaresponse.Content.ReadAsStringAsync();
                    var sistema = JsonConvert.DeserializeObject<Sistema>(sistemaJson);

                    //Pantalla
                    var Pantallaresponse = await _api.GetAsync($"Pantallas/SearchID?idPantalla={temp.idPantalla}&idSistema={temp.idSistema}");
                    var PantallaJson = await Pantallaresponse.Content.ReadAsStringAsync();
                    var pantalla = JsonConvert.DeserializeObject<Pantalla>(PantallaJson);

                    await GuardarBitacoraSimpleAsync("Agregar", $"Se Agrego la Pantalla '{pantalla.Nombre} al Sistema '{sistema.nombre}' ", "/Pantallas/Agregar");
                    TempData["SuccessMessage"] = await response.Content.ReadAsStringAsync();
                    return RedirectToAction("List", "Pantallas");
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError(string.Empty, $"Error al guardar: {error}");
                    return View(temp);
                }
            }

            ModelState.AddModelError(string.Empty, "Error al obtener la lista de usuarios.");
            return View(temp);

        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Eliminar(int idPantalla, int idSistema)
        {
            if (!await TienePermisoAsync("/Pantallas/Eliminar", 8080))
            {
                return RedirectToAction("AccessDenied", "Usuarios");
            }
            var response = await _api.GetAsync($"Pantallas/SearchID?idPantalla={idPantalla}&idSistema={idSistema}");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                Pantalla pantalla = JsonConvert.DeserializeObject<Pantalla>(json);
                return View(pantalla);
            }

            TempData["ErrorMessage"] = "No se encontró la pantalla.";
            return RedirectToAction("List", new { idSistema });
        }


        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarConfirmar(Pantalla pantalla)
        {
            if (!await TienePermisoAsync("/Pantallas/Eliminar", 8080))
            {
                return RedirectToAction("AccessDenied", "Usuarios");
            }

            int idPantalla = pantalla.idPantalla;
            int idSistema = pantalla.idSistema;

            //sistema
            var Sistemaresponse = await _api.GetAsync($"Sistema/SearchID?id={idSistema}");
            var sistemaJson = await Sistemaresponse.Content.ReadAsStringAsync();
            var sistema = JsonConvert.DeserializeObject<Sistema>(sistemaJson);

            //Pantalla
            var Pantallaresponse = await _api.GetAsync($"Pantallas/SearchID?idPantalla={idPantalla}&idSistema={idSistema}");
            var PantallaJson = await Pantallaresponse.Content.ReadAsStringAsync();
            var pantallaname = JsonConvert.DeserializeObject<Pantalla>(PantallaJson);

            if (Pantallaresponse.IsSuccessStatusCode && Sistemaresponse.IsSuccessStatusCode)
            {
                await GuardarBitacoraSimpleAsync("Eliminar", $"Se Elimino la Pantalla '{pantallaname.Nombre} del Sistema '{sistema.nombre}' ", "/Pantallas/Eliminar");
            }
           

            if (idPantalla == 0 || idSistema == 0)
            {
                TempData["Error"] = "Datos incompletos para la eliminación.";
                return RedirectToAction("List", new { idSistema });
            }

            var response = await _api.DeleteAsync($"Pantallas/Delete?idPantalla={idPantalla}&idSistema={idSistema}");

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Pantalla eliminada correctamente.";
            }
            else
            {
                TempData["Error"] = "No se pudo eliminar la pantalla.";
            }

            return RedirectToAction("List", new { idSistema });
        }


        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Editar(int idPantalla, int idSistema)
        {
            if (!await TienePermisoAsync("/Pantallas/Editar", 8080))
            {
                return RedirectToAction("AccessDenied", "Usuarios");
            }

            Pantalla pantalla = null;

            // Aquí puedes ajustar la ruta si tu API necesita ambos parámetros
            HttpResponseMessage response = await _api.GetAsync($"Pantallas/SearchID?idPantalla={idPantalla}&idSistema={idSistema}");

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                pantalla = JsonConvert.DeserializeObject<Pantalla>(result);

                // Si quieres guardar el idSistema también para usarlo en la vista:
                ViewBag.idSistema = idSistema;
            }

            return View(pantalla);
        }


        [Authorize]
        [HttpPost]
        public async Task<IActionResult> EditarConfirmar([Bind] Pantalla temp)
        {
            if (!await TienePermisoAsync("/Pantallas/Editar", 8080))
            {
                return RedirectToAction("AccessDenied", "Usuarios");
            }

            HttpResponseMessage response = await _api.PutAsJsonAsync("Pantallas/Update", temp);

            if (response.IsSuccessStatusCode)
            {
                //sistema
                var Sistemaresponse = await _api.GetAsync($"Sistema/SearchID?id={temp.idSistema}");
                var sistemaJson = await Sistemaresponse.Content.ReadAsStringAsync();
                var sistema = JsonConvert.DeserializeObject<Sistema>(sistemaJson);

                //Pantalla
                var Pantallaresponse = await _api.GetAsync($"Pantallas/SearchID?idPantalla={temp.idPantalla}&idSistema={temp.idSistema}");
                var PantallaJson = await Pantallaresponse.Content.ReadAsStringAsync();
                var pantalla = JsonConvert.DeserializeObject<Pantalla>(PantallaJson);

                await GuardarBitacoraSimpleAsync("Editar", $"Se Edito la Pantalla '{pantalla.Nombre} del Sistema '{sistema.nombre}' ", "/Pantallas/Editar");
                return RedirectToAction("List");
            }

            TempData["Error"] = "Error al actualizar el paquete.";
            return View(temp);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Detalles(int idPantalla, int idSistema)
        {
            if (!await TienePermisoAsync("/Pantallas/Detalles", 8080))
            {
                return RedirectToAction("AccessDenied", "Usuarios");
            }
            Pantalla pantalla = null;

            HttpResponseMessage response = await _api.GetAsync($"Pantallas/SearchID?idPantalla={idPantalla}&idSistema={idSistema}");

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                pantalla = JsonConvert.DeserializeObject<Pantalla>(result);

                // Opcional: pasar idSistema a la vista si lo necesitas
                ViewBag.idSistema = idSistema;
            }

            return View(pantalla);
        }

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


    }//cierre class
}
