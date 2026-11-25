using System.ComponentModel.DataAnnotations;

namespace Veterinaria.Entidades.Dtos;

// Roles propios del negocio (los decide el servidor, nunca el cliente).
public static class Roles
{
    public const string Recepcionista = "Recepcionista";
    public const string Veterinario = "Veterinario";
}

// DTO de entrada para POST api/auth/login.
public class LoginDto
{
    [Required(ErrorMessage = "El correo es obligatorio.")]
    [EmailAddress(ErrorMessage = "El correo no tiene un formato válido.")]
    public string Correo { get; set; } = string.Empty;

    [Required(ErrorMessage = "La clave es obligatoria.")]
    public string Clave { get; set; } = string.Empty;
}

// DTO de entrada para POST api/auth/registro: no incluye el rol.
public class RegistroDto
{
    [Required(ErrorMessage = "El correo es obligatorio.")]
    [EmailAddress(ErrorMessage = "El correo no tiene un formato válido.")]
    [StringLength(120)]
    public string Correo { get; set; } = string.Empty;

    [Required(ErrorMessage = "La clave es obligatoria.")]
    [StringLength(50, MinimumLength = 8, ErrorMessage = "La clave debe tener entre 8 y 50 caracteres.")]
    public string Clave { get; set; } = string.Empty;
}

// DTO de salida del login.
public class TokenDto
{
    public string Token { get; set; } = string.Empty;
    public DateTime Expira { get; set; }
}

// DTO de salida del registro (nunca expone el hash de la clave).
public class UsuarioDto
{
    public int Id { get; set; }
    public string Correo { get; set; } = string.Empty;
    public string Rol { get; set; } = string.Empty;
    public DateTime FechaRegistro { get; set; }
}

// DTO de GET api/auth/perfil: se arma con los claims del token.
public class PerfilDto
{
    public string Id { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public string Rol { get; set; } = string.Empty;
    public DateTime Expira { get; set; }
}
