USE master;
GO

IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'BibliotecaDB')
BEGIN
    CREATE DATABASE BibliotecaDB;
END
GO

USE BibliotecaDB;
GO

-- =============================================
-- TABLAS
-- =============================================

-- 1. Tabla Autores
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Autores]') AND type in (N'U'))
BEGIN
    CREATE TABLE Autores (
        AutorId INT IDENTITY(1,1) PRIMARY KEY,
        Nombre VARCHAR(100) NOT NULL,
        Activo BIT NOT NULL DEFAULT 1
    );
END
GO

-- 2. Tabla Libros
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Libros]') AND type in (N'U'))
BEGIN
    CREATE TABLE Libros (
        LibroId INT IDENTITY(1,1) PRIMARY KEY,
        Titulo VARCHAR(150) NOT NULL,
        AutorId INT NOT NULL,
        Ejemplares INT NOT NULL DEFAULT 1,
        Precio DECIMAL(10,2) NULL,
        FechaPublicacion DATETIME NULL,
        Activo BIT NOT NULL DEFAULT 1,
        CONSTRAINT FK_Libros_Autores FOREIGN KEY (AutorId) REFERENCES Autores(AutorId)
    );
END
GO

-- Asegurar columnas opcionales en Libros si la tabla ya existía
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Libros]') AND name = 'Precio')
BEGIN
    ALTER TABLE Libros ADD Precio DECIMAL(10,2) NULL;
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Libros]') AND name = 'FechaPublicacion')
BEGIN
    ALTER TABLE Libros ADD FechaPublicacion DATETIME NULL;
END
GO

-- 3. Tabla Socios
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Socios]') AND type in (N'U'))
BEGIN
    CREATE TABLE Socios (
        SocioId INT IDENTITY(1,1) PRIMARY KEY,
        Nombre VARCHAR(100) NOT NULL,
        DNI VARCHAR(15) NOT NULL UNIQUE,
        Email VARCHAR(100) NULL,
        Telefono VARCHAR(20) NULL,
        Activo BIT NOT NULL DEFAULT 1
    );
END
GO

-- Asegurar columna Telefono en Socios si la tabla ya existía
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Socios]') AND name = 'Telefono')
BEGIN
    ALTER TABLE Socios ADD Telefono VARCHAR(20) NULL;
END
GO

-- 4. Tabla Prestamos
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Prestamos]') AND type in (N'U'))
BEGIN
    CREATE TABLE Prestamos (
        PrestamoId INT IDENTITY(1,1) PRIMARY KEY,
        SocioId INT NOT NULL,
        FechaPrestamo DATETIME NOT NULL DEFAULT GETDATE(),
        FechaLimite DATETIME NOT NULL,
        Estado VARCHAR(20) NOT NULL DEFAULT 'Prestado',
        CONSTRAINT FK_Prestamos_Socios FOREIGN KEY (SocioId) REFERENCES Socios(SocioId)
    );
END
GO

-- 5. Tabla DetallePrestamo
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DetallePrestamo]') AND type in (N'U'))
BEGIN
    CREATE TABLE DetallePrestamo (
        DetalleId INT IDENTITY(1,1) PRIMARY KEY,
        PrestamoId INT NOT NULL,
        LibroId INT NOT NULL,
        Cantidad INT NOT NULL DEFAULT 1,
        CONSTRAINT FK_Detalle_Prestamos FOREIGN KEY (PrestamoId) REFERENCES Prestamos(PrestamoId),
        CONSTRAINT FK_Detalle_Libros FOREIGN KEY (LibroId) REFERENCES Libros(LibroId)
    );
END
GO

-- Asegurar columna Cantidad en DetallePrestamo si la tabla ya existía
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[DetallePrestamo]') AND name = 'Cantidad')
BEGIN
    ALTER TABLE DetallePrestamo ADD Cantidad INT NOT NULL DEFAULT 1;
END
GO

