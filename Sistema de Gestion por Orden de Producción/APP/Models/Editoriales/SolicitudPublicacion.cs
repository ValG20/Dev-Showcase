namespace EditorialUCR.Models.Editoriales
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    
        public class SolicitudPublicacion
        {
            [Key]
            [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
            public int Id { get; set; }

            // ===== Información del Libro =====
            [Required(ErrorMessage = "El título es obligatorio.")]
            [StringLength(300)]
            public string Titulo { get; set; }
            [StringLength(300)]
            public string? Subtitulo { get; set; }

            [Required(ErrorMessage = "El nombre de la serie es obligatorio.")]
            [StringLength(200)]
            public string Serie { get; set; }

            [Required(ErrorMessage = "El público meta es obligatorio.")]
            [StringLength(500)]
            public string PublicoMeta { get; set; }

            // Palabras clave (almacenadas como texto separado por comas)
            [Required]
            [StringLength(700)]
            public string PalabrasClave { get; set; }

            // ===== Datos Personales =====
            [Required, StringLength(100)]
            public string Nombre { get; set; } 

            [Required, StringLength(150)]
            public string Apellidos { get; set; }

            [Required, StringLength(100)]
            public string Nacionalidad { get; set; } 

            [Required, StringLength(50)]
            public string TipoCedula { get; set; } 

            [Required, StringLength(50)]
            public string NumeroCedula { get; set; }

            [Required, StringLength(50)]
            public string EstadoCivil { get; set; } 

            [Required, StringLength(100)]
            public string Profesion { get; set; }

            [Required, StringLength(500)]
            public string Direccion { get; set; }

            [StringLength(50)]
            public string? TelHabitacion { get; set; }

            [Required, StringLength(50)]
            public string TelCelular { get; set; } 

            [StringLength(50)]
            public string? TelOficina { get; set; }

            [Required, EmailAddress, StringLength(150)]
            public string Correo { get; set; }

            // ===== Funciones en la Publicación =====
            [StringLength(300)]
            public string? Funciones { get; set; } // Almacenadas como texto separado por comas (Autor, Editor, etc.)

            [StringLength(200)]
            public string? OtraFuncion { get; set; }

            // ===== Auditoría =====
            public DateTime FechaRegistro { get; set; } = DateTime.Now;
        }

}
