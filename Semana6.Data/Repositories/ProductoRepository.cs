
using Microsoft.Data.SqlClient;
using Semana6.Data.Models;
using System.Data;

using Semana6.Data.Repositories;

using Microsoft.Data.SqlClient;
using Semana6.Data.Models;
using System.Data;

namespace Semana6.Data.Repositories
{
    public class ProductoRepository
    {
        public async Task<List<Producto>> ListarAsync()
        {
            List<Producto> productos = new();

            using SqlConnection conexion = new(Conexion.Cadena);

            using SqlCommand comando =
                new SqlCommand("sp_Productos_Listar", conexion);

            comando.CommandType = CommandType.StoredProcedure;

            await conexion.OpenAsync();

            using SqlDataReader lector =
                await comando.ExecuteReaderAsync();

            while (await lector.ReadAsync())
            {
                productos.Add(new Producto
                {
                    ProductoID = Convert.ToInt32(lector["ProductoID"]),

                    NombreProducto =
                        lector["NombreProducto"]?.ToString(),

                    ProveedorID =
                        lector["ProveedorID"] == DBNull.Value
                        ? null
                        : Convert.ToInt32(lector["ProveedorID"]),

                    CategoriaID =
                        lector["CategoriaID"] == DBNull.Value
                        ? null
                        : Convert.ToInt32(lector["CategoriaID"]),

                    CantidadPorUnidad =
                        lector["CantidadPorUnidad"]?.ToString(),

                    PrecioUnidad =
                        lector["PrecioUnidad"] == DBNull.Value
                        ? null
                        : Convert.ToDecimal(lector["PrecioUnidad"]),

                    UnidadesEnExistencia =
                        lector["UnidadesEnExistencia"] == DBNull.Value
                        ? null
                        : Convert.ToInt16(lector["UnidadesEnExistencia"]),

                    UnidadesEnPedido =
                        lector["UnidadesEnPedido"] == DBNull.Value
                        ? null
                        : Convert.ToInt16(lector["UnidadesEnPedido"]),

                    NivelDeReorden =
                        lector["NivelDeReorden"] == DBNull.Value
                        ? null
                        : Convert.ToInt16(lector["NivelDeReorden"]),

                    Descontinuado =
                        lector["Descontinuado"] != DBNull.Value &&
                        Convert.ToBoolean(lector["Descontinuado"]),

                    Activo =
                        lector["Activo"] != DBNull.Value &&
                        Convert.ToBoolean(lector["Activo"])
                });
            }

            return productos;
        }

        public async Task InsertarAsync(Producto producto)
        {
            using SqlConnection conexion =
                new SqlConnection(Conexion.Cadena);

            using SqlCommand comando =
                new SqlCommand("sp_Productos_Insertar", conexion);

            comando.CommandType = CommandType.StoredProcedure;

            comando.Parameters.AddWithValue(
                "@NombreProducto",
                producto.NombreProducto ?? "");

            comando.Parameters.AddWithValue(
                "@ProveedorID",
                (object?)producto.ProveedorID ?? DBNull.Value);

            comando.Parameters.AddWithValue(
                "@CategoriaID",
                (object?)producto.CategoriaID ?? DBNull.Value);

            comando.Parameters.AddWithValue(
                "@CantidadPorUnidad",
                (object?)producto.CantidadPorUnidad ?? DBNull.Value);

            comando.Parameters.AddWithValue(
                "@PrecioUnidad",
                (object?)producto.PrecioUnidad ?? DBNull.Value);

            comando.Parameters.AddWithValue(
                "@UnidadesEnExistencia",
                (object?)producto.UnidadesEnExistencia ?? DBNull.Value);

            comando.Parameters.AddWithValue(
                "@UnidadesEnPedido",
                (object?)producto.UnidadesEnPedido ?? DBNull.Value);

            comando.Parameters.AddWithValue(
                "@NivelDeReorden",
                (object?)producto.NivelDeReorden ?? DBNull.Value);

            comando.Parameters.AddWithValue(
                "@Descontinuado",
                producto.Descontinuado);

            await conexion.OpenAsync();

            await comando.ExecuteNonQueryAsync();
        }

        public async Task ActualizarAsync(Producto producto)
        {
            using SqlConnection conexion =
                new SqlConnection(Conexion.Cadena);

            using SqlCommand comando =
                new SqlCommand("sp_Productos_Actualizar", conexion);

            comando.CommandType = CommandType.StoredProcedure;

            comando.Parameters.AddWithValue(
                "@ProductoID",
                producto.ProductoID);

            comando.Parameters.AddWithValue(
                "@NombreProducto",
                producto.NombreProducto ?? "");

            comando.Parameters.AddWithValue(
                "@ProveedorID",
                (object?)producto.ProveedorID ?? DBNull.Value);

            comando.Parameters.AddWithValue(
                "@CategoriaID",
                (object?)producto.CategoriaID ?? DBNull.Value);

            comando.Parameters.AddWithValue(
                "@CantidadPorUnidad",
                (object?)producto.CantidadPorUnidad ?? DBNull.Value);

            comando.Parameters.AddWithValue(
                "@PrecioUnidad",
                (object?)producto.PrecioUnidad ?? DBNull.Value);

            comando.Parameters.AddWithValue(
                "@UnidadesEnExistencia",
                (object?)producto.UnidadesEnExistencia ?? DBNull.Value);

            comando.Parameters.AddWithValue(
                "@UnidadesEnPedido",
                (object?)producto.UnidadesEnPedido ?? DBNull.Value);

            comando.Parameters.AddWithValue(
                "@NivelDeReorden",
                (object?)producto.NivelDeReorden ?? DBNull.Value);

            comando.Parameters.AddWithValue(
                "@Descontinuado",
                producto.Descontinuado);

            await conexion.OpenAsync();

            await comando.ExecuteNonQueryAsync();
        }

        public async Task DesactivarAsync(int productoID)
        {
            using SqlConnection conexion =
                new SqlConnection(Conexion.Cadena);

            using SqlCommand comando =
                new SqlCommand("sp_Productos_Desactivar", conexion);

            comando.CommandType = CommandType.StoredProcedure;

            comando.Parameters.AddWithValue(
                "@ProductoID",
                productoID);

            await conexion.OpenAsync();

            await comando.ExecuteNonQueryAsync();
        }
    }
}
