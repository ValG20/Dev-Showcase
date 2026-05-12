using APISeguridad.Model;

namespace AppWebHotelBeach.Models.Dto
{
    public class UsuarioSistema
    {
        public int idUsuario { get; set; }
        public int idSistema { get; set; }
        public string nombre { get; set; }
        public string correo { get; set; }
        public string clave { get; set; }
        public string estado { get; set; }
        public bool SolicitarPermisos { get; set; } = true;
        public List<TodosLosPermisos> permisos { get; set; }

    }
}
