using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppWebSeguridad.Models
{
    public class Pantalla
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] public int idPantalla { get; set; }
        [Required]
        public int idSistema { get; set; }
        [Required]
        public string Nombre { get; set; }
        [Required]
        public string Descripcion { get; set; }
        [Required]
        public string Ruta { get; set; }

    }
}
