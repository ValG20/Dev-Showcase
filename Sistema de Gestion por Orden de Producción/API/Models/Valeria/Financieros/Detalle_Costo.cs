namespace APIEditorialUCR.Model
{
    using APIBitacoras.Model;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    // Corresponde a la tabla Detalle_Costos
    // Depende de Presupuesto (Models/Finanzas/Presupuesto.cs)
    public class Detalle_Costo
    {
        [Key]
        public int id_detalle { get; set; }

        [ForeignKey("Presupuesto")]
        public int id_presupuesto { get; set; }
        public string tipo_concepto { get; set; }
        public string descripcion { get; set; }
        public decimal cantidad { get; set; }
        public decimal costo_unitario { get; set; }
        public decimal subtotal { get; set; }

        // Propiedad de navegación
        public Presupuestos Presupuesto { get; set; }

    }
}
