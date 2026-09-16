using System;
using System.Collections.Generic;
using System.Text;

namespace Semana5.Models
{
    public class Proveedor
    {
        public int ProveedorID { get; set; }
        public string CompaniaNombre { get; set; }
        public string NombreContacto { get; set; }
        public string Ciudad { get; set; }
        public string Telefono { get; set; }
        public bool Activo { get; set; }
    }
}
