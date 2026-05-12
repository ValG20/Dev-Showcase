namespace EditorialUCR.Models.Central
{
    public class Autor_propuesta
    {
        public int id_autor_propuesta { get; set; }

        public int? id_propuesta { get; set; }

        public int? id_autor { get; set; }

        public string funcion_en_obra { get; set; }
    }
}
