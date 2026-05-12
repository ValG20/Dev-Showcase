using System.ComponentModel.DataAnnotations;

namespace EditorialUCR.Models.SolicitudPublicacion
{
    public class Acta_Propuesta
    {
        [Key]
        public int id_acta_propuesta { get; set; }
        public int id_acta { get; set; }
        public int id_propuesta { get; set; }
        public string dictamen { get; set; }
        public string observaciones_comite { get; set; }
    }

}
