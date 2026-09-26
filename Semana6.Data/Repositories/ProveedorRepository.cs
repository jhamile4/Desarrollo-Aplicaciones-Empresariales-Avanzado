using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Data.SqlClient;
using Semana6.Data.Models;
using System.Data;

namespace Semana6.Data.Repositories
{
    public class ProveedorRepository
    {
        public async Task<List<Proveedor>> ListarAsync()
        {
            List<Proveedor> lista = new();

            using SqlConnection conexion = new(Conexion.Cadena);
            using SqlCommand comando = new("sp_Proveedores_Listar", conexion);

            comando.CommandType = CommandType.StoredProcedure;

            await conexion.OpenAsync();

            using SqlDataReader lector = await comando.ExecuteReaderAsync();

            while (await lector.ReadAsync())
            {
                lista.Add(new Proveedor
                {
                    ProveedorID = Convert.ToInt32(lector["ProveedorID"]),
                    CompaniaNombre = lector["CompaniaNombre"]?.ToString(),
                    NombreContacto = lector["NombreContacto"]?.ToString(),
                    CargoContacto = lector["CargoContacto"]?.ToString(),
                    Direccion = lector["Direccion"]?.ToString(),
                    Ciudad = lector["Ciudad"]?.ToString(),
                    CodigoPostal = lector["CodigoPostal"]?.ToString(),
                    Pais = lector["Pais"]?.ToString(),
                    Telefono = lector["Telefono"]?.ToString(),
                    Fax = lector["Fax"]?.ToString(),
                    Activo = lector["Activo"] != DBNull.Value &&
                             Convert.ToBoolean(lector["Activo"])
                });
            }

            return lista;
        }

        public async Task DesactivarAsync(int id)
        {
            using SqlConnection conexion = new(Conexion.Cadena);
            using SqlCommand comando = new("sp_Proveedores_Desactivar", conexion);

            comando.CommandType = CommandType.StoredProcedure;
            comando.Parameters.AddWithValue("@ProveedorID", id);

            await conexion.OpenAsync();
            await comando.ExecuteNonQueryAsync();
        }
    }
}
