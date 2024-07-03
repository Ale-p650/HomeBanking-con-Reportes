using ManejoPresupuestos.Servicio.Interfaces;
using System.Security.Claims;

namespace ManejoPresupuestos.Servicio
{
    public class ServicioUsuarios : IServicioUsuarios
    {
        private readonly HttpContext _httpContext;

        public ServicioUsuarios(IHttpContextAccessor httpContextAccessor)
        {
            _httpContext = httpContextAccessor.HttpContext;
        }

        public int ObtenerUsuario()
        {
            if (_httpContext.User.Identity.IsAuthenticated)
            {
                int idClaim = int.Parse
                    (_httpContext.User.Claims.Where(x => x.Type == ClaimTypes.NameIdentifier)
                    .FirstOrDefault().Value);

                return idClaim;
            }
            else
            {
                throw new ApplicationException("El usuario no está autenticado");
            }

            
        }
    }
}
