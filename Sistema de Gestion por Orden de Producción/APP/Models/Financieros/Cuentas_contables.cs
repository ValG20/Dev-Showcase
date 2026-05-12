using System.ComponentModel.DataAnnotations;

namespace EditorialUCR.Models.Financieros
{
    public class CuentaContable
    {
        [Key]
        public int id_cuenta { get; set; }
        public string codigo_cuenta { get; set; }
        public string nombre_cuenta { get; set; }

    }
}