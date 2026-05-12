using AppSeguridad.Models;
using AppWebSeguridad.Models;
using AppWebSeguridad.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace AppSeguridad.Controllers
{
    public class PermisosUsuariosController : Controller
    {

        private ApiSeguridad _client = null;

        private HttpClient _api = null;

        public PermisosUsuariosController()
        {
            _client = new ApiSeguridad();
            _api = _client.IniciarApi();
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> ObtenerPantallasPorSistema(int idSistema)//Agregado sharon 17/6/2025
        {

            var response = await _api.GetAsync($"Pantallas/ObtenerPorSistema?idSistema={idSistema}");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return Content(json, "application/json");
            }

            return StatusCode((int)response.StatusCode);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> List()
        {
            if (!await TienePermisoAsync("/PermisosUsuarios/PermisosDelUsuario", 8080)) // Verifica si el usuario tiene permiso para acceder a esta pantalla
            {
                return RedirectToAction("AccessDenied", "Usuarios");
            }
            List<PermisoUsuario> listado = new List<PermisoUsuario>();
            HttpResponseMessage response = await _api.GetAsync("Usuarios/List");

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;

                listado = JsonConvert.DeserializeObject<List<PermisoUsuario>>(result);
            }
            return View(listado);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Agregar(int id)//Modificado por Sharon 17/6/2025
        {
            if (!await TienePermisoAsync("/PermisosUsuarios/Agregar", 8080)) // Verifica si el usuario tiene permiso para acceder a esta pantalla
            {
                return RedirectToAction("AccessDenied", "Usuarios");
            }
            // Consultar info del usuario por su ID
            var userResponse = await _api.GetAsync($"Usuarios/SearchID?id={id}");
            if (userResponse.IsSuccessStatusCode)
            {
                var userJson = await userResponse.Content.ReadAsStringAsync();
                var usuario = JsonConvert.DeserializeObject<Usuario>(userJson);
                ViewBag.CorreoUsuario = usuario.correo; // Mostralo en la vista
            }

            // Cargar los sistemas
            var sistemasResponse = await _api.GetAsync("Sistema/List");
            if (sistemasResponse.IsSuccessStatusCode)
            {
                var json = await sistemasResponse.Content.ReadAsStringAsync();
                var sistemas = JsonConvert.DeserializeObject<List<Sistema>>(json);
                ViewBag.Sistemas = new SelectList(sistemas, "idSistema", "nombre");
            }

            var modelo = new PermisoUsuario
            {
                IdUsuario = id
            };

            return View(modelo);
        }



        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Agregar([Bind] PermisoUsuario temp)
        {
            if (!await TienePermisoAsync("/PermisosUsuarios/Agregar", 8080)) // Verifica si el usuario tiene permiso para acceder a esta pantalla
            {
                return RedirectToAction("AccessDenied", "Usuarios");
            }
            if (!ModelState.IsValid)
            {
                return View(temp);
            }
            var resultado = await _api.PostAsJsonAsync("PermisosUsuarios/Save", temp);
            if (resultado.IsSuccessStatusCode)
            {
                var UsuarioResponse = await _api.GetAsync($"Usuarios/SearchID?id={temp.IdUsuario}");
                var UsuarioJson = await UsuarioResponse.Content.ReadAsStringAsync();
                var usuario = JsonConvert.DeserializeObject<Usuario>(UsuarioJson);

                //sistema
                var Sistemaresponse = await _api.GetAsync($"Sistema/SearchID?id={temp.idSistema}");
                var sistemaJson = await Sistemaresponse.Content.ReadAsStringAsync();
                var sistema = JsonConvert.DeserializeObject<Sistema>(sistemaJson);

                //Pantalla
                var Pantallaresponse = await _api.GetAsync($"Pantallas/SearchID?idPantalla={temp.IdPantalla}&idSistema={temp.idSistema}");
                var PantallaJson = await Pantallaresponse.Content.ReadAsStringAsync();
                var pantalla = JsonConvert.DeserializeObject<Pantalla>(PantallaJson);

                await GuardarBitacoraSimpleAsync("Agregar", $"Se Agregaron Permisos al Usuario '{usuario.nombre}', de la Pantalla '{pantalla.Nombre}', del Sistema '{sistema.nombre}'", "/PermisosUsuarios/Agregar");
                return RedirectToAction("List", "Usuarios"); 
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
        public async Task<IActionResult> Details(int idUsuario, int idSistema, int idPantalla)
        {
            if (!await TienePermisoAsync("/PermisosUsuarios/Details", 8080)) // Verifica si el usuario tiene permiso para acceder a esta pantalla
            {
                return RedirectToAction("AccessDenied", "Usuarios");
            }
            PermisoUsuario user = null;

            string url = $"PermisosUsuarios/SearchID?idUsuario={idUsuario}&idSistema={idSistema}&idPantalla={idPantalla}";

            HttpResponseMessage response = await _api.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                user = JsonConvert.DeserializeObject<PermisoUsuario>(json);
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
        public async Task<IActionResult> PermisosDelUsuario(int idUsuario)
        {
            if (!await TienePermisoAsync("/PermisosUsuarios/PermisosDelUsuario", 8080))
            {
                return RedirectToAction("AccessDenied", "Usuarios");
            }

            List<PermisoUsuario> listado = new List<PermisoUsuario>();

            // Obtener todos los permisos
            HttpResponseMessage response = await _api.GetAsync("/PermisosUsuarios/List");

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                listado = JsonConvert.DeserializeObject<List<PermisoUsuario>>(result);
            }
            else
            {
                ViewBag.Mensaje = "No se pudieron obtener los permisos.";
            }

            // Aplicar filtro solo por idUsuario
            listado = listado
                .Where(p => p.IdUsuario == idUsuario)
                .ToList();

            ViewBag.IdUsuario = idUsuario;

            return View(listado);
        }



        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Delete(int idUsuario, int idSistema, int idPantalla)
        {
            if (!await TienePermisoAsync("/PermisosUsuarios/Delete", 8080)) // Verifica si el usuario tiene permiso para acceder a esta pantalla
            {
                return RedirectToAction("AccessDenied", "Usuarios");
            }

            string url = $"PermisosUsuarios/SearchID?idUsuario={idUsuario}&idSistema={idSistema}&idPantalla={idPantalla}";

            HttpResponseMessage response = await _api.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                PermisoUsuario cliente = JsonConvert.DeserializeObject<PermisoUsuario>(json);
                return View(cliente);
            }

            return NotFound($"No se encontró ningún usuario con el ID .");
        }



        [Authorize]
        [HttpPost]
        public async Task<IActionResult> DeleteSystem(int idUsuario, int idSistema, int idPantalla)
        {
            if (!await TienePermisoAsync("/PermisosUsuarios/Delete", 8080)) // Verifica si el usuario tiene permiso para acceder a esta pantalla
            {
                return RedirectToAction("AccessDenied", "Usuarios");
            }

            var UsuarioResponse = await _api.GetAsync($"Usuarios/SearchID?id={idUsuario}");
            var UsuarioJson = await UsuarioResponse.Content.ReadAsStringAsync();
            var usuario = JsonConvert.DeserializeObject<Usuario>(UsuarioJson);

            //sistema
            var Sistemaresponse = await _api.GetAsync($"Sistema/SearchID?id={idSistema}");
            var sistemaJson = await Sistemaresponse.Content.ReadAsStringAsync();
            var sistema = JsonConvert.DeserializeObject<Sistema>(sistemaJson);

            //Pantalla
            var Pantallaresponse = await _api.GetAsync($"Pantallas/SearchID?idPantalla={idPantalla}&idSistema={idSistema}");
            var PantallaJson = await Pantallaresponse.Content.ReadAsStringAsync();
            var pantalla = JsonConvert.DeserializeObject<Pantalla>(PantallaJson);

            if (Pantallaresponse.IsSuccessStatusCode && Sistemaresponse.IsSuccessStatusCode && UsuarioResponse.IsSuccessStatusCode)
            {
                await GuardarBitacoraSimpleAsync("Eliminar", $"Se Eliminaron Permisos al Usuario '{usuario.nombre}', de la Pantalla '{pantalla.Nombre}', del Sistema '{sistema.nombre}'", "/PermisosUsuarios/Delete");
            }

            string url = $"PermisosUsuarios/Delete?idUsuario={idUsuario}&idSistema={idSistema}&idPantalla={idPantalla}";


            HttpResponseMessage response = await _api.DeleteAsync(url);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("List", "Usuarios");
            }
            TempData["Error"] = "No se pudo eliminar el usuario.";
            return RedirectToAction("PermisosDelUsuario");

        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Edit(int idUsuario, int idSistema, int idPantalla)
        {
            if (!await TienePermisoAsync("/PermisosUsuarios/Edit", 8080)) // Verifica si el usuario tiene permiso para acceder a esta pantalla
            {
                return RedirectToAction("AccessDenied", "Usuarios");
            }
            // Obtener permiso actual
            var permisoResp = await _api.GetAsync($"PermisosUsuarios/SearchID?idUsuario={idUsuario}&idSistema={idSistema}&idPantalla={idPantalla}");
            if (!permisoResp.IsSuccessStatusCode)
                return NotFound("Permiso no encontrado.");

            var json = await permisoResp.Content.ReadAsStringAsync();
            var permiso = JsonConvert.DeserializeObject<PermisoUsuario>(json);

            // Obtener correo del usuario
            var usuarioResp = await _api.GetAsync($"Usuarios/SearchID?id={idUsuario}");
            if (usuarioResp.IsSuccessStatusCode)
            {
                var usuarioJson = await usuarioResp.Content.ReadAsStringAsync();
                var usuario = JsonConvert.DeserializeObject<Usuario>(usuarioJson);
                ViewBag.CorreoUsuario = usuario.correo;
            }

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
        public async Task<IActionResult> Edit(PermisoUsuario permisoEditado)
        {
            if (!await TienePermisoAsync("/PermisosUsuarios/Edit", 8080)) // Verifica si el usuario tiene permiso para acceder a esta pantalla
            {
                return RedirectToAction("AccessDenied", "Usuarios");
            }
            if (!ModelState.IsValid)
            {
                return View(permisoEditado);
            }

            HttpResponseMessage response = await _api.PutAsJsonAsync("PermisosUsuarios/Edit", permisoEditado);

            if (response.IsSuccessStatusCode)
            {
                var UsuarioResponse = await _api.GetAsync($"Usuarios/SearchID?id={permisoEditado.IdUsuario}");
                var UsuarioJson = await UsuarioResponse.Content.ReadAsStringAsync();
                var usuario = JsonConvert.DeserializeObject<Usuario>(UsuarioJson);

                //sistema
                var Sistemaresponse = await _api.GetAsync($"Sistema/SearchID?id={permisoEditado.idSistema}");
                var sistemaJson = await Sistemaresponse.Content.ReadAsStringAsync();
                var sistema = JsonConvert.DeserializeObject<Sistema>(sistemaJson);

                //Pantalla
                var Pantallaresponse = await _api.GetAsync($"Pantallas/SearchID?idPantalla={permisoEditado.IdPantalla}&idSistema={permisoEditado.idSistema}");
                var PantallaJson = await Pantallaresponse.Content.ReadAsStringAsync();
                var pantalla = JsonConvert.DeserializeObject<Pantalla>(PantallaJson);

                if (Pantallaresponse.IsSuccessStatusCode && Sistemaresponse.IsSuccessStatusCode && UsuarioResponse.IsSuccessStatusCode)
                {
                    await GuardarBitacoraSimpleAsync("Editar", $"Se Editaron los Permisos del Usuario '{usuario.nombre}', de la Pantalla '{pantalla.Nombre}', del Sistema '{sistema.nombre}'", "/PermisosUsuarios/Edit");
                }

                return RedirectToAction("PermisosDelUsuario", new { id = permisoEditado.IdUsuario });
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
