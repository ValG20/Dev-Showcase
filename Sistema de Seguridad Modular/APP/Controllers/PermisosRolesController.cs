using AppSeguridad.Models;
using AppWebSeguridad.Models;
using AppWebSeguridad.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace AppSeguridad.Controllers
{
    public class PermisosRolesController : Controller
    {
        private ApiSeguridad _client = null;

        private HttpClient _api = null;

        public PermisosRolesController()
        {
            _client = new ApiSeguridad();
            _api = _client.IniciarApi();
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> List(int? idRol, int? idSistema)
        {
            if (!await TienePermisoAsync("/PermisosRoles/List", 8080))
            {
                return RedirectToAction("AccessDenied", "Usuarios");
            }
            List<PermisoRol> listado = new List<PermisoRol>();

            HttpResponseMessage response = await _api.GetAsync("PermisosRoles/List");

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                listado = JsonConvert.DeserializeObject<List<PermisoRol>>(result);
            }

            if (idRol.HasValue)
            {
                listado = listado.Where(p => p.idRol == idRol.Value).ToList();
                ViewBag.IdRol = idRol.Value;
            }

            if (idSistema.HasValue)
            {
                listado = listado.Where(p => p.idSistema == idSistema.Value).ToList();
                ViewBag.IdSistema = idSistema.Value;
            }

            return View(listado);
        }


        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Agregar(int idSistema, int idRol)
        {
            if (!await TienePermisoAsync("/PermisosRoles/Agregar", 8080))
            {
                return RedirectToAction("AccessDenied", "Usuarios");
            }
            // Obtener nombre del sistema
            var sistemaResponse = await _api.GetAsync($"Sistema/SearchID?id={idSistema}");
            if (sistemaResponse.IsSuccessStatusCode)
            {
                var sistemaJson = await sistemaResponse.Content.ReadAsStringAsync();
                var sistema = JsonConvert.DeserializeObject<Sistema>(sistemaJson);
                ViewBag.NombreSistema = sistema?.nombre ?? "Sistema no encontrado";
            }
            else
            {
                ViewBag.NombreSistema = "Sistema no encontrado";
            }

            var permiso = new PermisoRol
            {
                idSistema = idSistema,
                idRol = idRol
            };

            var response = await _api.GetAsync($"Pantallas/ObtenerPorSistema?idSistema={idSistema}");
            var pantallas = new List<Pantalla>();

            if (response.IsSuccessStatusCode)
            {
                var contenido = await response.Content.ReadAsStringAsync();
                pantallas = JsonConvert.DeserializeObject<List<Pantalla>>(contenido);
            }

            ViewBag.Pantallas = pantallas;

            return View(permiso);
        }


        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Agregar([Bind] PermisoRol temp)
        {
            if (!await TienePermisoAsync("/PermisosRoles/Agregar", 8080))
            {
                return RedirectToAction("AccessDenied", "Usuarios");
            }
            if (!ModelState.IsValid)
            {
                return View(temp);
            }
            var resultado = await _api.PostAsJsonAsync("PermisosRoles/Save", temp);
            if (resultado.IsSuccessStatusCode)
            {

                //sistema
                var Sistemaresponse = await _api.GetAsync($"Sistema/SearchID?id={temp.idSistema}");
                var sistemaJson = await Sistemaresponse.Content.ReadAsStringAsync();
                var sistema = JsonConvert.DeserializeObject<Sistema>(sistemaJson);

                if (temp.idRol!=0)
                {
                    //rol
                    var Rolresponse = await _api.GetAsync($"Role/SearchID?idRol={temp.idRol}");
                    var rolJson = await Rolresponse.Content.ReadAsStringAsync();
                    var rol = JsonConvert.DeserializeObject<Rol>(rolJson); 

                    await GuardarBitacoraSimpleAsync("Agregar", $"Se agregaron Permisos para el rol '{rol.nombre}' del Sistema '{sistema.nombre}'", "/PermisosRoles/Agregar");
                }

                await GuardarBitacoraSimpleAsync("Agregar", $"Se agregaron Permisos para el Rol del Sistema '{sistema.nombre}'", "/PermisosRoles/Agregar");


                return RedirectToAction("List", "Roles");
            }
            else
            {
                string respuesta = await resultado.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, $"Error: {respuesta}");
                return View(temp);
            }
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Details(int idRol, int idSistema, int idPantalla)
        {
            if (!await TienePermisoAsync("/PermisosRoles/Details", 8080))
            {
                return RedirectToAction("AccessDenied", "Usuarios");
            }
            PermisoRol user = null;

            string url = $"PermisosRoles/SearchID?idRol={idRol}&idSistema={idSistema}&idPantalla={idPantalla}";
            HttpResponseMessage response = await _api.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                user = JsonConvert.DeserializeObject<PermisoRol>(json);
            }
            else
            {
                ViewBag.Mensaje = $"No se encontró ningún permiso con el id ";
                return View("Details", null);
            }

            return View("Details", user);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Delete(int idRol, int idSistema, int idPantalla)
        {
            if (!await TienePermisoAsync("/PermisosRoles/Delete", 8080))
            {
                return RedirectToAction("AccessDenied", "Usuarios");
            }

            string url = $"PermisosRoles/SearchID?idRol={idRol}&idSistema={idSistema}&idPantalla={idPantalla}";

            HttpResponseMessage response = await _api.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                PermisoRol cliente = JsonConvert.DeserializeObject<PermisoRol>(json);
                return View();
            }

            return NotFound($"No se encontró ningún usuario con el ID .");
        }



        [Authorize]
        [HttpPost]
        public async Task<IActionResult> DeleteSystem(int idRol, int idSistema, int idPantalla)
        {
            if (!await TienePermisoAsync("/PermisosRoles/Delete", 8080))
            {
                return RedirectToAction("AccessDenied", "Usuarios");
            }

            string url = $"PermisosRoles/Delete?idRol={idRol}&idSistema={idSistema}&idPantalla={idPantalla}";

            //rol
            var Rolresponse = await _api.GetAsync($"Role/SearchID?idRol={idRol}");
            var rolJson = await Rolresponse.Content.ReadAsStringAsync();
            var rol = JsonConvert.DeserializeObject<Rol>(rolJson);

            //sistema
            var Sistemaresponse = await _api.GetAsync($"Sistema/SearchID?id={idSistema}");
            var sistemaJson = await Sistemaresponse.Content.ReadAsStringAsync();
            var sistema = JsonConvert.DeserializeObject<Sistema>(sistemaJson);

            HttpResponseMessage response = await _api.DeleteAsync(url);

            if (response.IsSuccessStatusCode)
            {

                await GuardarBitacoraSimpleAsync("Eliminar", $"Se Eliminaron los Permisos para el rol '{rol.nombre}', del Sistema '{sistema.nombre}'", "/PermisosRoles/Delete");
                return RedirectToAction("List");
            }
            TempData["Error"] = "No se pudo eliminar el usuario.";
            return RedirectToAction("List", "Roles");

        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Edit(int  idRol, int idSistema, int idPantalla)
        {
            if (!await TienePermisoAsync("/PermisosRoles/Edit", 8080))
            {
                return RedirectToAction("AccessDenied", "Usuarios");
            }
            // Obtener permiso actual
            var permisoResp = await _api.GetAsync($"PermisosRoles/SearchID?idRol={idRol}&idSistema={idSistema}&idPantalla={idPantalla}");
            if (!permisoResp.IsSuccessStatusCode)
                return NotFound("Permiso no encontrado.");

            var json = await permisoResp.Content.ReadAsStringAsync();
            var permiso = JsonConvert.DeserializeObject<PermisoRol>(json);

            
            // Cargar sistemas
            var sistemasResp = await _api.GetAsync("Sistema/List");
            if (sistemasResp.IsSuccessStatusCode)
            {
                var sistemasJson = await sistemasResp.Content.ReadAsStringAsync();
                var sistemas = JsonConvert.DeserializeObject<List<Sistema>>(sistemasJson);
                ViewBag.Sistemas = new SelectList(sistemas, "idSistema", "nombre", permiso.idSistema);
            }

            // (Opcional) Podés cargar pantallas de ese sistema si querés que se cargue lista inicial
            var pantallasResp = await _api.GetAsync($"Pantallas/ObtenerPorSistema?idSistema={idSistema}");
            if (pantallasResp.IsSuccessStatusCode)
            {
                var pantallasJson = await pantallasResp.Content.ReadAsStringAsync();
                var pantallas = JsonConvert.DeserializeObject<List<Pantalla>>(pantallasJson)
                                .Where(p => p.Ruta.EndsWith("/List"))
                                .ToList();

                ViewBag.Pantallas = new SelectList(pantallas, "idPantalla", "Ruta", permiso.IdPantalla);
            }



            return View(permiso);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Edit(PermisoRol permisoEditado)
        {
            if (!await TienePermisoAsync("/PermisosRoles/Edit", 8080))
            {
                return RedirectToAction("AccessDenied", "Usuarios");
            }
            if (!ModelState.IsValid)
            {
                return View(permisoEditado);
            }

            HttpResponseMessage response = await _api.PutAsJsonAsync("PermisosRoles/Edit", permisoEditado);

            if (response.IsSuccessStatusCode)
            {
                //rol
                var Rolresponse = await _api.GetAsync($"Role/SearchID?idRol={permisoEditado.idRol}");
                var rolJson = await Rolresponse.Content.ReadAsStringAsync();
                var rol = JsonConvert.DeserializeObject<Rol>(rolJson);

                //sistema
                var Sistemaresponse = await _api.GetAsync($"Sistema/SearchID?id={permisoEditado.idSistema}");
                var sistemaJson = await Sistemaresponse.Content.ReadAsStringAsync();
                var sistema = JsonConvert.DeserializeObject<Sistema>(sistemaJson);

                await GuardarBitacoraSimpleAsync("Eliminar", $"Se Editaron los Permisos del Rol '{rol.nombre}', del Sistema '{sistema.nombre}'", "/PermisosRoles/Edit");
                return RedirectToAction("List");
            }

            string error = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError(string.Empty, $"Error al editar: {error}");
            return View(permisoEditado);
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
