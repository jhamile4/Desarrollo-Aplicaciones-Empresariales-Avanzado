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
    public partial class CamionesWindow : Window
    {
        public static List<Camion> ListaCamiones = new List<Camion>();

        public CamionesWindow()
        {
            InitializeComponent();
        }

        private void BtnRegistrarCamion_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPlacaCamion.Text) ||
                string.IsNullOrWhiteSpace(txtMarca.Text) ||
                string.IsNullOrWhiteSpace(txtModelo.Text) ||
                string.IsNullOrWhiteSpace(txtAnio.Text))
            {
                MessageBox.Show("Por favor, complete todos los campos.",
                                "Validación",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                return;
            }

            Camion nuevoCamion = new Camion
            {
                Placa = txtPlacaCamion.Text,
                Marca = txtMarca.Text,
                Modelo = txtModelo.Text,
                Anio = txtAnio.Text
            };

            ListaCamiones.Add(nuevoCamion);

            MessageBox.Show("Camión registrado correctamente.",
                            "Registro exitoso",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);
        }
    }
}