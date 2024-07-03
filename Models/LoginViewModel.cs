using System.ComponentModel.DataAnnotations;

namespace ManejoPresupuestos.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [EmailAddress(ErrorMessage = "Correo Invalido")]
        public string Mail { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}
