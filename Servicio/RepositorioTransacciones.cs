 using Dapper;
using ManejoPresupuestos.Models;
using ManejoPresupuestos.Servicio.Interfaces;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuestos.Servicio
{
    public class RepositorioTransacciones : IRepositorioTransacciones
    {
        private readonly string _connectionString;

        public RepositorioTransacciones(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("Conexion");
        }

        public async Task<Transaccion> GetByID(int id, int usuarioId)
        {
            using var connection = new SqlConnection(_connectionString);
            string query = @"SELECT Transacciones.*, Categorias.TipoOperacionId FROM Transacciones
                            INNER JOIN Categorias
                            ON Categorias.Id = Transacciones.CategoriaId
                            WHERE Transacciones.Id = @Id 
                            AND Transacciones.UsuarioId = @usuarioId";
            return await connection.QueryFirstOrDefaultAsync<Transaccion>(query,new {id,usuarioId});
        }

        public async Task Crear (Transaccion transaccion)
        {
            using var connection = new SqlConnection(_connectionString);

            var id = await connection.QuerySingleAsync<int>
                ("Transacciones_Insertar",
                new
                {
                    transaccion.UsuarioId,
                    transaccion.FechaTransaccion,
                    transaccion.Monto,
                    transaccion.CategoriaId,
                    transaccion.CuentaId,
                    transaccion.Nota
                },
                commandType: System.Data.CommandType.StoredProcedure);

            transaccion.Id = id;
        }

        public async Task Actualizar(Transaccion transaccion, int montoAnterior, int cuentaAnteriorId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync("Transacciones_Actualizar",
                new
                {
                    transaccion.Id,
                    transaccion.FechaTransaccion,
                    transaccion.Monto,
                    transaccion.CategoriaId,
                    transaccion.CuentaId,
                    transaccion.Nota,
                    montoAnterior,
                    cuentaAnteriorId
                }, commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task Borrar(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection
            .ExecuteAsync("Transacciones_Borrar", 
            new { id }, commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<Transaccion>> ObtenerPorCuentaId(TransaccionesPorCuenta_Model modelo)
        {
            using var connection = new SqlConnection(_connectionString);

            string query = @"SELECT Transacciones.Id,Transacciones.Monto,
            Transacciones.FechaTransaccion, Categorias.Nombre as Categoria,
            Cuentas.Nombre as Cuenta, Categorias.TipoOperacionId
            FROM Transacciones
            INNER JOIN Categorias
            ON Categorias.Id = Transacciones.CategoriaId
            INNER JOIN Cuentas
            ON Cuentas.Id = Transacciones.CuentaId
            WHERE Transacciones.CuentaId = @CuentaId 
            AND Transacciones.UsuarioId = @UsuarioId
            AND FechaTransaccion BETWEEN @FechaInicio AND @FechaFin";

            var transacciones = await connection.QueryAsync
                <Transaccion>(query, modelo);

            return transacciones;
        }

        public async Task<IEnumerable<Transaccion>> ObtenerPorUsuarioId
            (ParametroTransaccionDiariasDeUsuario parametro)
        {
            using var connection = new SqlConnection(_connectionString);

            string query = @"SELECT Transacciones.Id,Transacciones.Monto,
            Transacciones.FechaTransaccion, Categorias.Nombre as Categoria,
            Cuentas.Nombre as Cuenta, Categorias.TipoOperacionId
            FROM Transacciones
            INNER JOIN Categorias
            ON Categorias.Id = Transacciones.CategoriaId
            INNER JOIN Cuentas
            ON Cuentas.Id = Transacciones.CuentaId
            WHERE
            Transacciones.UsuarioId = @UsuarioId
            AND FechaTransaccion BETWEEN @FechaInicio AND @FechaFin
            ORDER BY FechaTransaccion DESC";

            var transacciones = await connection.QueryAsync
                <Transaccion>(query, parametro);

            return transacciones;
        }

        public async Task<IEnumerable<ReporteSemanalQuery>> ObtenerPorSemana
            (ParametroTransaccionDiariasDeUsuario modelo)
        {
            using var connection = new SqlConnection(_connectionString);

            string query = @"SELECT DATEDIFF(d,@fechaInicio,FechaTransaccion) / 7 + 1 as Semana,
                        SUM(Monto) as Monto, cat.TipoOperacionId
                        FROM Transacciones
                        INNER JOIN Categorias cat
                        ON cat.Id = Transacciones.CategoriaId
                        WHERE Transacciones.UsuarioId = @usuarioId AND
                        FechaTransaccion BETWEEN @fechaInicio and @fechaFin
                        GROUP BY DATEDIFF(d,@fechaInicio,FechaTransaccion) / 7,cat.TipoOperacionId";

            return await connection.QueryAsync<ReporteSemanalQuery>(query, modelo);
        }

        public async Task<IEnumerable<ReporteMensualQuery>> ObtenerPorMes
            (int usuarioId,int año)
        {
            using var connection = new SqlConnection(_connectionString);

            string query = @"SELECT MONTH(FechaTransaccion) as Mes, SUM(Monto), cat.TipoOperacionId
            FROM Transacciones INNER JOIN Categorias cat ON cat.Id = Transacciones.CategoriaId
            WHERE Transacciones.UsuarioId = @usuarioId AND YEAR(FechaTransaccion) = @Año
            GROUP BY MONTH(FechaTransaccion), cat.TipoOperacionId";

            return await connection.QueryAsync<ReporteMensualQuery>(query, new { usuarioId, año });
        }
    }
}
