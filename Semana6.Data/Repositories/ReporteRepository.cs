using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Data.SqlClient;
using Semana6.Data.Models;
using System.Data;

namespace Semana6.Data.Repositories
{
    public class ReporteRepository
    {
        public async Task<List<ReportePedido>> ObtenerAsync(
            DateTime fechaInicio,
            DateTime fechaFin)
        {
            List<ReportePedido> lista = new();

            using SqlConnection conexion = new(Conexion.Cadena);
            using SqlCommand comando =
                new("sp_Reporte_Pedidos", conexion);

            comando.CommandType = CommandType.StoredProcedure;

            comando.Parameters.AddWithValue(
                "@FechaInicio",
                fechaInicio);

            comando.Parameters.AddWithValue(
                "@FechaFin",
                fechaFin);

            await conexion.OpenAsync();

            using SqlDataReader lector =
                await comando.ExecuteReaderAsync();

            while (await lector.ReadAsync())
            {
                lista.Add(new ReportePedido
                {
                    PedidoID = Convert.ToInt32(lector["PedidoID"]),
                    FechaPedido = Convert.ToDateTime(lector["FechaPedido"]),
                    Destinatario = lector["Destinatario"]?.ToString(),
                    CiudadDestino = lector["CiudadDestino"]?.ToString(),
                    PaisDestino = lector["PaisDestino"]?.ToString(),
                    NombreProducto = lector["NombreProducto"]?.ToString(),
                    Cantidad = Convert.ToInt16(lector["Cantidad"]),
                    PrecioUnidad = Convert.ToDecimal(lector["PrecioUnidad"]),
                    Descuento = Convert.ToDecimal(lector["Descuento"])
                });
            }

            return lista;
        }
    }
}
