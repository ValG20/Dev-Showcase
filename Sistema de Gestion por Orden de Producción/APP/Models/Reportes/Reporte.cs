namespace EditorialUCR.Models.Reportes
{
    using System.ComponentModel.DataAnnotations;
    using System.Collections.Generic;
    using System; // Agregado para DateTime y DateOnly

    // Corresponde a la tabla Reportes
    public class Reporte
    {
        [Key]
        public int id_reporte { get; set; }
        public string tipo_reporte { get; set; }
        public string titulo { get; set; }
        public string descripcion { get; set; }
        public int? usuario_generador { get; set; }
        public DateTime fecha_generacion { get; set; }
        public string ruta_archivo { get; set; }
        public string formato { get; set; }
        public DateOnly? periodo_inicio { get; set; } // Usando DateOnly para DATE
        public DateOnly? periodo_fin { get; set; }
        public string estado { get; set; }

        // Propiedad de navegación a los detalles del reporte
        public ICollection<DetalleReporte> DetallesReporte { get; set; }
    }
}
