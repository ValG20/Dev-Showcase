using System.ComponentModel.DataAnnotations;

namespace AppWebSeguridad.Models
{
    public class UsuariosRoles
    {
        [Key]
        public int idUsuario { get; set; }
        public int idRol { get; set; }
    }
}
