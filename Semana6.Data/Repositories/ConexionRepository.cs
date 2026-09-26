using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Data.SqlClient;

namespace Semana6.Data.Repositories
{
    public class ConexionRepository
    {
        public async Task<bool> ProbarConexionAsync()
        {
            using SqlConnection conexion = new SqlConnection(Conexion.Cadena);

            await conexion.OpenAsync();

            return conexion.State == System.Data.ConnectionState.Open;
        }
    }
}