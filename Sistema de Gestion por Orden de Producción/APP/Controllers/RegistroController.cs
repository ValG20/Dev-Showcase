using EditorialUCR.Models;
using EditorialUCR.Services;
using Microsoft.AspNetCore.Mvc;

namespace EditorialUCR.Controllers
{
    public class RegistroController : Controller
    {
        private readonly ISeguridadService _seguridad;

        public RegistroController(ISeguridadService seguridad)
        {
            _seguridad = seguridad;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(new RegistroViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Index(RegistroViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                return View(modelo);
            }

            var ok = await _seguridad.RegistrarAsync(
                modelo.Usuario,
                modelo.Correo,
                modelo.Password);

            if (!ok)
            {
                //asociar el error al campo Correo
                ModelState.AddModelError(nameof(modelo.Correo), "Ya existe una cuenta con ese correo.");
                return View(modelo);
            }

            TempData["Mensaje"] = "Cuenta creada correctamente. Ahora puedes iniciar sesión.";
            return RedirectToAction("Index", "Login");
        }
    }
}
