using System.ComponentModel.DataAnnotations;

namespace EditorialUCR.Models.SolicitudPublicacion
{
    public class Actas_Sesion
    {
        [Key]
        public int id_acta { get; set; }
        public DateTime fecha_sesion { get; set; }
        public TimeSpan hora_inicio { get; set; }
        public TimeSpan hora_fin { get; set; }
        public string lugar { get; set; }
        public string orden_dia { get; set; }
        public string desarrollo_puntos { get; set; }
        public int id_secretario { get; set; }
    }

}
