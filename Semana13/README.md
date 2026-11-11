# Semana 13 – API en capas con DTOs y AutoMapper

**Tema: Clínica Veterinaria.** Es una Web API REST en 4 capas, hecha con ASP.NET Core (.NET 10), EF Core (Database First) y AutoMapper.

```
Semana13/
├── Script/VeterinariaDB.sql          ← crea la BD con datos de ejemplo
├── Veterinaria.slnx                  ← solución con los 4 proyectos
├── Veterinaria.Entidades/            entidades (Propietario, Mascota) + Dtos/
├── Veterinaria.Datos/                Models/VeterinariaContext + Repositorios/ (interfaz + clase)
├── Veterinaria.Negocio/              Servicios/ (interfaz + clase), Mapeos/MappingProfile, Excepciones/
└── Veterinaria.Api/                  Controllers/, Program.cs, Veterinaria.Api.http (pruebas)
```
Referencias: **Api → Negocio → Datos → Entidades**.

---

## 1. Cómo ejecutarlo
1. **Base de datos.** En SSMS (`.\SQLEXPRESS`) abre y ejecuta `Script/VeterinariaDB.sql` con F5. Crea `VeterinariaDB` con 4 propietarios y 7 mascotas. Vuelve a ejecutarlo antes de cada ronda de pruebas para empezar desde cero.
2. **Cadena de conexión.** No está en el repositorio, como pide la guía; se guarda en *User Secrets*. En esta PC ya está configurada. En otra PC se ejecuta una sola vez, dentro de `Semana13`:
   ```bash
   dotnet user-secrets set "ConnectionStrings:VeterinariaDB" "Server=.\SQLEXPRESS;Database=VeterinariaDB;Trusted_Connection=True;TrustServerCertificate=True;" --project Veterinaria.Api
   ```
   En Visual Studio: clic derecho en `Veterinaria.Api` → *Administrar secretos de usuario*.
3. **Ejecutar.** Abre `Veterinaria.slnx` en Visual Studio. Comprueba que `Veterinaria.Api` sea el proyecto de inicio y presiona **F5**. Se abre **Swagger** en `http://localhost:5030/swagger`.
4. **Probar.** Usa Swagger, o abre `Veterinaria.Api/Veterinaria.Api.http` y haz clic en *Enviar solicitud* en cada bloque. Están ordenados y cada uno indica el código esperado.

---

## 2. Explicación del tema (texto para el informe)

**Problema que resuelve.** Una clínica veterinaria necesita registrar a los dueños de las mascotas y a sus animales, consultar las mascotas de cada dueño y mantener los datos consistentes: sin dueños duplicados, sin borrar dueños que tienen mascotas y con un límite de mascotas por dueño.

**Entidades y relación**
- **Propietario**: IdPropietario, Dni (único), Nombres, Apellidos, Telefono, Correo, FechaRegistro.
- **Mascota**: IdMascota, Nombre, Especie, Raza, FechaNacimiento, Peso, IdPropietario (FK).
- **Relación 1:N.** Un propietario tiene muchas mascotas y cada mascota pertenece a un propietario.
  - Clave foránea: `Mascota.IdPropietario`.
  - Navegaciones: `Mascota.IdPropietarioNavigation` y `Propietario.Mascota`.

**Reglas de negocio.** Están en la capa Negocio, lanzan `ReglaNegocioException` y el controlador responde **409 Conflict**:
1. No se puede registrar un propietario con un DNI que ya existe.
2. No se puede eliminar un propietario que todavía tiene mascotas.
3. Un propietario puede tener como máximo **3 mascotas**. Se valida al crear y al trasladar una mascota a otro dueño.
4. La fecha de nacimiento de la mascota no puede ser futura.
5. Un mismo propietario no puede tener dos mascotas con el mismo nombre.

---

## 3. Endpoints (11)

