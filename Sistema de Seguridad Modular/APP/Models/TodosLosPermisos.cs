namespace AppWebSeguridad.Models
{
    public class TodosLosPermisos
    {
        public int idUsuario { get; set; }
        public int idPantalla { get; set; }
        public int idSistema { get; set; }
        public string nombrePantalla { get; set; }
        public int permisoInsertar { get; set; }
        public int permisoModificar { get; set; }
        public int permisoBorrar { get; set; }
        public int permisoConsultar { get; set; }
        public string fuente { get; set; } // 'DIRECTO' o 'ROL'
        public string nombreRol { get; set; } // puede ser null si es DIRECTO
    }
}
