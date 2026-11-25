using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

// API Gateway: único punto de entrada para el cliente.
// Reenvía a la Veterinaria API (S14) y al microservicio de Vacunas según ocelot.json.
var builder = WebApplication.CreateBuilder(args);

// En local se usa ocelot.json; si existe ocelot.{Entorno}.json (por ejemplo ocelot.Production.json
// con los hosts públicos de la nube), se usa ese.
var archivoOcelot = File.Exists(Path.Combine(builder.Environment.ContentRootPath, $"ocelot.{builder.Environment.EnvironmentName}.json"))
    ? $"ocelot.{builder.Environment.EnvironmentName}.json"
    : "ocelot.json";
builder.Configuration.AddJsonFile(archivoOcelot, optional: false, reloadOnChange: true);

// El gateway valida el JWT (misma clave, emisor y audiencia que los servicios) antes de reenviar.
var clave = builder.Configuration["Jwt:Clave"];
if (string.IsNullOrWhiteSpace(clave) || clave.Length < 32)
    throw new InvalidOperationException(
        "Falta la clave Jwt:Clave (mínimo 32 caracteres). Configúrala con dotnet user-secrets o una variable de entorno.");

builder.Services
    .AddAuthentication()
    .AddJwtBearer("Bearer", options =>
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
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(clave))
        };
    });

builder.Services.AddOcelot(builder.Configuration);

var app = builder.Build();

app.Logger.LogInformation("Gateway usando la configuración {Archivo}", archivoOcelot);

await app.UseOcelot();

app.Run();
