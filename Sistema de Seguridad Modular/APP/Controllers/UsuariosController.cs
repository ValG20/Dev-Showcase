using AppSeguridad.Models;
using AppSeguridad.Models.DTO;
using AppWebSeguridad.Models;
using AppWebSeguridad.Models.DTO;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;

namespace AppSeguridad.Controllers
{
    public class UsuariosController : Controller
    {
        private ApiSeguridad _client = null;

        private HttpClient _api = null;

        private const int _idSistema = 8080; // Definimos el idSistema como constante

        public UsuariosController()
        {
            _client = new ApiSeguridad();
            _api = _client.IniciarApi();
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> List()
        {
            if (!await TienePermisoAsync("/Usuarios/List", 8080))
            {
                return RedirectToAction("AccessDenied", "Usuarios");
            }
            try
            {
                // Obtener permisos de la sesión
                var permisosJson = HttpContext.Session.GetString("Permisos");
                var permisos = JsonConvert.DeserializeObject<List<PermisoVista>>(permisosJson);

                // ID de la pantalla de "Usuarios" (ajusta al ID real en tu BD)
                var idPantallaResponse = await _api.GetAsync($"Pantallas/SearchByRuta?idSistema=8080&ruta={Uri.EscapeDataString("/Usuarios/List")}");

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
                List<Usuario> listado = new List<Usuario>();

                HttpResponseMessage response = await _api.GetAsync("Usuarios/List");

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    listado = JsonConvert.DeserializeObject<List<Usuario>>(result);
                }

                return View(listado);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en List (Usuarios): {ex.Message}");
                TempData["ErrorMessage"] = "Error interno al obtener la lista de usuarios";
                return View(new List<Usuario>());
            }
        }


        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Agregar()
        {
            if (!await TienePermisoAsync("/Usuarios/Agregar", 8080))
            {
                return RedirectToAction("AccessDenied", "Usuarios");
            }
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Agregar([Bind] Usuario temp)
        {
            if (!await TienePermisoAsync("/Usuarios/Agregar", 8080))
            {
                return RedirectToAction("AccessDenied", "Usuarios");
            }
            // 1. Obtener todos los usuarios existentes
            HttpResponseMessage responseList = await _api.GetAsync("Usuarios/List");

            if (responseList.IsSuccessStatusCode)
            {
                var json = await responseList.Content.ReadAsStringAsync();
                var clientesExistentes = JsonConvert.DeserializeObject<List<Usuario>>(json);

                // 2. Validar duplicados
                bool cedulaExistente = clientesExistentes.Any(c => c.idUsuario == temp.idUsuario);
                bool emailExistente = clientesExistentes.Any(c => c.correo == temp.correo);

                if (cedulaExistente)
                    ModelState.AddModelError("idUsuario", "Ya existe un usuario con este id.");

                if (emailExistente)
                    ModelState.AddModelError("correo", "Ya existe un usuario con este correo electrónico.");

                if (cedulaExistente || emailExistente)
                    return View(temp);

                // 3. Crear el nuevo usuario
                var jsonContent = new StringContent(JsonConvert.SerializeObject(temp), Encoding.UTF8, "application/json");
                HttpResponseMessage response = await _api.PostAsync("Usuarios/Save", jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    // 4. Buscar el usuario recién creado por correo
                    HttpResponseMessage searchResponse = await _api.GetAsync($"Usuarios/SearchByCorreo?correo={Uri.EscapeDataString(temp.correo)}");

                    int idUsuarioCreado = 0;

                    if (searchResponse.IsSuccessStatusCode)
                    {
                        var usuarioJson = await searchResponse.Content.ReadAsStringAsync();
                        var usuarioCreado = JsonConvert.DeserializeObject<Usuario>(usuarioJson);

                        if (usuarioCreado != null && usuarioCreado.idUsuario > 0)
                        {
                            idUsuarioCreado = usuarioCreado.idUsuario;

                            await GuardarBitacoraSimpleAsync("Agregar", $"Se Agrego el Usuario '{temp.nombre}'", "/Usuarios/Agregar");

                            TempData["SuccessMessage"] = "Usuario creado correctamente";
                        }
                    }

                    await GuardarBitacoraSimpleAsync("Agregar", $"Se Agrego el Usuario  '{temp.nombre}'", "/Usuarios/Agregar");

                    
                    // 6. Redirigir según lo seleccionado
                    var asignarRol = Request.Form["chkRol"] == "on";
                    var asignarPermiso = Request.Form["chkPermiso"] == "on";

                    if (asignarRol)
                        return RedirectToAction("Agregar", "UsuariosRoles", new { id = idUsuarioCreado });

                    if (asignarPermiso)
                        return RedirectToAction("Agregar", "PermisosUsuarios", new { id = idUsuarioCreado });

                    TempData["ErrorMessage"] = "Debe seleccionar al menos una opción: Asignar rol o Asignar permiso.";
                    return RedirectToAction("Editar", new { id = idUsuarioCreado });
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
        public async Task<IActionResult> Details(int id)
        {
            if (!await TienePermisoAsync("/Usuarios/Details", 8080))
            {
                return RedirectToAction("AccessDenied", "Usuarios");
            }
            Usuario user = null;
            HttpResponseMessage response = await _api.GetAsync($"Usuarios/SearchID?id={id}");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                user = JsonConvert.DeserializeObject<Usuario>(json);
            }
            else
            {
                ViewBag.Mensaje = $"No se encontró ningún usuario con el id {id}";
                return View("Details", null);
            }

            return View("Details", user);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (!await TienePermisoAsync("/Usuarios/Edit", 8080))
            {
                return RedirectToAction("AccessDenied", "Usuarios");
            }
            Usuario user = null;

            HttpResponseMessage response = await _api.GetAsync($"Usuarios/SearchID?id={id}");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                user = JsonConvert.DeserializeObject<Usuario>(json);
                return View(user); 
            }

            return NotFound(); 
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Edit([Bind]Usuario temp)
        {
            if (!await TienePermisoAsync("/Usuarios/Edit", 8080))
            {
                return RedirectToAction("AccessDenied", "Usuarios");
            }

            if (!ModelState.IsValid)
            {
                return View(temp);
            }

            HttpResponseMessage response = await _api.PutAsJsonAsync($"Usuarios/Update", temp);
            if (response.IsSuccessStatusCode)
            {
                await GuardarBitacoraSimpleAsync("Editar", $"Se Edito el Usuario con el correo '{temp.correo}'", "/Usuarios/Edit");
            }
            else
            {
                ViewBag.Mensaje = "Error al editar del usuario.";
            }

            return View(temp);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> DeleteUser(int id)
        {
            if (!await TienePermisoAsync("/Usuarios/Delete", 8080))
            {
                return RedirectToAction("AccessDenied", "Usuarios");
            }

            var UsuarioResponse = await _api.GetAsync($"Usuarios/SearchID?id={id}");
            var UsuarioJson = await UsuarioResponse.Content.ReadAsStringAsync();
            var temp = JsonConvert.DeserializeObject<Usuario>(UsuarioJson);

            Usuario usuario = new Usuario
            {
                idUsuario=temp.idUsuario,
                nombre=temp.nombre,
                correo=temp.correo,
                clave=temp.clave,
                estado="Inactivo",
            };

            HttpResponseMessage response = await _api.PutAsJsonAsync($"Usuarios/Update", usuario);
            if (response.IsSuccessStatusCode)
            {
                await GuardarBitacoraSimpleAsync("Inavilitar", $"Se Inavilito el Usuario con el correo '{temp.correo}'", "/Usuarios/List");
            }
            else
            {
                ViewBag.Mensaje = "Error al editar del usuario.";
            }

            return RedirectToAction("List");

        }

        private async Task<int?> ObtenerIdPantallaAsync(string ruta)
            {
            try
            {
                HttpResponseMessage response = await _api.GetAsync($"Pantallas/SearchByRuta?idSistema={_idSistema}&ruta={Uri.EscapeDataString(ruta)}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var pantalla = JsonConvert.DeserializeObject<Pantalla>(json);
                    return pantalla?.idPantalla;
                }

                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error al obtener pantalla: {error}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Excepción al obtener pantalla: {ex.Message}");
                return null;
            }
        }

        // Método para guardar bitácora
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


        //Metodos para login y logout
        // GET: Usuarios/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Login(string correo, string clave)
        {
            if (string.IsNullOrWhiteSpace(correo) || string.IsNullOrWhiteSpace(clave))
            {
                ModelState.AddModelError(string.Empty, "Correo y clave son obligatorios.");
                return View();
            }

            // Buscar usuario por correo (ignorar mayúsculas/minúsculas)
            HttpResponseMessage response = await _api.GetAsync($"Usuarios/SearchByCorreo?correo={correo}");

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Usuario no encontrado.");
                return View();
            }

            var jsonResult = await response.Content.ReadAsStringAsync();
            var usuario = JsonConvert.DeserializeObject<Usuario>(jsonResult);

            if (usuario == null)
            {
                ModelState.AddModelError(string.Empty, "Usuario no encontrado.");
                return View();
            }

            if (!string.Equals(usuario.estado, "activo", StringComparison.OrdinalIgnoreCase))
            {
                ModelState.AddModelError(string.Empty, "Usuario inactivo. Contacte al administrador.");
                return View();
            }

            if (usuario.clave != clave)
            {
                ModelState.AddModelError(string.Empty, "Clave incorrecta.");
                return View();
            }

            // Preparar objeto para autenticación
            var dto = new UsuarioSistema
            {
                idUsuario = usuario.idUsuario,
                nombre = usuario.nombre,
                correo = usuario.correo,
                clave = clave,
                estado = usuario.estado,
                idSistema = 8080, // ← ID de esta aplicación en particular
                SolicitarPermisos = true,
                permisos = new List<TodosLosPermisos>()
            };


            var jsonContent = new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json");
            HttpResponseMessage loginResponse = await _api.PostAsync("Usuarios/Autenticar", jsonContent);

            if (loginResponse.IsSuccessStatusCode)
            {
                var loginJson = await loginResponse.Content.ReadAsStringAsync();
                var loginResponseObj = JsonConvert.DeserializeObject<AutorizacionResponse>(loginJson);
                HttpContext.Session.SetString("Permisos", JsonConvert.SerializeObject(loginResponseObj.Permisos));
                HttpContext.Session.SetString("Pantallas", JsonConvert.SerializeObject(loginResponseObj.Pantallas));

                if (loginResponseObj != null && loginResponseObj.Resultado)
                {
                    // Guardar el token y el ID en sesión
                    HttpContext.Session.SetString("JWToken", loginResponseObj.Token);
                    HttpContext.Session.SetInt32("IdUsuario", usuario.idUsuario);

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, usuario.nombre),
                        new Claim(ClaimTypes.Email, usuario.correo),
                        new Claim(ClaimTypes.Role, loginResponseObj.NombreRol ?? "SinRol"),
                        new Claim("Token", loginResponseObj.Token)
                    };

                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, loginResponseObj?.Msj ?? "Error en autenticación.");
                    return View();
                }
            }
            else
            {
                var errorJson = await loginResponse.Content.ReadAsStringAsync();

                if (!string.IsNullOrWhiteSpace(errorJson))
                {
                    try
                    {
                        var errorResponse = JsonConvert.DeserializeObject<AutorizacionResponse>(errorJson);
                        var mensaje = !string.IsNullOrWhiteSpace(errorResponse?.Msj)
                            ? errorResponse.Msj
                            : "Ocurrió un error inesperado durante la autenticación.";

                        ModelState.AddModelError(string.Empty, mensaje);
                    }
                    catch
                    {
                        // Si no se puede deserializar, mostrar el mensaje plano recibido
                        ModelState.AddModelError(string.Empty, errorJson);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "El servidor no devolvió información de error.");
                }

                return View();
            }

        }


        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Usuarios");
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
