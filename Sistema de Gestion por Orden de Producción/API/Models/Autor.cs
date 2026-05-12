using System;
using System.ComponentModel.DataAnnotations;

namespace APIBitacoras.Model
{
    public class Autor
    {
        [Key]
        public int id_autor { get; set; }

        public string nombre_apellidos { get; set; }

        public string nacionalidad { get; set; }

        public string tipo_cedula { get; set; }

        public string documento_identidad { get; set; }

        public string estado_civil { get; set; }

        public string profesion { get; set; }

        public DateTime fecha_actualizacion { get; set; }

        public string correo_electronico { get; set; }
    }
}