-- =============================================
-- DATOS DE PRUEBA INITIALES
-- =============================================

IF NOT EXISTS (SELECT * FROM Autores)
BEGIN
    INSERT INTO Autores (Nombre, Activo) VALUES
    ('Gabriel García Márquez', 1),
    ('Mario Vargas Llosa', 1),
    ('Isabel Allende', 1),
    ('Jorge Luis Borges', 1),
    ('Miguel de Cervantes', 1);
END
GO

IF NOT EXISTS (SELECT * FROM Libros)
BEGIN
    INSERT INTO Libros (Titulo, AutorId, Ejemplares, Precio, FechaPublicacion, Activo) VALUES
    ('Cien años de soledad', 1, 5, 85.00, '1967-05-30', 1),
    ('La ciudad y los perros', 2, 3, 65.50, '1963-10-15', 1),
    ('La casa de los espíritus', 3, 4, 72.00, '1982-01-01', 1),
    ('Ficciones', 4, 2, 50.00, '1944-01-01', 1),
    ('Don Quijote de la Mancha', 5, 6, 99.90, '1605-01-16', 1);
END
GO

IF NOT EXISTS (SELECT * FROM Socios)
BEGIN
    INSERT INTO Socios (Nombre, DNI, Email, Telefono, Activo) VALUES
    ('Juan Pérez Gómez', '72345678', 'juan.perez@email.com', '987654321', 1),
    ('María López Torres', '45678912', 'maria.lopez@email.com', '912345678', 1),
    ('Carlos Mendoza Ruiz', '12345678', 'carlos.mendoza@email.com', '998877665', 1);
END
GO

IF NOT EXISTS (SELECT * FROM Prestamos)
BEGIN
    INSERT INTO Prestamos (SocioId, FechaPrestamo, FechaLimite, Estado) VALUES
    (1, '2026-10-01', '2026-10-15', 'Prestado'),
    (2, '2026-09-20', '2026-10-04', 'Vencido'),
    (3, '2026-10-05', '2026-10-20', 'Prestado');

    INSERT INTO DetallePrestamo (PrestamoId, LibroId, Cantidad) VALUES
    (1, 1, 1),
    (2, 2, 1),
    (3, 3, 1),
    (3, 4, 1);
END
GO

-- =============================================
-- PROCEDIMIENTOS ALMACENADOS: AUTORES
-- =============================================

CREATE OR ALTER PROCEDURE sp_ListarAutoresActivos
AS
BEGIN
    SET NOCOUNT ON;
    SELECT AutorId, Nombre
    FROM Autores
    WHERE Activo = 1
    ORDER BY Nombre ASC;
END
GO

-- =============================================
-- PROCEDIMIENTOS ALMACENADOS: LIBROS
-- =============================================

CREATE OR ALTER PROCEDURE sp_ListarLibrosActivos
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        L.LibroId,
        L.Titulo,
        L.AutorId,
        A.Nombre AS AutorNombre,
        L.Ejemplares,
        L.Precio,
        L.FechaPublicacion,
        L.Activo
    FROM Libros L
    INNER JOIN Autores A ON L.AutorId = A.AutorId
    WHERE L.Activo = 1
    ORDER BY L.LibroId DESC;
END
GO

CREATE OR ALTER PROCEDURE sp_BuscarLibrosPorTitulo
    @Titulo VARCHAR(150)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        L.LibroId,
        L.Titulo,
        L.AutorId,
        A.Nombre AS AutorNombre,
        L.Ejemplares,
        L.Precio,
        L.FechaPublicacion,
        L.Activo
    FROM Libros L
    INNER JOIN Autores A ON L.AutorId = A.AutorId
    WHERE L.Activo = 1 AND L.Titulo LIKE '%' + ISNULL(@Titulo, '') + '%'
    ORDER BY L.LibroId DESC;
END
GO

