# Semana 14 – API en capas con autenticación JWT

Es la misma **Veterinaria API** de la Semana 13 (solución en 4 capas: Entidades, Datos, Negocio y Api), ahora protegida con **JWT**. Incluye registro e inicio de sesión, token firmado, endpoints con `[Authorize]` y dos roles propios del negocio: **Recepcionista** y **Veterinario**.

## Qué se agregó a la solución de la Semana 13

| Capa | Archivos nuevos o cambiados |
|---|---|
| Script | `Script/02_Usuarios.sql`: tabla `Usuario` (Correo único, ClaveHash, Rol) y un usuario por rol |
| Entidades | `Usuario.cs` y `Dtos/AuthDtos.cs` (LoginDto, RegistroDto sin rol, TokenDto, UsuarioDto, PerfilDto, constantes de Roles) |
| Datos | `DbSet<Usuario>` en el contexto (agregado a mano) e `IUsuarioRepositorio` / `UsuarioRepositorio` (buscar por correo, agregar, guardar) |
| Negocio | `IAuthServicio` / `AuthServicio` (BCrypt, rol asignado por el servidor, generación del JWT), `Seguridad/JwtOpciones.cs` y 2 mapeos nuevos en el MappingProfile |
| Api | `AuthController` (registro, login, perfil), `[Authorize]` en los controladores y `Program.cs` con AddAuthentication/AddJwtBearer y Swagger con el botón Authorize |

Paquetes: `BCrypt.Net-Next` 4.2 y `System.IdentityModel.Tokens.Jwt` 8.x en Negocio, y `Microsoft.AspNetCore.Authentication.JwtBearer` 10.x en Api.

```bash
dotnet add Veterinaria.Negocio package BCrypt.Net-Next
dotnet add Veterinaria.Negocio package System.IdentityModel.Tokens.Jwt --version "8.*"
dotnet add Veterinaria.Api package Microsoft.AspNetCore.Authentication.JwtBearer --version "10.*"
```

---

## 1. Cómo ejecutarlo

1. **Base de datos.** En SSMS (`.\SQLEXPRESS`) ejecuta, en este orden:
   1. `Script/VeterinariaDB.sql`
   2. `Script/02_Usuarios.sql`
2. **Configuración secreta.** Va en User Secrets, nunca en el repositorio. En esta PC ya está; en otra se configura una vez, dentro de `Semana14`:
   ```bash
   dotnet user-secrets set "ConnectionStrings:VeterinariaDB" "Server=.\SQLEXPRESS;Database=VeterinariaDB;Trusted_Connection=True;TrustServerCertificate=True;" --project Veterinaria.Api
   ```
   ```bash
   dotnet user-secrets set "Jwt:Clave" "<una clave aleatoria de 32 caracteres o más>" --project Veterinaria.Api
   ```
   En `appsettings.json` solo están Emisor, Audiencia y MinutosExpiracion; la Clave queda vacía. Si falta la clave, o tiene menos de 32 caracteres, la API no arranca y muestra un mensaje claro.
3. **Ejecutar.** Abre `Veterinaria.slnx` y presiona **F5** con el perfil **https**. Se abre Swagger en `https://localhost:7290/swagger`.
4. **Usuarios de prueba.** La clave de ambos es `Clave123*`:

   | Correo | Rol |
   |---|---|
   | recepcion@veterinaria.pe | Recepcionista |
   | veterinario@veterinaria.pe | Veterinario |

5. **Probar.**
   - En Swagger: `POST /api/Auth/login`, copia el `token`, pulsa **Authorize**, pégalo y prueba.
   - O abre `Veterinaria.Api/Veterinaria.Api.http`: envía los dos login primero y los demás bloques usan los tokens solos.

---

## 2. Tema y roles

La API de la clínica veterinaria gestiona a los **propietarios** y a sus **mascotas**, que son los pacientes. Ahora cada persona del personal entra con su usuario, y la API decide qué puede hacer según su rol:

