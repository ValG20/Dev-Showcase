using System.ComponentModel.DataAnnotations;

namespace Sistema_Editorial.Model
{
    public class Pedido
    {
        [Key]
        public int id_pedido { get; set; }
        public string tipo_pedido { get; set; }
        public int id_propuesta { get; set; }
        public DateTime? fecha_creacion { get; set; }
        public string? orden_servicio { get; set; }
        public string? titulo_trabajo { get; set; }
        public string? dependencia { get; set; }
        public string? funcionario { get; set; }
        public string? estado { get; set; }
        public string? correo_funcionario { get; set; }
        public string? telefono_funcionario { get; set; }
        public string? fax { get; set; }
        public string? consentimiento_cliente { get; set; }
        public string? sello { get; set; }
        public string? responsable_vb { get; set; }
        public string? observaciones_generales { get; set; }
        public int cant_cd { get; set; }
        public DateTime? fecha_actualizacion { get; set; }
        public string? campos_servicio_extra { get; set; }
    }
}
