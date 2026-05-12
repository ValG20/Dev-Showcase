using System.ComponentModel.DataAnnotations;

namespace APIEditorialUCR.Model
{
    public class Direccion_Autor
    {
        [Key]
        public int id_autor { get; set; }
        public string direccion { get; set; }
    }
}
