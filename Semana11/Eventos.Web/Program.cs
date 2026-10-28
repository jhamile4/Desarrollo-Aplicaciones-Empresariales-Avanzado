using Eventos.Web.Data;
using Eventos.Web.Models;
using Eventos.Web.Repositorios;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Contexto de EF Core: la cadena de conexión se lee de appsettings.json
builder.Services.AddDbContext<EventosContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("EventosDB")));

// Repositorios (una instancia por petición HTTP)
builder.Services.AddScoped<EventoRepositorio>();
builder.Services.AddScoped<AsistenteRepositorio>();
builder.Services.AddScoped<InscripcionRepositorio>();

var app = builder.Build();

// Datos iniciales: se cargan al iniciar, solo si las tablas están vacías.
// La base ya debe existir (migraciones aplicadas con Update-Database / dotnet ef database update).
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<EventosContext>();

    if (!await context.Categorias.AnyAsync())
    {
        context.Categorias.AddRange(
            new Categoria { Nombre = "Tecnología" },
            new Categoria { Nombre = "Música" },
            new Categoria { Nombre = "Negocios" });
        await context.SaveChangesAsync();
    }

    if (!await context.Eventos.AnyAsync())
    {
        var categorias = await context.Categorias.ToDictionaryAsync(c => c.Nombre, c => c.Id);
        var hoy = DateTime.Today;

        context.Eventos.AddRange(
            new Evento { Titulo = "Conferencia de Inteligencia Artificial", Fecha = hoy.AddDays(10).AddHours(9), Lugar = "Auditorio Tecsup", Aforo = 120, Precio = 50.00m, CategoriaId = categorias["Tecnología"] },
            new Evento { Titulo = "Taller de .NET y EF Core", Fecha = hoy.AddDays(20).AddHours(15), Lugar = "Laboratorio 301", Aforo = 3, Precio = 0.00m, CategoriaId = categorias["Tecnología"] },
            new Evento { Titulo = "Concierto Acústico de Primavera", Fecha = hoy.AddDays(30).AddHours(19), Lugar = "Explanada principal", Aforo = 300, Precio = 35.00m, CategoriaId = categorias["Música"] },
            new Evento { Titulo = "Foro de Emprendimiento 2026", Fecha = hoy.AddDays(-5).AddHours(10), Lugar = "Sala de conferencias B", Aforo = 80, Precio = 20.00m, CategoriaId = categorias["Negocios"] });
        await context.SaveChangesAsync();
    }

    if (!await context.Asistentes.AnyAsync())
    {
        context.Asistentes.AddRange(
            new Asistente { Nombres = "Ana Lucía Torres", Dni = "70123456", Correo = "ana.torres@correo.com" },
            new Asistente { Nombres = "Bruno Díaz Paredes", Dni = "45879632", Correo = "bruno.diaz@correo.com" },
            new Asistente { Nombres = "Carla Quispe Mamani", Dni = "71234890", Correo = "carla.quispe@correo.com" },
            new Asistente { Nombres = "Diego Flores Chávez", Dni = "08765432", Correo = "diego.flores@correo.com" },
            new Asistente { Nombres = "Elena Huamán Ríos", Dni = "72345619", Correo = "elena.huaman@correo.com" });
        await context.SaveChangesAsync();
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