| # | Método | Ruta | Respuestas |
|---|---|---|---|
| 1 | GET | `api/propietarios?buscar=&dni=` (filtros) | 200 |
| 2 | GET | `api/propietarios/{id}` | 200, 404 |
| 3 | GET | `api/propietarios/{id}/mascotas` (**ruta anidada**) | 200, 404 |
| 4 | POST | `api/propietarios` | 201, 400, 409 |
| 5 | PUT | `api/propietarios/{id}` | 204, 400, 404 |
| 6 | DELETE | `api/propietarios/{id}` | 204, 404, 409 |
| 7 | GET | `api/mascotas?especie=&propietarioId=&pesoMax=` (filtros) | 200 |
| 8 | GET | `api/mascotas/{id}` | 200, 404 |
| 9 | POST | `api/mascotas` | 201, 400, 409 |
| 10 | PUT | `api/mascotas/{id}` | 204, 400, 404, 409 |
| 11 | DELETE | `api/mascotas/{id}` | 204, 404 |

- Los **400** los genera solo `[ApiController]` cuando fallan las Data Annotations de los DTOs de entrada, y responde con `ValidationProblem`.
- Swagger publica **solo DTOs**: `PropietarioDto`, `PropietarioCrearDto`, `PropietarioActualizarDto`, `MascotaDto`, `MascotaCrearDto` y `MascotaActualizarDto`. Nunca muestra las entidades.

---

## 4. Comandos utilizados

```bash
dotnet new sln -n Veterinaria
dotnet new classlib -n Veterinaria.Entidades
dotnet new classlib -n Veterinaria.Datos
dotnet new classlib -n Veterinaria.Negocio
dotnet new webapi -n Veterinaria.Api --use-controllers
dotnet sln add Veterinaria.Entidades Veterinaria.Datos Veterinaria.Negocio Veterinaria.Api
dotnet add Veterinaria.Api reference Veterinaria.Negocio
dotnet add Veterinaria.Negocio reference Veterinaria.Datos
dotnet add Veterinaria.Datos reference Veterinaria.Entidades
dotnet add Veterinaria.Datos package Microsoft.EntityFrameworkCore.SqlServer --version "10.*"
dotnet add Veterinaria.Api package Microsoft.EntityFrameworkCore.Design --version "10.*"
dotnet add Veterinaria.Api package Microsoft.EntityFrameworkCore.Tools --version "10.*"
dotnet add Veterinaria.Api package Swashbuckle.AspNetCore.SwaggerUI
dotnet add Veterinaria.Negocio package AutoMapper

dotnet ef dbcontext scaffold "Name=ConnectionStrings:VeterinariaDB" Microsoft.EntityFrameworkCore.SqlServer --project Veterinaria.Datos --startup-project Veterinaria.Api --output-dir Models --context VeterinariaContext --no-onconfiguring --no-pluralize --force
```
- `Name=ConnectionStrings:VeterinariaDB` lee la cadena desde User Secrets, así no queda escrita en ningún archivo.
- `--no-pluralize` evita que EF convierta "Mascota" en "Mascotum".
- Después del scaffold, `Mascota.cs` y `Propietario.cs` se movieron al proyecto Entidades con el namespace `Veterinaria.Entidades`.

---

## 5. Sustentación de los mapeos (MappingProfile)

| Mapeo | Origen → Destino | ¿Por qué en ese sentido? | Configuración especial |
|---|---|---|---|
| `Propietario → PropietarioDto` | Entidad → DTO de lectura | Es lo que la API devuelve al cliente; la entidad nunca sale de la API. | `ForMember Id` toma `IdPropietario` (los nombres difieren). `ForMember NombreCompleto` une `Nombres + " " + Apellidos`. `ForMember CantidadMascotas` cuenta `Mascota.Count`, así no se devuelve la colección. |
| `PropietarioCrearDto → Propietario` | DTO de entrada → Entidad | Llega del cliente en el POST y se convierte en una entidad nueva para guardarla; no se devuelve. | `Ignore` en `IdPropietario` (lo genera la BD con IDENTITY), en `FechaRegistro` (DEFAULT SYSDATETIME) y en `Mascota` (navegación). |
| `PropietarioActualizarDto → Propietario` | DTO de entrada → Entidad existente | Se usa con `Map(dto, entidad)` en el PUT para actualizar el objeto que EF ya sigue. | `Ignore` en `IdPropietario`, `FechaRegistro` y `Mascota`, y también en `Dni`: el DNI identifica a la persona y no se cambia. |
| `Mascota → MascotaDto` | Entidad → DTO de lectura | Es lo que la API devuelve; incluye el nombre del dueño en vez del objeto relacionado. | `ForMember Id` ← `IdMascota`. `ForMember PropietarioId` ← `IdPropietario`. `ForMember Propietario` con `MapFrom` toma el nombre completo de `IdPropietarioNavigation`. `ForMember EdadAnios` calcula la edad desde `FechaNacimiento`. |
| `MascotaCrearDto → Mascota` | DTO de entrada → Entidad | Llega del cliente en el POST y se guarda como entidad nueva. | `ForMember IdPropietario` ← `PropietarioId` (el DTO usa un nombre más claro). `Ignore` en `IdMascota` (IDENTITY) y en `IdPropietarioNavigation`. |
| `MascotaActualizarDto → Mascota` | DTO de entrada → Entidad existente | Se usa con `Map(dto, entidad)` en el PUT; también permite trasladar la mascota a otro dueño. | Igual que el anterior: `ForMember IdPropietario` ← `PropietarioId`, e `Ignore` en `IdMascota` y la navegación. |

