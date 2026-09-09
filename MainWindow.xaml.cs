using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Wpfseman2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void BtnIngresar_Click(object sender, RoutedEventArgs e)
        {
            string usuario = txtUsuario.Text;
            string password = txtPassword.Password;

            if (usuario == "admin" && password == "1234")
            {
                MenuWindow1 menu = new MenuWindow1();
                menu.Show();
                this.Close();
            }
            else
            {
                txtMensaje.Foreground = Brushes.Red;
                txtMensaje.Text = "Usuario o contraseña incorrectos.";
            }
        }
    }
}