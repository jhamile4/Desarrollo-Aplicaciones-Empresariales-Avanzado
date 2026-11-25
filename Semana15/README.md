# Semana 15 – Microservicios con API Gateway y Ocelot

El cliente solo conoce el **gateway**. El gateway reenvía cada petición al microservicio que corresponde:

```
                    ┌──────────────────────────────┐
 Cliente ──HTTP──►  │  GatewayApi (Ocelot)  :5100  │  valida el JWT en las rutas protegidas
                    └──────┬───────────────┬───────┘
          /gateway/auth,   │               │  /gateway/vacunas...
          /gateway/mascotas│               │
          /gateway/propietarios            ▼
                           ▼        ┌──────────────────────────┐
   ┌────────────────────────────┐   │ Veterinaria.Vacunas :5056│  microservicio 2
   │ Veterinaria.Api      :5055 │   │ datos en memoria + JWT   │
   │ (API de la S14 + JWT/roles)│   └──────────────────────────┘
   └─────────────┬──────────────┘
                 ▼
          VeterinariaDB (SQL Server en local · PostgreSQL en la nube)
```

| Proyecto | Puerto local | Qué hace |
|---|---|---|
| `GatewayApi` | 5100 | Ocelot 25: único punto de entrada; valida el JWT antes de reenviar |
| `Veterinaria.Api` | 5055 | Microservicio 1: la API en capas de la S14 (propietarios, mascotas, login, roles) |
| `Veterinaria.Vacunas` | 5056 | Microservicio 2: catálogo de vacunas de la clínica. `GET /api/vacunas` y `GET /api/vacunas/{id}`, en memoria y con JWT obligatorio |

Los dos microservicios no comparten base de datos ni clases. Solo comparten la **misma clave, emisor y audiencia del JWT**.

## Tabla de rutas (ocelot.json)

| Upstream (cliente → gateway) | Método | Downstream local | Downstream en la nube | Token |
|---|---|---|---|---|
| `/gateway/auth/{everything}` | POST | `localhost:5055 /api/auth/{everything}` | `veterinaria-api-jm.onrender.com:443 /api/auth/{everything}` | No |
| `/gateway/auth/perfil` | GET | `localhost:5055 /api/auth/perfil` | `veterinaria-api-jm.onrender.com:443 /api/auth/perfil` | Sí |
| `/gateway/mascotas{everything}` | GET | `localhost:5055 /api/mascotas{everything}` | `veterinaria-api-jm.onrender.com:443 /api/mascotas{everything}` | No |
| `/gateway/mascotas{everything}` | POST, PUT, DELETE | `localhost:5055 /api/mascotas{everything}` | `veterinaria-api-jm.onrender.com:443 /api/mascotas{everything}` | Sí |
| `/gateway/propietarios{everything}` | GET | `localhost:5055 /api/propietarios{everything}` | `veterinaria-api-jm.onrender.com:443 /api/propietarios{everything}` | No* |
| `/gateway/propietarios{everything}` | POST, PUT, DELETE | `localhost:5055 /api/propietarios{everything}` | `veterinaria-api-jm.onrender.com:443 /api/propietarios{everything}` | Sí |
| `/gateway/vacunas{everything}` | GET | `localhost:5056 /api/vacunas{everything}` | `veterinaria-vacunas-jm.onrender.com:443 /api/vacunas{everything}` | Sí |

\* El gateway deja pasar los GET, pero la API de la S14 sigue exigiendo token en `/api/propietarios` (su propia matriz). Por eso, sin token, responde 401 desde la API. El rol (403) también lo sigue decidiendo la API.

- Local: `GatewayApi/ocelot.json`.
- Nube: `GatewayApi/ocelot.Production.json`, con https y puerto 443.
- Prueba con Docker: `GatewayApi/ocelot.Docker.json`.

El gateway elige el archivo según el entorno (`ocelot.{Entorno}.json` si existe; si no, `ocelot.json`).

---

## 1. Ejecutar en tu PC

1. **Base de datos** en SSMS, en este orden:
   1. `Script/VeterinariaDB.sql`
   2. `Script/02_Usuarios.sql`
2. **Secretos.** Ya están configurados en esta PC con User Secrets: la misma `Jwt:Clave` en los 3 proyectos y la cadena de conexión en la API. En otra PC, ejecuta `dotnet user-secrets set "Jwt:Clave" "<clave>"` en cada proyecto.
3. **Arrancar los tres:**
   - **Visual Studio:** clic derecho en la solución → *Configurar proyectos de inicio* → *Varios proyectos* → marca *Iniciar* en `Veterinaria.Api`, `Veterinaria.Vacunas` y `GatewayApi` (perfil **http**).
   - **Terminal:** abre tres consolas dentro de `Semana15` y ejecuta un comando en cada una:
     ```bash
     dotnet run --project Veterinaria.Api --launch-profile http
     ```
     ```bash
     dotnet run --project Veterinaria.Vacunas --launch-profile http
     ```
     ```bash
     dotnet run --project GatewayApi --launch-profile http
     ```
4. **Probar** con `GatewayApi/GatewayApi.http`. Todas las peticiones van al puerto **5100** del gateway.
   - Para el **502**, detén `Veterinaria.Vacunas` y llama a `/gateway/vacunas`.
   - Usuarios de prueba, clave `Clave123*`: `recepcion@veterinaria.pe` (Recepcionista) y `veterinario@veterinaria.pe` (Veterinario).

Los puertos 5000 y 5040 los ocupa Windows en esta PC; por eso se usan 5055, 5056 y 5100.

---

## 2. Publicar en la nube (Render + Neon, planes gratuitos)

