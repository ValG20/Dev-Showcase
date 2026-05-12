using System.ComponentModel.DataAnnotations;

namespace APIEditorialUCR.Model
{
    public class Portada
    {
        [Key]
        public int id_portada { get; set; }
        public int id_pedido { get; set; }
        public string tipo_impresion { get; set; }
        public string tipo_papel { get; set; }
        public string tamano_corte { get; set; }
        public string salen_tc { get; set; }
    }
}
