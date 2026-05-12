using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
//Bitacora de actividades del sistema, se registra cada acción realizada por los usuarios en el sistema, incluyendo la fecha, hora, usuario, pantalla y acción realizada. Esta información es útil para auditorías y seguimiento de actividades sospechosas.
namespace APISeguridad.Model
{
    public class Bitacora
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int idBitacora { get; set; }

        [Required]
        public int idUsuario { get; set; }
        
        [Required]
        public int idSistema { get; set; }
       
        [Required]
        public int idPantalla { get; set; }

        [Required]
        public DateTime fecha { get; set; }

        [Required]
        public string accion { get; set; }

        [Required]
        public string detalle { get; set; }
    }
}
