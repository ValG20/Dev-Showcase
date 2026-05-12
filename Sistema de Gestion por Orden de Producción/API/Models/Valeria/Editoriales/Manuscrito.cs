namespace APIEditorialUCR.Model
{
    using APIBitacoras.Model;
    using Sistema_Editorial.Model;
    using System; // Agregado para DateTime
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    // Corresponde a la tabla Manuscrito
    // Depende de Propuesta (Models/Core/Propuesta.cs)
    public class Manuscrito
    {
        [Key]
        public int id_manuscrito { get; set; }

        [ForeignKey("Propuesta")]
        public int id_propuesta { get; set; }

        public int id_autor { get; set; }
        public string titulo_trabajo { get; set; }
        public DateTime fecha_envio { get; set; }
        public string estado { get; set; }
        public string observaciones_generales { get; set; }

        // Propiedad de navegación
        public Presupuestos Propuesta { get; set; }


        // Para Archivos_Adjuntos (Archivos de Manuscrito)
        public ICollection<Archivo_manuscrito> ArchivosManuscrito { get; set; }
    }
}