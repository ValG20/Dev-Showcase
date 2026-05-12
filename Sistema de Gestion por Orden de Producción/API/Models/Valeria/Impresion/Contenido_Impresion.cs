namespace Sistema_Editorial.Model.Valeria.Impresion
{
    using Sistema_Editorial.Model;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    // Corresponde a la tabla Contenido_Impresion
    // Depende de Pedido (Models/Core/Pedido.cs)
    public class Contenido_Impresion
    {
        [Key]
        public int id_contenido { get; set; }

        [ForeignKey("Pedido")]
        public int id_pedido { get; set; }
        public string tipo_impresion { get; set; }
        public string tipo_papel { get; set; }
        public string tamano_corte { get; set; }
        public string salen_tc { get; set; }
    }
}