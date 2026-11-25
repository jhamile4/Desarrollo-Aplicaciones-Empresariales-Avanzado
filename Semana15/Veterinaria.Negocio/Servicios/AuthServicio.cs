using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using Veterinaria.Datos.Repositorios;
using Veterinaria.Entidades;
using Veterinaria.Entidades.Dtos;
using Veterinaria.Negocio.Excepciones;
using Veterinaria.Negocio.Seguridad;

namespace Veterinaria.Negocio.Servicios;

// Registro (hash BCrypt + rol asignado por el servidor) y login (genera el JWT).
public class AuthServicio : IAuthServicio
{
    // Todo usuario nuevo entra como Recepcionista; el rol Veterinario lo asigna la clínica (UPDATE en la BD).
    private const string RolPorDefecto = Roles.Recepcionista;

    private readonly IUsuarioRepositorio _usuarios;
    private readonly IMapper _mapper;
    private readonly JwtOpciones _jwt;

    public AuthServicio(IUsuarioRepositorio usuarios, IMapper mapper, JwtOpciones jwt)
    {
        _usuarios = usuarios;
        _mapper = mapper;
        _jwt = jwt;
    }

    public async Task<UsuarioDto> RegistrarAsync(RegistroDto dto)
    {
        var correo = dto.Correo.Trim().ToLowerInvariant();

        // Regla: el correo es único.
        if (await _usuarios.ExisteCorreoAsync(correo))
            throw new ReglaNegocioException($"El correo {correo} ya está registrado.");

        var usuario = _mapper.Map<Usuario>(dto);
        usuario.Correo = correo;
        usuario.ClaveHash = BCrypt.Net.BCrypt.HashPassword(dto.Clave);   // nunca se guarda la clave en texto
        usuario.Rol = RolPorDefecto;                                        // el rol lo decide el servidor

        await _usuarios.AgregarAsync(usuario);
        return _mapper.Map<UsuarioDto>(usuario);
    }

    // Devuelve null tanto si el correo no existe como si la clave es incorrecta (el controlador responde 401).
    public async Task<TokenDto?> LoginAsync(LoginDto dto)
    {
        var usuario = await _usuarios.ObtenerPorCorreoAsync(dto.Correo.Trim().ToLowerInvariant());
        if (usuario == null || !BCrypt.Net.BCrypt.Verify(dto.Clave, usuario.ClaveHash))
            return null;

        return GenerarToken(usuario);
    }

    private TokenDto GenerarToken(Usuario usuario)
    {
        var expira = DateTime.UtcNow.AddMinutes(_jwt.MinutosExpiracion);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, usuario.IdUsuario.ToString()),
            new(JwtRegisteredClaimNames.Email, usuario.Correo),
            new("role", usuario.Rol),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        var credenciales = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Clave)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwt.Emisor,
            audience: _jwt.Audiencia,
            claims: claims,
            expires: expira,
            signingCredentials: credenciales);

        return new TokenDto
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            Expira = expira
        };
    }
}
