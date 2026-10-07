using Biblioteca.Web.Models;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Biblioteca.Web.Repositorios
{
    public class PrestamoRepositorio : IPrestamoRepositorio
    {
        private readonly string _connectionString;

        public PrestamoRepositorio(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ConexionBiblioteca")
                ?? throw new InvalidOperationException("La cadena de conexión 'ConexionBiblioteca' no se encuentra configurada en appsettings.json.");
        }

        private SqlConnection CreateConnection() => new SqlConnection(_connectionString);

        public async Task<IEnumerable<PrestamoReporte>> ReportePorFechasAsync(DateTime? desde, DateTime? hasta)
        {
            using var connection = CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@FechaDesde", desde);
            parameters.Add("@FechaHasta", hasta);

            return await connection.QueryAsync<PrestamoReporte>(
                "sp_ReportePrestamosPorFechas",
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }
    }
}
