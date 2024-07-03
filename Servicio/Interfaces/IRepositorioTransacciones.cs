using ManejoPresupuestos.Models;

namespace ManejoPresupuestos.Servicio.Interfaces
{
    public interface IRepositorioTransacciones
    {
        Task Actualizar(Transaccion transaccion, int montoAnterior, int cuentaAnterior);
        Task Borrar(int id);
        Task Crear(Transaccion transaccion);
        Task<Transaccion> GetByID(int id, int usuarioId);
        Task<IEnumerable<Transaccion>> ObtenerPorCuentaId(TransaccionesPorCuenta_Model modelo);
        Task<IEnumerable<ReporteMensualQuery>> ObtenerPorMes(int usuarioId, int año);
        Task<IEnumerable<ReporteSemanalQuery>> ObtenerPorSemana(ParametroTransaccionDiariasDeUsuario modelo);
        Task<IEnumerable<Transaccion>> ObtenerPorUsuarioId(ParametroTransaccionDiariasDeUsuario parametro);
    }
}
