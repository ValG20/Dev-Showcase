using System.ComponentModel.DataAnnotations;

namespace APIEditorialUCR.Model
{
    public class Propuesta
    {
        [Key]
        public int id_propuesta { get; set; }
        public int id_usuario { get; set; }
        public int id_producto { get; set; }
        public DateTime fecha_envio { get; set; }
        public string estado { get; set; }
        public string observaciones_usuario { get; set; }
    }
}
