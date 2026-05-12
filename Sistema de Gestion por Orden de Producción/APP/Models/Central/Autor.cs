namespace EditorialUCR.Models.Central
{
    public class Autor
    {
        public int id_autor { get; set; }
        public string nombre_apellidos { get; set; }
        public string nacionalidad { get; set; }
        public string tipo_cedula { get; set; } // <<-- NUEVO CAMPO
        public string documento_identidad { get; set; }
        public string estado_civil { get; set; }
        public string profesion { get; set; }
        public DateTime fecha_actualizacion { get; set; }
        public string correo_electronico { get; set; }
        public string direccion { get; set; } // <<-- NUEVO CAMPO
        public string telefono_habitacion { get; set; } // <<-- NUEVO CAMPO
        public string telefono_celular { get; set; } // <<-- NUEVO CAMPO
        public string telefono_oficina { get; set; } // <<-- NUEVO CAMPO
    }
}
