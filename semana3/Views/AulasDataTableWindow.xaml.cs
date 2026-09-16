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

using System.Data;
using System.Windows;
using Microsoft.Data.SqlClient;
using Semana3.Data;

namespace Semana3.Views
{
    public partial class AulasDataTableWindow : Window
    {
        public AulasDataTableWindow()
        {
            InitializeComponent();
            CargarAulas();
        }

        private void CargarAulas()
        {
            DataTable tabla = new DataTable();

            using (var conexion = Conexion.ObtenerConexion())
            {
                string query = "SELECT AulaId, Nombre, Capacidad FROM Aulas";
                SqlDataAdapter adapter = new SqlDataAdapter(query, conexion);
                adapter.Fill(tabla); // Abre y cierra la conexión automáticamente
            }

            dgAulas.ItemsSource = tabla.DefaultView;
        }
    }
}
