using AutoMapper;
using ManejoPresupuestos.Models;
using ManejoPresupuestos.Servicio;
using ManejoPresupuestos.Servicio.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ManejoPresupuestos.Controllers
{
    public class CuentasController : Controller
    {
        private readonly IServicioUsuarios _servicioUsuarios;
        private readonly IRepositorioTipoCuenta _repositorioTipoCuenta;
        private readonly IRepositorioCuentas _repositorioCuentas;
        private readonly IRepositorioTransacciones _repositorioTransacciones;
        private readonly IServicioReportes _servicioReportes;
        private readonly IMapper _mapper;

        public CuentasController(IServicioUsuarios servicioUsuarios,
            IRepositorioTipoCuenta repositorioTipoCuenta,
            IRepositorioCuentas repositorioCuentas,
            IRepositorioTransacciones repositorioTransacciones,
            IServicioReportes servicioReportes,
            IMapper mapper)
        {
            _servicioUsuarios = servicioUsuarios;
            _repositorioTipoCuenta = repositorioTipoCuenta;
            _repositorioCuentas = repositorioCuentas;
            _repositorioTransacciones = repositorioTransacciones;
            _servicioReportes = servicioReportes;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var usuarioId = _servicioUsuarios.ObtenerUsuario();
            var cuentasConTipoCuenta = await _repositorioCuentas.GetCuentasAsync(usuarioId);

            var modelo = cuentasConTipoCuenta
                .GroupBy(x => x.TipoCuenta)
                .Select(grupo => new CuentaIndex_ViewModel
                {
                    TipoCuenta = grupo.Key,
                    Cuentas = grupo.AsEnumerable()
                }).ToList();

            return View(modelo);
        }

        [HttpGet]
        public async Task<IActionResult> Crear()
        {
            int idUsuario = _servicioUsuarios.ObtenerUsuario();
            var tiposCuentas = await  _repositorioTipoCuenta.GetAsync(idUsuario);
            var modelo = new CuentaCrear_ViewModel();
            modelo.TiposCuentas = await ObtenerItems(idUsuario);
            return View(modelo);
        }

        [HttpPost]
        public async Task<IActionResult> Crear(CuentaCrear_ViewModel cuenta)
        {
            var usuarioId = _servicioUsuarios.ObtenerUsuario();

            var tipoCuenta = await _repositorioTipoCuenta.GetByIDAsync(usuarioId, cuenta.TipoCuentaId);

            if (tipoCuenta is null)
            {
                return View(cuenta);
            }

            //if (!ModelState.IsValid)
            //{
            //    cuenta.TiposCuentas = await ObtenerItems(usuarioId);
            //    return View(cuenta);
            //}
            await _repositorioCuentas.Crear(cuenta);

            return (RedirectToAction("Index"));

        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            int idUsuario = _servicioUsuarios.ObtenerUsuario();

            var cuenta = await _repositorioCuentas.GetByID(idUsuario, id);
            
            if(cuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            var modelo = _mapper.Map<CuentaCrear_ViewModel>(cuenta);
            

            modelo.TiposCuentas = await ObtenerItems(idUsuario);

            return View(modelo);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(CuentaCrear_ViewModel cuentaEditar)
        {
            int idUsuario = _servicioUsuarios.ObtenerUsuario();
            var cuenta = await _repositorioCuentas.GetByID(idUsuario, cuentaEditar.Id);

            if(cuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            var tipoCuenta = await _repositorioTipoCuenta.GetByIDAsync(idUsuario, cuenta.Id);
            
            if(tipoCuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            await _repositorioCuentas.Actualizar(cuentaEditar);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Borrar(int id)
        {
            int idUsuario = _servicioUsuarios.ObtenerUsuario();

            var cuenta = await _repositorioCuentas.GetByID(idUsuario, id);

            if (cuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            return View(cuenta);
        }

        [HttpPost]
        public async Task<IActionResult> BorrarCuenta(int id)
        {
            int idUsuario = _servicioUsuarios.ObtenerUsuario();

            var cuenta = await _repositorioCuentas.GetByID(idUsuario, id);

            if (cuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            await _repositorioCuentas.Borrar(id);

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Detalle(int id,int mes, int año)
        {
            var usuarioId = _servicioUsuarios.ObtenerUsuario();
            var cuenta = await _repositorioCuentas.GetByID(usuarioId, id);

            if (cuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            ViewBag.Cuenta = cuenta.Nombre;


            var modelo = await _servicioReportes.ObtenerReporteTransaccionesPorCuenta
                (usuarioId, id, mes, año, ViewBag);



            return View(modelo);
        }


        private async Task< IEnumerable<SelectListItem>> ObtenerItems (int usuarioID)
        {
            var tiposCuentas = await _repositorioTipoCuenta.GetAsync(usuarioID);

            return tiposCuentas.Select(x => new SelectListItem(x.Nombre, x.Id.ToString()));
        }

    }
}
