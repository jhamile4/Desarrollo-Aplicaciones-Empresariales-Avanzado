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
    public partial class ProductosWindow : Window
    {
        private int productoIdSeleccionado = 0;

        public ProductosWindow()
        {
            InitializeComponent();
            CargarProveedores();
            CargarCategorias();
            CargarProductos();
        }

        private void CargarProveedores()
        {
            List<Proveedor> lista = new List<Proveedor>();
            using (var conexion = Conexion.ObtenerConexion())
            {
                conexion.Open();
                using (var cmd = new SqlCommand("sp_Proveedor_Buscar", conexion))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@NombreContacto", System.DBNull.Value);
                    cmd.Parameters.AddWithValue("@Ciudad", System.DBNull.Value);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Proveedor { ProveedorID = reader.GetInt32(0), CompaniaNombre = reader.GetString(1) });
                        }
                    }
                }
            }
            cbProveedores.ItemsSource = lista;
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
                            lista.Add(new Categoria { CategoriaID = reader.GetInt32(0), NombreCategoria = reader.GetString(1) });
                        }
                    }
                }
            }
            cbCategorias.ItemsSource = lista;
        }

        private void CargarProductos()
        {
            List<Producto> lista = new List<Producto>();
            using (var conexion = Conexion.ObtenerConexion())
            {
                conexion.Open();
                using (var cmd = new SqlCommand("sp_Producto_Listar", conexion))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Producto
                            {
                                ProductoID = reader.GetInt32(0),
                                NombreProducto = reader.GetString(1),
                                ProveedorID = reader.GetInt32(2),
                                CategoriaID = reader.GetInt32(3),
                                CantidadPorUnidad = reader.IsDBNull(4) ? "" : reader.GetString(4),
                                PrecioUnidad = reader.GetDecimal(5),
                                UnidadesEnExistencia = reader.GetInt16(6),
                                UnidadesEnPedido = reader.GetInt16(7),
                                NivelDeReorden = reader.GetInt16(8),
                                Descontinuado = reader.GetBoolean(9),
                                Activo = reader.GetBoolean(10)
                            });
                        }
                    }
                }
            }
            dgProductos.ItemsSource = lista;
        }

        private void DgProductos_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (dgProductos.SelectedItem is Producto p)
            {
                productoIdSeleccionado = p.ProductoID;
                txtNombre.Text = p.NombreProducto;
                txtCantidadPorUnidad.Text = p.CantidadPorUnidad;
                cbProveedores.SelectedValue = p.ProveedorID;
                cbCategorias.SelectedValue = p.CategoriaID;
                txtPrecio.Text = p.PrecioUnidad.ToString();
                txtExistencia.Text = p.UnidadesEnExistencia.ToString();
            }
        }

        private void BtnNuevo_Click(object sender, RoutedEventArgs e)
        {
            productoIdSeleccionado = 0;
            txtNombre.Text = "";
            txtCantidadPorUnidad.Text = "";
            txtPrecio.Text = "";
            txtExistencia.Text = "";
            dgProductos.SelectedItem = null;
        }

        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text) || cbProveedores.SelectedValue == null || cbCategorias.SelectedValue == null)
            {
                MessageBox.Show("Nombre, proveedor y categoría son obligatorios.");
                return;
            }

            if (!decimal.TryParse(txtPrecio.Text, out decimal precio))
            {
                MessageBox.Show("El precio debe ser un número válido.");
                return;
            }

            if (!short.TryParse(txtExistencia.Text, out short existencia))
            {
                MessageBox.Show("Las unidades en existencia deben ser un número entero válido.");
                return;
            }

            using (var conexion = Conexion.ObtenerConexion())
            {
                conexion.Open();

                if (productoIdSeleccionado == 0)
                {
                    using (var cmd = new SqlCommand("sp_Producto_Insertar", conexion))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@NombreProducto", txtNombre.Text.Trim());
                        cmd.Parameters.AddWithValue("@ProveedorID", (int)cbProveedores.SelectedValue);
                        cmd.Parameters.AddWithValue("@CategoriaID", (int)cbCategorias.SelectedValue);
                        cmd.Parameters.AddWithValue("@CantidadPorUnidad", txtCantidadPorUnidad.Text.Trim());
                        cmd.Parameters.AddWithValue("@PrecioUnidad", precio);
                        cmd.Parameters.AddWithValue("@UnidadesEnExistencia", existencia);
                        cmd.Parameters.AddWithValue("@UnidadesEnPedido", (short)0);
                        cmd.Parameters.AddWithValue("@NivelDeReorden", (short)0);
                        cmd.ExecuteNonQuery();
                    }
                    MessageBox.Show("Producto creado correctamente.");
                }
                else
                {
                    using (var cmd = new SqlCommand("sp_Producto_Actualizar", conexion))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ProductoID", productoIdSeleccionado);
                        cmd.Parameters.AddWithValue("@NombreProducto", txtNombre.Text.Trim());
                        cmd.Parameters.AddWithValue("@ProveedorID", (int)cbProveedores.SelectedValue);
                        cmd.Parameters.AddWithValue("@CategoriaID", (int)cbCategorias.SelectedValue);
                        cmd.Parameters.AddWithValue("@CantidadPorUnidad", txtCantidadPorUnidad.Text.Trim());
                        cmd.Parameters.AddWithValue("@PrecioUnidad", precio);
                        cmd.Parameters.AddWithValue("@UnidadesEnExistencia", existencia);
                        cmd.Parameters.AddWithValue("@UnidadesEnPedido", (short)0);
                        cmd.Parameters.AddWithValue("@NivelDeReorden", (short)0);
                        cmd.ExecuteNonQuery();
                    }
                    MessageBox.Show("Producto actualizado correctamente.");
                }
            }

            BtnNuevo_Click(sender, e);
            CargarProductos();
        }

        private void BtnEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (productoIdSeleccionado == 0)
            {
                MessageBox.Show("Selecciona un producto de la lista para eliminar.");
                return;
            }

            var confirmar = MessageBox.Show("¿Seguro que deseas eliminar este producto?",
                                             "Confirmar", MessageBoxButton.YesNo);
            if (confirmar != MessageBoxResult.Yes) return;

            using (var conexion = Conexion.ObtenerConexion())
            {
                conexion.Open();
                using (var cmd = new SqlCommand("sp_Producto_Eliminar", conexion))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ProductoID", productoIdSeleccionado);
                    cmd.ExecuteNonQuery();
                }
            }

            BtnNuevo_Click(sender, e);
            CargarProductos();
        }
    }
}