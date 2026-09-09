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
    /// Lógica de interacción para IngresosWindow.xaml
    /// </summary>
    public partial class IngresosWindow : Window
    {
        public static List<Ingreso> ListaIngresos = new List<Ingreso>();
        public IngresosWindow()
        {
            InitializeComponent();
        }
        private void BtnRegistrar_Click(object sender, RoutedEventArgs e)
        {
            if (cmbTipoDocumento.SelectedItem == null ||
                string.IsNullOrWhiteSpace(txtNumeroDocumento.Text) ||
                string.IsNullOrWhiteSpace(txtPlaca.Text) ||
                cmbTurno.SelectedItem == null ||
                string.IsNullOrWhiteSpace(txtConductor.Text) ||
                string.IsNullOrWhiteSpace(txtCliente.Text) ||
                dpFecha.SelectedDate == null ||
                string.IsNullOrWhiteSpace(txtPeso.Text))
            {
                MessageBox.Show("Por favor, complete todos los campos.",
                                "Validación",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                return;
            }

            Ingreso nuevoIngreso = new Ingreso
            {
                TipoDocumento = ((ComboBoxItem)cmbTipoDocumento.SelectedItem).Content.ToString(),
                NumeroDocumento = txtNumeroDocumento.Text,
                Placa = txtPlaca.Text,
                Turno = ((ComboBoxItem)cmbTurno.SelectedItem).Content.ToString(),
                Conductor = txtConductor.Text,
                Cliente = txtCliente.Text,
                Fecha = dpFecha.SelectedDate.Value.ToString("dd/MM/yyyy"),
                Peso = txtPeso.Text
            };

            ListaIngresos.Add(nuevoIngreso);

            MessageBox.Show("Ingreso registrado correctamente.",
                            "Registro exitoso",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);
        }
    }
}
