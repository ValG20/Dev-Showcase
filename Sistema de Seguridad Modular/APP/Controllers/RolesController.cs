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
    public class RolesController : Controller
    {
        private ApiSeguridad _client = null;

        private HttpClient _api = null;

        public RolesController()
        {
            _client = new ApiSeguridad();
            _api = _client.IniciarApi();
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> List()
        {
            if (!await TienePermisoAsync("/Roles/List", 8080))
            {
                return RedirectToAction("AccessDenied", "Usuarios");
            }
            try
            {
                // Obtener permisos de la sesión
                var permisosJson = HttpContext.Session.GetString("Permisos");
                var permisos = JsonConvert.DeserializeObject<List<PermisoVista>>(permisosJson);

                var idPantallaResponse = await _api.GetAsync($"Pantallas/SearchByRuta?idSistema=8080&ruta={Uri.EscapeDataString("/Roles/List")}");

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


                // Llamada a la API
                List<Rol> listado = new List<Rol>();

                HttpResponseMessage response = await _api.GetAsync("Role/List");

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    listado = JsonConvert.DeserializeObject<List<Rol>>(result);
                }

                return View(listado);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en List (Roles): {ex.Message}");
                TempData["ErrorMessage"] = "Error interno al obtener la lista de roles";
                return View(new List<Rol>());
            }
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Agregar()//muestra la ventana de crear pantalla
        {
            if (!await TienePermisoAsync("/Roles/Agregar", 8080))
            {
                return RedirectToAction("AccessDenied", "Usuarios");
            }
            try
            {
                var sistemas = await ObtenerSistemasAsync();

                // Inicializa con una lista vacía si es null
                sistemas = sistemas ?? new List<Sistema>();

                // Crea el SelectList asegurando que SelectedValues no sea null
                var selectList = new SelectList(
                    items: sistemas,
                    dataValueField: "idSistema",
                    dataTextField: "nombre",
                    selectedValue: null); // Puedes pasar una lista vacía si lo prefieres

                ViewBag.Sistemas = selectList;
                return View(new Rol());
            }
            catch (Exception ex)
            {
                // Crea un SelectList vacío en caso de error
                ViewBag.Sistemas = new SelectList(new List<Sistema>(), "idSistema", "nombre");
                return View(new Rol());
            }

        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Agregar([Bind] Rol temp)
        {
            if (!await TienePermisoAsync("/Roles/Agregar", 8080))
            {
                return RedirectToAction("AccessDenied", "Usuarios");
            }
            try
            {
                // El idRol ya no es necesario establecerlo manualmente

                if (!ModelState.IsValid)
                {
                    var sistemas = await ObtenerSistemasAsync();
                    ViewBag.Sistemas = new SelectList(sistemas ?? new List<Sistema>(), "idSistema", "nombre");
                    return View(temp);
                }

                var response = await _api.PostAsJsonAsync("Role/Save", temp);

                if (response.IsSuccessStatusCode)
                {
                    //Sistema
                    var Sistemaresponse = await _api.GetAsync($"Sistema/SearchID?id={temp.idSistema}");
                    var sistemaJson = await Sistemaresponse.Content.ReadAsStringAsync();
                    var sistema = JsonConvert.DeserializeObject<Sistema>(sistemaJson);

                    await GuardarBitacoraSimpleAsync("Agregar", $"Se Agrego el Rol '{temp.nombre} al  sistema{sistema.nombre}'", "/Roles/Agregar");
                    return RedirectToAction("Agregar", "PermisosRoles", new {  idSistema = temp.idSistema  });
                }

                var sistemasParaError = await ObtenerSistemasAsync();
                ViewBag.Sistemas = new SelectList(sistemasParaError ?? new List<Sistema>(), "idSistema", "nombre");
                ModelState.AddModelError(string.Empty, "Error al guardar el rol.");
                return View(temp);
            }
            catch (Exception ex)
            {
                var sistemas = await ObtenerSistemasAsync();
                ViewBag.Sistemas = new SelectList(sistemas ?? new List<Sistema>(), "idSistema", "nombre");
                ModelState.AddModelError(string.Empty, $"Error inesperado: {ex.Message}");
                return View(temp);
            }
        }


        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Eliminar(int idRol, int idSistema)
        {
            if (!await TienePermisoAsync("/Roles/Eliminar", 8080))
            {
                return RedirectToAction("AccessDenied", "Usuarios");
            }
            HttpResponseMessage response = await _api.GetAsync($"Role/SearchID?idRol={idRol}&idSistema={idSistema}");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                Rol rol = JsonConvert.DeserializeObject<Rol>(json);
                return View(rol);
            }

            return NotFound($"No se encontró ningún rol con ID {idRol} y sistema {idSistema}.");
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> EliminarConfirmar(Rol rol)
        {
            if (!await TienePermisoAsync("/Roles/Eliminar", 8080))
            {
                return RedirectToAction("AccessDenied", "Usuarios");
            }
            try
            {
                if (rol.idRol == 0 || rol.idSistema == 0)
                {
                    TempData["ErrorMessage"] = "Datos inválidos recibidos.";
                    return RedirectToAction("List");
                }

                //Sistema
                var Sistemaresponse = await _api.GetAsync($"Sistema/SearchID?id={rol.idSistema}");
                var sistemaJson = await Sistemaresponse.Content.ReadAsStringAsync();
                var sistema = JsonConvert.DeserializeObject<Sistema>(sistemaJson);

                // Registrar en bitácora
                if (Sistemaresponse.IsSuccessStatusCode)
                {
                    await GuardarBitacoraSimpleAsync("Eliminar", $"Se Elimino el Rol '{rol.nombre}' del Sistema '{sistema.nombre}'", "/Roles/Eliminar");
                }
                

                HttpResponseMessage response = await _api.DeleteAsync($"Role/Delete?pIdRole={rol.idRol}&pIdSistema={rol.idSistema}");

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Rol eliminado correctamente";
                    return RedirectToAction("List");
                }

                TempData["ErrorMessage"] = "No se pudo eliminar el rol.";
                return RedirectToAction("List");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al eliminar el rol: {ex.Message}";
                return RedirectToAction("List");
            }
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Editar(int idRol, int idSistema)
        {
            if (!await TienePermisoAsync("/Roles/Editar", 8080))
            {
                return RedirectToAction("AccessDenied", "Usuarios");
            }
            Rol rol = null;

            HttpResponseMessage response = await _api.GetAsync($"Role/SearchID?idRol={idRol}&idSistema={idSistema}");

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                rol = JsonConvert.DeserializeObject<Rol>(result);
            }

            return View(rol);
        }


        [Authorize]
        [HttpPost]
        public async Task<IActionResult> EditarConfirmar([Bind] Rol temp)
        {
            if (!await TienePermisoAsync("/Roles/Editar", 8080))
            {
                return RedirectToAction("AccessDenied", "Usuarios");
            }
            try
            {
                HttpResponseMessage response = await _api.PutAsJsonAsync("Role/Update", temp);

                if (response.IsSuccessStatusCode)
                {
                    //Sistema
                    var Sistemaresponse = await _api.GetAsync($"Sistema/SearchID?id={temp.idSistema}");
                    var sistemaJson = await Sistemaresponse.Content.ReadAsStringAsync();
                    var sistema = JsonConvert.DeserializeObject<Sistema>(sistemaJson);

                    // Registrar en bitácora
                    await GuardarBitacoraSimpleAsync("Editar", $"Se Edito el Rol '{temp.nombre} del Sistema '{sistema.nombre}'", "/Roles/Editar");

                    TempData["SuccessMessage"] = "Rol actualizado correctamente";
                    return RedirectToAction("List");
                }

                TempData["ErrorMessage"] = "Error al actualizar el rol.";
                return View("Editar", temp);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al actualizar el rol: {ex.Message}";
                return View("Editar", temp);
            }
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Detalles(int idRol)
        {
            if (!await TienePermisoAsync("/Roles/Detalles", 8080))
            {
                return RedirectToAction("AccessDenied", "Usuarios");
            }
            Rol rol = null;

            HttpResponseMessage response = await _api.GetAsync($"Role/SearchID?idRol={idRol}");

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                rol = JsonConvert.DeserializeObject<Rol>(result);
            }

            return View(rol);
        }

        /// <summary>
        /// Metodo para el combo box
        /// </summary>
        /// <returns></returns>
        private async Task<List<Sistema>> ObtenerSistemasAsync()
        {
            try
            {
                HttpResponseMessage response = await _api.GetAsync("Sistema/List");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<Sistema>>(json);
                }
                return new List<Sistema>();
            }
            catch
            {
                return new List<Sistema>();
            }
        }

        private async Task<int?> ObtenerIdPantallaAsync(int idSistema, string ruta)
        {
            try
            {
                HttpResponseMessage response = await _api.GetAsync($"Pantallas/SearchByRuta?idSistema={idSistema}&ruta={Uri.EscapeDataString(ruta)}");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var pantalla = JsonConvert.DeserializeObject<Pantalla>(json);
                    return pantalla?.idPantalla;
                }
                return null;
            }
            catch
            {
                return null;
            }
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
