namespace EditorialUCR.Models.Editoriales
{
    public class Diseno
    {
        public int id_diseno { get; set; }
        public int id_usuario { get; set; }
        public int id_propuesta { get; set; }
        public DateTime fecha_envio { get; set; }
        public string descripcion { get; set; }
        public string estado { get; set; }
        public string observaciones { get; set; }
    }
}
