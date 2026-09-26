using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Data.SqlClient;
using Semana6.Data.Models;
using System.Data;

namespace Semana6.Data.Repositories
{
    public class PedidoRepository
    {
        public async Task<List<Pedido>> ListarAsync()
        {
            List<Pedido> lista = new();

            using SqlConnection conexion = new(Conexion.Cadena);
            using SqlCommand comando = new("sp_Pedidos_Listar", conexion);

            comando.CommandType = CommandType.StoredProcedure;

            await conexion.OpenAsync();

            using SqlDataReader lector = await comando.ExecuteReaderAsync();

            while (await lector.ReadAsync())
            {
                lista.Add(new Pedido
                {
                    PedidoID = Convert.ToInt32(lector["PedidoID"]),
                    ClienteID = lector["ClienteID"] == DBNull.Value ? null : Convert.ToInt32(lector["ClienteID"]),
                    EmpleadoID = lector["EmpleadoID"] == DBNull.Value ? null : Convert.ToInt32(lector["EmpleadoID"]),
                    FechaPedido = lector["FechaPedido"] == DBNull.Value ? null : Convert.ToDateTime(lector["FechaPedido"]),
                    FechaRequerida = lector["FechaRequerida"] == DBNull.Value ? null : Convert.ToDateTime(lector["FechaRequerida"]),
                    FechaEnvio = lector["FechaEnvio"] == DBNull.Value ? null : Convert.ToDateTime(lector["FechaEnvio"]),
                    TransportistaID = lector["TransportistaID"] == DBNull.Value ? null : Convert.ToInt32(lector["TransportistaID"]),
                    Destinatario = lector["Destinatario"]?.ToString(),
                    CiudadDestino = lector["CiudadDestino"]?.ToString(),
                    PaisDestino = lector["PaisDestino"]?.ToString(),
                    Activo = lector["Activo"] != DBNull.Value &&
                             Convert.ToBoolean(lector["Activo"])
                });
            }

            return lista;
        }

        public async Task DesactivarAsync(int id)
        {
            using SqlConnection conexion = new(Conexion.Cadena);
            using SqlCommand comando = new("sp_Pedidos_Desactivar", conexion);

            comando.CommandType = CommandType.StoredProcedure;
            comando.Parameters.AddWithValue("@PedidoID", id);

            await conexion.OpenAsync();
            await comando.ExecuteNonQueryAsync();
        }
    }
}
