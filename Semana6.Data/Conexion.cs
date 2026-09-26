using System;
using System.Collections.Generic;
using System.Text;

using System.Configuration;

namespace Semana6.Data
{
    public static class Conexion
    {
        public static string Cadena
        {
            get
            {
                return ConfigurationManager
                    .ConnectionStrings["NeptunoDB"]
                    .ConnectionString;
            }
        }
    }
}