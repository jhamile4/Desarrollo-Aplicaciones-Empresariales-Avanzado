# Semana 11 – EF Core Code First: Eventos e inscripciones

Aplicación web **ASP.NET Core MVC (.NET 10)** de eventos e inscripciones con **Entity Framework Core – Code First**: la base de datos `EventosDB` se crea **a partir de las clases C#** mediante **migraciones**. No se usó ningún script SQL, ni `EnsureCreated`, ni Fluent API, ni `FromSqlRaw`.

```
Semana11/Eventos.Web/
├── Models/            Categoria, Evento, Asistente, Inscripcion  (solo Data Annotations)
├── Data/              EventosContext (un DbSet por entidad, sin OnModelCreating)
├── Migrations/        ← las 3 migraciones + EventosContextModelSnapshot.cs
├── Repositorios/      EventoRepositorio, AsistenteRepositorio, InscripcionRepositorio
├── ViewModels/        EventoResumen (evento + inscritos + cupos, no es tabla)
├── Controllers/       Home, Eventos, Asistentes, Inscripciones
├── Views/             vistas fuertemente tipadas + _Layout con el menú
├── appsettings.json   cadena de conexión (Database=EventosDB)
└── Program.cs         AddDbContext, AddScoped y datos iniciales
```

---

## 1. Cómo abrirlo y ejecutarlo

La base **EventosDB ya está creada** en tu `.\SQLEXPRESS`, con las 3 migraciones aplicadas.

- **Visual Studio:** abre `Semana11/Eventos.Web/Eventos.Web.csproj` y presiona **F5**.
- **Terminal**, dentro de `Semana11/Eventos.Web`:
  ```bash
  dotnet run
  ```
  y abre `http://localhost:5195`.

Si algún día necesitas crear la base en otra PC, solo ejecuta `Update-Database` (o `dotnet ef database update`): se aplican las 3 migraciones y, al arrancar la app, se cargan los datos iniciales.

---

## 2. Comandos de migración utilizados

Se usó la CLI `dotnet ef` (herramienta instalada una sola vez con `dotnet tool install --global dotnet-ef`), dentro de la carpeta `Eventos.Web`:

```bash
# Migración 1: las 4 entidades + el contexto
dotnet ef migrations add InitialCreate
dotnet ef database update

# Migración 2: Evento.Modalidad  (antes de aplicarla se cambió defaultValue: "" por "Presencial")
dotnet ef migrations add AgregarModalidadEvento
dotnet ef database update

# Migración 3: Asistente.Telefono
dotnet ef migrations add AgregarTelefonoAsistente
dotnet ef database update
```

Equivalente en Visual Studio (Consola del Administrador de paquetes):
```
Add-Migration InitialCreate
Update-Database
Add-Migration AgregarModalidadEvento
Update-Database
Add-Migration AgregarTelefonoAsistente
Update-Database
```

Paquetes: `Microsoft.EntityFrameworkCore.SqlServer`, `Microsoft.EntityFrameworkCore.Design` y `Microsoft.EntityFrameworkCore.Tools` (10.0.13).

### Evidencia (tabla __EFMigrationsHistory)
```sql
USE EventosDB;
SELECT MigrationId, ProductVersion FROM __EFMigrationsHistory ORDER BY MigrationId;
```
Resultado obtenido (3 filas, iguales a los archivos de la carpeta Migrations):

| MigrationId | ProductVersion |
|---|---|
| 20261028211252_InitialCreate | 10.0.13 |
| 20261028211400_AgregarModalidadEvento | 10.0.13 |
| 20261028211436_AgregarTelefonoAsistente | 10.0.13 |

Carpeta `Migrations`:
```
20261028211252_InitialCreate.cs
20261028211252_InitialCreate.Designer.cs
20261028211400_AgregarModalidadEvento.cs
20261028211400_AgregarModalidadEvento.Designer.cs
20261028211436_AgregarTelefonoAsistente.cs
20261028211436_AgregarTelefonoAsistente.Designer.cs
EventosContextModelSnapshot.cs
```

---

## 3. Qué se hizo y por qué (paso a paso)

### Code First
Primero se escriben las clases en C# y EF Core genera la base. Cada cambio en las clases se convierte en una **migración**: un archivo con el código para aplicar (`Up`) o revertir (`Down`) ese cambio. La tabla `__EFMigrationsHistory` guarda qué migraciones ya se aplicaron.

### Entidades con Data Annotations (sin Fluent API)
| Requisito | Cómo se declaró |
|---|---|
| Clave | `[Key] public int Id` |
| Obligatorio / longitud | `[Required]`, `[StringLength(50)]` |
| Únicos | `[Index(nameof(Nombre), IsUnique = true)]` sobre la clase (Categoria.Nombre, Asistente.Dni, Asistente.Correo) |
| Decimal | `[Column(TypeName = "decimal(10,2)")]` en Evento.Precio, más `[Range]` para que no sea negativo |
| Aforo 1–5000 | `[Range(1, 5000)]` |
| DNI de 8 dígitos | `[RegularExpression(@"^\d{8}$")]` |
| Correo | `[EmailAddress]` |
| Relaciones | propiedad FK (`CategoriaId`, `EventoId`, `AsistenteId`) + navegación con `[ForeignKey]` |

Al Correo se le puso `[StringLength(100)]` porque SQL Server no permite un índice único sobre `nvarchar(max)`.

