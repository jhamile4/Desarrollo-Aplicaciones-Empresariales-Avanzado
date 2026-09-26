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


using Semana6.Data;
using Semana6.Data.Models;
using Semana6.Data.Repositories;

namespace semana6.Views
{
    public partial class NuevoProductoWindow : Window
    {
        private readonly ProductoRepository _repository;
        private readonly Producto? _producto;

        public NuevoProductoWindow()
        {
            InitializeComponent();
            _repository = new ProductoRepository();
        }

        public NuevoProductoWindow(Producto producto)
        {
            InitializeComponent();

            _repository = new ProductoRepository();
            _producto = producto;

            Title = "Editar Producto";

            txtNombre.Text = producto.NombreProducto ?? "";
            txtProveedor.Text = producto.ProveedorID?.ToString() ?? "";
            txtCategoria.Text = producto.CategoriaID?.ToString() ?? "";
            txtCantidad.Text = producto.CantidadPorUnidad ?? "";
            txtPrecio.Text = producto.PrecioUnidad?.ToString() ?? "";
            txtStock.Text = producto.UnidadesEnExistencia?.ToString() ?? "";
        }

        private async void Guardar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtNombre.Text))
                {
                    MessageBox.Show("Ingresa el nombre del producto.");
                    return;
                }

                Producto producto = new Producto
                {
                    NombreProducto = txtNombre.Text.Trim(),
                    ProveedorID = int.TryParse(txtProveedor.Text, out int proveedor) ? proveedor : null,
                    CategoriaID = int.TryParse(txtCategoria.Text, out int categoria) ? categoria : null,
                    CantidadPorUnidad = string.IsNullOrWhiteSpace(txtCantidad.Text) ? null : txtCantidad.Text.Trim(),
                    PrecioUnidad = decimal.TryParse(txtPrecio.Text, out decimal precio) ? precio : null,
                    UnidadesEnExistencia = short.TryParse(txtStock.Text, out short stock) ? stock : null,
                    Descontinuado = false,
                    Activo = true
                };

                if (_producto == null)
                {
                    await _repository.InsertarAsync(producto);
                }
                else
                {
                    producto.ProductoID = _producto.ProductoID;
                    producto.UnidadesEnPedido = _producto.UnidadesEnPedido;
                    producto.NivelDeReorden = _producto.NivelDeReorden;

                    await _repository.ActualizarAsync(producto);
                }

                MessageBox.Show(
                    _producto == null
                        ? "Producto registrado correctamente."
                        : "Producto actualizado correctamente.",
                    "Neptuno",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Ocurrió un error:\n\n" + ex.Message,
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}