using System;
using System.Collections.Generic;
using System.Text;

using System;

namespace Semana3.Models
{
    public class Reserva
    {
        public int ReservaId { get; set; }
        public int AulaId { get; set; }
        public int UsuarioId { get; set; }
        public DateTime Fecha { get; set; }
        public TimeSpan Hora { get; set; }
        public string Motivo { get; set; }

        // Campos extra para mostrar en el DataGrid (no están en la tabla, pero ayudan a leer)
        public string NombreAula { get; set; }
        public string NombreUsuario { get; set; }
    }
}