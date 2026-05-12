namespace AppWebSeguridad.Models.DTO
{
    public class PermisoVista
    {
        public int IdPantalla { get; set; }
        public string NombrePantalla { get; set; } // opcional
        public bool PuedeConsultar { get; set; }
        public bool PuedeInsertar { get; set; }
        public bool PuedeModificar { get; set; }
        public bool PuedeBorrar { get; set; }
    }

}
