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
-- TABLAS (Compatibles con Lab07)
-- =============================================

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Autores]') AND type in (N'U'))
BEGIN
    CREATE TABLE Autores (
        AutorId INT IDENTITY(1,1) PRIMARY KEY,
        Nombre NVARCHAR(100) NOT NULL,
        Nacionalidad NVARCHAR(50) NULL,
        Activo BIT NOT NULL DEFAULT 1
    );
END
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Libros]') AND type in (N'U'))
BEGIN
    CREATE TABLE Libros (
        LibroId INT IDENTITY(1,1) PRIMARY KEY,
        Titulo NVARCHAR(150) NOT NULL,
        ISBN NVARCHAR(20) NOT NULL UNIQUE,
        AutorId INT NOT NULL REFERENCES Autores(AutorId),
        Ejemplares INT NOT NULL DEFAULT 1,
        Activo BIT NOT NULL DEFAULT 1
    );
END
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Socios]') AND type in (N'U'))
BEGIN
    CREATE TABLE Socios (
        SocioId INT IDENTITY(1,1) PRIMARY KEY,
        DNI NVARCHAR(15) NOT NULL UNIQUE,
        Nombre NVARCHAR(100) NOT NULL,
        Email NVARCHAR(100) NULL,
        Activo BIT NOT NULL DEFAULT 1
    );
END
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Prestamos]') AND type in (N'U'))
BEGIN
    CREATE TABLE Prestamos (
        PrestamoId INT IDENTITY(1,1) PRIMARY KEY,
        SocioId INT NOT NULL REFERENCES Socios(SocioId),
        FechaPrestamo DATE NOT NULL DEFAULT GETDATE(),
        FechaLimite DATE NOT NULL,
        Estado NVARCHAR(20) NOT NULL DEFAULT N'Pendiente'
    );
END
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DetallePrestamo]') AND type in (N'U'))
BEGIN
    CREATE TABLE DetallePrestamo (
        PrestamoId INT NOT NULL REFERENCES Prestamos(PrestamoId),
        LibroId INT NOT NULL REFERENCES Libros(LibroId),
        FechaDevolucion DATE NULL,
        PRIMARY KEY (PrestamoId, LibroId)
    );
END
GO

-- =============================================
-- PROCEDIMIENTOS ALMACENADOS
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

CREATE OR ALTER PROCEDURE sp_ListarLibrosActivos
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        L.LibroId,
        L.Titulo,
        ISNULL(L.ISBN, '') AS ISBN,
        L.AutorId,
        A.Nombre AS AutorNombre,
        L.Ejemplares,
        L.Activo
    FROM Libros L
    INNER JOIN Autores A ON L.AutorId = A.AutorId
    WHERE L.Activo = 1
    ORDER BY L.LibroId DESC;
END
GO

CREATE OR ALTER PROCEDURE sp_BuscarLibrosPorTitulo
    @Titulo NVARCHAR(150)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        L.LibroId,
        L.Titulo,
        ISNULL(L.ISBN, '') AS ISBN,
        L.AutorId,
        A.Nombre AS AutorNombre,
        L.Ejemplares,
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
        ISNULL(L.ISBN, '') AS ISBN,
        L.AutorId,
        A.Nombre AS AutorNombre,
        L.Ejemplares,
        L.Activo
    FROM Libros L
    INNER JOIN Autores A ON L.AutorId = A.AutorId
    WHERE L.LibroId = @LibroId;
END
GO

CREATE OR ALTER PROCEDURE sp_InsertarLibro
    @Titulo NVARCHAR(150),
    @ISBN NVARCHAR(20) = NULL,
    @AutorId INT,
    @Ejemplares INT
AS
BEGIN
    SET NOCOUNT ON;
    
    IF @ISBN IS NULL OR TRIM(@ISBN) = ''
    BEGIN
        SET @ISBN = '978-' + CAST(ABS(CHECKSUM(NEWID())) % 899999 + 100000 AS VARCHAR(20));
    END

    INSERT INTO Libros (Titulo, ISBN, AutorId, Ejemplares, Activo)
    VALUES (@Titulo, @ISBN, @AutorId, @Ejemplares, 1);

    SELECT SCOPE_IDENTITY() AS LibroId;
END
GO

CREATE OR ALTER PROCEDURE sp_ActualizarLibro
    @LibroId INT,
    @Titulo NVARCHAR(150),
    @ISBN NVARCHAR(20) = NULL,
    @AutorId INT,
    @Ejemplares INT
AS
BEGIN
    SET NOCOUNT ON;
    
    IF @ISBN IS NULL OR TRIM(@ISBN) = ''
    BEGIN
        SELECT @ISBN = ISNULL(ISBN, '978-000000000') FROM Libros WHERE LibroId = @LibroId;
    END

    UPDATE Libros
    SET Titulo = @Titulo,
        ISBN = @ISBN,
        AutorId = @AutorId,
        Ejemplares = @Ejemplares
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

CREATE OR ALTER PROCEDURE sp_ListarSociosActivos
AS
BEGIN
    SET NOCOUNT ON;
    SELECT SocioId, Nombre, DNI, Email, Activo
    FROM Socios
    WHERE Activo = 1
    ORDER BY SocioId DESC;
END
GO

CREATE OR ALTER PROCEDURE sp_InsertarSocio
    @Nombre NVARCHAR(100),
    @DNI NVARCHAR(15),
    @Email NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO Socios (Nombre, DNI, Email, Activo)
    VALUES (@Nombre, @DNI, @Email, 1);

    SELECT SCOPE_IDENTITY() AS SocioId;
END
GO

CREATE OR ALTER PROCEDURE sp_ExisteSocioDNI
    @DNI NVARCHAR(15)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT COUNT(1) 
    FROM Socios 
    WHERE DNI = @DNI;
END
GO

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
