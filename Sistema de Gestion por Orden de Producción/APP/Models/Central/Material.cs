using AppWebEditorial.Validations;
using System.ComponentModel.DataAnnotations;

namespace AppWebEditorial.Models
{
    public class Material
    {
        [Key]
        public int id_material { get; set; }
        [Required(ErrorMessage = "El nombre del material es obligatorio.")]
        public string nombre_material { get; set; }

        [Required(ErrorMessage = "Debe seleccionar una categoría.")]
        public string categoria { get; set; }

        [Required(ErrorMessage = "La unidad de medida es obligatoria.")]
        public string unidad_medida { get; set; }

        [Required(ErrorMessage = "El stock actual es obligatorio.")]
        [Range(0, double.MaxValue, ErrorMessage = "No se permiten valores negativos.")]
        public double stock_actual { get; set; }

        [Required(ErrorMessage = "El stock mínimo es obligatorio.")]
        [Range(0, double.MaxValue, ErrorMessage = "No se permiten valores negativos.")]
        public double stock_minimo { get; set; }

        [Required(ErrorMessage = "La ubicación es obligatoria.")]
        public string ubicacion { get; set; }

        [Required(ErrorMessage = "El costo unitario es obligatorio.")]
        [Range(0, double.MaxValue, ErrorMessage = "No se permiten valores negativos.")]
        public double costo_unitario { get; set; }

        [Required(ErrorMessage = "El proveedor es obligatorio.")]
        public string proveedor { get; set; }

        [Required(ErrorMessage = "El estado es obligatorio.")]
        public string estado { get; set; }

        [Required(ErrorMessage = "La fecha es obligatoria")]
        [DataType(DataType.Date)]
        [FechaNoFuturaAttribute(ErrorMessage = "La fecha no puede ser futura")]
        public DateTime fecha_registro { get; set; }
    }
}
