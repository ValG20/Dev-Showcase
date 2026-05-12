using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppWebSeguridad.Models
{
    public class Usuario
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int idUsuario { get; set; }

        public string nombre { get; set; }

        public string correo { get; set; }

        public string clave { get; set; }

        public string estado { get; set; }
    }
}
