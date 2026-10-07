using Biblioteca.Web.Models;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Biblioteca.Web.Repositorios
{
    public class LibroRepositorio : ILibroRepositorio
    {
        private readonly string _connectionString;

        public LibroRepositorio(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ConexionBiblioteca")
                ?? throw new InvalidOperationException("La cadena de conexión 'ConexionBiblioteca' no se encuentra configurada en appsettings.json.");
        }

        private SqlConnection CreateConnection() => new SqlConnection(_connectionString);

        public async Task<IEnumerable<Libro>> ListarActivosAsync()
        {
            using var connection = CreateConnection();
            return await connection.QueryAsync<Libro>(
                "sp_ListarLibrosActivos",
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<IEnumerable<Libro>> BuscarPorTituloAsync(string titulo)
        {
            using var connection = CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@Titulo", titulo ?? string.Empty);

            return await connection.QueryAsync<Libro>(
                "sp_BuscarLibrosPorTitulo",
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<Libro?> ObtenerPorIdAsync(int id)
        {
            using var connection = CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@LibroId", id);

            return await connection.QueryFirstOrDefaultAsync<Libro>(
                "sp_ObtenerLibroPorId",
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<int> InsertarAsync(Libro libro)
        {
            using var connection = CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@Titulo", libro.Titulo);
            parameters.Add("@ISBN", libro.ISBN);
            parameters.Add("@AutorId", libro.AutorId);
            parameters.Add("@Ejemplares", libro.Ejemplares);

            return await connection.ExecuteScalarAsync<int>(
                "sp_InsertarLibro",
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<bool> ActualizarAsync(Libro libro)
        {
            using var connection = CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@LibroId", libro.LibroId);
            parameters.Add("@Titulo", libro.Titulo);
            parameters.Add("@ISBN", libro.ISBN);
            parameters.Add("@AutorId", libro.AutorId);
            parameters.Add("@Ejemplares", libro.Ejemplares);

            var rows = await connection.ExecuteAsync(
                "sp_ActualizarLibro",
                parameters,
                commandType: CommandType.StoredProcedure
            );
            return rows > 0;
        }

        public async Task<bool> EliminarLogicoAsync(int id)
        {
            using var connection = CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@LibroId", id);

            var rows = await connection.ExecuteAsync(
                "sp_EliminarLibroLogico",
                parameters,
                commandType: CommandType.StoredProcedure
            );
            return rows > 0;
        }

        public async Task<IEnumerable<Autor>> ListarAutoresActivosAsync()
        {
            using var connection = CreateConnection();
            return await connection.QueryAsync<Autor>(
                "sp_ListarAutoresActivos",
                commandType: CommandType.StoredProcedure
            );
        }
    }
}
