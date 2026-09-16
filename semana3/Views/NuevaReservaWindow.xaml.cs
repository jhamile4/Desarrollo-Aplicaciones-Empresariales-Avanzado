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
    public partial class NuevaReservaWindow : Window
    {
        public NuevaReservaWindow()
        {
            InitializeComponent();
            CargarAulas();
        }

        private void CargarAulas()
        {
            List<Aula> lista = new List<Aula>();

            using (var conexion = Conexion.ObtenerConexion())
            {
                conexion.Open();
                string query = "SELECT AulaId, Nombre, Capacidad FROM Aulas";
                using (var cmd = new SqlCommand(query, conexion))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lista.Add(new Aula
                        {
                            AulaId = reader.GetInt32(0),
                            Nombre = reader.GetString(1),
                            Capacidad = reader.GetInt32(2)
                        });
                    }
                }
            }

            cbAulas.ItemsSource = lista;
            if (lista.Count > 0) cbAulas.SelectedIndex = 0;
        }

        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            txtMensaje.Foreground = System.Windows.Media.Brushes.Red;

            if (cbAulas.SelectedValue == null)
            {
                txtMensaje.Text = "Selecciona un aula.";
                return;
            }

            if (!DateTime.TryParse(txtFecha.Text.Trim(), out DateTime fecha))
            {
                txtMensaje.Text = "Fecha inválida. Usa el formato aaaa-mm-dd.";
                return;
            }

            if (!TimeSpan.TryParse(txtHora.Text.Trim(), out TimeSpan hora))
            {
                txtMensaje.Text = "Hora inválida. Usa el formato hh:mm.";
                return;
            }

            int aulaId = (int)cbAulas.SelectedValue;
            string motivo = txtMotivo.Text.Trim();
            int usuarioId = SesionActual.UsuarioId; // ver Paso 16 más abajo

            using (var conexion = Conexion.ObtenerConexion())
            {
                conexion.Open();

                // 1. Verificar que no exista ya una reserva igual (misma aula, fecha y hora)
                string queryExiste = "SELECT COUNT(*) FROM Reservas WHERE AulaId = @aulaId AND Fecha = @fecha AND Hora = @hora";
                using (var cmdExiste = new SqlCommand(queryExiste, conexion))
                {
                    cmdExiste.Parameters.AddWithValue("@aulaId", aulaId);
                    cmdExiste.Parameters.AddWithValue("@fecha", fecha.Date);
                    cmdExiste.Parameters.AddWithValue("@hora", hora);

                    int existe = (int)cmdExiste.ExecuteScalar();

                    if (existe > 0)
                    {
                        txtMensaje.Text = "Ya existe una reserva para esa aula, fecha y hora.";
                        return;
                    }
                }

                // 2. Insertar la nueva reserva
                string queryInsert = @"INSERT INTO Reservas (AulaId, UsuarioId, Fecha, Hora, Motivo)
                                        VALUES (@aulaId, @usuarioId, @fecha, @hora, @motivo)";
                using (var cmdInsert = new SqlCommand(queryInsert, conexion))
                {
                    cmdInsert.Parameters.AddWithValue("@aulaId", aulaId);
                    cmdInsert.Parameters.AddWithValue("@usuarioId", usuarioId);
                    cmdInsert.Parameters.AddWithValue("@fecha", fecha.Date);
                    cmdInsert.Parameters.AddWithValue("@hora", hora);
                    cmdInsert.Parameters.AddWithValue("@motivo", (object)motivo ?? DBNull.Value);

                    cmdInsert.ExecuteNonQuery();
                }
            }

            txtMensaje.Foreground = System.Windows.Media.Brushes.Green;
            txtMensaje.Text = "¡Reserva guardada correctamente!";
        }
    }
}