Las navegaciones se declararon anulables (`Categoria?`), así `ModelState.IsValid` no falla por ellas.

### Migración 2 y el defaultValue
Cuando ya había 4 eventos guardados, se agregó `Modalidad` (obligatoria). EF generó la columna con `defaultValue: ""`. Se cambió **solo esa línea** por `defaultValue: "Presencial"` antes de aplicar la migración, para que los eventos existentes quedaran con "Presencial" y no vacíos. Se comprobó en SQL Server: los 4 eventos quedaron como Presencial.

### Datos iniciales (Program.cs)
Al arrancar la app, para cada tabla: `if (!await context.X.AnyAsync())` → `AddRange(...)` → `SaveChangesAsync()`. Se cargaron 3 categorías, 4 eventos (uno de ellos ya pasado, para probar la validación) y 5 asistentes. Se ejecutó después de aplicar `InitialCreate`.

### Repositorios
Los controladores no usan `EventosContext`, solo los repositorios (registrados con `AddScoped`). Todo es `async` y las lecturas usan `AsNoTracking()`.

**Inscritos y cupos en una sola consulta:**
```csharp
.Select(e => new EventoResumen {
    Evento = ...,
    Inscritos = e.Inscripciones.Count(i => i.Estado == "Confirmada")
})
```
EF lo traduce a un `SELECT COUNT(*)` como subconsulta. Cupos = Aforo − Inscritos.

### Validaciones de la inscripción (ModelState.AddModelError)
1. El evento no ha pasado: `evento.Fecha < DateTime.Now` → error.
2. El asistente no está ya inscrito: `AnyAsync(... Estado == "Confirmada")`.
3. Aforo no completo: `CountAsync(Confirmadas) >= Aforo` → error.

Se guarda con `Estado = "Confirmada"`. Al cancelar pasa a `"Cancelada"` y el cupo se libera.

### Eliminación lógica, PRG y TempData
Dar de baja un evento pone `Activo = false` + `SaveChangesAsync()` (no hay DELETE). Cada POST correcto redirige al Index (Post/Redirect/Get) y el mensaje se muestra con `TempData`.

---

## 4. Datos de prueba que ya están en la base

Durante las pruebas quedaron estos registros, que sirven para tus capturas:
- El evento **"Webinar de Prueba (editado)"** fue creado, editado y dado de baja. En SQL Server aparece con `Activo = 0`, pero ya no se ve en la app.
- La asistente **Fabiola Rojas Vega** tiene teléfono. Los demás no, porque se registraron antes de la migración 3.
- Hay **4 inscripciones**: el Taller de .NET (aforo 3) tiene 2 confirmadas y 1 cancelada.

## 5. Capturas sugeridas

1. Inicio, con el menú Eventos / Asistentes / Inscripciones.
2. Eventos: listado con inscritos/cupos, filtro por categoría, Details, Create (combo de Modalidad), Edit y Delete.
3. Create de evento con errores (aforo 9999, precio negativo) para mostrar las Data Annotations.
4. Asistentes: Index con Teléfono, y Create con DNI o correo repetido (`70123456` / `ana.torres@correo.com`).
5. Inscripciones, Create:
   - el Foro de Emprendimiento muestra "ya pasó",
   - inscribir otra vez a Bruno en el Taller muestra "ya inscrito",
   - llenar el Taller (inscribe a alguien más; con 3 se llena) y luego intentar otra muestra "aforo completo".
6. Inscripciones, Index: cancelar una.
7. **SSMS:** la consulta de `__EFMigrationsHistory` (3 filas) y las tablas de EventosDB.
8. **Visual Studio:** el Explorador de soluciones con la carpeta **Migrations** expandida, mostrando `EventosContextModelSnapshot.cs`.
9. El archivo `..._AgregarModalidadEvento.cs` con `defaultValue: "Presencial"`.

---

## 6. Observaciones y conclusiones (borrador, adáptalo)

**Observaciones**
- Con Code First no se escribió ningún script: la base, las tablas, las claves foráneas y los índices únicos los generó EF a partir de las Data Annotations.
- Para declarar un índice único sobre Correo hubo que limitar su longitud (`StringLength`), porque SQL Server no indexa `nvarchar(max)`.
- Al agregar una columna obligatoria a una tabla con datos, EF propone `defaultValue: ""`. Cambiarlo por "Presencial" permitió que los eventos existentes conservaran un valor válido.
- Cada migración genera dos archivos (`.cs` y `.Designer.cs`) y actualiza `EventosContextModelSnapshot.cs`, que EF usa para detectar el siguiente cambio.
- Declarar las navegaciones como anulables evitó errores de `ModelState` sin usar `ModelState.Remove`.

**Conclusiones**
- Code First permite versionar la base de datos junto con el código: cada cambio queda registrado como una migración y se puede reproducir en cualquier equipo con `Update-Database`.
- Las Data Annotations sirven a la vez para definir el esquema de la base y para validar los formularios, en el servidor y en el cliente.
- El patrón repositorio mantiene los controladores simples y concentra las consultas LINQ y los `SaveChangesAsync`.
- `AnyAsync` y `CountAsync` permiten validar reglas de negocio (duplicados y aforo) de forma eficiente, sin traer datos innecesarios.
- La eliminación lógica y la cancelación por estado conservan el historial y no rompen las relaciones.
