using ManejoPresupuestos.Models;

namespace ManejoPresupuestos.Servicio.Interfaces
{
    public interface IRepositorioUsuarios
    {
        Task<Usuario> BuscarUsuarioPorMail(string emailNormalizado);
        Task<int> CrearUsuario(Usuario usuario);
    }
}
