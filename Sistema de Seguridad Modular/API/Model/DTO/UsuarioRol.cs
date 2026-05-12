using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APISeguridad.Model.DTO
{
    public class UsuarioRol
    {
        public int idUsuario { get; set; }
        public int idSistema { get; set; }
        public int idRol { get; set; }
        public string nombre { get; set; }
        public string correo { get; set; }
        public string clave { get; set; }
        public string estado { get; set; }
        public string nombreRol { get; set; }

    }
}
