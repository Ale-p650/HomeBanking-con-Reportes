using System.ComponentModel.DataAnnotations;

namespace ManejoPresupuestos.Models
{
    public class Transaccion
    {
        public int Id { get; set; }

        public int UsuarioId { get; set; }


        [Display(Name ="Fecha de Transaccion")]
        [DataType(DataType.Date)]
        public DateTime FechaTransaccion { get; set; } = DateTime.Today;

        public int Monto { get; set; }

        public int TipoTransaccionId { get; set; }


        [StringLength(maximumLength:1000,ErrorMessage ="La nota no puede exceder los {1} caracteres")]
        public string? Nota { get; set; }


        [Display(Name ="Cuenta")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar una cuenta")]
        public int CuentaId { get; set; }


        [Display(Name ="Categoria")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar una categoria")]
        public int CategoriaId { get; set; }

        [Display(Name = "Tipo de Operacion")]
        
        public TipoOperacion tipoOperacionId { get; set; } = TipoOperacion.Ingreso;

        public string Cuenta { get; set; }
        public string Categoria { get; set; }
    }
}
