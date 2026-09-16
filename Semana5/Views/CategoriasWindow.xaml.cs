using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using System.Collections.Generic;
using System.Windows;
using Microsoft.Data.SqlClient;
using Semana5.Data;
using Semana5.Models;

namespace Semana5.Views
{
    public partial class CategoriasWindow : Window
    {
        private int categoriaIdSeleccionada = 0;

        public CategoriasWindow()
        {
            InitializeComponent();
            CargarCategorias();
        }

        private void CargarCategorias()
        {
            List<Categoria> lista = new List<Categoria>();

            using (var conexion = Conexion.ObtenerConexion())
            {
                conexion.Open();
                using (var cmd = new SqlCommand("sp_Categoria_Listar", conexion))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Categoria
                            {
                                CategoriaID = reader.GetInt32(0),
                                NombreCategoria = reader.GetString(1),
                                Descripcion = reader.IsDBNull(2) ? "" : reader.GetString(2),
                                Activo = reader.GetBoolean(3)
                            });
                        }
                    }
                }
            }

            dgCategorias.ItemsSource = lista;
        }

        private void DgCategorias_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (dgCategorias.SelectedItem is Categoria cat)
            {
                categoriaIdSeleccionada = cat.CategoriaID;
                txtNombre.Text = cat.NombreCategoria;
                txtDescripcion.Text = cat.Descripcion;
            }
        }

        private void BtnNuevo_Click(object sender, RoutedEventArgs e)
        {
            categoriaIdSeleccionada = 0;
            txtNombre.Text = "";
            txtDescripcion.Text = "";
            dgCategorias.SelectedItem = null;
        }

        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("El nombre de la categoría es obligatorio.");
                return;
            }

            using (var conexion = Conexion.ObtenerConexion())
            {
                conexion.Open();

                if (categoriaIdSeleccionada == 0)
                {
                    // INSERTAR usando ExecuteNonQuery
                    using (var cmd = new SqlCommand("sp_Categoria_Insertar", conexion))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@NombreCategoria", txtNombre.Text.Trim());
                        cmd.Parameters.AddWithValue("@Descripcion", (object)txtDescripcion.Text.Trim() ?? System.DBNull.Value);
                        cmd.ExecuteNonQuery();
                    }
                    MessageBox.Show("Categoría creada correctamente.");
                }
                else
                {
                    // ACTUALIZAR usando ExecuteNonQuery
                    using (var cmd = new SqlCommand("sp_Categoria_Actualizar", conexion))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@CategoriaID", categoriaIdSeleccionada);
                        cmd.Parameters.AddWithValue("@NombreCategoria", txtNombre.Text.Trim());
                        cmd.Parameters.AddWithValue("@Descripcion", (object)txtDescripcion.Text.Trim() ?? System.DBNull.Value);
                        cmd.ExecuteNonQuery();
                    }
                    MessageBox.Show("Categoría actualizada correctamente.");
                }
            }

            BtnNuevo_Click(sender, e);
            CargarCategorias();
        }

        private void BtnEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (categoriaIdSeleccionada == 0)
            {
                MessageBox.Show("Selecciona una categoría de la lista para eliminar.");
                return;
            }

            var confirmar = MessageBox.Show("¿Seguro que deseas eliminar esta categoría?",
                                             "Confirmar", MessageBoxButton.YesNo);
            if (confirmar != MessageBoxResult.Yes) return;

            using (var conexion = Conexion.ObtenerConexion())
            {
                conexion.Open();
                // ELIMINACIÓN LÓGICA usando ExecuteNonQuery (actualiza Activo = 0, nunca DELETE físico)
                using (var cmd = new SqlCommand("sp_Categoria_Eliminar", conexion))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CategoriaID", categoriaIdSeleccionada);
                    cmd.ExecuteNonQuery();
                }
            }

            BtnNuevo_Click(sender, e);
            CargarCategorias();
        }
    }
}