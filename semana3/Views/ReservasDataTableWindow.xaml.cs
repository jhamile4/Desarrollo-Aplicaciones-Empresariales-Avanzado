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
    public partial class ReservasDataTableWindow : Window
    {
        public ReservasDataTableWindow()
        {
            InitializeComponent();
            CargarReservas();
        }

        private void CargarReservas()
        {
            DataTable tabla = new DataTable();

            using (var conexion = Conexion.ObtenerConexion())
            {
                string query = @"SELECT r.ReservaId, a.Nombre AS Aula, u.NombreCompleto AS Usuario,
                                         r.Fecha, r.Hora, r.Motivo
                                  FROM Reservas r
                                  INNER JOIN Aulas a ON r.AulaId = a.AulaId
                                  INNER JOIN Usuarios u ON r.UsuarioId = u.UsuarioId";
                SqlDataAdapter adapter = new SqlDataAdapter(query, conexion);
                adapter.Fill(tabla);
            }

            dgReservas.ItemsSource = tabla.DefaultView;
        }
    }
}