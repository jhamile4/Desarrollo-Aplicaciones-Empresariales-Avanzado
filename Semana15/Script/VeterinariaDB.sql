/* =====================================================================
   Semana 13 - API en capas con DTOs y AutoMapper
   Tema: Clínica Veterinaria
   Relación 1:N -> un Propietario tiene muchas Mascotas
   ===================================================================== */
USE master;
GO

IF DB_ID(N'VeterinariaDB') IS NOT NULL
BEGIN
    ALTER DATABASE VeterinariaDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE VeterinariaDB;
END
GO

CREATE DATABASE VeterinariaDB;
GO

USE VeterinariaDB;
GO

/* ----------------------------- TABLAS ----------------------------------- */
CREATE TABLE Propietario (
    IdPropietario  INT IDENTITY(1,1) NOT NULL
        CONSTRAINT PK_Propietario PRIMARY KEY,
    Dni            CHAR(8)        NOT NULL
        CONSTRAINT UQ_Propietario_Dni UNIQUE,
    Nombres        NVARCHAR(100)  NOT NULL,
    Apellidos      NVARCHAR(100)  NOT NULL,
    Telefono       NVARCHAR(15)   NOT NULL,
    Correo         NVARCHAR(120)  NULL,
    FechaRegistro  DATETIME2(0)   NOT NULL
        CONSTRAINT DF_Propietario_FechaRegistro DEFAULT SYSDATETIME()
);
GO

CREATE TABLE Mascota (
    IdMascota        INT IDENTITY(1,1) NOT NULL
        CONSTRAINT PK_Mascota PRIMARY KEY,
    Nombre           NVARCHAR(50)   NOT NULL,
    Especie          NVARCHAR(30)   NOT NULL,
    Raza             NVARCHAR(50)   NULL,
    FechaNacimiento  DATE           NOT NULL,
    Peso             DECIMAL(5,2)   NOT NULL
        CONSTRAINT CK_Mascota_Peso CHECK (Peso > 0),
    IdPropietario    INT            NOT NULL
        CONSTRAINT FK_Mascota_Propietario
        REFERENCES Propietario (IdPropietario)
);
GO

CREATE INDEX IX_Mascota_Propietario ON Mascota (IdPropietario);
GO

/* ------------------------- DATOS DE EJEMPLO ----------------------------- */
INSERT INTO Propietario (Dni, Nombres, Apellidos, Telefono, Correo) VALUES
    ('70123456', N'Lucía',   N'Ramírez Soto',  N'987654321', N'lucia.ramirez@correo.com'),
    ('45879632', N'Marco',   N'Torres Vega',   N'986123456', N'marco.torres@correo.com'),
    ('71234890', N'Valeria', N'Quispe Mamani', N'999888777', NULL),
    ('08765432', N'Andrés',  N'Flores Chávez', N'955444333', N'andres.flores@correo.com');
GO

INSERT INTO Mascota (Nombre, Especie, Raza, FechaNacimiento, Peso, IdPropietario) VALUES
    (N'Firulais', N'Perro', N'Labrador',         '2020-03-15', 28.50, 1),
    (N'Michi',    N'Gato',  N'Siamés',           '2021-07-02',  4.20, 1),
    (N'Rocky',    N'Perro', N'Bulldog Francés',  '2019-11-20', 12.30, 2),
    (N'Luna',     N'Gato',  NULL,                '2022-01-10',  3.80, 2),
    (N'Kiwi',     N'Ave',   N'Periquito',        '2023-05-05',  0.05, 3),
    (N'Toby',     N'Perro', N'Mestizo',          '2018-08-30', 18.00, 3),
    (N'Nala',     N'Perro', N'Golden Retriever', '2021-12-24', 30.10, 3);
-- El propietario 4 (Andrés) no tiene mascotas: sirve para probar el DELETE (204).
GO

/* -------------------------- VERIFICACIÓN -------------------------------- */
SELECT 'Propietario' AS Tabla, COUNT(*) AS Filas FROM Propietario
UNION ALL SELECT 'Mascota', COUNT(*) FROM Mascota;
GO
