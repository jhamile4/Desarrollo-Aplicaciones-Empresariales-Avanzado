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
    public partial class SalidasWindow : Window
    {
        public static List<Salida> ListaSalidas = new List<Salida>();

        public SalidasWindow()
        {
            InitializeComponent();
        }

        private void BtnRegistrarSalida_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPlacaSalida.Text) ||
                string.IsNullOrWhiteSpace(txtConductorSalida.Text) ||
                cmbTurnoSalida.SelectedItem == null ||
                dpFechaSalida.SelectedDate == null ||
                string.IsNullOrWhiteSpace(txtPesoSalida.Text))
            {
                MessageBox.Show("Por favor, complete todos los campos.",
                                "Validación",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                return;
            }

            Salida nuevaSalida = new Salida
            {
                Placa = txtPlacaSalida.Text,
                Conductor = txtConductorSalida.Text,
                Turno = ((ComboBoxItem)cmbTurnoSalida.SelectedItem).Content.ToString(),
                Fecha = dpFechaSalida.SelectedDate.Value.ToString("dd/MM/yyyy"),
                Peso = txtPesoSalida.Text,
                Observacion = txtObservacion.Text
            };

            ListaSalidas.Add(nuevaSalida);

            MessageBox.Show("Salida registrada correctamente.",
                            "Registro exitoso",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);
        }
    }
}