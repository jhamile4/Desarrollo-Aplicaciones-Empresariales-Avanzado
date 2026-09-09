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

namespace Wpfseman2
{
    public partial class ProductosWindow : Window
    {
        public static List<Producto> ListaProductos = new List<Producto>();

        public ProductosWindow()
        {
            InitializeComponent();
        }

        private void BtnRegistrarProducto_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombreProducto.Text) ||
                string.IsNullOrWhiteSpace(txtCategoria.Text) ||
                string.IsNullOrWhiteSpace(txtUnidad.Text))
            {
                MessageBox.Show("Por favor, complete los campos obligatorios.",
                                "Validación",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                return;
            }

            Producto nuevoProducto = new Producto
            {
                Nombre = txtNombreProducto.Text,
                Categoria = txtCategoria.Text,
                Unidad = txtUnidad.Text,
                Descripcion = txtDescripcion.Text
            };

            ListaProductos.Add(nuevoProducto);

            MessageBox.Show("Producto registrado correctamente.",
                            "Registro exitoso",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);

            txtNombreProducto.Clear();
            txtCategoria.Clear();
            txtUnidad.Clear();
            txtDescripcion.Clear();
        }
    }
}