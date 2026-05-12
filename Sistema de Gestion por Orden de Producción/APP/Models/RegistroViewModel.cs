// Models/RegistroViewModel.cs
using System.ComponentModel.DataAnnotations;

namespace EditorialUCR.Models
{
    public class RegistroViewModel
    {
        [Required]
        public string Usuario { get; set; } = "";

        [Required, EmailAddress]
        public string Correo { get; set; } = "";

        [Required, DataType(DataType.Password)]
        public string Password { get; set; } = "";

        [Required, DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Las contraseñas no coinciden.")]
        public string ConfirmarPassword { get; set; } = "";
    }
}

