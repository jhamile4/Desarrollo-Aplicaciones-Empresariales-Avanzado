using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Windows;
using Semana5.Views;

namespace Semana5
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnCategorias_Click(object sender, RoutedEventArgs e)
        {
            new CategoriasWindow().Show();
        }

        private void BtnProveedores_Click(object sender, RoutedEventArgs e)
        {
            new ProveedoresWindow().Show();
        }

        private void BtnProductos_Click(object sender, RoutedEventArgs e)
        {
            new ProductosWindow().Show();
        }

        private void BtnPedidos_Click(object sender, RoutedEventArgs e)
        {
            new PedidosWindow().Show();
        }
    }
}