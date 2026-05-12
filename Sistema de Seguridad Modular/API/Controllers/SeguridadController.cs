using APISeguridad.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace APISeguridad.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SeguridadController : ControllerBase
    {
        private readonly PermisoRepository _repo;

        public SeguridadController(IConfiguration config)
        {
            _repo = new PermisoRepository(config.GetConnectionString("StringConexion"));
        }

        [HttpGet("{idUsuario}")]
        public IActionResult GetPermisos(int idUsuario)
        {
            var permisos = _repo.ObtenerPermisosUsuario(idUsuario);
            return Ok(permisos);
        }

    }
}