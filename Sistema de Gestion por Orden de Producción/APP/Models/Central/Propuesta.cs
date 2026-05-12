
using EditorialUCR.Models.Editoriales;
using System; 
using System.ComponentModel.DataAnnotations;

namespace EditorialUCR.Models.Central
{ 
    public class Propuesta
    {
        [Key]
        public int id_propuesta { get; set; }
        public int id_usuario { get; set; }
        public int? id_producto { get; set; }
        public DateTime fecha_envio { get; set; }
        public string estado { get; set; }
        public string observaciones_usuario { get; set; }

    }
}