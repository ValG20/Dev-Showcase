namespace EditorialUCR.Models.SolicitudPublicacion
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    
        public class Propuesta_Editorial
        {
            [Key]
            [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
            public int id_propuesta { get; set; }

            // ===== Información del Libro =====
            [Required(ErrorMessage = "El título es obligatorio.")]
            [StringLength(300)]
            public string titulo_obra { get; set; } = string.Empty;

            [StringLength(300)]
             public string subtitulo { get; set; }

        [Required(ErrorMessage = "El nombre de la serie es obligatorio.")]
            [StringLength(200)]
            public string serie_coleccion { get; set; } = string.Empty;

            [Required(ErrorMessage = "El público meta es obligatorio.")]
            [StringLength(500)]
            public string publico_meta { get; set; } = string.Empty;

            // Palabras clave (almacenadas como texto separado por comas)
            [Required]
            [StringLength(700)]
            public string palabras_claves { get; set; } = string.Empty;
    
            public DateTime fecha_creacion { get; set; }
        }

}
