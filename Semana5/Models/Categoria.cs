using System;
using System.Collections.Generic;
using System.Text;

namespace Semana5.Models
{
    public class Categoria
    {
        public int CategoriaID { get; set; }
        public string NombreCategoria { get; set; }
        public string Descripcion { get; set; }
        public bool Activo { get; set; }
    }
}