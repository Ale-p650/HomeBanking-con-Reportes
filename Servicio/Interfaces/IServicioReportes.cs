using ManejoPresupuestos.Models;

namespace ManejoPresupuestos.Servicio.Interfaces
{
    public interface IServicioReportes
    {
        ReporteTransacciones GenerarReporteTransacciones(DateTime fechaInicio, DateTime fechaFin, IEnumerable<Transaccion> transacciones);
        Task<IEnumerable<ReporteSemanalQuery>> ObtenerReporteSemanal(int usuarioId, int mes, int año, dynamic ViewBag);
        Task<ReporteTransacciones> ObtenerReporteTransacciones(int usuarioId, int mes, int año, dynamic ViewBag);
        Task<ReporteTransacciones> ObtenerReporteTransaccionesPorCuenta(int usuarioId, int cuentaId, int mes, int año, dynamic ViewBag);
    }
}
