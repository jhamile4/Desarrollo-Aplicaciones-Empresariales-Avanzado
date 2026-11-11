namespace Veterinaria.Negocio.Excepciones;

// Se lanza cuando se incumple una regla de negocio; el controlador la convierte en 409 Conflict.
public class ReglaNegocioException : Exception
{
    public ReglaNegocioException(string mensaje) : base(mensaje)
    {
    }
}
