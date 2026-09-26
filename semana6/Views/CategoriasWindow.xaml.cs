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

using System.Windows;
using Semana6.Data.Repositories;

namespace semana6.Views
{
    public partial class CategoriasWindow : Window
    {
        private readonly CategoriaRepository _repository;

        public CategoriasWindow()
        {
            InitializeComponent();

            _repository = new CategoriaRepository();

            Loaded += CategoriasWindow_Loaded;
        }

        private async void CategoriasWindow_Loaded(
            object sender,
            RoutedEventArgs e)
        {
            try
            {
                dgCategorias.ItemsSource =
                    await _repository.ListarAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al cargar categorías:\n\n" + ex.Message,
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }
}
