using ManejoPresupuestos.Models;

namespace ManejoPresupuestos.Servicio.Interfaces
{
    public interface IRepositorioCuentas
    {
        Task Actualizar(CuentaCrear_ViewModel cuenta);
        Task Borrar(int id);
        public Task Crear(Cuenta cuenta);
        Task<Cuenta> GetByID(int usuarioID, int id);
        Task<IEnumerable<Cuenta>> GetCuentasAsync(int usuarioID);
    }
}
