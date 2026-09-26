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

using System.Windows;
using Semana6.Data.Repositories;

namespace semana6.Views
{
    public partial class ProveedoresWindow : Window
    {
        private readonly ProveedorRepository _repository;

        public ProveedoresWindow()
        {
            InitializeComponent();

            _repository = new ProveedorRepository();

            Loaded += ProveedoresWindow_Loaded;
        }

        private async void ProveedoresWindow_Loaded(
            object sender,
            RoutedEventArgs e)
        {
            await CargarProveedoresAsync();
        }

        private async Task CargarProveedoresAsync()
        {
            try
            {
                dgProveedores.ItemsSource =
                    await _repository.ListarAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al cargar proveedores:\n\n" + ex.Message,
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }
}
