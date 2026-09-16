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

using System.Collections.Generic;
using System.Windows;
using Microsoft.Data.SqlClient;
using Semana3.Data;
using Semana3.Models;

namespace Semana3.Views
{
    public partial class AulasObjetosWindow : Window
    {
        public AulasObjetosWindow()
        {
            InitializeComponent();
            CargarAulas("");
        }

        private void CargarAulas(string filtroNombre)
        {
            List<Aula> lista = new List<Aula>();

            using (var conexion = Conexion.ObtenerConexion())
            {
                conexion.Open();

                string query = "SELECT AulaId, Nombre, Capacidad FROM Aulas WHERE Nombre LIKE @nombre";
                using (var cmd = new SqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@nombre", "%" + filtroNombre + "%");

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
            }

            dgAulas.ItemsSource = lista;
        }

        private void BtnBuscar_Click(object sender, RoutedEventArgs e)
        {
            CargarAulas(txtBuscar.Text.Trim());
        }

        private void BtnMostrarTodas_Click(object sender, RoutedEventArgs e)
        {
            txtBuscar.Text = "";
            CargarAulas("");
        }
    }
}