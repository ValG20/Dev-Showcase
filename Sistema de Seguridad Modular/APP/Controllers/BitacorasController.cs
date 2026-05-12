using AppSeguridad.Models;
using AppWebSeguridad.Models;
using AppWebSeguridad.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Net.Http.Headers;


namespace AppSeguridad.Controllers
{
    public class BitacorasController : Controller
    {
        private ApiSeguridad _client = null;

        private HttpClient _api = null;

        public BitacorasController()
        {
            _client = new ApiSeguridad();
            _api = _client.IniciarApi();
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> List(string idUsuario = null, string idSistema = null, string accion = null)
        {
            if (!await TienePermisoAsync("/Bitacoras/List", 8080))
                return RedirectToAction("AccessDenied", "Usuarios");

            List<Bitacora> listado = new List<Bitacora>();

            // Obtener bitácoras
            var response = await _api.GetAsync("Bitacoras/List");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                listado = JsonConvert.DeserializeObject<List<Bitacora>>(json);
            }

            // Aplicar filtros
            if (!string.IsNullOrEmpty(idUsuario))
                listado = listado.Where(b => b.idUsuario.ToString() == idUsuario).ToList();

            if (!string.IsNullOrEmpty(idSistema))
                listado = listado.Where(b => b.idSistema.ToString() == idSistema).ToList();

            if (!string.IsNullOrEmpty(accion))
                listado = listado.Where(b => b.accion.Equals(accion, StringComparison.OrdinalIgnoreCase)).ToList();

            // Cargar combos
            var usuarios = await ObtenerUsuarios();
            var sistemas = await ObtenerSistemas();

            ViewBag.Usuarios = new SelectList(usuarios, "idUsuario", "nombre", idUsuario);
            ViewBag.Sistemas = new SelectList(sistemas, "idSistema", "nombre", idSistema);
            ViewBag.AccionSeleccionada = accion;

            return View(listado);
        }

        private async Task<List<Sistema>> ObtenerSistemas()
        {
            var response = await _api.GetAsync("Sistema/List");
            if (!response.IsSuccessStatusCode)
                return new List<Sistema>();

            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<Sistema>>(json);
        }


        private async Task<List<Usuario>> ObtenerUsuarios()
        {
            var response = await _api.GetAsync("Usuarios/List");
            if (!response.IsSuccessStatusCode) return new List<Usuario>();

            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<Usuario>>(json);
        }







        public async Task<IActionResult> Create([Bind] Bitacora temp)
        {
            if (!await TienePermisoAsync("/Bitacoras/Agregar", 8080))
            {
                return RedirectToAction("AccessDenied", "Usuarios");
            }

            if (!ModelState.IsValid)
            {
                return View(temp);
            }

            var resultado = await _api.PostAsJsonAsync("Bitacoras/Save", temp);

            if (resultado.IsSuccessStatusCode)
            {
                return RedirectToAction("List");
            }
            else
            {
                string respuesta = await resultado.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, $"Error: {respuesta}");
                return View(temp);
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
