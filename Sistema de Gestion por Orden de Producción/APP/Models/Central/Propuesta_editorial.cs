namespace EditorialUCR.Models.Central
{
    public class Propuesta_editorial
    {
        public int id_propuesta { get; set; }
        public DateTime fecha_creacion { get; set; }
        public string titulo_obra { get; set; }
        public string subtitulo { get; set; }
        public string serie_coleccion { get; set; }
        public string publico_meta { get; set; }
        public string palabras_claves { get; set; }
    }
}
