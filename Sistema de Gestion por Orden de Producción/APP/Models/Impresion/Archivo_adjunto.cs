namespace EditorialUCR.Models.Impresion
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    // Corresponde a la tabla Archivos_Adjuntos_Propuesta
    public class Archivo_adjunto
    {
        [Key]
        public int id_archivo { get; set; }

        [ForeignKey("Propuesta")]
        public int? id_propuesta { get; set; }

        [MaxLength(255)]
        public string nombre_archivo { get; set; }

        [MaxLength(255)]
        public string tipo_archivo { get; set; }

        [MaxLength(255)]
        public string ruta_almacenamiento { get; set; }

        // Propiedad de navegación
        //public Propuesta_editorial Propuesta { get; set; }
    }
}
