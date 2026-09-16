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

using System;
using System.Collections.Generic;
using System.Windows;
using Microsoft.Data.SqlClient;
using Semana3.Data;
using Semana3.Models;

namespace Semana3.Views
{
    public partial class ReservasObjetosWindow : Window
    {
        public ReservasObjetosWindow()
        {
            InitializeComponent();
            CargarReservas(null);
        }

        private void CargarReservas(DateTime? fecha)
        {
            List<Reserva> lista = new List<Reserva>();

            using (var conexion = Conexion.ObtenerConexion())
            {
                conexion.Open();

                string query = @"SELECT r.ReservaId, r.AulaId, r.UsuarioId, r.Fecha, r.Hora, r.Motivo,
                                         a.Nombre AS NombreAula, u.NombreCompleto AS NombreUsuario
                                  FROM Reservas r
                                  INNER JOIN Aulas a ON r.AulaId = a.AulaId
                                  INNER JOIN Usuarios u ON r.UsuarioId = u.UsuarioId";

                if (fecha.HasValue)
                    query += " WHERE r.Fecha = @fecha";

                using (var cmd = new SqlCommand(query, conexion))
                {
                    if (fecha.HasValue)
                        cmd.Parameters.AddWithValue("@fecha", fecha.Value.Date);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Reserva
                            {
                                ReservaId = reader.GetInt32(0),
                                AulaId = reader.GetInt32(1),
                                UsuarioId = reader.GetInt32(2),
                                Fecha = reader.GetDateTime(3),
                                Hora = reader.GetTimeSpan(4),
                                Motivo = reader.IsDBNull(5) ? "" : reader.GetString(5),
                                NombreAula = reader.GetString(6),
                                NombreUsuario = reader.GetString(7)
                            });
                        }
                    }
                }
            }

            dgReservas.ItemsSource = lista;
        }

        private void BtnBuscar_Click(object sender, RoutedEventArgs e)
        {
            if (DateTime.TryParse(txtFecha.Text.Trim(), out DateTime fecha))
            {
                CargarReservas(fecha);
            }
            else
            {
                MessageBox.Show("Ingresa una fecha válida, ejemplo: 2026-09-10");
            }
        }

        private void BtnMostrarTodas_Click(object sender, RoutedEventArgs e)
        {
            txtFecha.Text = "";
            CargarReservas(null);
        }
    }
}