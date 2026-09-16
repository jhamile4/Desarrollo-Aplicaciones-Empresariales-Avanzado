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
using Semana3.Views;

namespace Semana3
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnAulasDataTable_Click(object sender, RoutedEventArgs e)
        {
            new AulasDataTableWindow().Show();
        }

        private void BtnAulasObjetos_Click(object sender, RoutedEventArgs e)
        {
            new AulasObjetosWindow().Show();
        }

        private void BtnReservasDataTable_Click(object sender, RoutedEventArgs e)
        {
            new ReservasDataTableWindow().Show();
        }

        private void BtnReservasObjetos_Click(object sender, RoutedEventArgs e)
        {
            new ReservasObjetosWindow().Show();
        }

        private void BtnNuevaReserva_Click(object sender, RoutedEventArgs e)
        {
            new NuevaReservaWindow().Show();
        }
    }
}