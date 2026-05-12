namespace EditorialUCR.Models.SolicitudPublicacion
{
    public class SolicitudPublicacionViewModel
    {
        // ===== Propuesta_Editorial =====
        public string Titulo { get; set; }
        public string? Subtitulo { get; set; }
        public string Serie { get; set; }
        public string PublicoMeta { get; set; }
        public string PalabrasClave { get; set; }

        // ===== Autor =====
        public string Nombre { get; set; }
        public string Apellidos { get; set; }
        public string Nacionalidad { get; set; }
        public string TipoCedula { get; set; }
        public string NumeroCedula { get; set; }
        public string EstadoCivil { get; set; }
        public string Profesion { get; set; }
        public string Direccion { get; set; }
        public string? TelHabitacion { get; set; }
        public string TelCelular { get; set; }
        public string? TelOficina { get; set; }
        public string Correo { get; set; }

        // ===== Funciones (para Autor_Propuesta) =====
        public List<string> Funciones { get; set; }
        public string? OtraFuncion { get; set; }
    }
}