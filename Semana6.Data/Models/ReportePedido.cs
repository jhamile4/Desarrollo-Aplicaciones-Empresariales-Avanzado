using System;
using System.Collections.Generic;
using System.Text;

namespace Semana6.Data.Models
{
    public class ReportePedido
    {
        public int PedidoID { get; set; }
        public DateTime? FechaPedido { get; set; }
        public string? Destinatario { get; set; }
        public string? CiudadDestino { get; set; }
        public string? PaisDestino { get; set; }
        public string? NombreProducto { get; set; }
        public short Cantidad { get; set; }
        public decimal PrecioUnidad { get; set; }
        public decimal Descuento { get; set; }
    }
}
