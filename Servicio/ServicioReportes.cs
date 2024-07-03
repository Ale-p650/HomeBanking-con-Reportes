using ManejoPresupuestos.Models;
using ManejoPresupuestos.Servicio.Interfaces;

namespace ManejoPresupuestos.Servicio
{
    public class ServicioReportes : IServicioReportes
    {
        private readonly IRepositorioTransacciones _repositorioTransacciones;
        private readonly HttpContext _httpContext;

        public ServicioReportes(IRepositorioTransacciones repositorioTransacciones,
            IHttpContextAccessor httpContextAccessor)
        {
            _repositorioTransacciones = repositorioTransacciones;
            _httpContext = httpContextAccessor.HttpContext;
        }

        public async Task<IEnumerable<ReporteSemanalQuery>> ObtenerReporteSemanal
            (int usuarioId,int mes, int año, dynamic ViewBag)
        {
            (DateTime fechaInicio, DateTime fechaFin) = GenerarFechas(mes, año);

            var parametro = new ParametroTransaccionDiariasDeUsuario()
            {
                UsuarioId = usuarioId,
                FechaInicio = fechaInicio,
                FechaFin = fechaFin
            };

            AsignarValoresViewBag(ViewBag, fechaInicio);

            var modelo = await _repositorioTransacciones.ObtenerPorSemana(parametro);

            return modelo;
        }

        public async Task<ReporteTransacciones> ObtenerReporteTransacciones
            (int usuarioId, int mes, int año, dynamic ViewBag)
        {
            (DateTime fechaInicio, DateTime fechaFin) = GenerarFechas(mes, año);

            var parametro = new ParametroTransaccionDiariasDeUsuario()
            {
                UsuarioId = usuarioId,
                FechaInicio = fechaInicio,
                FechaFin = fechaFin
            };

            var transacciones = await _repositorioTransacciones
                .ObtenerPorUsuarioId(parametro);

            var modelo = GenerarReporteTransacciones(fechaInicio, fechaFin, transacciones);

            AsignarValoresViewBag(ViewBag, fechaInicio);

            return modelo;
        }

        public async Task<ReporteTransacciones> ObtenerReporteTransaccionesPorCuenta
            (int usuarioId, int cuentaId, int mes, int año, dynamic ViewBag)
        {
            (DateTime fechaInicio, DateTime fechaFin) = GenerarFechas(mes, año);

            var obtenerTransaccionesPorCuenta = new TransaccionesPorCuenta_Model()
            {
                CuentaId = cuentaId,
                UsuarioId = usuarioId,
                FechaInicio = fechaInicio,
                FechaFin = fechaFin
            };

            var transacciones = await _repositorioTransacciones
                .ObtenerPorCuentaId(obtenerTransaccionesPorCuenta);

            var modelo = GenerarReporteTransacciones(fechaInicio, fechaFin, transacciones);
            AsignarValoresViewBag(ViewBag, fechaInicio);

            return modelo;
        }

        public ReporteTransacciones GenerarReporteTransacciones(DateTime fechaInicio, DateTime fechaFin, IEnumerable<Transaccion> transacciones)
        {
            var modelo = new ReporteTransacciones();

            var transaccionesPorFecha = transacciones
                .OrderByDescending(x => x.FechaTransaccion)
                .GroupBy(x => x.FechaTransaccion)
                .Select(grupo => new ReporteTransacciones.TransaccionesPorDia
                {
                    FechaTransaccion = grupo.Key,
                    Transacciones = grupo.AsEnumerable()
                });

            modelo.Transacciones = transaccionesPorFecha;
            modelo.FechaInicio = fechaInicio;
            modelo.FechaFin = fechaFin;

            return modelo;
        }

        private void AsignarValoresViewBag(dynamic ViewBag, DateTime fechaInicio)
        {
            ViewBag.mesAnterior = fechaInicio.AddMonths(-1).Month;
            ViewBag.añoAnterior = fechaInicio.AddMonths(-1).Year;
            ViewBag.mesPosterior = fechaInicio.AddMonths(1).Month;
            ViewBag.añoPosterior = fechaInicio.AddMonths(1).Year;
            ViewBag.urlRetorno = _httpContext.Request.Path + _httpContext.Request.QueryString;
        }


        private (DateTime fechaInicio, DateTime fechaFin) GenerarFechas(int mes, int año)
        {
            DateTime fechaInicio;
            DateTime fechaFin;

            if (mes < 1 || mes > 12 || año < 1900 || año > 2024)
            {
                fechaInicio =
                    new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            }
            else
            {
                fechaInicio =
                    new DateTime(año, mes, 1);
            }

            fechaFin = fechaInicio.AddMonths(1).AddDays(-1);

            return (fechaInicio, fechaFin);
        }
    }
}
