using System.ComponentModel.DataAnnotations;

namespace APIEditorialUCR.Model
{
        public class Autor_Propuesta
        {
            [Key]
            public int id_autor_propuesta { get; set; }
            public int id_propuesta { get; set; }
            public int id_autor { get; set; }
            public string funcion_en_obra { get; set; }
        }
}
