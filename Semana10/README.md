# Semana 10 – EF Core Database First: Hotel y reservas

Aplicación web **ASP.NET Core MVC (.NET 10)** de reservas de hotel con **Entity Framework Core – Database First** (sin migraciones ni procedimientos almacenados).

```
Semana10/
├── Script/
│   ├── HotelDB.sql          ← script del docente (crea la BD y los datos de prueba)
│   └── Observaciones.sql    ← ALTER TABLE del paso 13
└── Hotel.Web/
    ├── Controllers/         HomeController, HabitacionesController, ReservasController
    ├── Models/              ← GENERADO por scaffold (no se modifica a mano)
    ├── Repositorios/        HabitacionRepositorio, ReservaRepositorio (LINQ + SaveChangesAsync)
    ├── Views/               Home, Habitaciones, Reservas, Shared/_Layout
    ├── wwwroot/css/site.css ← diseño (tema claro moderno)
    ├── appsettings.json     ← cadena de conexión "HotelDB"
    └── Program.cs           ← AddDbContext + AddScoped de los repositorios
```

---

## 1. Cómo abrirlo y ejecutarlo (para tomar tus capturas)

1. **Base de datos** (ya la dejé creada, pero si quieres repetirlo):
   - En SQL Server Management Studio conéctate a `.\SQLEXPRESS`.
   - Abre y ejecuta `Script/HotelDB.sql` (F5).
   - Abre y ejecuta `Script/Observaciones.sql` (agrega la columna `Observaciones`).
   > Ojo: `HotelDB.sql` **borra y recrea** la base; si lo vuelves a ejecutar, ejecuta después `Observaciones.sql`.
2. **Ejecutar la app**:
   - Visual Studio: abre `Hotel.Web/Hotel.Web.csproj` y presiona **F5** (o ▶).
   - VS Code / terminal, dentro de `Semana10/Hotel.Web`:
     ```bash
     dotnet run
     ```
     y abre `http://localhost:5117`.

Las reservas de prueba tienen fechas **relativas al día en que se ejecutó el script**, por eso siempre hay reservas próximas para probar la disponibilidad.

---

## 2. Comando de scaffold utilizado

Herramienta (una sola vez):
```bash
dotnet tool install --global dotnet-ef
```

Generar el modelo (dentro de `Hotel.Web`):
```bash
dotnet ef dbcontext scaffold "Server=.\SQLEXPRESS;Database=HotelDB;Trusted_Connection=True;TrustServerCertificate=True;" Microsoft.EntityFrameworkCore.SqlServer --output-dir Models --context HotelContext
```

Regenerar después del `ALTER TABLE Reserva ADD Observaciones NVARCHAR(200) NULL;`:
```bash
dotnet ef dbcontext scaffold "Server=.\SQLEXPRESS;Database=HotelDB;Trusted_Connection=True;TrustServerCertificate=True;" Microsoft.EntityFrameworkCore.SqlServer --output-dir Models --context HotelContext --force
```

Equivalente en Visual Studio (Consola del Administrador de paquetes):
```
Scaffold-DbContext "Server=.\SQLEXPRESS;Database=HotelDB;Trusted_Connection=True;TrustServerCertificate=True;" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models -Context HotelContext -Force
```

Después de cada scaffold se **borra el método `OnConfiguring`** de `Models/HotelContext.cs` (trae la cadena escrita en el código). La cadena vive en `appsettings.json` y el contexto se registra en `Program.cs`:

```csharp
builder.Services.AddDbContext<HotelContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("HotelDB")));
builder.Services.AddScoped<HabitacionRepositorio>();
builder.Services.AddScoped<ReservaRepositorio>();
```

Paquetes NuGet: `Microsoft.EntityFrameworkCore.SqlServer`, `Microsoft.EntityFrameworkCore.Design`, `Microsoft.EntityFrameworkCore.Tools` (todos 10.0.13).

---

## 3. Qué se hizo y por qué (explicación paso a paso)

### Database First
La base de datos ya existe (la crea el script). EF Core **lee el esquema** y genera las clases: `Habitacion`, `Huesped`, `Reserva`, `TipoHabitacion` y el contexto `HotelContext`. No hay migraciones: si la BD cambia, se vuelve a generar con `--force`.

Las propiedades de navegación se llaman como las genera EF: `IdTipoHabitacionNavigation`, `IdHuespedNavigation`, `IdHabitacionNavigation`, y los `DbSet` en plural automático (`Habitacions`, `Reservas`...).

### Repositorios (paso 6)
Los controladores **nunca** usan `HotelContext`. Reciben por constructor un repositorio y el repositorio recibe el contexto:

| Repositorio | Qué hace |
|---|---|
| `HabitacionRepositorio` | listar activas (con tipo, filtro y orden), obtener por id, verificar número repetido, tipos para el combo, crear, actualizar, **baja lógica** |
| `ReservaRepositorio` | habitaciones disponibles, verificar cruce, listar reservas, huéspedes/habitaciones para los combos, crear (Estado = "Pendiente"), cancelar |

