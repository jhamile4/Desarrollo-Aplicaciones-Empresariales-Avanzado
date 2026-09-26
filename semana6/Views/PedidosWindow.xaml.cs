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
    public partial class PedidosWindow : Window
    {
        private readonly PedidoRepository _repository;

        public PedidosWindow()
        {
            InitializeComponent();

            _repository = new PedidoRepository();

            Loaded += PedidosWindow_Loaded;
        }

        private async void PedidosWindow_Loaded(
            object sender,
            RoutedEventArgs e)
        {
            try
            {
                dgPedidos.ItemsSource =
                    await _repository.ListarAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al cargar pedidos:\n\n" + ex.Message,
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }
}
