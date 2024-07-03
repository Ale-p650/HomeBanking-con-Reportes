using ManejoPresupuestos.Validaciones;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace ManejoPresupuestos.Models
{
    public class Cuenta
    {
        public int Id { get; set; }

        [Required(ErrorMessage ="ERROR El campo{0} es requerido")]
        [StringLength(maximumLength:40,MinimumLength =3)]
        [PrimeraLetraMayuscula]
        public string Nombre { get; set; }

        [Display(Name ="Tipo de Cuenta")]
        public int TipoCuentaId { get; set; }
        public int Balance { get; set; }

        [MaxLength(1000)]
        public string? Descripcion { get; set; }

        public string TipoCuenta { get; set; }
    }
}
