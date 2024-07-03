using ManejoPresupuestos.Models;
using ManejoPresupuestos.Servicio.Interfaces;
using Microsoft.AspNetCore.Mvc;


namespace ManejoPresupuestos.Controllers
{
    public class TipoCuentaController : Controller
    {
        

        private readonly IRepositorioTipoCuenta _repositorio;
        private readonly IServicioUsuarios _servicio;

        
        public TipoCuentaController(IRepositorioTipoCuenta repositorio,IServicioUsuarios servicio)
        {
            _repositorio = repositorio;
            _servicio = servicio;
        }

        public async Task<IActionResult> Index()
        {
            int usuarioId = _servicio.ObtenerUsuario();

            var tiposCuentas = await _repositorio.GetAsync(usuarioId);

            return View(tiposCuentas);
        }

        [HttpGet]
        public IActionResult Crear()
        {
            
             return View();
        }

        [HttpPost]
        public async Task< IActionResult > Crear(TipoCuenta tipoCuenta)
        {
            if (!ModelState.IsValid)
            {
                return View(tipoCuenta);
            }

            tipoCuenta.Orden = 0;
            tipoCuenta.UsuarioId = _servicio.ObtenerUsuario();


            await _repositorio.CrearAsync(tipoCuenta);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> ExisteTipoCuenta(string nombre)
        {
            int usuarioId = _servicio.ObtenerUsuario();


            bool existe = await _repositorio.ExisteAsync(nombre, usuarioId);

            if (existe)
            {
                return Json($"Ya existe {nombre}");
            }

            return Json(true);

        }

        [HttpGet]
        public async Task<ActionResult> Editar(int id)
        {
            int usuarioId = _servicio.ObtenerUsuario();

            var tipoCuenta = await _repositorio.GetByIDAsync(usuarioId, id);

            if (tipoCuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            return View(tipoCuenta);

        }

        [HttpPost]
        public async Task<ActionResult> Editar(TipoCuenta tipocuenta)
        {
            int usuarioId = _servicio.ObtenerUsuario();

            var tipoCuenta = _repositorio.GetByIDAsync(usuarioId, tipocuenta.Id);

            if (tipoCuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            await _repositorio.ActualizarAsync(tipocuenta);

            return RedirectToAction("Index");

        }

        [HttpGet]
        public async Task<IActionResult> Borrar (int id)
        {
            int usuarioId = _servicio.ObtenerUsuario();

            var tipocuenta = await _repositorio.GetByIDAsync(usuarioId, id);

            if(tipocuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            
            return View(tipocuenta);
        }

        [HttpPost]
        public async Task<IActionResult> BorrarTipoCuenta(TipoCuenta tipoCuenta)
        {
            int usuarioId = _servicio.ObtenerUsuario();

            var tipocuenta = await _repositorio.GetByIDAsync(usuarioId, tipoCuenta.Id);

            if (tipocuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            await _repositorio.Borrar(tipoCuenta.Id);

            return RedirectToAction("Index");
        }


        [HttpPost]
        public async Task< IActionResult> Ordenar([FromBody] int[] ids)
        {
            var usuarioId = _servicio.ObtenerUsuario();

            var tiposCuentas = await _repositorio.GetAsync(usuarioId);

            var idTiposCuentas = tiposCuentas.Select(x => x.Id);

            var idTiposCuentasNoSonDelUsuario = ids.Except(idTiposCuentas).ToList();

            if (idTiposCuentasNoSonDelUsuario.Count > 0)
            {
                return Forbid();
            }

            var tiposCuentasOrdenados = ids.Select((valor, indice) =>
            new TipoCuenta() { Id = valor, Orden = indice + 1 });

            await _repositorio.Ordenar(tiposCuentasOrdenados);

            return Ok();
        }

    }
}
