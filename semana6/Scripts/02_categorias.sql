USE NeptunoDB;
GO

CREATE OR ALTER PROCEDURE sp_Categorias_Listar
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        CategoriaID,
        NombreCategoria,
        Descripcion,
        Activo
    FROM Categorias
    WHERE Activo = 1
    ORDER BY NombreCategoria;
END;
GO

CREATE OR ALTER PROCEDURE sp_Categorias_Insertar
    @NombreCategoria NVARCHAR(15),
    @Descripcion NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Categorias
    (
        NombreCategoria,
        Descripcion,
        Activo
    )
    VALUES
    (
        @NombreCategoria,
        @Descripcion,
        1
    );
END;
GO

CREATE OR ALTER PROCEDURE sp_Categorias_Actualizar
    @CategoriaID INT,
    @NombreCategoria NVARCHAR(15),
    @Descripcion NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Categorias
    SET
        NombreCategoria = @NombreCategoria,
        Descripcion = @Descripcion
    WHERE CategoriaID = @CategoriaID
      AND Activo = 1;
END;
GO

CREATE OR ALTER PROCEDURE sp_Categorias_Desactivar
    @CategoriaID INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Categorias
    SET Activo = 0
    WHERE CategoriaID = @CategoriaID;
END;
GO