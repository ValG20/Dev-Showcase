namespace EditorialUCR.Models.Editoriales
{
    public class Archivo_disenno
    {
        public int id_archivo { get; set; }
        public int id_diseno { get; set; }
        public string nombre_archivo { get; set; }
        public string tipo_archivo { get; set; }
        public string ruta_almacenamiento { get; set; }
        public DateTime fecha_subida { get; set; }
    }
}
