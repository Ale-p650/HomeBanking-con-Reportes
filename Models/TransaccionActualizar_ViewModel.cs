namespace ManejoPresupuestos.Models
{
    public class TransaccionActualizar_ViewModel : TransaccionCrear_ViewModel
    {
        public int CuentaAnteriorId { get; set; }
        public int MontoAnterior { get; set; }

        public string urlRetorno { get; set; }
    }
}
