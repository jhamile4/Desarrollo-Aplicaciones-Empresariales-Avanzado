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
    /// <summary>
    /// Lógica de interacción para MenuWindow1.xaml
    /// </summary>
    public partial class MenuWindow1 : Window
    {
        public MenuWindow1()
        {
            InitializeComponent();
        }
        private void BtnIngresos_Click(object sender, RoutedEventArgs e)
        {
            IngresosWindow ingresos = new IngresosWindow();
            ingresos.Show();
        }
        private void BtnConductores_Click(object sender, RoutedEventArgs e)
        {
            ConductoresWindow conductores = new ConductoresWindow();
            conductores.Show();
        }
        private void BtnReporteIngresos_Click(object sender, RoutedEventArgs e)
        {
            ListaIngresosWindow lista = new ListaIngresosWindow();
            lista.Show();
        }
        private void BtnSalidas_Click(object sender, RoutedEventArgs e)
        {
            SalidasWindow salidas = new SalidasWindow();
            salidas.Show();
        }
        private void BtnTransportistas_Click(object sender, RoutedEventArgs e)
        {
            TransportistasWindow transportistas = new TransportistasWindow();
            transportistas.Show();
        }
        private void BtnCamiones_Click(object sender, RoutedEventArgs e)
        {
            CamionesWindow camiones = new CamionesWindow();
            camiones.Show();
        }
        private void BtnProductos_Click(object sender, RoutedEventArgs e)
        {
            ProductosWindow productos = new ProductosWindow();
            productos.Show();
        }
    }
}
