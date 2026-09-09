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
    public partial class TransportistasWindow : Window
    {
        public static List<Transportista> ListaTransportistas = new List<Transportista>();

        public TransportistasWindow()
        {
            InitializeComponent();
        }

        private void BtnRegistrarTransportista_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombreTransportista.Text) ||
                string.IsNullOrWhiteSpace(txtRuc.Text) ||
                string.IsNullOrWhiteSpace(txtTelefono.Text))
            {
                MessageBox.Show("Por favor, complete todos los campos.",
                                "Validación",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                return;
            }

            Transportista nuevoTransportista = new Transportista
            {
                Nombre = txtNombreTransportista.Text,
                Ruc = txtRuc.Text,
                Telefono = txtTelefono.Text
            };

            ListaTransportistas.Add(nuevoTransportista);

            MessageBox.Show("Transportista registrado correctamente.",
                            "Registro exitoso",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);
        }
    }
}