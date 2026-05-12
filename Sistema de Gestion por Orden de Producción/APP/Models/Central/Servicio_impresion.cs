using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EditorialUCR.Models.Central
{
    public class Servicio_impresion
    {
        public int id_servicio { get; set; }
        public int id_pedido { get; set; }
        public DateTime fecha_solicitud { get; set; }
        [Required(ErrorMessage = "El tamaño es obligatorio.")]
        public string tamaño { get; set; }

        [Required(ErrorMessage = "El tipo de cartulina de portada es obligatorio.")]
        public string portada_tipo_cartulina { get; set; }

        [Required(ErrorMessage = "El origen del contenido es obligatorio.")]
        public string contenido_origen { get; set; }

        [Required(ErrorMessage = "La separación de colores es obligatoria.")]
        public string colores_separacion { get; set; }

        [Required(ErrorMessage = "El tipo de impresión es obligatorio.")]
        public string impresion_tipo { get; set; }

        [Required(ErrorMessage = "El sello de la unidad es obligatorio.")]
        public string sello_unidad { get; set; }
        [Required(ErrorMessage = "La cantidad de páginas es obligatoria.")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad de páginas debe ser mayor a 0.")]
        public int cantidad_paginas { get; set; }

        [Required(ErrorMessage = "El tipo de papel es obligatorio.")]
        public string tipo_papel { get; set; }
        public string opciones_impresion { get; set; }
        public bool cd_adjunto { get; set; }
        public bool textos_digitados { get; set; }
        public bool archivo_enviado_email { get; set; }
        public bool original_y_copias { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "La numeración desde debe ser positiva.")]
        public int numeracion_desde { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "La numeración hasta debe ser positiva.")]
        public int numeracion_hasta { get; set; }
        public bool en_blocks_ejemplares { get; set; }
        public string encolado { get; set; }
        public string fuente_financiamiento { get; set; }
        public string cargado_a { get; set; }
        public string codigo_presupuestario { get; set; }
        [Required(ErrorMessage = "Los datos de contacto son obligatorios.")]
        public string datos_contacto { get; set; }

        // Este campo puede venir como archivo, ya lo manejas en el controller
        public string autorizacion_firma { get; set; }

        public string sello_recibido_por_siedin { get; set; }
        public string retiro_firma { get; set; }
        public DateTime fecha_retiro { get; set; }
        public string recepcionista { get; set; }
        public DateTime fecha_hora_recepcion { get; set; }

     
    }
}
