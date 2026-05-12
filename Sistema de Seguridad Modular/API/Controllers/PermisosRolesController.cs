using APISeguridad.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace APISeguridad.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PermisosRolesController : ControllerBase
    {
        private readonly DbContextSeguridad _context = null;

        public PermisosRolesController(DbContextSeguridad pContext)
        {
            _context = pContext;
        }

        [HttpGet("List")]
        public List<PermisosRoles> List()
        {
            List<PermisosRoles> temp = _context.permisosRoles.ToList();

            return temp;
        }

        [HttpGet("SearchID")]
        public IActionResult SearchID(int idRol, int idSistema, int idPantalla)
        {
            var temp = _context.permisosRoles.FirstOrDefault(x => x.idRol == idRol && x.idSistema == idSistema  && x.IdPantalla == idPantalla);

            if (temp == null)
            {
                return NotFound($"No existe un permiso con el identificador");
            }
            return Ok(temp);
        }

        [HttpPost("Save")]
        public string Save(PermisosRoles temp)
        {
            string msj = "Permisos del usuario guardados correctamente.";
            temp.PermisoConsultar = 1;
            try
            {
                _context.permisosRoles.Add(temp);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                msj = ex.InnerException?.Message ?? ex.Message;
            }
            return msj;
        }


        [HttpDelete("Delete")]
        public string Delete(int idRol, int idSistema, int idPantalla)
        {
            string msg = "Permisos del usuario eliminado...";

            try
            {
                var permiso = _context.permisosRoles.FirstOrDefault(r => r.idRol == idRol && r.idSistema == idSistema && r.IdPantalla == idPantalla);

                if (permiso == null)
                {
                    msg = "No existe";
                }
                else
                {

                    _context.permisosRoles.Remove(permiso);
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                msg = ex.InnerException?.Message ?? ex.Message;
            }

            return msg;
        }


        [HttpPut("Edit")]
        public IActionResult Edit([FromBody] PermisosRoles permisoEditado)
        {
            if (permisoEditado == null)
            {
                return BadRequest("Datos inválidos.");
            }

            var permisoExistente = _context.permisosRoles.FirstOrDefault(p =>
                p.idRol == permisoEditado.idRol &&
                p.idSistema == permisoEditado.idSistema &&
                p.IdPantalla == permisoEditado.IdPantalla);

            if (permisoExistente == null)
            {
                return NotFound("No se encontró el permiso del rol.");
            }

            // Actualizar los campos
            permisoExistente.PermisoInsertar = permisoEditado.PermisoInsertar;
            permisoExistente.PermisoModificar = permisoEditado.PermisoModificar;
            permisoExistente.PermisoBorrar = permisoEditado.PermisoBorrar;
            permisoExistente.PermisoConsultar = 1;

            _context.SaveChanges();

            return Ok("Permiso actualizado correctamente.");
        }

    }
}
