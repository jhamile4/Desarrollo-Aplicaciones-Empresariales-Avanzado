using System;
using System.Collections.Generic;
using System.Text;

using System;

namespace Semana5.Models
{
    public class DetallePedido
    {
        public int PedidoID { get; set; }
        public DateTime FechaPedido { get; set; }
        public string Destinatario { get; set; }
        public string CiudadDestino { get; set; }
        public int ProductoID { get; set; }
        public decimal PrecioUnidad { get; set; }
        public short Cantidad { get; set; }
        public float Descuento { get; set; }
        public decimal SubTotal { get; set; }
    }
}