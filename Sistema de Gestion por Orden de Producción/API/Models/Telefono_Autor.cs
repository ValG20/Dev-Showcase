using System.ComponentModel.DataAnnotations;

namespace APIEditorialUCR.Model
{
    public class Telefono_Autor
    {
        [Key]
        public int id_autor { get; set; }
        public string telefono { get; set; }
    }
}
