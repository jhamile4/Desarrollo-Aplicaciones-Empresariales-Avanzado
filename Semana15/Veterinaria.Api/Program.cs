using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Veterinaria.Datos.Models;
using Veterinaria.Datos.Repositorios;
using Veterinaria.Negocio.Mapeos;
using Veterinaria.Negocio.Seguridad;
using Veterinaria.Negocio.Servicios;

var builder = WebApplication.CreateBuilder(args);

// Configuración JWT: Emisor, Audiencia y MinutosExpiracion en appsettings.json;
// la Clave (mínimo 32 caracteres) solo en User Secrets: dotnet user-secrets set "Jwt:Clave" "..."
var jwt = builder.Configuration.GetSection("Jwt").Get<JwtOpciones>() ?? new JwtOpciones();
if (string.IsNullOrWhiteSpace(jwt.Clave) || jwt.Clave.Length < 32)
    throw new InvalidOperationException(
        "Falta la clave Jwt:Clave (mínimo 32 caracteres). Configúrala con dotnet user-secrets.");
builder.Services.AddSingleton(jwt);

// Add services to the container.

builder.Services.AddControllers();

// OpenAPI con el esquema Bearer para que Swagger muestre el botón Authorize
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
        document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Pega aquí el token obtenido en POST /api/Auth/login (sin la palabra Bearer)."
        };
        return Task.CompletedTask;
    });

    // Candado solo en los endpoints protegidos ([Authorize] sin [AllowAnonymous])
    options.AddOperationTransformer((operation, context, cancellationToken) =>
    {
        var metadata = context.Description.ActionDescriptor.EndpointMetadata;
        var protegido = metadata.OfType<IAuthorizeData>().Any() && !metadata.OfType<IAllowAnonymous>().Any();
        if (protegido)
        {
            operation.Security ??= new List<OpenApiSecurityRequirement>();
            operation.Security.Add(new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference("Bearer", context.Document)] = new List<string>()
            });
        }
        return Task.CompletedTask;
    });
});

// DbContext: la cadena de conexión NO está en el repositorio (User Secrets en local,
// variable de entorno ConnectionStrings__VeterinariaDB en la nube).
// Semana 15: en la nube la base gratuita es PostgreSQL, así que el proveedor se elige por
// configuración (BaseDatos:Proveedor = SqlServer | PostgreSql). La lógica de la API no cambia.
var proveedorBd = builder.Configuration["BaseDatos:Proveedor"] ?? "SqlServer";
var cadenaConexion = builder.Configuration.GetConnectionString("VeterinariaDB");
builder.Services.AddDbContext<VeterinariaContext>(options =>
{
    if (proveedorBd.Equals("PostgreSql", StringComparison.OrdinalIgnoreCase))
        options.UseNpgsql(cadenaConexion);
    else
        options.UseSqlServer(cadenaConexion);
});

// Repositorios (capa Datos)
builder.Services.AddScoped<IPropietarioRepositorio, PropietarioRepositorio>();
builder.Services.AddScoped<IMascotaRepositorio, MascotaRepositorio>();
builder.Services.AddScoped<IUsuarioRepositorio, UsuarioRepositorio>();

// Servicios (capa Negocio)
builder.Services.AddScoped<IPropietarioServicio, PropietarioServicio>();
builder.Services.AddScoped<IMascotaServicio, MascotaServicio>();
builder.Services.AddScoped<IAuthServicio, AuthServicio>();

// AutoMapper con el MappingProfile de la capa Negocio
builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());

// Autenticación JWT: valida emisor, audiencia, vencimiento y firma
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;   // conserva los nombres sub, email y role del token
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwt.Emisor,
            ValidateAudience = true,
            ValidAudience = jwt.Audiencia,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(30),
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Clave)),
            NameClaimType = "email",
            RoleClaimType = "role"
        };
    });
builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    // Swagger UI sobre el documento OpenAPI (/openapi/v1.json)
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "Veterinaria API v1");
        options.DocumentTitle = "Veterinaria API";
    });
}

app.UseHttpsRedirection();

// Primero se identifica al usuario (token) y luego se revisan sus permisos
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
