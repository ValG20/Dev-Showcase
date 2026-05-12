namespace EditorialUCR.Models.Impresion
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    // Corresponde a la tabla Ejemplares_Costos
    // Depende de Pedido (Models/Core/Pedido.cs)
    public class EjemplarCosto
    {
        [Key]
        public int id_ejemplar { get; set; }

        [ForeignKey("Pedido")]
        public int id_pedido { get; set; }
        public int cantidad { get; set; }
        public decimal costo { get; set; }

    
    }
}