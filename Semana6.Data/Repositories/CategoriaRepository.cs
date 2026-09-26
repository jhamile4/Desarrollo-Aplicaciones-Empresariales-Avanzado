using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Data.SqlClient;
using Semana6.Data.Models;
using System.Data;

namespace Semana6.Data.Repositories
{
    public class CategoriaRepository
    {
        public async Task<List<Categoria>> ListarAsync()
        {
            List<Categoria> lista = new();

            using SqlConnection conexion = new(Conexion.Cadena);
            using SqlCommand comando = new("sp_Categorias_Listar", conexion);

            comando.CommandType = CommandType.StoredProcedure;

            await conexion.OpenAsync();

            using SqlDataReader lector = await comando.ExecuteReaderAsync();

            while (await lector.ReadAsync())
            {
                lista.Add(new Categoria
                {
                    CategoriaID = Convert.ToInt32(lector["CategoriaID"]),
                    NombreCategoria = lector["NombreCategoria"]?.ToString(),
                    Descripcion = lector["Descripcion"]?.ToString(),
                    Activo = lector["Activo"] != DBNull.Value &&
                             Convert.ToBoolean(lector["Activo"])
                });
            }

            return lista;
        }

        public async Task InsertarAsync(Categoria categoria)
        {
            using SqlConnection conexion = new(Conexion.Cadena);
            using SqlCommand comando = new("sp_Categorias_Insertar", conexion);

            comando.CommandType = CommandType.StoredProcedure;

            comando.Parameters.AddWithValue(
                "@NombreCategoria",
                categoria.NombreCategoria ?? "");

            comando.Parameters.AddWithValue(
                "@Descripcion",
                (object?)categoria.Descripcion ?? DBNull.Value);

            await conexion.OpenAsync();
            await comando.ExecuteNonQueryAsync();
        }

        public async Task ActualizarAsync(Categoria categoria)
        {
            using SqlConnection conexion = new(Conexion.Cadena);
            using SqlCommand comando = new("sp_Categorias_Actualizar", conexion);

            comando.CommandType = CommandType.StoredProcedure;

            comando.Parameters.AddWithValue("@CategoriaID", categoria.CategoriaID);
            comando.Parameters.AddWithValue(
                "@NombreCategoria",
                categoria.NombreCategoria ?? "");
            comando.Parameters.AddWithValue(
                "@Descripcion",
                (object?)categoria.Descripcion ?? DBNull.Value);

            await conexion.OpenAsync();
            await comando.ExecuteNonQueryAsync();
        }

        public async Task DesactivarAsync(int id)
        {
            using SqlConnection conexion = new(Conexion.Cadena);
            using SqlCommand comando = new("sp_Categorias_Desactivar", conexion);

            comando.CommandType = CommandType.StoredProcedure;
            comando.Parameters.AddWithValue("@CategoriaID", id);

            await conexion.OpenAsync();
            await comando.ExecuteNonQueryAsync();
        }
    }
}
