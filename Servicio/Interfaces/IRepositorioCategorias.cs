using ManejoPresupuestos.Models;

namespace ManejoPresupuestos.Servicio.Interfaces
{
    public interface IRepositorioCategorias
    {
        Task Actualizar(Categoria categoria);
        Task Borrar(int id);

        /// <summary>
        /// Esta es la descripción del método. Explica qué hace el método.
        /// </summary>
        /// 
        Task<int> Contar(int usuarioId);
        Task Crear(Categoria categoria);
        Task<IEnumerable<Categoria>> GetAsync(int idUsuario, TipoOperacion tipoOperacion);
        Task<IEnumerable<Categoria>> GetAsync(int idUsuario, PaginacionViewModel paginacion);
        Task<Categoria> GetByIDAsync(int idUsuario, int id);
    }
}
