using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Veterinaria.Entidades.Dtos;
using Veterinaria.Negocio.Excepciones;
using Veterinaria.Negocio.Servicios;

namespace Veterinaria.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IAuthServicio _servicio;

    public AuthController(IAuthServicio servicio)
    {
        _servicio = servicio;
    }

    // POST api/auth/registro  (público; el rol lo asigna el servidor)
    [HttpPost("registro")]
    [AllowAnonymous]
    [ProducesResponseType<UsuarioDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UsuarioDto>> Registro(RegistroDto dto)
    {
        try
        {
            var usuario = await _servicio.RegistrarAsync(dto);
            return StatusCode(StatusCodes.Status201Created, usuario);
        }
        catch (ReglaNegocioException ex)
        {
            return Conflict(new { mensaje = ex.Message });
        }
    }

    // POST api/auth/login  (público)
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType<TokenDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<TokenDto>> Login(LoginDto dto)
    {
        var token = await _servicio.LoginAsync(dto);

        // Mismo 401 y mismo mensaje si el correo no existe o si la clave es incorrecta.
        if (token == null)
            return Unauthorized(new { mensaje = "Correo o clave incorrectos." });

        return Ok(token);
    }

    // GET api/auth/perfil  (solo autenticados: los datos salen de los claims del token, no de la BD)
    [HttpGet("perfil")]
    [Authorize]
    [ProducesResponseType<PerfilDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public ActionResult<PerfilDto> Perfil()
    {
        var exp = User.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Exp)?.Value;

        var perfil = new PerfilDto
        {
            Id = User.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value ?? string.Empty,
            Correo = User.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Email)?.Value ?? string.Empty,
            Rol = User.Claims.FirstOrDefault(c => c.Type == "role")?.Value ?? string.Empty,
            Expira = exp == null ? default : DateTimeOffset.FromUnixTimeSeconds(long.Parse(exp)).UtcDateTime
        };

        return Ok(perfil);
    }
}
