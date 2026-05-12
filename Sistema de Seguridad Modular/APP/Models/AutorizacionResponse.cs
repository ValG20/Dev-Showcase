using AppWebSeguridad.Models;
using AppWebSeguridad.Models.DTO;

namespace AppSeguridad.Models
{
    public class AutorizacionResponse
    {
        public string Token { get; set; }
        public bool Resultado { get; set; }
        public string Msj { get; set; }
        public string NombreRol { get; set; }
        public List<PermisoVista> Permisos { get; set; }
        public List<Pantalla> Pantallas { get; set; }

    }
}
