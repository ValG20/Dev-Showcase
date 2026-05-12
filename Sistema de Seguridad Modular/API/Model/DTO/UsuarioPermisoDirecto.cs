namespace APISeguridad.Model.DTO
{
    public class UsuarioPermisoDirecto
    {
        public int idUsuario { get; set; }
        public string correo { get; set; }
        public string clave { get; set; }
        public int idSistema { get; set; }
        public string nombre { get; set; }
        public string estado { get; set; }
    }
}
