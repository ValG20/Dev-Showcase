
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APISeguridad.Model
{
    public class Sistema
    {
        
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? idSistema { get; set; }

        [Required]
        public string nombre { get; set; }

        [Required]
        public string descripcion { get; set; }
    }
}

