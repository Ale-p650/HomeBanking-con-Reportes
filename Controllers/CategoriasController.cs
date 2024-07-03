using ManejoPresupuestos.Models;
using ManejoPresupuestos.Servicio;
using ManejoPresupuestos.Servicio.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ManejoPresupuestos.Controllers
{
    public class CategoriasController : Controller
    {
        private readonly IRepositorioCategorias _repositorioCategorias;
        private readonly IServicioUsuarios _servicioUsuarios;

        public CategoriasController(IRepositorioCategorias repositorioCategorias,
            IServicioUsuarios servicioUsuarios)
        {
            _repositorioCategorias = repositorioCategorias;
            _servicioUsuarios = servicioUsuarios;
        }

        public async Task< IActionResult > Index(PaginacionViewModel paginacion)
        {
            var usuarioId = _servicioUsuarios.ObtenerUsuario();
            var categorias = await _repositorioCategorias.GetAsync(usuarioId,paginacion);
            int totalCategorias = await _repositorioCategorias.Contar(usuarioId);

            PaginacionRespuesta<Categoria> paginacionVM = new()
            {
                Elementos = categorias,
                BaseURL = Url.Action(),
                Pagina = paginacion.Pagina,
                CantidadTotalRecords = totalCategorias,
                RecordsPorPagina = paginacion.RecordsPorPagina
            };

            return View(paginacionVM);
        }

        [HttpGet]

        public async Task<IActionResult> Crear()
        {
            return View();
        }

        [HttpPost]

        public async Task<IActionResult> Crear(Categoria categoria)
        {
            if (!ModelState.IsValid)
            {
                return View(categoria);
            }

            var usuarioId = _servicioUsuarios.ObtenerUsuario();

            categoria.UsuarioId = usuarioId;

            await _repositorioCategorias.Crear(categoria);

            return RedirectToAction("Index");
        }

        [HttpGet]

        public async Task<IActionResult> Editar(int id)
        {
            var usuarioId = _servicioUsuarios.ObtenerUsuario();
            var categoria = await _repositorioCategorias.GetByIDAsync(usuarioId,id);

            if(categoria is null)
            {
                RedirectToAction("NoEncontrado", "Home");
            }

            return View(categoria);
        }

        [HttpPost]

        public async Task<IActionResult> Editar(Categoria categoriaEditar)
        {
            var usuarioId = _servicioUsuarios.ObtenerUsuario();
            var categoria = _repositorioCategorias.GetByIDAsync(usuarioId,categoriaEditar.Id);

            if (categoria is null)
            {
                RedirectToAction("NoEncontrado", "Home");
            }

            categoriaEditar.UsuarioId = usuarioId;

            if (!ModelState.IsValid)
            {
                return View(categoria);
            }

            await _repositorioCategorias.Actualizar(categoriaEditar);

            return RedirectToAction("Index");
        }

        [HttpGet]

        public async Task<IActionResult> Borrar(int id)
        {
            var usuarioId = _servicioUsuarios.ObtenerUsuario();
            var categoria = await _repositorioCategorias.GetByIDAsync(usuarioId, id);

            if (categoria is null)
            {
                RedirectToAction("NoEncontrado", "Home");
            }

            return View(categoria);
        }

        [HttpPost]

        public async Task<IActionResult> BorrarCategoria(int id)
        {
            var usuarioId = _servicioUsuarios.ObtenerUsuario();
            var categoria = _repositorioCategorias.GetByIDAsync(usuarioId,id);

            if (categoria is null)
            {
                RedirectToAction("NoEncontrado", "Home");
            }

            await _repositorioCategorias.Borrar(id);

            return RedirectToAction("Index");
        }
    }
}
