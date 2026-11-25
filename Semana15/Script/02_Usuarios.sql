/* =====================================================================
   Semana 14 - Autenticación JWT
   Agrega la tabla Usuario a VeterinariaDB (ejecutar después de VeterinariaDB.sql)
   Roles del negocio: Recepcionista y Veterinario
   ===================================================================== */
USE VeterinariaDB;
GO

IF OBJECT_ID(N'dbo.Usuario', N'U') IS NOT NULL
    DROP TABLE dbo.Usuario;
GO

CREATE TABLE Usuario (
    IdUsuario      INT IDENTITY(1,1) NOT NULL
        CONSTRAINT PK_Usuario PRIMARY KEY,
    Correo         NVARCHAR(120)  NOT NULL
        CONSTRAINT UQ_Usuario_Correo UNIQUE,
    ClaveHash      NVARCHAR(100)  NOT NULL,       -- hash BCrypt, nunca la clave en texto
    Rol            NVARCHAR(20)   NOT NULL
        CONSTRAINT DF_Usuario_Rol DEFAULT N'Recepcionista'
        CONSTRAINT CK_Usuario_Rol CHECK (Rol IN (N'Recepcionista', N'Veterinario')),
    FechaRegistro  DATETIME2(0)   NOT NULL
        CONSTRAINT DF_Usuario_FechaRegistro DEFAULT SYSDATETIME()
);
GO

/* ----------------------- USUARIOS DE PRUEBA ----------------------------
   Clave de prueba de ambos: Clave123*
   Los hash se obtuvieron registrando cada usuario por la API
   (POST api/auth/registro, que siempre asigna el rol Recepcionista) y
   copiándolos desde la tabla. Luego se cambió el rol con un UPDATE.
   ---------------------------------------------------------------------- */
INSERT INTO Usuario (Correo, ClaveHash, Rol) VALUES
    (N'recepcion@veterinaria.pe',   N'$2a$11$FSSPTBLWLzrDjAX6z6ygBOtJ6beXKal0qXAsZXRdb3rl0YmX5FLmK', N'Recepcionista'),
    (N'veterinario@veterinaria.pe', N'$2a$11$pBVaS.uPAfxORekRxeUxB.osbbBs4/9j0hCoB.DivtZB3KNOF7CwS', N'Recepcionista');
GO

-- El rol Veterinario lo asigna la clínica, nunca el cliente al registrarse:
UPDATE Usuario SET Rol = N'Veterinario' WHERE Correo = N'veterinario@veterinaria.pe';
GO

SELECT IdUsuario, Correo, Rol FROM Usuario;
GO
