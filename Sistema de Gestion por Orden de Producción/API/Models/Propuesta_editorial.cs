using System.ComponentModel.DataAnnotations;

namespace Sistema_Editorial.Model
{
    public class Propuesta_Editorial
    {
        [Key]
        public int id_propuesta { get; set; }
        public DateTime fecha_creacion { get; set; }
        public string titulo_obra { get; set; }
        public string subtitulo { get; set; }
        public string serie_coleccion { get; set; }
        public string publico_meta { get; set; }
        public string palabras_claves { get; set; }
    }

}
