using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace APIEditorialUCR.Model
{
    public class Acta_Propuesta
    {
        [Key]
        public int id_acta_propuesta { get; set; }
        public int id_acta { get; set; }
        public int id_propuesta { get; set; }
        [Column(TypeName = "VARBINARY(MAX)")]
        public string dictamen { get; set; }
        public string observaciones_comite { get; set; }
    }
}
