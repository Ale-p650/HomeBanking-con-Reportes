using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ManejoPresupuestos.Models
{
    public class TransaccionCrear_ViewModel : Transaccion
    {
        public IEnumerable<SelectListItem>? Cuentas { get; set; }

        public IEnumerable<SelectListItem>? Categorias { get; set; }
    }
}
