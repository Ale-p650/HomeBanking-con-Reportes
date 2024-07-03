using Dapper;
using ManejoPresupuestos.Models;
using ManejoPresupuestos.Servicio.Interfaces;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuestos.Servicio
{
    public class RepositorioCuentas : IRepositorioCuentas
    {
        private readonly string _connectionString;

        public RepositorioCuentas(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("Conexion");
        }

        public async Task Crear(Cuenta cuenta)
        {
            using var connection = new SqlConnection(_connectionString);

            var query = @"INSERT INTO Cuentas (Nombre,TipoCuentaID,Balance,Descripcion)
                VALUES (@Nombre,@TipoCuentaId,@Balance,@Descripcion);
                 SELECT SCOPE_IDENTITY();                           ";

            var id = await connection.ExecuteScalarAsync<int>(query,cuenta);

            cuenta.Id = id;
        }

        public async Task<IEnumerable<Cuenta>> GetCuentasAsync(int usuarioID)
        {
            using var connection = new SqlConnection(_connectionString);

            var query = @"SELECT Cuentas.Id, Cuentas.Nombre, 
                Balance, tc.Nombre AS TipoCuenta FROM Cuentas
                INNER JOIN TiposCuentas tc
                ON tc.Id = Cuentas.TipoCuentaId
                WHERE tc.UsuarioId = @UsuarioId
                ORDER BY tc.Orden";

            return await connection.QueryAsync<Cuenta>(query, new { usuarioID });
        }

        public async Task<Cuenta> GetByID(int usuarioID, int id)
        {
            using var connection = new SqlConnection(_connectionString);

            var query = @"
                SELECT
                Cuentas.Id, Cuentas.Nombre, 
                Balance,Descripcion, TiposCuentas.Id
                FROM Cuentas
                INNER JOIN TiposCuentas
                ON TiposCuentas.Id = Cuentas.TipoCuentaId
                WHERE TiposCuentas.UsuarioId = @UsuarioId AND Cuentas.Id = @Id
                ";

            return await connection.QueryFirstOrDefaultAsync<Cuenta>(query, new { usuarioID, id });
        }

        public async Task Actualizar(CuentaCrear_ViewModel cuenta)
        {
            using var connection = new SqlConnection(_connectionString);

            string query = @"UPDATE Cuentas
                            SET Nombre = @Nombre, Balance = @Balance, 
                            Descripcion = @Descripcion, TipoCuentaId = @TipoCuentaId
                            WHERE Id = @Id;";

            await connection.ExecuteAsync(query, cuenta);
        }

        public async Task Borrar (int id)
        {
            using var connection = new SqlConnection(_connectionString);

            string query = "DELETE Cuentas WHERE Id = @id";

            await connection.ExecuteAsync(query, new { id });
        }
    }
}
