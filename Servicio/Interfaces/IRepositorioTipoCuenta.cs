using Microsoft.AspNetCore;
using ManejoPresupuestos.Models;

namespace ManejoPresupuestos.Servicio.Interfaces
{
    public interface IRepositorioTipoCuenta
    {
        Task ActualizarAsync(TipoCuenta tipoCuenta);
        Task Borrar(int id);
        Task CrearAsync(TipoCuenta tipoCuenta);

        Task<bool> ExisteAsync(string nombre, int usuarioId);

        Task<IEnumerable<TipoCuenta>> GetAsync(int usuarioId);
        Task<TipoCuenta> GetByIDAsync(int usuarioId, int id);
        Task Ordenar(IEnumerable<TipoCuenta> tiposCuentasOrdenados);
    }
}
