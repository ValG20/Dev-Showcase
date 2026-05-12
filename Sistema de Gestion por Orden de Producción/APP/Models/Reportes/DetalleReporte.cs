namespace EditorialUCR.Models.Reportes
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    // Corresponde a la tabla Detalle_Reporte
    // Depende de Reporte (Models/Reporting/Reporte.cs)
    public class DetalleReporte
    {
        [Key]
        public int id_detalle { get; set; }

        [ForeignKey("Reporte")]
        public int id_reporte { get; set; }
        public int id_referencia { get; set; }
        public string tipo_referencia { get; set; }

        // Propiedad de navegación
        public Reporte Reporte { get; set; }
    }
}
