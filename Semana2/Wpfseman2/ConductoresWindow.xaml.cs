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
    /// Lógica de interacción para ConductoresWindow.xaml
    /// </summary>
    public partial class ConductoresWindow : Window
    {
        public static List<Conductor> ListaConductores = new List<Conductor>();
        public ConductoresWindow()
        {
            InitializeComponent();
        }
        private void BtnRegistrarConductor_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombreConductor.Text) ||
                string.IsNullOrWhiteSpace(txtLicencia.Text) ||
                string.IsNullOrWhiteSpace(txtTransporte.Text))
            {
                MessageBox.Show("Por favor, complete todos los campos.",
                                "Validación",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                return;
            }
            Conductor nuevoConductor = new Conductor
            {
                Nombre = txtNombreConductor.Text,
                Licencia = txtLicencia.Text,
                Transporte = txtTransporte.Text
            };

            ListaConductores.Add(nuevoConductor);

            MessageBox.Show("Conductor registrado correctamente.",
                            "Registro exitoso",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);
        }
        private void BtnVerConductores_Click(object sender, RoutedEventArgs e)
        {
            ListaConductoresWindow lista = new ListaConductoresWindow();
            lista.Show();
        }
    }
}
