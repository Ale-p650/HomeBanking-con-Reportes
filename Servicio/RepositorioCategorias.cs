using Dapper;
using ManejoPresupuestos.Models;
using ManejoPresupuestos.Servicio.Interfaces;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuestos.Servicio
{
    public class RepositorioCategorias : IRepositorioCategorias
    {
        private readonly string _connectionString;

        public RepositorioCategorias(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("Conexion");
        }

        public async Task Crear(Categoria categoria)
        {
            using var connection = new SqlConnection(_connectionString);

            string query = @"INSERT INTO Categorias 
                          (Nombre,TipoOperacionId,UsuarioId)
                            VALUES (@Nombre,@TipoOperacionId,@UsuarioId);

                            SELECT SCOPE_IDENTITY();       ";

            int id=await connection.QuerySingleAsync<int>(query, categoria);

            categoria.Id = id;
        }

        public async Task<IEnumerable<Categoria>> GetAsync(int idUsuario,
            PaginacionViewModel paginacion)
        {
            using var connection = new SqlConnection(_connectionString);

            string query = 
                @$"SELECT * FROM Categorias 
                WHERE UsuarioId = @idUsuario
                ORDER BY Nombre
                OFFSET {paginacion.RecordsASaltar} ROWS
                FETCH NEXT {paginacion.RecordsPorPagina} ROWS ONLY";

            return await connection.QueryAsync<Categoria>(query, new { idUsuario });
        }

        
        public async Task<int> Contar(int usuarioId)
        {
            
            using var connection = new SqlConnection(_connectionString);
            string query = "SELECT COUNT(*) FROM Categorias WHERE UsuarioId = @usuarioId ";
            return await connection.ExecuteScalarAsync<int>(query, new { usuarioId });
        }

        public async Task<IEnumerable<Categoria>> GetAsync(int idUsuario,TipoOperacion tipoOperacion)
        {
            using var connection = new SqlConnection(_connectionString);

            string query = @"SELECT * FROM Categorias
                    WHERE UsuarioId = @idUsuario AND TipoOperacionId = @tipoOperacion";

            return await connection.QueryAsync<Categoria>(query, new { idUsuario, tipoOperacion });
        }

        public async Task<Categoria> GetByIDAsync(int idUsuario,int id)
        {
            using var connection = new SqlConnection(_connectionString);

            string query = "SELECT * FROM Categorias WHERE Id=@Id AND UsuarioId = @IdUsuario";

            return await
            connection.QueryFirstOrDefaultAsync<Categoria>(query, new { id,idUsuario });
        }

        public async Task Actualizar(Categoria categoria)
        {
            using var connection = new SqlConnection(_connectionString);

            string query =
                @"UPDATE Categorias
                SET Nombre = @Nombre, TipoOperacionId = @TipoOperacionId 
                WHERE Id=@id";

            await connection.ExecuteAsync(query,categoria);
        }

        public async Task Borrar(int id)
        {
            using var connection = new SqlConnection(_connectionString);

            string query =
                @"DELETE Categorias WHERE Id=@id";

            await connection.ExecuteAsync(query, new {id});
        }
    }

}
