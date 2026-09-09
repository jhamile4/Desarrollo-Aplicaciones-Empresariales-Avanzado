using System;
using System.Collections.Generic;
using System.Text;

namespace semana1
{
    public class Cliente : Usuario
    {
        public double LitrosCargados { get; set; }

        public Cliente(string nombre, double saldo, double litrosCargados)
            : base(nombre, saldo)
        {
            LitrosCargados = litrosCargados;
        }

        public override void MostrarInformacion()
        {
            Console.WriteLine($"Nombre: {Nombre}");
            Console.WriteLine($"Saldo: {Saldo}");
            Console.WriteLine($"Litros cargados: {LitrosCargados}");
        }
    }
}