Todo es `async` (`ToListAsync`, `AnyAsync`, `FirstOrDefaultAsync`, `SaveChangesAsync`) y las lecturas usan `AsNoTracking()`. No hay SQL escrito a mano.

### Eliminación lógica (paso 2 y 7)
"Eliminar" una habitación hace `Activo = false` + `SaveChangesAsync()`. El registro sigue en la tabla, solo deja de mostrarse.

### Validación sin tocar Models (paso 5)
Como no se pueden agregar `[Required]` ni `[Display]` en Models:
- Las navegaciones no vienen en el formulario, así que se quitan: `ModelState.Remove("IdTipoHabitacionNavigation")`, etc. (En Reserva también `Estado`, porque lo pone el servidor.)
- Las reglas de negocio se validan en el controlador con `ModelState.AddModelError` (número repetido, fechas, cruce de reservas).
- Las etiquetas en español se escriben directamente en las vistas.

### Post / Redirect / Get + TempData (paso 8)
Cada `POST` válido guarda y hace `RedirectToAction("Index")`; el mensaje se guarda en `TempData["Exito"]` y el `_Layout` lo muestra como alerta verde. Así, si recargas la página, no se vuelve a enviar el formulario.

### Disponibilidad – una sola consulta LINQ (paso 9)
```csharp
_context.Habitacions.AsNoTracking()
    .Include(h => h.IdTipoHabitacionNavigation)
    .Where(h => h.Activo
             && !h.Reservas.Any(r => r.Estado != "Cancelada"
                                  && r.FechaIngreso < salida
                                  && r.FechaSalida > ingreso))
    .OrderBy(h => h.Numero)
    .ToListAsync();
```
Una reserva "choca" si no está cancelada, empieza antes de que yo salga y termina después de que yo entre. Noches = `salida.DayNumber - ingreso.DayNumber`; total = noches × precio por noche.

### Reservas (paso 10)
- **Create**: combos de huéspedes y habitaciones activas; valida salida > ingreso, ingreso ≥ hoy y que la habitación no esté reservada; se guarda con `Estado = "Pendiente"`. Desde *Disponibilidad* el botón **Reservar** abre el formulario con la habitación y fechas ya elegidas.
- **Index**: huésped, habitación, fechas, noches, estado (con colores), total y botón **Cancelar** solo en Pendiente/Confirmada. También tiene filtros por estado.

### Paso 13 – Observaciones
Se ejecutó el `ALTER TABLE`, se regeneró con `--force` (apareció `public string? Observaciones` en `Reserva.cs`), se volvió a quitar `OnConfiguring`, y el campo aparece en **Nueva reserva** (con contador de 200 caracteres).

---

## 4. Capturas sugeridas (para el informe)

1. Inicio (`/`) – menú con Habitaciones, Disponibilidad y Reservas.
2. Habitaciones – listado completo y luego filtrado por tipo (ej. Doble).
3. Nueva habitación – intenta guardar el número **101** → error "Ya existe una habitación con el número 101".
4. Nueva habitación válida (ej. 401) → alerta verde en el listado (TempData).
5. Editar y Detalle de una habitación.
6. Dar de baja (pantalla de confirmación) → alerta y la habitación desaparece. En SSMS: `SELECT * FROM Habitacion` muestra `Activo = 0`.
7. Disponibilidad – busca de **mañana a dentro de 3 días**: 101/102/103 no aparecen (están reservadas).
8. Nueva reserva con fecha pasada / salida antes de ingreso → errores; con una habitación ocupada → "ya está reservada".
9. Nueva reserva válida con **Observaciones** → aparece Pendiente en Reservas.
10. Reservas – cancelar una → estado Cancelada.
11. `Models/Reserva.cs` mostrando la propiedad `Observaciones` y el comando de scaffold con `--force`.

---

## 5. Observaciones y conclusiones (borrador, adáptalo)

**Observaciones**
- El scaffold vuelve a generar `OnConfiguring` cada vez que se usa `--force`, así que hay que quitarlo de nuevo.
- Como las entidades tienen propiedades de navegación no anulables, `ModelState.IsValid` falla si no se quitan con `ModelState.Remove`.
- Las columnas `DATE` de SQL Server se generan como `DateOnly`, lo que simplifica calcular noches con `DayNumber`.
- La restricción `UNIQUE` del número incluye habitaciones dadas de baja, por eso la validación de número repetido revisa todas las filas.

**Conclusiones**
- Database First permite trabajar sobre una base existente: el esquema manda y el modelo se regenera cuando cambia, sin migraciones.
- El patrón repositorio separa el acceso a datos de los controladores y deja todas las consultas LINQ en un solo lugar.
- Con `Any`/`!Any` la disponibilidad se resuelve en una sola consulta que EF traduce a `NOT EXISTS` en SQL.
- Post/Redirect/Get con TempData evita reenvíos duplicados y da una mejor experiencia al usuario.
- La eliminación lógica conserva el historial de reservas sin romper las claves foráneas.
