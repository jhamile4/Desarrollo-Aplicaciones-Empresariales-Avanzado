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



using semana6.Views;

namespace semana6
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Inicio_Click(object sender, RoutedEventArgs e)
        {
            // Ya estamos en el inicio.
        }

        private void Productos_Click(object sender, RoutedEventArgs e)
        {
            ProductosWindow ventana = new ProductosWindow();
            ventana.ShowDialog();
        }

        private void Categorias_Click(object sender, RoutedEventArgs e)
        {
            CategoriasWindow ventana = new CategoriasWindow();
            ventana.ShowDialog();
        }

        private void Proveedores_Click(object sender, RoutedEventArgs e)
        {
            ProveedoresWindow ventana = new ProveedoresWindow();
            ventana.ShowDialog();
        }

        private void Pedidos_Click(object sender, RoutedEventArgs e)
        {
            PedidosWindow ventana = new PedidosWindow();
            ventana.ShowDialog();
        }

        private void ReportePedidos_Click(object sender, RoutedEventArgs e)
        {
            ReportePedidosWindow ventana = new ReportePedidosWindow();
            ventana.ShowDialog();
        }
    }
}