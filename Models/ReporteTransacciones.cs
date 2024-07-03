namespace ManejoPresupuestos.Models
{
    public class ReporteTransacciones
    {
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public IEnumerable<TransaccionesPorDia> Transacciones { get; set; }
        public int BalanceDepositos => Transacciones.Sum(x => x.BalanceDepositos);
        public int BalanceRetiros => Transacciones.Sum(x => x.BalanceRetiros);

        public int Total => BalanceDepositos - BalanceRetiros ;

        public class TransaccionesPorDia
        {
            public DateTime FechaTransaccion { get; set; }
            public IEnumerable<Transaccion> Transacciones { get; set; }

            public int BalanceDepositos
                => Transacciones
                .Where(x => x.tipoOperacionId == TipoOperacion.Ingreso)
                .Sum(x => x.Monto);
            
            public int BalanceRetiros
                => Transacciones
                .Where(x => x.tipoOperacionId == TipoOperacion.Gasto)
                .Sum(x => x.Monto);
        }
    }
}
