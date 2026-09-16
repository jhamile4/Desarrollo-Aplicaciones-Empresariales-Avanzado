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
using Semana5.Data;
using Semana5.Models;

namespace Semana5.Views
{
    public partial class PedidosWindow : Window
    {
        private int pedidoIdSeleccionado = 0;

        public PedidosWindow()
        {
            InitializeComponent();
            CargarPedidos();
        }

        private void CargarPedidos()
        {
            List<Pedido> lista = new List<Pedido>();
            using (var conexion = Conexion.ObtenerConexion())
            {
                conexion.Open();
                using (var cmd = new SqlCommand("sp_Pedido_Listar", conexion))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Pedido
                            {
                                PedidoID = reader.GetInt32(0),
                                ClienteID = reader.GetInt32(1),
                                EmpleadoID = reader.GetInt32(2),
                                FechaPedido = reader.GetDateTime(3),
                                FechaRequerida = reader.GetDateTime(4),
                                FechaEnvio = reader.IsDBNull(5) ? (DateTime?)null : reader.GetDateTime(5),
                                TransportistaID = reader.GetInt32(6),
                                Destinatario = reader.IsDBNull(7) ? "" : reader.GetString(7),
                                CiudadDestino = reader.IsDBNull(8) ? "" : reader.GetString(8),
                                PaisDestino = reader.IsDBNull(9) ? "" : reader.GetString(9),
                                Activo = reader.GetBoolean(10)
                            });
                        }
                    }
                }
            }
            dgPedidos.ItemsSource = lista;
        }

        private void DgPedidos_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (dgPedidos.SelectedItem is Pedido p)
            {
                pedidoIdSeleccionado = p.PedidoID;
                txtClienteID.Text = p.ClienteID.ToString();
                txtEmpleadoID.Text = p.EmpleadoID.ToString();
                txtFechaPedido.Text = p.FechaPedido.ToString("yyyy-MM-dd");
                txtFechaRequerida.Text = p.FechaRequerida.ToString("yyyy-MM-dd");
                txtTransportistaID.Text = p.TransportistaID.ToString();
                txtDestinatario.Text = p.Destinatario;
            }
        }

        private void BtnNuevo_Click(object sender, RoutedEventArgs e)
        {
            pedidoIdSeleccionado = 0;
            txtClienteID.Text = "";
            txtEmpleadoID.Text = "";
            txtFechaPedido.Text = "";
            txtFechaRequerida.Text = "";
            txtTransportistaID.Text = "";
            txtDestinatario.Text = "";
            dgPedidos.SelectedItem = null;
        }

        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(txtClienteID.Text, out int clienteId) ||
                !int.TryParse(txtEmpleadoID.Text, out int empleadoId) ||
                !int.TryParse(txtTransportistaID.Text, out int transportistaId) ||
                !DateTime.TryParse(txtFechaPedido.Text, out DateTime fechaPedido) ||
                !DateTime.TryParse(txtFechaRequerida.Text, out DateTime fechaRequerida))
            {
                MessageBox.Show("Revisa que todos los campos numéricos y fechas sean válidos.");
                return;
            }

            using (var conexion = Conexion.ObtenerConexion())
            {
                conexion.Open();

                if (pedidoIdSeleccionado == 0)
                {
                    using (var cmd = new SqlCommand("sp_Pedido_Insertar", conexion))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ClienteID", clienteId);
                        cmd.Parameters.AddWithValue("@EmpleadoID", empleadoId);
                        cmd.Parameters.AddWithValue("@FechaPedido", fechaPedido);
                        cmd.Parameters.AddWithValue("@FechaRequerida", fechaRequerida);
                        cmd.Parameters.AddWithValue("@FechaEnvio", (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@TransportistaID", transportistaId);
                        cmd.Parameters.AddWithValue("@Destinatario", txtDestinatario.Text.Trim());
                        cmd.Parameters.AddWithValue("@CiudadDestino", "");
                        cmd.Parameters.AddWithValue("@PaisDestino", "");
                        cmd.ExecuteNonQuery();
                    }
                    MessageBox.Show("Pedido creado correctamente.");
                }
                else
                {
                    using (var cmd = new SqlCommand("sp_Pedido_Actualizar", conexion))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@PedidoID", pedidoIdSeleccionado);
                        cmd.Parameters.AddWithValue("@ClienteID", clienteId);
                        cmd.Parameters.AddWithValue("@EmpleadoID", empleadoId);
                        cmd.Parameters.AddWithValue("@FechaPedido", fechaPedido);
                        cmd.Parameters.AddWithValue("@FechaRequerida", fechaRequerida);
                        cmd.Parameters.AddWithValue("@FechaEnvio", (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@TransportistaID", transportistaId);
                        cmd.Parameters.AddWithValue("@Destinatario", txtDestinatario.Text.Trim());
                        cmd.Parameters.AddWithValue("@CiudadDestino", "");
                        cmd.Parameters.AddWithValue("@PaisDestino", "");
                        cmd.ExecuteNonQuery();
                    }
                    MessageBox.Show("Pedido actualizado correctamente.");
                }
            }

            BtnNuevo_Click(sender, e);
            CargarPedidos();
        }

        private void BtnEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (pedidoIdSeleccionado == 0)
            {
                MessageBox.Show("Selecciona un pedido de la lista para eliminar.");
                return;
            }

            var confirmar = MessageBox.Show("¿Seguro que deseas eliminar este pedido?",
                                             "Confirmar", MessageBoxButton.YesNo);
            if (confirmar != MessageBoxResult.Yes) return;

            using (var conexion = Conexion.ObtenerConexion())
            {
                conexion.Open();
                using (var cmd = new SqlCommand("sp_Pedido_Eliminar", conexion))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@PedidoID", pedidoIdSeleccionado);
                    cmd.ExecuteNonQuery();
                }
            }

            BtnNuevo_Click(sender, e);
            CargarPedidos();
        }

        private void BtnReporte_Click(object sender, RoutedEventArgs e)
        {
            if (!DateTime.TryParse(txtDesde.Text, out DateTime desde) ||
                !DateTime.TryParse(txtHasta.Text, out DateTime hasta))
            {
                MessageBox.Show("Ingresa fechas válidas para el reporte (aaaa-mm-dd).");
                return;
            }

            List<DetallePedido> lista = new List<DetallePedido>();
            using (var conexion = Conexion.ObtenerConexion())
            {
                conexion.Open();
                using (var cmd = new SqlCommand("sp_DetallePedidos_ListarPorFecha", conexion))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@FechaInicio", desde);
                    cmd.Parameters.AddWithValue("@FechaFin", hasta);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new DetallePedido
                            {
                                PedidoID = reader.GetInt32(0),
                                FechaPedido = reader.GetDateTime(1),
                                Destinatario = reader.IsDBNull(2) ? "" : reader.GetString(2),
                                CiudadDestino = reader.IsDBNull(3) ? "" : reader.GetString(3),
                                ProductoID = reader.GetInt32(4),
                                PrecioUnidad = reader.GetDecimal(5),
                                Cantidad = reader.GetInt16(6),
                                Descuento = (float)reader.GetDecimal(7),
                                SubTotal = reader.GetDecimal(8)
                            });
                        }
                    }
                }
            }
            dgReporte.ItemsSource = lista;
        }
    }
}