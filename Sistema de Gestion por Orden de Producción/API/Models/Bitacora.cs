using System.ComponentModel.DataAnnotations;

namespace APIBitacoras.Model
{
    public class Bitacora
    {
        [Key]
        public int id_bitacora { get; set; }

        public int id_usuario { get; set; }

        public DateTime fecha { get; set; }

        public String accion { get; set; }

        public String detalle { get; set; }

    }
}
