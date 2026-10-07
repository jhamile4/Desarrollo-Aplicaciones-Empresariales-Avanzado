using Biblioteca.Web.Models;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Biblioteca.Web.Repositorios
{
    public class SocioRepositorio : ISocioRepositorio
    {
        private readonly string _connectionString;

        public SocioRepositorio(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ConexionBiblioteca")
                ?? throw new InvalidOperationException("La cadena de conexión 'ConexionBiblioteca' no se encuentra configurada en appsettings.json.");
        }

        private SqlConnection CreateConnection() => new SqlConnection(_connectionString);

        public async Task<IEnumerable<Socio>> ListarActivosAsync()
        {
            using var connection = CreateConnection();
            return await connection.QueryAsync<Socio>(
                "sp_ListarSociosActivos",
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<int> InsertarAsync(Socio socio)
        {
            using var connection = CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@Nombre", socio.Nombre);
            parameters.Add("@DNI", socio.DNI);
            parameters.Add("@Email", socio.Email);
            parameters.Add("@Telefono", socio.Telefono);

            return await connection.ExecuteScalarAsync<int>(
                "sp_InsertarSocio",
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<bool> ExisteDNIAsync(string dni)
        {
            using var connection = CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@DNI", dni);

            var count = await connection.ExecuteScalarAsync<int>(
                "sp_ExisteSocioDNI",
                parameters,
                commandType: CommandType.StoredProcedure
            );
            return count > 0;
        }
    }
}
