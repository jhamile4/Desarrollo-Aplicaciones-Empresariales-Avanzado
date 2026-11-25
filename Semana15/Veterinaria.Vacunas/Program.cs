using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Veterinaria.Vacunas;

// Microservicio 2: catálogo de vacunas de la clínica (datos en memoria).
// Valida el mismo JWT que la API de la Semana 14 (misma clave, emisor y audiencia)
// y exige autenticación en todos sus endpoints.
var builder = WebApplication.CreateBuilder(args);

var clave = builder.Configuration["Jwt:Clave"];
if (string.IsNullOrWhiteSpace(clave) || clave.Length < 32)
    throw new InvalidOperationException(
        "Falta la clave Jwt:Clave (mínimo 32 caracteres). Configúrala con dotnet user-secrets o una variable de entorno.");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Emisor"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audiencia"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(30),
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(clave)),
            NameClaimType = "email",
            RoleClaimType = "role"
        };
    });
builder.Services.AddAuthorization();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

// Todos los endpoints exigen token (RequireAuthorization en el grupo)
var vacunas = app.MapGroup("/api/vacunas").RequireAuthorization();

// GET /api/vacunas?especie=Perro
vacunas.MapGet("/", (string? especie) =>
{
    var lista = string.IsNullOrWhiteSpace(especie)
        ? CatalogoVacunas.Lista
        : CatalogoVacunas.Lista.Where(v => v.Especie.Equals(especie, StringComparison.OrdinalIgnoreCase)).ToList();
    return Results.Ok(lista);
});

// GET /api/vacunas/{id}
vacunas.MapGet("/{id:int}", (int id) =>
{
    var vacuna = CatalogoVacunas.Lista.FirstOrDefault(v => v.Id == id);
    return vacuna is null
        ? Results.NotFound(new { mensaje = $"No existe la vacuna {id}." })
        : Results.Ok(vacuna);
});

// Raíz informativa (pública) para comprobar que el servicio está vivo en la nube
app.MapGet("/", () => Results.Ok(new { servicio = "Veterinaria.Vacunas", estado = "activo" }));

app.Run();
