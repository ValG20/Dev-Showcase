namespace EditorialUCR.Models.Financieros
{
    using System; // Agregado para DateTime
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    // Corresponde a la tabla Presupuestos (necesaria para Detalle_Costos)
    // Depende de Pedido (Models/Core/Pedido.cs)
    public class Presupuesto
    {
        [Key]
        public int id_presupuesto { get; set; }

        [ForeignKey("Pedido")]
        public int id_pedido { get; set; }
        public DateTime fecha_generacion { get; set; }
        public decimal total_materiales { get; set; }
        public decimal total_mano_obra { get; set; }
        public decimal total_general { get; set; }
        public int? generado_por { get; set; }
        public int? aprobado_por { get; set; }
        public string estado { get; set; }
        public string observaciones { get; set; }

     

        // Para Detalle_Costos
        public ICollection<DetalleCosto> DetallesCosto { get; set; }
    }
}