**¿Por qué no se usó `ReverseMap()`?** Los DTOs de lectura y de entrada son distintos a propósito. Los de lectura tienen campos calculados (NombreCompleto, EdadAnios, CantidadMascotas) que no deben volver a la entidad. Los de entrada no tienen Id ni datos que el cliente no debe tocar. Un `ReverseMap` crearía mapeos que no se usan o que permitirían sobrescribir datos protegidos.

**Uso en los servicios:**
- Crear: `_mapper.Map<Mascota>(dto)`.
- Leer: `_mapper.Map<List<MascotaDto>>(lista)`.
- Actualizar: `_mapper.Map(dto, mascotaExistente)` y luego `SaveChangesAsync`.

---

## 6. Capturas sugeridas
1. Explorador de soluciones con los 4 proyectos expandidos.
2. `Veterinaria.Negocio/Mapeos/MappingProfile.cs` completo, con los 6 `CreateMap` visibles.
3. Swagger: lista de endpoints y la sección *Schemas* (solo DTOs).
4. Una captura por código, desde Swagger o desde el archivo `.http`:
   - **200** GET (listar, filtro, por id, ruta anidada).
   - **201** POST, mostrando el header `Location`.
   - **204** PUT y DELETE.
   - **400** POST con datos inválidos.
   - **404** id 99.
   - **409** una por cada regla.

Antes de las capturas, ejecuta el script SQL otra vez para que los ids coincidan con los del `.http`.

---

## 7. Observaciones y conclusiones (borrador)

**Observaciones**
- El scaffold singulariza con reglas del inglés: "Mascota" se convirtió en "Mascotum". Se resolvió con `--no-pluralize`.
- La cadena de conexión no se guardó en el repositorio. Se usó *User Secrets* y el scaffold con `Name=ConnectionStrings:VeterinariaDB`.
- Las entidades no usan `[JsonIgnore]`: como la API solo devuelve DTOs, no existe el ciclo Propietario → Mascota → Propietario al serializar JSON.
- Las columnas de la BD (`IdPropietario`, `IdMascota`) tienen nombres distintos a los de los DTOs (`Id`, `PropietarioId`), por eso se configuraron con `ForMember` y `MapFrom`.
- `[ApiController]` valida las Data Annotations de los DTOs de entrada y devuelve 400 sin escribir código extra.
- AutoMapper 16 muestra una advertencia de licencia en el log; en desarrollo es normal.

**Conclusiones**
- Separar la solución en capas (Entidades, Datos, Negocio y API) reparte las responsabilidades: el controlador solo recibe y responde, el servicio aplica las reglas y el repositorio accede a los datos.
- Los DTOs protegen el modelo de datos: el cliente solo ve y envía lo necesario, y los cambios en la base no rompen el contrato de la API.
- AutoMapper evita el código repetitivo de copiar propiedades. Un único MappingProfile centraliza y documenta todas las conversiones.
- Las interfaces y la inyección de dependencias desacoplan las capas y facilitan el mantenimiento y las pruebas.
- Una excepción propia de negocio, traducida a 409, da respuestas HTTP claras y separa los errores de validación (400) de los incumplimientos de reglas (409).
