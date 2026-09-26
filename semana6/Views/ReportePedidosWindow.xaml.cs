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
    public partial class ReportePedidosWindow : Window
    {
        private readonly ReporteRepository _repository;

        public ReportePedidosWindow()
        {
            InitializeComponent();

            _repository = new ReporteRepository();

            dpFechaInicio.SelectedDate =
                DateTime.Today.AddMonths(-1);

            dpFechaFin.SelectedDate =
                DateTime.Today;
        }

        private async void GenerarReporte_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (dpFechaInicio.SelectedDate == null ||
                dpFechaFin.SelectedDate == null)
            {
                MessageBox.Show(
                    "Selecciona las dos fechas.",
                    "Validación");

                return;
            }

            DateTime inicio =
                dpFechaInicio.SelectedDate.Value;

            DateTime fin =
                dpFechaFin.SelectedDate.Value;

            if (inicio > fin)
            {
                MessageBox.Show(
                    "La fecha inicial no puede ser mayor que la fecha final.",
                    "Validación");

                return;
            }

            try
            {
                dgReporte.ItemsSource =
                    await _repository.ObtenerAsync(inicio, fin);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al generar el reporte:\n\n" + ex.Message,
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }
}