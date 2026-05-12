using System.ComponentModel.DataAnnotations;

namespace EditorialUCR.Models.SolicitudPublicacion
{
    public class Historial_Cambios_Propuesta
    {
        [Key]
        public int id_historial { get; set; }
        public int id_propuesta { get; set; }
        public DateTime fecha_modificacion { get; set; }
        public int usuario_modificador { get; set; }
        public string detalle_cambios { get; set; }
    }

}
