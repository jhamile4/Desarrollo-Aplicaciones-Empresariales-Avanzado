using System;
using System.Collections.Generic;
using System.Text;

namespace semana1
{
    public class Empleado : Usuario
    {
        public double Sueldo { get; set; }

        public Empleado(string nombre, double saldo, double sueldo)
            : base(nombre, saldo)
        {
            Sueldo = sueldo;
        }

        public override void MostrarInformacion()
        {
            Console.WriteLine($"Nombre: {Nombre}");
            Console.WriteLine($"Saldo: {Saldo}");
            Console.WriteLine($"Sueldo: {Sueldo}");
        }
    }
}
