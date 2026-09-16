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
    public partial class ProveedoresWindow : Window
    {
        private int proveedorIdSeleccionado = 0;

        public ProveedoresWindow()
        {
            InitializeComponent();
            BuscarProveedores(null, null);
        }

        private void BuscarProveedores(string contacto, string ciudad)
        {
            List<Proveedor> lista = new List<Proveedor>();

            using (var conexion = Conexion.ObtenerConexion())
            {
                conexion.Open();
                using (var cmd = new SqlCommand("sp_Proveedor_Buscar", conexion))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@NombreContacto", (object)contacto ?? System.DBNull.Value);
                    cmd.Parameters.AddWithValue("@Ciudad", (object)ciudad ?? System.DBNull.Value);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Proveedor
                            {
                                ProveedorID = reader.GetInt32(0),
                                CompaniaNombre = reader.GetString(1),
                                NombreContacto = reader.IsDBNull(2) ? "" : reader.GetString(2),
                                Ciudad = reader.IsDBNull(3) ? "" : reader.GetString(3),
                                Telefono = reader.IsDBNull(4) ? "" : reader.GetString(4),
                                Activo = reader.GetBoolean(5)
                            });
                        }
                    }
                }
            }

            dgProveedores.ItemsSource = lista;
        }

        private void DgProveedores_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (dgProveedores.SelectedItem is Proveedor p)
            {
                proveedorIdSeleccionado = p.ProveedorID;
                txtCompania.Text = p.CompaniaNombre;
                txtContacto.Text = p.NombreContacto;
                txtCiudad.Text = p.Ciudad;
                txtTelefono.Text = p.Telefono;
            }
        }

        private void BtnNuevo_Click(object sender, RoutedEventArgs e)
        {
            proveedorIdSeleccionado = 0;
            txtCompania.Text = "";
            txtContacto.Text = "";
            txtCiudad.Text = "";
            txtTelefono.Text = "";
            dgProveedores.SelectedItem = null;
        }

        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCompania.Text))
            {
                MessageBox.Show("El nombre de la compañía es obligatorio.");
                return;
            }

            using (var conexion = Conexion.ObtenerConexion())
            {
                conexion.Open();

                if (proveedorIdSeleccionado == 0)
                {
                    using (var cmd = new SqlCommand("sp_Proveedor_Insertar", conexion))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@CompaniaNombre", txtCompania.Text.Trim());
                        cmd.Parameters.AddWithValue("@NombreContacto", txtContacto.Text.Trim());
                        cmd.Parameters.AddWithValue("@Ciudad", txtCiudad.Text.Trim());
                        cmd.Parameters.AddWithValue("@Telefono", txtTelefono.Text.Trim());
                        cmd.ExecuteNonQuery();
                    }
                    MessageBox.Show("Proveedor creado correctamente.");
                }
                else
                {
                    using (var cmd = new SqlCommand("sp_Proveedor_Actualizar", conexion))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ProveedorID", proveedorIdSeleccionado);
                        cmd.Parameters.AddWithValue("@CompaniaNombre", txtCompania.Text.Trim());
                        cmd.Parameters.AddWithValue("@NombreContacto", txtContacto.Text.Trim());
                        cmd.Parameters.AddWithValue("@Ciudad", txtCiudad.Text.Trim());
                        cmd.Parameters.AddWithValue("@Telefono", txtTelefono.Text.Trim());
                        cmd.ExecuteNonQuery();
                    }
                    MessageBox.Show("Proveedor actualizado correctamente.");
                }
            }

            BtnNuevo_Click(sender, e);
            BuscarProveedores(null, null);
        }

        private void BtnEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (proveedorIdSeleccionado == 0)
            {
                MessageBox.Show("Selecciona un proveedor de la lista para eliminar.");
                return;
            }

            var confirmar = MessageBox.Show("¿Seguro que deseas eliminar este proveedor?",
                                             "Confirmar", MessageBoxButton.YesNo);
            if (confirmar != MessageBoxResult.Yes) return;

            using (var conexion = Conexion.ObtenerConexion())
            {
                conexion.Open();
                using (var cmd = new SqlCommand("sp_Proveedor_Eliminar", conexion))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ProveedorID", proveedorIdSeleccionado);
                    cmd.ExecuteNonQuery();
                }
            }

            BtnNuevo_Click(sender, e);
            BuscarProveedores(null, null);
        }

        private void BtnBuscar_Click(object sender, RoutedEventArgs e)
        {
            string contacto = string.IsNullOrWhiteSpace(txtBuscarContacto.Text) ? null : txtBuscarContacto.Text.Trim();
            string ciudad = string.IsNullOrWhiteSpace(txtBuscarCiudad.Text) ? null : txtBuscarCiudad.Text.Trim();
            BuscarProveedores(contacto, ciudad);
        }

        private void BtnMostrarTodos_Click(object sender, RoutedEventArgs e)
        {
            txtBuscarContacto.Text = "";
            txtBuscarCiudad.Text = "";
            BuscarProveedores(null, null);
        }
    }
}