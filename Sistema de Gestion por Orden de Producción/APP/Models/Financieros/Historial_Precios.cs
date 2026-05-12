using System.ComponentModel.DataAnnotations;

namespace EditorialUCR.Models.Financieros
{
    public class Historial_Precios
    {
        [Key]
        public int id_historial { get; set; }

        public int id_producto { get; set; }

        public int usuario_modificador { get; set; }

        public DateTime fecha_modificacion { get; set; }

        public decimal precio_anterior { get; set; }

        public decimal precio_nuevo{ get; set; }

    }
}
