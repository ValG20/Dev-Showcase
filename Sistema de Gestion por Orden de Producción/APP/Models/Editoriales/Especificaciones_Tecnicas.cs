using System.ComponentModel.DataAnnotations;

namespace APIEditorialUCR.Model
{
    public class Especificaciones_Tecnicas
    {
        [Key]
        public int id_tecnica { get; set; }
        public int id_pedido { get; set; }
        public int num_paginas { get; set; }
        public string tipo_impreso { get; set; }
        public int no_pag_color { get; set; }
        public string departamento_impresion { get; set; }
        public string contenido_portada { get; set; }
        public int cts_num_impresiones { get; set; }
        public int cant_pliegos_1 { get; set; }
        public int cant_pliegos_2 { get; set; }
        public int cant_pliegos_3 { get; set; }
    }
}
