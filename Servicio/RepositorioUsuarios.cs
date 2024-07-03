using Dapper;
using ManejoPresupuestos.Models;
using ManejoPresupuestos.Servicio.Interfaces;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuestos.Servicio
{
    public class RepositorioUsuarios : IRepositorioUsuarios
    {
        private readonly string _connectionString;

        public RepositorioUsuarios(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("Conexion");
        }

        public async Task<int> CrearUsuario(Usuario usuario)
        {
            usuario.EmailNormalizado = usuario.Email.ToUpper();

            using var connection = new SqlConnection(_connectionString);

            string query = @"INSERT INTO Usuarios
                (Email,EmailNormalizado,PasswordHash)
                VALUES (@Email,@EmailNormalizado,@PasswordHash)

                SELECT SCOPE_IDENTITY()";

            int usuarioId = await connection.QuerySingleAsync<int>(query, usuario);

            await connection.ExecuteAsync("CrearDatosUsuarioNuevo",
                new { usuarioId }, commandType: System.Data.CommandType.StoredProcedure);

            return usuarioId;
        }

        public async Task<Usuario> BuscarUsuarioPorMail(string emailNormalizado)
        {
            using var connection = new SqlConnection(_connectionString);

            string query = @"SELECT * FROM Usuarios 
                           WHERE EmailNormalizado = @EmailNormalizado";

            return await connection.
                QuerySingleOrDefaultAsync<Usuario>(query, new { emailNormalizado });
        }


    }
}
