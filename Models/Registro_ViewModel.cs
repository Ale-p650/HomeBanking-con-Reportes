using System.ComponentModel.DataAnnotations;

namespace ManejoPresupuestos.Models
{
    public class Registro_ViewModel
    {
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [EmailAddress(ErrorMessage ="Correo Invalido")]
        public string Mail { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string Password { get; set; }
    }
}
