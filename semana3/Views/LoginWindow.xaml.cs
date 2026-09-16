using semana3;
using Semana3.Data;
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

namespace Semana3.Views
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void btnIngresar_Click(object sender, RoutedEventArgs e)
        {
            string usuario = txtUsuario.Text.Trim();
            string password = txtPassword.Password.Trim();

            if (string.IsNullOrEmpty(usuario) || string.IsNullOrEmpty(password))
            {
                txtError.Text = "Debes ingresar usuario y contraseña.";
                return;
            }

            using (var conexion = Conexion.ObtenerConexion())
            {
                conexion.Open();

                string query = "SELECT COUNT(*) FROM Usuarios WHERE Username = @user AND Password = @pass";
                using (var cmd = new Microsoft.Data.SqlClient.SqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@user", usuario);
                    cmd.Parameters.AddWithValue("@pass", password);

                    int count = (int)cmd.ExecuteScalar();

                    if (count > 0)
                    {
                        // Login correcto: abrir ventana principal
                        // Obtener el UsuarioId antes de abrir la ventana principal
                        string queryId = "SELECT UsuarioId FROM Usuarios WHERE Username = @user";
                        using (var cmdId = new Microsoft.Data.SqlClient.SqlCommand(queryId, conexion))
                        {
                            cmdId.Parameters.AddWithValue("@user", usuario);
                            Semana3.Data.SesionActual.UsuarioId = (int)cmdId.ExecuteScalar();
                            Semana3.Data.SesionActual.NombreUsuario = usuario;
                        }

                        // Login correcto: abrir ventana principal
                        MainWindow mainWindow = new MainWindow();
                        mainWindow.Show();
                        this.Close();
                    }
                    else
                    {
                        txtError.Text = "Usuario o contraseña incorrectos.";
                    }
                }
            }
        }
    }
}
