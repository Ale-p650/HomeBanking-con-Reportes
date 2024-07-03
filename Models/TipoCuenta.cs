using ManejoPresupuestos.Controllers;
using ManejoPresupuestos.Validaciones;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ManejoPresupuestos.Models
{
    public class TipoCuenta
    {
        public int Id { get; set; }

        [Remote(controller:"TipoCuenta",action: "ExisteTipoCuenta")]
        [ContieneNumero]
        [Required(ErrorMessage ="El Campo {0} es requerido")]
        [StringLength(maximumLength:10,MinimumLength =3,ErrorMessage = "El campo {0} debe tener entre {2} y {1} caracteres")]
        public string Nombre { get; set; }

        public int UsuarioId { get; set; }
        public int Orden { get; set; }
    }
}
