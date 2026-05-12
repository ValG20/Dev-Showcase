using System.ComponentModel.DataAnnotations;

namespace APIEditorialUCR.Model
{
    public class Correccion_Retroalimentacion
    {
        [Key]
        public int id_correccion { get; set; }
        public int id_manuscrito { get; set; }
        public int id_usuario { get; set; }
        public DateTime fecha_envio { get; set; }
        public string tipo { get; set; }
        public string comentarios { get; set; }
        public string archivo_adjunto { get; set; }
    }

}
