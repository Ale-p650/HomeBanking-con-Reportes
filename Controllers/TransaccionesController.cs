using AutoMapper;
using ClosedXML.Excel;
using ManejoPresupuestos.Models;
using ManejoPresupuestos.Servicio;
using ManejoPresupuestos.Servicio.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace ManejoPresupuestos.Controllers
{
    
    public class TransaccionesController :Controller
    {
        private readonly IServicioUsuarios _servicioUsuarios;
        private readonly IRepositorioCuentas _repositorioCuentas;
        private readonly IRepositorioCategorias _repositorioCategorias;
        private readonly IRepositorioTransacciones _repositorioTransacciones;
        private readonly IServicioReportes _servicioReportes;
        private readonly IMapper _mapper;

        public TransaccionesController(IServicioUsuarios servicioUsuarios,
            IRepositorioCuentas repositorioCuentas,
            IRepositorioCategorias repositorioCategorias,
            IRepositorioTransacciones repositorioTransacciones,
            IServicioReportes servicioReportes,
            IMapper mapper)
        {
            _servicioUsuarios = servicioUsuarios;
            _repositorioCuentas = repositorioCuentas;
            _repositorioCategorias = repositorioCategorias;
            _repositorioTransacciones = repositorioTransacciones;
            _servicioReportes = servicioReportes;
            _mapper = mapper;
        }

        [Authorize]
        public async Task<IActionResult> Index(int mes, int año)
        {
            int idUsuario = _servicioUsuarios.ObtenerUsuario();

            var modelo = await _servicioReportes.ObtenerReporteTransacciones(idUsuario, mes, año, ViewBag);

            return View(modelo);

            
        }

        public async Task<IActionResult> Semanal(int mes, int año)
        {

            int usuarioId = _servicioUsuarios.ObtenerUsuario();

            IEnumerable<ReporteSemanalQuery> transacciones =
                await _servicioReportes.ObtenerReporteSemanal(usuarioId, mes, año, ViewBag);

            var agrupado = transacciones.GroupBy(x => x.Semana)
                .Select(x => new ReporteSemanalQuery()
                {
                    Semana = x.Key,
                    Ingresos = x.Where(x => x.TipoOperacionId == TipoOperacion.Ingreso)
                    .Select(x => x.Monto).FirstOrDefault(),
                    Gastos = x.Where(x => x.TipoOperacionId == TipoOperacion.Gasto)
                    .Select(x => x.Monto).FirstOrDefault()
                }).ToList();

            if(año==0 || mes ==0)
            {
                var hoy = DateTime.Today;
                año = hoy.Year;
                mes = hoy.Month;
            }

            var fechaReferencia = new DateTime(año, mes, 1);
            var diasDelMes = Enumerable.Range(1, fechaReferencia.AddMonths(1).AddDays(-1).Day);

            var diasSegmentados = diasDelMes.Chunk(7).ToList();

            for (int i = 0; i < diasSegmentados.Count; i++)
            {
                var semana = i + 1;

                var fechaInicio = new DateTime(año, mes, diasSegmentados[i].First());
                var fechaFin = new DateTime(año, mes, diasSegmentados[i].Last());
                var grupoSemana = agrupado.FirstOrDefault(x => x.Semana == semana);

                if(grupoSemana is null)
                {
                    agrupado.Add(new ReporteSemanalQuery()
                    {
                        Semana = semana,
                        FechaInicio = fechaInicio,
                        FechaFin = fechaFin

                    });
                }
                else
                {
                    grupoSemana.FechaInicio = fechaInicio;
                    grupoSemana.FechaFin = fechaFin;

                }
            }

            agrupado = agrupado.OrderByDescending(x => x.Semana).ToList();

            var modelo = new ReporteSemanal_ViewModel()
            {
                TransaccionesPorSemana = agrupado,
                FechaReferencia = fechaReferencia
            };

            return View(modelo);
        }

        public async Task<IActionResult> Mensual(int año)
        {
            int usuarioId = _servicioUsuarios.ObtenerUsuario();

            if(año == 0)
            {
                año = DateTime.Now.Year;
            }

            var transaccionesPorMes = await _repositorioTransacciones.ObtenerPorMes(usuarioId, año);

            var transaccionesAgrupadas = transaccionesPorMes.GroupBy(x => x.Mes)
                .Select(x => new ReporteMensualQuery()
                {
                    Mes = x.Key,
                    Ingreso = x.Where(x => x.TipoOperacionId == TipoOperacion.Ingreso)
                    .Select(x => x.Monto).FirstOrDefault(),
                    Gasto = x.Where(x => x.TipoOperacionId == TipoOperacion.Gasto)
                    .Select(x => x.Monto).FirstOrDefault()

                }).ToList();

            for (int mes = 1; mes <= 12; mes++)
            {
                var transaccion = transaccionesAgrupadas.FirstOrDefault(x => x.Mes == mes);
                var fechaReferencia = new DateTime(año, mes, 1);

                if(transaccion is null)
                {
                    transaccionesAgrupadas.Add(new ReporteMensualQuery()
                    {
                        Mes = mes,
                        FechaReferencia = fechaReferencia
                    });
                }
                else
                {
                    transaccion.FechaReferencia = fechaReferencia;
                }
            }

            transaccionesAgrupadas = transaccionesAgrupadas.OrderByDescending(x => x.Mes).ToList();

            var modelo = new ReporteMensual_ViewModel()
            {
                Año = año,
                TransaccionesPorMes = transaccionesAgrupadas
            };

            return View(modelo);
        }

        public async Task<IActionResult> ExcelReporte()
        {
            return View();
        }


        [HttpGet]
        public async Task<FileResult> ExcelReportePorMes(int mes, int año)
        {
            DateTime fechaInicio = new DateTime(año, mes, 1);
            DateTime fechaFin = fechaInicio.AddMonths(1).AddDays(-1);
            int usuarioId = _servicioUsuarios.ObtenerUsuario();

            var transacciones = await _repositorioTransacciones
                .ObtenerPorUsuarioId(new ParametroTransaccionDiariasDeUsuario()
                {
                    FechaInicio = fechaInicio,
                    FechaFin = fechaFin,
                    UsuarioId = usuarioId
                });

            string nombreDeArchivo = $"ManejoPresupuestos - {fechaInicio.ToString("MMM yyyy")}.xlsx";

            return GenerarExcel(nombreDeArchivo, transacciones);
        }

        [HttpGet]
        public async Task<FileResult> ExcelReportePorAño(int año)
        {
            DateTime fechaInicio = new DateTime(año, 1, 1);
            DateTime fechaFin = fechaInicio.AddYears(1).AddDays(-1);

            int usuarioId = _servicioUsuarios.ObtenerUsuario();

            var transacciones = await _repositorioTransacciones
                .ObtenerPorUsuarioId(new ParametroTransaccionDiariasDeUsuario()
                {
                    FechaInicio = fechaInicio,
                    FechaFin = fechaFin,
                    UsuarioId = usuarioId
                });

            string nombreArchivo = $"ManejoPresupuestos - Año {año}.xlsx";

            return GenerarExcel(nombreArchivo, transacciones);
        }

        [HttpGet]
        public async Task<FileResult> ExportarExcelTodo()
        {
            DateTime fechaInicio = new DateTime(1900, 1, 1);
            DateTime fechaFin = DateTime.Today.AddDays(1);

            int usuarioId = _servicioUsuarios.ObtenerUsuario();

            var transacciones = await _repositorioTransacciones
                .ObtenerPorUsuarioId(new ParametroTransaccionDiariasDeUsuario()
                {
                    FechaInicio = fechaInicio,
                    FechaFin = fechaFin,
                    UsuarioId = usuarioId
                });

            string nombreArchivo = $"ManejoPresupuestos - Hasta {fechaFin.ToString("dd MMMM yyyy")}.xlsx";

            return GenerarExcel(nombreArchivo, transacciones);
        }



        private FileResult GenerarExcel(string nombreDeArchivo,
            IEnumerable<Transaccion> transacciones)
        {
            DataTable dataTable = new DataTable("Transacciones");


            dataTable.Columns.AddRange(new DataColumn[]
            {
                new DataColumn("Fecha"),
                new DataColumn("Cuenta"),
                new DataColumn("Categoria"),
                new DataColumn("Nota"),
                new DataColumn("Monto"),
                new DataColumn("Tipo de Operacion")
            });


            foreach(var transaccion in transacciones)
            {
                dataTable.Rows.Add(
                    transaccion.FechaTransaccion,
                    transaccion.Cuenta,
                    transaccion.Categoria,
                    transaccion.Nota,
                    transaccion.Monto,
                    transaccion.tipoOperacionId
                    );
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dataTable);

                using(MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        nombreDeArchivo);
                }
            }
        }

        public async Task<IActionResult> Calendario()
        {
            return View();
        }



        public async Task<IActionResult> Crear()
        {
            int usuarioId = _servicioUsuarios.ObtenerUsuario();
            var modelo = new TransaccionCrear_ViewModel();
            modelo.Cuentas = await ObtenerCuentas(usuarioId);

            return View(modelo);

        }

        [HttpPost]

        public async Task<IActionResult> Crear(TransaccionCrear_ViewModel modelo)
        {
            int usuarioId = _servicioUsuarios.ObtenerUsuario();

            modelo.Cuentas = await ObtenerCuentas(usuarioId);
            modelo.Categorias = await ObtenerCategorias(usuarioId, modelo.tipoOperacionId);

            if (!ModelState.IsValid)
            {
                modelo.Cuentas = await ObtenerCuentas(usuarioId);
                modelo.Categorias = await ObtenerCategorias(usuarioId,modelo.tipoOperacionId);
                return View(modelo);
            }

            var cuenta = await _repositorioCuentas.GetByID(usuarioId, modelo.CuentaId);

            if(cuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            var categorias = await _repositorioCategorias.GetByIDAsync(usuarioId, modelo.CategoriaId );

            if(categorias is null)
            {
                return RedirectToAction("NoEncontrado", "Home");

            }

            modelo.UsuarioId = usuarioId;

            if(modelo.tipoOperacionId == TipoOperacion.Gasto)
            {
                modelo.Monto *= -1;
            }

            await _repositorioTransacciones.Crear(modelo);

            return RedirectToAction("index");

        }

        private async Task<IEnumerable<SelectListItem>> ObtenerCuentas(int usuarioId)
        {
            var cuentas = await _repositorioCuentas.GetCuentasAsync(usuarioId);

            return cuentas.Select(x => new SelectListItem(x.Nombre, x.Id.ToString()));
        }

        private async Task<IEnumerable<SelectListItem>> ObtenerCategorias(int usuarioId,TipoOperacion tipoOperacion)
        {
            var categorias = await _repositorioCategorias.GetAsync(usuarioId, tipoOperacion);

            return categorias.Select(x => new SelectListItem(x.Nombre, x.Id.ToString()));
        }

        [HttpPost]
        public async Task<IActionResult> ObtenerCategorias([FromBody] TipoOperacion tipoOperacion)
        {
            int usuarioId = _servicioUsuarios.ObtenerUsuario();
            var categorias = await ObtenerCategorias(usuarioId, tipoOperacion);
            return Ok(categorias);
        }

        public async Task<IActionResult> Editar(int id, string urlRetorno=null)
        {
            var usuarioId = _servicioUsuarios.ObtenerUsuario();
            var transaccion =await _repositorioTransacciones.GetByID(id, usuarioId);

            if(transaccion is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            var modelo = _mapper.Map<TransaccionActualizar_ViewModel>(transaccion);

            if(modelo.tipoOperacionId== TipoOperacion.Gasto)
            {
                modelo.MontoAnterior = modelo.Monto * -1;
            }

            modelo.CuentaAnteriorId = transaccion.CuentaId;
            modelo.Categorias = await ObtenerCategorias(usuarioId, transaccion.tipoOperacionId);
            modelo.Cuentas = await ObtenerCuentas(usuarioId);
            modelo.urlRetorno = urlRetorno;

            return View(modelo);
        }

        [HttpPost]

        public async Task<IActionResult> Editar(TransaccionActualizar_ViewModel modelo)
        {
            var usuarioId = _servicioUsuarios.ObtenerUsuario();

            var cuenta = await _repositorioCuentas.GetByID(usuarioId, modelo.CuentaId);

            if(cuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            var categoria = await _repositorioCategorias.GetByIDAsync(usuarioId, modelo.CategoriaId);

            if (categoria is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            var transaccion = _mapper.Map<Transaccion>(modelo);

            modelo.MontoAnterior = modelo.Monto;

            if(modelo.tipoOperacionId == TipoOperacion.Gasto)
            {
                transaccion.Monto *= -1;
            }

            await _repositorioTransacciones.
                Actualizar(transaccion, modelo.MontoAnterior, modelo.CuentaAnteriorId);

            if (string.IsNullOrEmpty(modelo.urlRetorno))
            {
                return RedirectToAction("Index");
            }
            else
            {
                return LocalRedirect(modelo.urlRetorno);
            }

            
        }

        [HttpPost]
        public async Task<IActionResult> Borrar(int id, string urlRetorno = null)
        {
            int usuarioId = _servicioUsuarios.ObtenerUsuario();

            var transaccion = await _repositorioTransacciones.GetByID(id, usuarioId);

            if(transaccion is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            await _repositorioTransacciones.Borrar(id);

            if (string.IsNullOrEmpty(urlRetorno))
            {
                return RedirectToAction("Index");
            }
            else
            {
                return LocalRedirect(urlRetorno);
            }
        }
    }
}