CREATE OR ALTER PROCEDURE sp_ObtenerLibroPorId
    @LibroId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        L.LibroId,
        L.Titulo,
        L.AutorId,
        A.Nombre AS AutorNombre,
        L.Ejemplares,
        L.Precio,
        L.FechaPublicacion,
        L.Activo
    FROM Libros L
    INNER JOIN Autores A ON L.AutorId = A.AutorId
    WHERE L.LibroId = @LibroId;
END
GO

CREATE OR ALTER PROCEDURE sp_InsertarLibro
    @Titulo VARCHAR(150),
    @AutorId INT,
    @Ejemplares INT,
    @Precio DECIMAL(10,2) = NULL,
    @FechaPublicacion DATETIME = NULL
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO Libros (Titulo, AutorId, Ejemplares, Precio, FechaPublicacion, Activo)
    VALUES (@Titulo, @AutorId, @Ejemplares, @Precio, @FechaPublicacion, 1);

    SELECT SCOPE_IDENTITY() AS LibroId;
END
GO

CREATE OR ALTER PROCEDURE sp_ActualizarLibro
    @LibroId INT,
    @Titulo VARCHAR(150),
    @AutorId INT,
    @Ejemplares INT,
    @Precio DECIMAL(10,2) = NULL,
    @FechaPublicacion DATETIME = NULL
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Libros
    SET Titulo = @Titulo,
        AutorId = @AutorId,
        Ejemplares = @Ejemplares,
        Precio = @Precio,
        FechaPublicacion = @FechaPublicacion
    WHERE LibroId = @LibroId;
END
GO

CREATE OR ALTER PROCEDURE sp_EliminarLibroLogico
    @LibroId INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Libros
    SET Activo = 0
    WHERE LibroId = @LibroId;
END
GO

-- =============================================
-- PROCEDIMIENTOS ALMACENADOS: SOCIOS
-- =============================================

CREATE OR ALTER PROCEDURE sp_ListarSociosActivos
AS
BEGIN
    SET NOCOUNT ON;
    SELECT SocioId, Nombre, DNI, Email, Telefono, Activo
    FROM Socios
    WHERE Activo = 1
    ORDER BY SocioId DESC;
END
GO

CREATE OR ALTER PROCEDURE sp_InsertarSocio
    @Nombre VARCHAR(100),
    @DNI VARCHAR(15),
    @Email VARCHAR(100) = NULL,
    @Telefono VARCHAR(20) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO Socios (Nombre, DNI, Email, Telefono, Activo)
    VALUES (@Nombre, @DNI, @Email, @Telefono, 1);

    SELECT SCOPE_IDENTITY() AS SocioId;
END
GO

CREATE OR ALTER PROCEDURE sp_ExisteSocioDNI
    @DNI VARCHAR(15)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT COUNT(1) 
    FROM Socios 
    WHERE DNI = @DNI;
END
GO

-- =============================================
-- PROCEDIMIENTOS ALMACENADOS: PRESTAMOS / REPORTE
-- =============================================

CREATE OR ALTER PROCEDURE sp_ReportePrestamosPorFechas
    @FechaDesde DATETIME = NULL,
    @FechaHasta DATETIME = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        P.PrestamoId,
        S.Nombre AS SocioNombre,
        S.DNI AS SocioDNI,
        L.Titulo AS LibroTitulo,
        P.FechaPrestamo,
        P.FechaLimite,
        P.Estado
    FROM Prestamos P
    INNER JOIN Socios S ON P.SocioId = S.SocioId
    INNER JOIN DetallePrestamo DP ON P.PrestamoId = DP.PrestamoId
    INNER JOIN Libros L ON DP.LibroId = L.LibroId
    WHERE (@FechaDesde IS NULL OR P.FechaPrestamo >= @FechaDesde)
      AND (@FechaHasta IS NULL OR P.FechaPrestamo <= DATEADD(DAY, 1, @FechaHasta))
    ORDER BY P.FechaPrestamo DESC;
END
GO
