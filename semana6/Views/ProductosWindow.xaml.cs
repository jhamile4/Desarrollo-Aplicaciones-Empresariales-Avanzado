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


using Semana6.Data.Repositories;

namespace semana6.Views
{
    public partial class ProductosWindow : Window
    {
        private readonly ProductoRepository _repository;

        public ProductosWindow()
        {
            InitializeComponent();

            _repository = new ProductoRepository();

            Loaded += ProductosWindow_Loaded;
        }

        private async void ProductosWindow_Loaded(
            object sender,
            RoutedEventArgs e)
        {
            await CargarProductosAsync();
        }

        private async Task CargarProductosAsync()
        {
            try
            {
                dgProductos.ItemsSource =
                    await _repository.ListarAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "No se pudieron cargar los productos.\n\n" + ex.Message,
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private async void Nuevo_Click(
            object sender,
            RoutedEventArgs e)
        {
            NuevoProductoWindow ventana =
                new NuevoProductoWindow();

            bool? resultado = ventana.ShowDialog();

            if (resultado == true)
            {
                await CargarProductosAsync();
            }
        }

        private async void Editar_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (dgProductos.SelectedItem == null)
            {
                MessageBox.Show("Selecciona un producto.");
                return;
            }

            var producto = (Semana6.Data.Models.Producto)dgProductos.SelectedItem;

            NuevoProductoWindow ventana =
                new NuevoProductoWindow(producto);

            bool? resultado = ventana.ShowDialog();

            if (resultado == true)
            {
                await CargarProductosAsync();
            }
        }

        private async void Desactivar_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (dgProductos.SelectedItem == null)
            {
                MessageBox.Show("Selecciona un producto.");
                return;
            }

            var producto = (Semana6.Data.Models.Producto)dgProductos.SelectedItem;

            MessageBoxResult respuesta = MessageBox.Show(
                $"¿Deseas desactivar el producto '{producto.NombreProducto}'?",
                "Confirmar",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (respuesta != MessageBoxResult.Yes)
                return;

            try
            {
                await _repository.DesactivarAsync(producto.ProductoID);

                MessageBox.Show("Producto desactivado correctamente.");

                await CargarProductosAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "No se pudo desactivar:\n\n" + ex.Message,
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }
}