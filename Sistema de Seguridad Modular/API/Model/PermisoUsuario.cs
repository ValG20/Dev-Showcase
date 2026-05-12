using System.ComponentModel.DataAnnotations;

namespace APISeguridad.Model
{
    public class PermisoUsuario
    {
        
        public int IdUsuario { get; set; }
        public int IdSistema { get; set; }
        public int IdPantalla { get; set; }
        public int PermisoInsertar { get; set; } 
        public int PermisoModificar { get; set; } 
        public int PermisoBorrar { get; set; } 
        public int PermisoConsultar { get; set; } 
    }
}
