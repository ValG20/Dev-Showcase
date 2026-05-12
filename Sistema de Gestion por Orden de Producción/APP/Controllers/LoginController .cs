using Microsoft.AspNetCore.Mvc;
using EditorialUCR.Services;
using Microsoft.AspNetCore.Http;

namespace EditorialUCR.Controllers
{
    public class LoginController : Controller
    {
        private readonly ISeguridadService _seguridad;

        public LoginController(ISeguridadService seguridad)
        {
            _seguridad = seguridad;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index([FromBody] LoginDto modelo)
        {
            var resultado = await _seguridad.LoginAsync(modelo.UsuarioOCorreo, modelo.Password);

            // Guardar rol en sesión
            if (resultado.Exito)
            {
                HttpContext.Session.SetString("Rol", resultado.Rol);
            }

            return Ok(resultado);
        }

        [HttpPost]
        public IActionResult Logout()
        {
            // Limpiar sesión
            HttpContext.Session.Clear();

            // Redirigir al login
            return RedirectToAction("Index", "Login");
        }
    }

    public class LoginDto
    {
        public string UsuarioOCorreo { get; set; }
        public string Password { get; set; }
    }
}
