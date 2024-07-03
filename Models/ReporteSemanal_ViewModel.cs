namespace ManejoPresupuestos.Models
{
    public class ReporteSemanal_ViewModel
    {
        public decimal Ingresos => TransaccionesPorSemana.Sum(x => x.Ingresos);
        public decimal Gastos => TransaccionesPorSemana.Sum(x => x.Gastos);
        public decimal Total => Ingresos - Gastos;
        public DateTime FechaReferencia { get; set; }
        public IEnumerable<ReporteSemanalQuery> TransaccionesPorSemana { get; set; }
    }
}
