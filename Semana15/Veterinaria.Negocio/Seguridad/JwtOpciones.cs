namespace Veterinaria.Negocio.Seguridad;

// Se llena desde la sección "Jwt" de la configuración (appsettings.json + User Secrets).
public class JwtOpciones
{
    public string Clave { get; set; } = string.Empty;   // solo en User Secrets, nunca en el repositorio
    public string Emisor { get; set; } = string.Empty;
    public string Audiencia { get; set; } = string.Empty;
    public int MinutosExpiracion { get; set; } = 60;
}