- **Recepcionista**: atiende al público. Registra, actualiza y da de baja a los propietarios, y registra a las mascotas cuando llegan por primera vez. No puede modificar ni eliminar los datos clínicos de una mascota.
- **Veterinario**: atiende a los pacientes. Registra mascotas y es el único que actualiza sus datos clínicos (peso, raza...) o da de baja a una mascota. No gestiona la ficha de los propietarios.
- **Sin cuenta (público)**: puede consultar el listado de mascotas y el detalle de una, además de registrarse e iniciar sesión.

Todo usuario nuevo entra como **Recepcionista**. El rol Veterinario lo asigna la clínica con un `UPDATE` en la base, nunca el cliente.

## 3. Matriz de permisos

| Endpoint | Sin token | Rol Recepcionista | Rol Veterinario | Tipo |
|---|---|---|---|---|
| POST /api/auth/registro | Sí | Sí | Sí | Público |
| POST /api/auth/login | Sí | Sí | Sí | Público |
| GET /api/auth/perfil | No (401) | Sí | Sí | Autenticado |
| GET /api/mascotas | Sí | Sí | Sí | Público |
| GET /api/mascotas/{id} | Sí | Sí | Sí | Público |
| POST /api/mascotas | No (401) | Sí | Sí | Rol (Recepcionista o Veterinario) |
| PUT /api/mascotas/{id} | No (401) | No (403) | Sí | Rol Veterinario |
| DELETE /api/mascotas/{id} | No (401) | No (403) | Sí | Rol Veterinario |
| GET /api/propietarios | No (401) | Sí | Sí | Autenticado |
| GET /api/propietarios/{id} | No (401) | Sí | Sí | Autenticado |
| GET /api/propietarios/{id}/mascotas | No (401) | Sí | Sí | Autenticado |
| POST /api/propietarios | No (401) | Sí | No (403) | Rol Recepcionista |
| PUT /api/propietarios/{id} | No (401) | Sí | No (403) | Rol Recepcionista |
| DELETE /api/propietarios/{id} | No (401) | Sí | No (403) | Rol Recepcionista |

**Cómo se implementa:**
- Cada controlador lleva `[Authorize]` a nivel de clase.
- Los endpoints públicos llevan `[AllowAnonymous]`.
- Los que exigen un rol llevan `[Authorize(Roles = "...")]`.
- En Swagger solo los endpoints protegidos muestran candado.

---

## 4. Sustentación de seguridad

**¿Por qué un caso devolvió 401 y otro 403?**
- **401 Unauthorized** significa que la API *no sabe quién eres*: no se envió token, el token es inválido o venció, o las credenciales del login son incorrectas.
  - Ejemplo: `GET /api/propietarios` **sin token** devolvió 401, con el header `www-authenticate: Bearer`.
  - Otro ejemplo: el login con clave incorrecta también devolvió 401.
- **403 Forbidden** significa que la API *sí sabe quién eres* (el token es válido), pero tu rol no tiene permiso.
  - Ejemplo: el **Veterinario** hizo `POST /api/propietarios` y recibió 403, porque ese endpoint exige el rol Recepcionista.
  - Ejemplo inverso: la **Recepcionista** hizo `PUT /api/mascotas/2` y recibió 403.

**¿Qué claims incluiste en el token y por qué? ¿Qué no pondrías nunca dentro de un token?**
- `sub`: el Id del usuario. Identifica a quién pertenece el token.
- `email`: el correo, que sirve para mostrar quién está conectado (`GET /api/auth/perfil`).
- `role`: el rol (Recepcionista o Veterinario), que es lo que usa `[Authorize(Roles = ...)]`.
- `jti`: un identificador único del token.
- Además están los estándar: `exp` (vencimiento), `iss` (emisor) y `aud` (audiencia), que la API valida.
- **Nunca** pondría la contraseña (ni siquiera su hash), la cadena de conexión, la clave secreta, ni datos sensibles como tarjetas o información médica privada. El payload del JWT **no está cifrado**: solo está en Base64 y cualquiera puede leerlo, por ejemplo en jwt.io. La firma impide modificarlo, pero no impide leerlo.

