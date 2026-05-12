using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sistema_Editorial.Model
{

    public class Servicio_impresion
    {
        [Key]
        public int id_servicio { get; set; }
        public int id_pedido { get; set; }
        public DateTime fecha_solicitud { get; set; }
        public string tamaño { get; set; }
        public string portada_tipo_cartulina { get; set; }
        public string contenido_origen { get; set; }
        public string colores_separacion { get; set; }
        public string impresion_tipo { get; set; }
        public string sello_unidad { get; set; }
        public int cantidad_paginas { get; set; }
        public string tipo_papel { get; set; }
        public string opciones_impresion { get; set; }
        public bool cd_adjunto { get; set; }
        public bool textos_digitados { get; set; }
        public bool archivo_enviado_email { get; set; }
        public bool original_y_copias { get; set; }
        public int numeracion_desde { get; set; }
        public int numeracion_hasta { get; set; }
        public bool en_blocks_ejemplares { get; set; }
        public string encolado { get; set; }
        public string fuente_financiamiento { get; set; }
        public string cargado_a { get; set; }
        public string codigo_presupuestario { get; set; }
        public string datos_contacto { get; set; }
        public string autorizacion_firma { get; set; }
        public string sello_recibido_por_siedin { get; set; }
        public string retiro_firma { get; set; }
        public DateTime fecha_retiro { get; set; }
        public string recepcionista { get; set; }
        public DateTime fecha_hora_recepcion { get; set; }
    }
}
