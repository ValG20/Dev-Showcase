using System.ComponentModel.DataAnnotations;

namespace APIEditorialUCR.Model
{
    public class Cuentas_Contables
    {
        [Key]
        public int id_cuenta { get; set; }
        public string codigo_cuenta { get; set; }
        public string nombre_cuenta { get; set; }



    }
}
