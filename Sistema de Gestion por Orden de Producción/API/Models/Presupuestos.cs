using System.ComponentModel.DataAnnotations;

namespace APIBitacoras.Model
{
    public class Presupuestos
    {
        [Key]
        public int id_presupuesto { get; set; }
        public int id_pedido { get; set; }
        public DateTime fecha_generacion { get; set; }
        public decimal total_materiales { get; set; }
        public decimal total_mano_obra { get; set; }
        public decimal total_general { get; set; }
        public int generado_por { get; set; }
        public int aprobado_por { get; set; }
        public string estado { get; set; }
        public string observaciones { get; set; }

    }

}
