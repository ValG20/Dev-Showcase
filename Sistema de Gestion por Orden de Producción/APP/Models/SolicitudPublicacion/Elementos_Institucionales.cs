using System.ComponentModel.DataAnnotations;

namespace EditorialUCR.Models.SolicitudPublicacion
{
    public class Elementos_Institucionales
    {
        [Key]
        public int id_elemento { get; set; }
        public int id_pedido { get; set; }
        public string url_editorial { get; set; }
        public string logo_ucr { get; set; }
        public string siedin { get; set; }
        public string titulo_area { get; set; }
        public string telefono_area { get; set; }
    }

}
