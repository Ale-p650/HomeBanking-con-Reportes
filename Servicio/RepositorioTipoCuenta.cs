using Dapper;
using ManejoPresupuestos.Models;
using ManejoPresupuestos.Servicio.Interfaces;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuestos.Servicio
{
    public class RepositorioTipoCuenta : IRepositorioTipoCuenta
    {
        private readonly string _connectionString;

        public RepositorioTipoCuenta(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("Conexion");
        }

        public async Task CrearAsync(TipoCuenta tipoCuenta)
        {
            using(var connection = new SqlConnection(_connectionString))
            {
                var id = await connection.QuerySingleAsync<int>("TiposCuentas_Insertar",
                    new {nombre=tipoCuenta.Nombre, usuarioId=tipoCuenta.UsuarioId },
                    commandType: System.Data.CommandType.StoredProcedure);

                tipoCuenta.Id = id;
            }
        }

        public async Task<bool> ExisteAsync(string nombre, int usuarioId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                int existeTipoCuenta = await connection.QueryFirstOrDefaultAsync<int>
                    (@"SELECT 1 FROM TiposCuentas WHERE Nombre = @nombre AND UsuarioId = @usuarioId",new {nombre,usuarioId});

                return existeTipoCuenta == 1;
            }
        }

        public async Task < IEnumerable < TipoCuenta > > GetAsync(int usuarioId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                return await connection.QueryAsync<TipoCuenta>
                    (@"SELECT Id,Nombre,Orden FROM TiposCuentas WHERE UsuarioId = @usuarioId ORDER BY Orden", new {usuarioId });

                
            }
        }

        public async Task ActualizarAsync(TipoCuenta tipoCuenta)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.ExecuteAsync(
                    @"UPDATE TiposCuentas SET Nombre = @Nombre WHERE Id= @Id", tipoCuenta);

                
            }
        }

        public async Task<TipoCuenta> GetByIDAsync(int usuarioId, int id)
        {
            using var connection = new SqlConnection(_connectionString);

            string query = "SELECT Id,Nombre,Orden FROM TiposCuentas WHERE UsuarioId" +
                " = @usuarioId AND Id= @id";


            return await connection.QueryFirstOrDefaultAsync<TipoCuenta>(query, new {usuarioId,id});


            
        }

        public async Task Borrar(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.ExecuteAsync("DELETE FROM TiposCuentas WHERE Id=@id", new { id });
            }
        }

        public async Task Ordenar (IEnumerable<TipoCuenta> tiposCuentasOrdenados)
        {

            var query = "UPDATE TiposCuentas SET Orden=@Orden WHERE Id=@Id;";

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.ExecuteAsync(query,tiposCuentasOrdenados);
            }
        }
    }
}
