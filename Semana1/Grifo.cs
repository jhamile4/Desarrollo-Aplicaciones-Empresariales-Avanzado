using System;
using System.Collections.Generic;
using System.Text;

namespace semana1
{
    public class Grifo
    {
        private List<Usuario> usuarios;

        public Grifo()
        {
            usuarios = new List<Usuario>();
        }

        public void AgregarUsuario(Usuario usuario)
        {
            usuarios.Add(usuario);
        }

        public void MostrarUsuarios()
        {
            if (usuarios.Count == 0)
            {
                Console.WriteLine("No hay usuarios registrados.");
                return;
            }

            Console.WriteLine("\n===== LISTA DE USUARIOS =====");

            int contador = 1;

            foreach (Usuario usuario in usuarios)
            {
                Console.WriteLine($"\nUsuario #{contador}");
                usuario.MostrarInformacion();

                contador++;
            }
        }
    }
}
