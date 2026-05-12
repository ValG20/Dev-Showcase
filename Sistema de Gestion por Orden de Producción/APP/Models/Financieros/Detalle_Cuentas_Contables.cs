using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EditorialUCR.Models.Financieros
{
    public class Detalle_Cuentas_Contables
    {
        [Key]
        public int id_detalle { get; set; }

        // Clave foránea al Pedido
        public int id_pedido { get; set; }

        // Clave foránea a la Cuenta Contable (NOT NULL)
        public int id_cuenta { get; set; }
        public decimal costo_directo { get; set; }
        public decimal costo_indirecto { get; set; }
        public decimal libro_digital { get; set; }
        public decimal otros { get; set; }
    }
}
