using System.ComponentModel.DataAnnotations;

namespace Sistema_Editorial.Model
{
    public class Material
    {
        [Key]
        public int id_material { get; set; }
        public string nombre_material { get; set; }
        public string categoria { get; set; }
        public string unidad_medida { get; set; }
        public decimal stock_actual { get; set; }
        public decimal stock_minimo { get; set; }
        public string ubicacion { get; set; }
        public decimal costo_unitario { get; set; }
        public string proveedor { get; set; }
        public string estado { get; set; }
        public DateTime fecha_registro { get; set; }
    }
}