### Por qué estos proveedores
- **Render**: hosting gratuito de servicios web con **Docker**, sin tarjeta de crédito. Despliega directo desde GitHub y da HTTPS automático en `*.onrender.com`. En el plan gratuito el servicio se "duerme" tras unos 15 minutos sin uso, y el primer request tarda alrededor de un minuto en despertar.
- **Neon**: **PostgreSQL** gratuito y sin vencimiento (0.5 GB), sin tarjeta. Las bases SQL Server gratuitas en la nube son escasas o piden tarjeta (Azure). Por eso la API ahora puede usar PostgreSQL solo con la configuración `BaseDatos:Proveedor = PostgreSql` (paquete `Npgsql.EntityFrameworkCore.PostgreSQL`). El script se adaptó en `Script/VeterinariaDB.postgres.sql`.

Antes de subirlo, todo se probó con **Docker en local**: PostgreSQL 17 y los 3 servicios en contenedores (`docker-compose.yml`), con 21 de 21 pruebas correctas por el gateway.

### Pasos
1. **Neon:** crea una cuenta en neon.tech y un proyecto.
   1. En *SQL Editor* pega y ejecuta `Script/VeterinariaDB.postgres.sql`.
   2. En *Connection Details* elige el formato **.NET** y copia la cadena (`Host=...;Database=...;Username=...;Password=...;SSL Mode=Require`).
2. **Render:** crea una cuenta en render.com, conectada con tu GitHub, y elige **New → Blueprint**.
   1. Selecciona el repositorio. Render lee el `render.yaml` de la raíz y crea los 3 servicios.
   2. Te pedirá estos valores, que **no** están en el repo:
      - `Jwt__Clave` en los 3 servicios: la **misma** clave de 32 caracteres o más.
      - `ConnectionStrings__VeterinariaDB` en `veterinaria-api-jm`: la cadena de Neon.
3. **Revisa las direcciones.** Si Render le puso otra dirección a algún servicio (por ejemplo, con un sufijo), actualiza los `Host` y el `BaseUrl` de `GatewayApi/ocelot.Production.json`, y haz commit y push. Render vuelve a desplegar solo.
4. **Prueba:** `https://<tu-gateway>.onrender.com/gateway/mascotas`. El primer request puede tardar mientras los servicios despiertan.

---

## 3. Sustentación (para el video)

- **¿Qué cambia para el cliente?** Conoce una sola dirección (la del gateway) en vez de una por servicio. No necesita saber en qué servidor o puerto vive cada microservicio. Recibe la autenticación centralizada y un mismo formato de rutas (`/gateway/...`). Si un servicio se mueve, solo cambia `ocelot.json`, no el cliente.
- **Ejemplo de ruta:** en `/gateway/vacunas{everything}` (GET) → `localhost:5056 /api/vacunas{everything}` cambian el **prefijo** (`/gateway` por `/api`) y el **destino** (el gateway en el 5100 reenvía al servicio en el 5056). `{everything}` copia el resto de la URL (por ejemplo `/2` o `?especie=Gato`), así una sola ruta cubre la lista y el detalle.
- **¿Dónde se valida el token?**
  - En el **gateway**, en las rutas con `AuthenticationOptions` "Bearer": así rechaza el 401 antes de gastar una llamada al servicio.
  - En **cada servicio**, que vuelve a validarlo, porque no debe confiar en que todo el tráfico pase por el gateway. Se comprobó: llamar directo al 5056 sin token da 401.
  - El **rol** (403) lo decide la API.
  - Desventaja: la configuración JWT se repite en 3 lugares (y la clave debe ser la misma). Además hay más trabajo por petición, y si se cambia la clave hay que actualizar los tres.
- **Con el microservicio 2 detenido**, el gateway respondió **502 Bad Gateway** en `/gateway/vacunas`. Todo lo de la API de la S14 (login, mascotas, propietarios) siguió funcionando: el fallo quedó aislado. Desventajas que se notaron:
  - **Complejidad:** 3 proyectos, 3 puertos y 3 despliegues.
  - **Autenticación repetida:** la configuración JWT está en 3 lugares.
  - **Depuración más difícil:** un error puede estar en el gateway o en el servicio.

---

## 4. Observaciones y conclusiones (borrador)

**Observaciones**
- En Windows los puertos 5000 y 5040 estaban ocupados por el sistema; por eso se usaron 5055, 5056 y 5100.
- El comodín `{everything}` pegado al recurso (`/gateway/mascotas{everything}`) cubre `/gateway/mascotas`, `/gateway/mascotas/1` y también los query strings.
- Ocelot reenvía el encabezado `Authorization`, así que la API recibe el mismo token y aplica sus roles: el 403 lo devuelve la API, no el gateway.
- La base gratuita en la nube es PostgreSQL (Neon). Se agregó el proveedor Npgsql y se eligió por configuración, sin cambiar la lógica de la API. El script se adaptó (IDENTITY, NVARCHAR→VARCHAR, SYSDATETIME→LOCALTIMESTAMP y nombres entre comillas dobles).
- La clave JWT y la cadena de conexión nunca se suben al repositorio: en local van en User Secrets y en la nube como variables de entorno (`Jwt__Clave`, `ConnectionStrings__VeterinariaDB`).

**Conclusiones**
- El API Gateway simplifica al cliente: una sola dirección, autenticación centralizada y rutas uniformes. Los servicios quedan ocultos detrás.
- Validar el token en el gateway y también en cada servicio aplica defensa en profundidad, a costa de repetir configuración.
- Los microservicios aíslan los fallos (el 502 de Vacunas no afectó a la API). A cambio, aumentan la complejidad de configuración, despliegue y depuración.
- Docker permitió probar en local la misma configuración que se publica en la nube, incluido el cambio de SQL Server a PostgreSQL.
