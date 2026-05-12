using APIHotelBeach.SA.Services;
using APISeguridad.Model;
using APISeguridad.Model.DTO;
using AppWebHotelBeach.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APISeguridad.Controllers
{
    [ApiController]//permite al controlador interactuar como una Web API
    [Route("[controller]")] //Permite interpretar rutas para los métodos
    public class UsuariosController : ControllerBase
    {
        private readonly DbContextSeguridad _context;
        private readonly IAutorizacionServices autorizacionbServices;
        private readonly IConfiguration _config;
        private readonly PermisoRepository _repo;

        // Inyectas los servicios en el constructor
        public UsuariosController(DbContextSeguridad context, IAutorizacionServices autorizacionServices, IConfiguration config)
        {
            _context = context;
            autorizacionbServices = autorizacionServices;
            _repo = new PermisoRepository(config.GetConnectionString("StringConexion"));
        }

        [HttpPost]
        [Route("Autenticar")]
        public async Task<IActionResult> Autenticar(UsuarioSistema usuario)
        {
            // 1. Verificar que exista el usuario
            var user = await _context.usuarios.FirstOrDefaultAsync(u =>
                u.correo == usuario.correo &&
                u.clave == usuario.clave &&
                u.estado == "Activo");

            if (user == null)
                return Unauthorized("Usuario inválido o inactivo.");

            // 2. Obtener TODOS los permisos (directos y por rol) con tu repository
            var permisos = _repo.ObtenerPermisosUsuario(user.idUsuario);

            // 3. Verificar si tiene permisos
            if (permisos == null || permisos.Count == 0)
                return Unauthorized("El usuario no tiene permisos para este sistema.");

            // 4. Construir el DTO para el token (por ejemplo):
            var usuarioConPermisos = new UsuarioSistema
            {
                idUsuario = user.idUsuario,
                idSistema = usuario.idSistema,
                nombre = user.nombre,
                correo = user.correo,
                clave = user.clave,
                estado = user.estado,
                SolicitarPermisos = usuario.SolicitarPermisos,
                permisos = permisos
            };


            // 5. Generar el token (puede ser usando tu servicio actual)

            var autorizado = await autorizacionbServices.DevolverTokenConPermisos(usuarioConPermisos);

            return Ok(autorizado);

        }



        // Método 1: Acción HTTP GET para listar todos los usuarios
        // La ruta para acceder a este método sería: /[controller]/List
        [HttpGet("List")]
        public List<Usuario> List()
        {
            // Variable local de tipo lista que almacenará los usuarios recuperados de la base de datos.
            // Se accede a la tabla usuarios mediante el contexto de base de datos (_context).
            List<Usuario> temp = _context.usuarios.ToList();

            // Se retorna la lista de clientes al cliente que hizo la solicitud HTTP.
            return temp;
        }

        // Método 2: Acción HTTP GET para buscar un usuario por su identificador (ID).
        [HttpGet("SearchID")]
        public IActionResult SearchID(int id)
        {
            // Se utiliza el contexto (_context) y el ORM para buscar el primer usuario
            // que coincida con el ID proporcionado como parámetro.
            var temp = _context.usuarios.FirstOrDefault(x => x.idUsuario == id);

            // Validación: si no se encuentra el usuario, se retorna un mensaje indicando que no existe.
            if (temp == null)
            {
                // Retorna una respuesta 404 (Not Found) con un mensaje personalizado.
                return NotFound($"No existe un usuario con el identificador {id}.");
            }

            // Si el paquete fue encontrado, se retorna con un resultado HTTP 200 (OK).
            return Ok(temp);
        }

        [HttpGet("SearchByCorreo")]
        public IActionResult SearchByCorreo(string correo)
        {
            // Busca el primer usuario que tenga el correo igual al parámetro recibido (ignora mayúsculas/minúsculas)
            var temp = _context.usuarios.FirstOrDefault(x => x.correo.Trim().ToLower() == correo.Trim().ToLower());


            if (temp == null)
            {
                return NotFound($"No existe un usuario con el correo {correo}.");
            }

            return Ok(temp);
        }


        // Método 3: Acción HTTP POST encargada de guardar un nuevo usuario en la base de datos.
        [HttpPost("Save")]
        public string Save(Usuario temp)
        {
            // Mensaje inicial que se devolverá al usuario.
            string msj = "Usuario guardado correctamente.";

            try
            {
                // Se agrega el usuario recibido (temp) al contexto de base de datos.
                _context.usuarios.Add(temp);

                // Se aplican los cambios a la base de datos.
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                // En caso de que ocurra un error durante la inserción, se captura el detalle del error.
                // El mensaje de error reemplaza el mensaje inicial para informar al usuario.
                msj = ex.InnerException?.Message ?? ex.Message;
            }

            // Se retorna el mensaje final, ya sea de éxito o error.
            return msj;
        }

        // Método 4: Encargado de eliminar los datos de un usuario.
        [HttpDelete("Delete")]
        public string Delete(int pIdUsuario)
        {
            string msg = "Desactivar usuario...";
            try
            {
                // Se obtiene el usuario
                var usuario = _context.usuarios.FirstOrDefault(r => r.idUsuario == pIdUsuario);
                // Se verifica que no sea un valor nulo
                if (usuario == null)
                {
                    msg = "Usuario not found.";
                }
                else
                {
                    // Se realiza el borrado lógico cambiando el estado a "Inactivo"
                    usuario.estado = "Inactivo";
                    _context.SaveChanges();
                    msg = "Usuario desactivado correctamente.";
                }
            }
            catch (Exception ex)
            {
                msg = ex.InnerException?.Message ?? ex.Message;
            }
            return msg;
        }

        // Método 5:encargado de modificar los datos de un usuario.
        [HttpPut("Update")]
        public string Update(Usuario temp)
        {
            string msj = "Actualizando usuario...";

            try
            {
                // Se busca el usuario en la base de datos 
                var obj= _context.usuarios.FirstOrDefault(r => r.idUsuario == temp.idUsuario);

                if (obj == null)
                {
                    msj = "No existe el usuario.";
                }
                else
                {
                    // Se actualizan los datos del usuario con la nueva información proporcionada en 'temp'.
                    obj.nombre = temp.nombre;
                    obj.correo = temp.correo;
                    obj.clave = temp.clave;
                    obj.estado = temp.estado;

                    // Se marca el usuario como actualizado en el contexto de la base de datos.
                    _context.usuarios.Update(obj);

                    // Se aplican los cambios en la base de datos.
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                msj = ex.InnerException?.Message ?? ex.Message;
            }

            return msj;
        }


    }
}
