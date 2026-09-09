using semana1;
using System;

namespace ExamenPOO
{
    class Program
    {
        static void Main(string[] args)
        {
            Grifo grifo = new Grifo();

            int opcion;

            do
            {
                Console.Clear();

                Console.WriteLine("================================");
                Console.WriteLine("          GRIFO");
                Console.WriteLine("================================");
                Console.WriteLine("1. Mostrar información de usuarios");
                Console.WriteLine("2. Agregar cliente");
                Console.WriteLine("3. Agregar empleado");
                Console.WriteLine("4. Salir");
                Console.WriteLine("================================");
                Console.Write("Seleccione una opción: ");

                opcion = int.Parse(Console.ReadLine());

                switch (opcion)
                {
                    case 1:
                        grifo.MostrarUsuarios();
                        Pausar();
                        break;

                    case 2:
                        AgregarCliente(grifo);
                        break;

                    case 3:
                        AgregarEmpleado(grifo);
                        break;

                    case 4:
                        Console.WriteLine("Saliendo del programa...");
                        break;

                    default:
                        Console.WriteLine("Opción no válida.");
                        Pausar();
                        break;
                }

            } while (opcion != 4);
        }

        static void AgregarCliente(Grifo grifo)
        {
            Console.Clear();

            Console.WriteLine("===== AGREGAR CLIENTE =====");

            Console.Write("Nombre: ");
            string nombre = Console.ReadLine();

            Console.Write("Saldo: ");
            double saldo = double.Parse(Console.ReadLine());

            Console.Write("Litros cargados: ");
            double litros = double.Parse(Console.ReadLine());

            Cliente cliente = new Cliente(nombre, saldo, litros);

            grifo.AgregarUsuario(cliente);

            Console.WriteLine("\nCliente agregado correctamente.");
            Pausar();
        }

        static void AgregarEmpleado(Grifo grifo)
        {
            Console.Clear();

            Console.WriteLine("===== AGREGAR EMPLEADO =====");

            Console.Write("Nombre: ");
            string nombre = Console.ReadLine();

            Console.Write("Saldo: ");
            double saldo = double.Parse(Console.ReadLine());

            Console.Write("Sueldo: ");
            double sueldo = double.Parse(Console.ReadLine());

            Empleado empleado = new Empleado(nombre, saldo, sueldo);

            grifo.AgregarUsuario(empleado);

            Console.WriteLine("\nEmpleado agregado correctamente.");
            Pausar();
        }

        static void Pausar()
        {
            Console.WriteLine("\nPresione ENTER para continuar...");
            Console.ReadLine();
        }
    }
}
