using System.ComponentModel.DataAnnotations;

namespace Sistema_Editorial.Model
{
    public class Diseno
    {
        [Key]
        public int id_diseno { get; set; }
        public int id_usuario { get; set; }
        public int id_propuesta { get; set; }
        public DateTime fecha_envio { get; set; }
        public string descripcion { get; set; }
        public string estado { get; set; }
        public string observaciones { get; set; }
    }
}
