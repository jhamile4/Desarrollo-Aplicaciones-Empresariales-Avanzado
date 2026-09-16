using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Data.SqlClient;

namespace Semana3.Data
{
    public static class Conexion
    {
        // Ajusta el nombre del servidor si el tuyo es diferente
        private static readonly string cadena =
            "Server=localhost\\SQLEXPRESS;Database=ReservasDB;Trusted_Connection=True;TrustServerCertificate=True;";

        public static SqlConnection ObtenerConexion()
        {
            return new SqlConnection(cadena);
        }
    }
}