**¿Qué pasaría si guardaras la contraseña en texto en vez de su hash?**
Cualquiera con acceso a la base (un empleado, un backup filtrado o un ataque de inyección SQL) vería todas las contraseñas y podría entrar como cualquier usuario. Como la gente repite contraseñas, también podría entrar a sus otras cuentas (correo, banco). Con BCrypt se guarda un hash con *salt*, lento de calcular y que no se puede revertir. Para validar el login se usa `BCrypt.Verify`, sin conocer nunca la clave original.

**¿Por qué el rol sale del token firmado y no lo envía el cliente en el registro?**
Si el cliente pudiera enviar su rol, cualquiera se registraría como Veterinario y obtendría permisos que no le corresponden (escalada de privilegios). Por eso el `RegistroDto` no tiene rol y el servidor siempre asigna Recepcionista. Se probó: un registro que envió `"rol": "Veterinario"` quedó como Recepcionista. Después, en cada petición, el rol se lee del token, que está **firmado** con la clave secreta del servidor. Si alguien cambia el rol dentro del token, la firma deja de coincidir y la API responde 401; también se probó con un token alterado.

---

## 5. Pruebas realizadas

Se ejecutaron **54 pruebas automáticas** y todas devolvieron el código esperado: la matriz completa con sin token, Recepcionista y Veterinario, más 400, 404 y 409. Además se tomaron las capturas en Swagger.

| Código | Ejemplo |
|---|---|
| 200 | Login correcto, `GET /api/auth/perfil`, `GET /api/propietarios` con token, `GET /api/mascotas` sin token |
| 201 | `POST /api/auth/registro`, `POST /api/propietarios` (Recepcionista), `POST /api/mascotas` (Veterinario) |
| 204 | `PUT /api/propietarios/2` (Recepcionista), `PUT /api/mascotas/2` y `DELETE /api/mascotas/8` (Veterinario) |
| 400 | Registro con correo mal escrito y clave corta |
| 401 | Login con clave incorrecta o correo inexistente, `GET /api/propietarios` sin token, token alterado |
| 403 | Veterinario en `POST /api/propietarios`, Recepcionista en `PUT /api/mascotas/2` |
| 404 | `DELETE /api/mascotas/8` repetido, `GET /api/mascotas/99` |
| 409 | Registro con correo repetido, DNI repetido, eliminar propietario con mascotas, más de 3 mascotas |

---

## 6. Observaciones y conclusiones (borrador)

**Observaciones**
- Con `MapInboundClaims = false` los claims conservan sus nombres cortos (`sub`, `email`, `role`). Por eso se configuró `RoleClaimType = "role"` para que `[Authorize(Roles = ...)]` los reconozca.
- El login responde el mismo 401 y el mismo mensaje si el correo no existe o si la clave es incorrecta. Así no se revela qué correos están registrados.
- La clave del token no está en `appsettings.json`: está en User Secrets, y la API se niega a arrancar si mide menos de 32 caracteres.
- El orden importa: `UseAuthentication()` va antes de `UseAuthorization()`. Primero se identifica al usuario y después se revisan sus permisos.
- jwt.io muestra "signature verification failed" porque no conoce la clave secreta. Eso confirma que nadie sin la clave puede firmar un token válido.
- Se cambiaron los puertos (7290/5040) para poder ejecutar la Semana 13 y la Semana 14 a la vez.

**Conclusiones**
- JWT permite una autenticación sin estado: el servidor no guarda sesiones, solo valida la firma, el emisor, la audiencia y el vencimiento de cada token.
- Separar autenticación (401: ¿quién eres?) y autorización (403: ¿qué puedes hacer?) hace que la API responda de forma clara y predecible.
- Los roles propios del negocio y la matriz de permisos convierten las reglas de la clínica en código, con atributos fáciles de leer y mantener.
- Guardar contraseñas con BCrypt y decidir el rol en el servidor son dos medidas básicas para evitar el robo de credenciales y la escalada de privilegios.
- Mantener los secretos fuera del repositorio (User Secrets) evita que la clave del token o la cadena de conexión se filtren al publicar el código.
