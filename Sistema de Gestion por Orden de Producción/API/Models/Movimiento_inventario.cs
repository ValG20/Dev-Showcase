using System.ComponentModel.DataAnnotations;

namespace Sistema_Editorial.Model
{
    public class Movimiento_inventario
    {
        [Key]
        public int id_movimiento { get; set; }
        public int id_producto { get; set; }
        public string tipo_movimiento { get; set; }
        public decimal cantidad { get; set; }
        public int usuario_responsable { get; set; }
        public DateTime fecha_movimiento { get; set; }
        public string observaciones { get; set; }
    }
}
