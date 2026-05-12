using EditorialUCR.Services;
using Microsoft.AspNetCore.Mvc;

namespace EditorialUCR.Controllers
{
    [Route("[controller]")]
    public class RecuperacionController : Controller
    {
        private readonly ISeguridadService _seguridad;

        public RecuperacionController(ISeguridadService seguridad)
        {
            _seguridad = seguridad;
        }

        // -------- PASO 1: Solicitar código --------

        [HttpGet("Solicitud")]
        public IActionResult Solicitud()
        {
            return View();
        }

        [HttpPost("Solicitud")]
        public async Task<IActionResult> Solicitud([FromBody] SolicitudRecuperacionDto modelo)
        {
            if (modelo == null || string.IsNullOrWhiteSpace(modelo.Email))
                return BadRequest(new { message = "El correo es requerido." });

            var codigo = new Random().Next(100000, 999999).ToString();

            var ok = await _seguridad.EstablecerCodigoRecuperacionAsync(modelo.Email, codigo);

            if (!ok)
                return NotFound(new { message = "No existe una cuenta con ese correo." });

            // para pruebas también mandamos el código; en producción lo quitas
            return Ok(new
            {
                message = "Código de recuperación generado.",
                email = modelo.Email,
                codigo = codigo
            });
        }

        // -------- PASO 2: Validar código --------

        [HttpGet("Codigo")]
        public IActionResult Codigo()
        {
            return View();
        }

        [HttpPost("Codigo")]
        public async Task<IActionResult> Codigo([FromBody] ValidarCodigoDto modelo)
        {
            if (modelo == null ||
                string.IsNullOrWhiteSpace(modelo.Email) ||
                string.IsNullOrWhiteSpace(modelo.Codigo))
            {
                return BadRequest(new { message = "Datos inválidos." });
            }

            var esValido = await _seguridad.ValidarCodigoAsync(modelo.Email, modelo.Codigo);

            if (!esValido)
                return BadRequest(new { message = "El código es incorrecto o ha expirado." });

            // Código correcto → seguimos al paso 3
            return Ok(new { message = "Código válido." });
        }

        // -------- PASO 3: Nueva contraseña --------

        [HttpGet("Validar")]
        public IActionResult Validar()
        {
            return View();
        }

        [HttpPost("Validar")]
        public async Task<IActionResult> Validar([FromBody] CambiarPasswordDto modelo)
        {
            if (modelo == null ||
                string.IsNullOrWhiteSpace(modelo.Email) ||
                string.IsNullOrWhiteSpace(modelo.NuevaPassword))
            {
                return BadRequest(new { message = "Datos inválidos." });
            }

            var cambiado = await _seguridad.CambiarPasswordAsync(modelo.Email, modelo.NuevaPassword);

            if (!cambiado)
                return BadRequest(new { message = "No se pudo cambiar la contraseña." });

            return Ok(new { message = "Contraseña cambiada correctamente.", next = "/Login/Index" });
        }
    }

    // DTOs
    public class SolicitudRecuperacionDto
    {
        public string Email { get; set; } = "";
    }

    public class ValidarCodigoDto
    {
        public string Email { get; set; } = "";
        public string Codigo { get; set; } = "";
    }

    public class CambiarPasswordDto
    {
        public string Email { get; set; } = "";
        public string NuevaPassword { get; set; } = "";
    }
}
