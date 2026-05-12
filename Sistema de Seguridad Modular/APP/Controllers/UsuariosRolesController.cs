using AppSeguridad.Models;
using AppWebSeguridad.Models;
using AppWebSeguridad.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace AppSeguridad.Controllers
{
    public class UsuariosRolesController : Controller
    {
        private ApiSeguridad _client = null;
        private HttpClient _api = null;

        public UsuariosRolesController()
        {
            _client = new ApiSeguridad();
            _api = _client.IniciarApi();
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> List(int idUsuario)
        {
            if (await TienePermisoAsync("/UsuariosRoles/List", 8080))
            {
                return RedirectToAction("AccessDenied", "Usuarios");
            }
            ViewBag.idUsuario = idUsuario;

            List<Rol> listado = new List<Rol>();
            string url = "Role/List";
            HttpResponseMessage response = await _api.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                listado = JsonConvert.DeserializeObject<List<Rol>>(result);
            }

            return View(listado);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Agregar(int id)
        {
            if (!await TienePermisoAsync("/UsuariosRoles/Agregar", 8080))
            {
                return RedirectToAction("AccessDenied", "Usuarios");
            }
            // Llenar el ViewBag con los roles disponibles
            var roles = await ObtenerRolesAsync();
            ViewBag.Roles = new SelectList(roles, "idRol", "nombre");

            var permiso = new UsuariosRoles
            {
                idUsuario = id
            };
            return View(permiso);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Agregar([Bind] UsuariosRoles temp)
        {
            if (!await TienePermisoAsync("/UsuariosRoles/Agregar", 8080))
            {
                return RedirectToAction("AccessDenied", "Usuarios");
            }
            if (!ModelState.IsValid)
            {
                
                // Si hay error, se vuelve a cargar la lista de roles
                var roles = await ObtenerRolesAsync();
                ViewBag.Roles = new SelectList(roles, "idRol", "nombre");

                return View(temp);
            }

            var resultado = await _api.PostAsJsonAsync("UsuariosRoles/Save", temp);
            if (resultado.IsSuccessStatusCode)
            {
                //rol
                var Rolresponse = await _api.GetAsync($"Role/SearchID?idRol={temp.idRol}");
                var rolJson = await Rolresponse.Content.ReadAsStringAsync();
                var rol = JsonConvert.DeserializeObject<Rol>(rolJson);

                //Usuario
                var Usuarioresponse = await _api.GetAsync($"Usuarios/SearchID?id={temp.idUsuario}");
                var UsuarioJson = await Usuarioresponse.Content.ReadAsStringAsync();
                var usuario = JsonConvert.DeserializeObject<Rol>(UsuarioJson);

                await GuardarBitacoraSimpleAsync("Agregar", $"Se Agregaron Roles al Usuario '{rol.nombre}' en el Sistema '{usuario.nombre}'", "/UsuariosRoles/Agregar");
                return RedirectToAction("List", "Usuarios");
            }
            else
            {
                string respuesta = await resultado.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, $"Error: {respuesta}");

                var roles = await ObtenerRolesAsync();
                ViewBag.Roles = new SelectList(roles, "idRol", "nombre");

                return View(temp);
            }
        }

        [Authorize ]
        [HttpGet]
        public async Task<IActionResult> Details(int idUsuario, int idRol)
        {
            if (!await TienePermisoAsync("/UsuariosRoles/Details", 8080))
            {
                return RedirectToAction("AccessDenied", "Usuarios");
            }
            UsuariosRoles user = null;

            string url = $"UsuariosRoles/SearchID?idUsuario={idUsuario}&idRol={idRol}";
            HttpResponseMessage response = await _api.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                user = JsonConvert.DeserializeObject<UsuariosRoles>(json);
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
        public async Task<IActionResult> RolesDelUsuario(int idUsuario)
        {
            if (!await TienePermisoAsync("/UsuariosRoles/RolesDelUsuario", 8080))
            {
                return RedirectToAction("AccessDenied", "Usuarios");
            }
            List<UsuariosRoles> listado = new List<UsuariosRoles>();

            HttpResponseMessage response = await _api.GetAsync($"UsuariosRoles/SearchListUserRol?idUsuario={idUsuario}");

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                listado = JsonConvert.DeserializeObject<List<UsuariosRoles>>(result);
            }
            else
            {
                ViewBag.Mensaje = "No se pudieron obtener los permisos del usuario.";
            }

            ViewBag.idUsuario = idUsuario;
            return View(listado);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Delete(int idUsuario, int idRol)
        {
            if (!await TienePermisoAsync("/UsuariosRoles/Delete", 8080))
            {
                return RedirectToAction("AccessDenied", "Usuarios");
            }
            string url = $"UsuariosRoles/SearchID?idUsuario={idUsuario}&idRol={idRol}";

            HttpResponseMessage response = await _api.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                UsuariosRoles cliente = JsonConvert.DeserializeObject<UsuariosRoles>(json);
                return View(cliente);
            }

            return NotFound($"No se encontró ningún usuario con el ID.");
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> DeleteSystem(int idUsuario, int idRol)
        {
            if (!await TienePermisoAsync("/UsuariosRoles/Delete", 8080))
            {
                return RedirectToAction("AccessDenied", "Usuarios");
            }

            //rol
            var Rolresponse = await _api.GetAsync($"Role/SearchID?idRol={idRol}");
            var rolJson = await Rolresponse.Content.ReadAsStringAsync();
            var rol = JsonConvert.DeserializeObject<Rol>(rolJson);

            //Usuario
            var Usuarioresponse = await _api.GetAsync($"Usuarios/SearchID?id={idUsuario}");
            var UsuarioJson = await Usuarioresponse.Content.ReadAsStringAsync();
            var usuario = JsonConvert.DeserializeObject<Rol>(UsuarioJson);


            if (Rolresponse.IsSuccessStatusCode && Usuarioresponse.IsSuccessStatusCode)
            {
                await GuardarBitacoraSimpleAsync("Eliminar", $"Se Eliminaron Roles al Usuario '{rol.nombre}' en el Sistema '{usuario.nombre}'", "/UsuariosRoles/Delete");
            }
            

            string url = $"UsuariosRoles/Delete?idUsuario={idUsuario}&idRol={idRol}";

            HttpResponseMessage response = await _api.DeleteAsync(url);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("List", "Usuarios");
            }

            TempData["Error"] = "No se pudo eliminar el usuario.";
            return RedirectToAction("PermisosDelUsuario");
        }

        // Método que consulta los roles disponibles en la API
        private async Task<List<Rol>> ObtenerRolesAsync()
        {
            try
            {
                HttpResponseMessage response = await _api.GetAsync("Role/List");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<Rol>>(json);
                }
                return new List<Rol>();
            }
            catch
            {
                return new List<Rol>();
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
