namespace EditorialUCR.Models.Impresion
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    // Corresponde a la tabla Acabados_Finales
    // Depende de Pedido (Models/Core/Pedido.cs)
    public class AcabadoFinal
    {
        [Key]
        public int id_acabado { get; set; }

        [ForeignKey("Pedido")]
        public int id_pedido { get; set; }
        public int cant_copias { get; set; }
        public string tipos { get; set; }
        public string otro { get; set; }
        public string tecnologia_impresion_especial { get; set; }

     
    }
}