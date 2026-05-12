using System.ComponentModel.DataAnnotations;

namespace APIEditorialUCR.Model
{
    public class Archivo_adjunto
    {
        [Key]
        public int id_archivo { get; set; }
        public int id_propuesta { get; set; }
        public string nombre_archivo { get; set; }
        public string tipo_archivo { get; set; }
        public string ruta_almacenamiento { get; set; }
    }
}
