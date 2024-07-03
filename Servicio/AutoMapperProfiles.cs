using AutoMapper;
using ManejoPresupuestos.Models;

namespace ManejoPresupuestos.Servicio
{
    public class AutoMapperProfiles :Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Cuenta, CuentaCrear_ViewModel>().ReverseMap();
            CreateMap<Transaccion, TransaccionActualizar_ViewModel>().ReverseMap();
        }
    }
}
