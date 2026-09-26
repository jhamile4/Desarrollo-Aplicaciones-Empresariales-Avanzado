USE NeptunoDB;
GO

CREATE OR ALTER PROCEDURE sp_Productos_Listar
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        ProductoID,
        NombreProducto,
        ProveedorID,
        CategoriaID,
        CantidadPorUnidad,
        PrecioUnidad,
        UnidadesEnExistencia,
        UnidadesEnPedido,
        NivelDeReorden,
        Descontinuado,
        Activo
    FROM Productos
    WHERE Activo = 1
    ORDER BY NombreProducto;
END;
GO

CREATE OR ALTER PROCEDURE sp_Productos_Insertar
    @NombreProducto NVARCHAR(40),
    @ProveedorID INT = NULL,
    @CategoriaID INT = NULL,
    @CantidadPorUnidad NVARCHAR(20) = NULL,
    @PrecioUnidad DECIMAL(10,2) = NULL,
    @UnidadesEnExistencia SMALLINT = NULL,
    @UnidadesEnPedido SMALLINT = NULL,
    @NivelDeReorden SMALLINT = NULL,
    @Descontinuado BIT = 0
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Productos
    (
        NombreProducto,
        ProveedorID,
        CategoriaID,
        CantidadPorUnidad,
        PrecioUnidad,
        UnidadesEnExistencia,
        UnidadesEnPedido,
        NivelDeReorden,
        Descontinuado,
        Activo
    )
    VALUES
    (
        @NombreProducto,
        @ProveedorID,
        @CategoriaID,
        @CantidadPorUnidad,
        @PrecioUnidad,
        @UnidadesEnExistencia,
        @UnidadesEnPedido,
        @NivelDeReorden,
        @Descontinuado,
        1
    );
END;
GO

CREATE OR ALTER PROCEDURE sp_Productos_Actualizar
    @ProductoID INT,
    @NombreProducto NVARCHAR(40),
    @ProveedorID INT = NULL,
    @CategoriaID INT = NULL,
    @CantidadPorUnidad NVARCHAR(20) = NULL,
    @PrecioUnidad DECIMAL(10,2) = NULL,
    @UnidadesEnExistencia SMALLINT = NULL,
    @UnidadesEnPedido SMALLINT = NULL,
    @NivelDeReorden SMALLINT = NULL,
    @Descontinuado BIT = 0
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Productos
    SET
        NombreProducto = @NombreProducto,
        ProveedorID = @ProveedorID,
        CategoriaID = @CategoriaID,
        CantidadPorUnidad = @CantidadPorUnidad,
        PrecioUnidad = @PrecioUnidad,
        UnidadesEnExistencia = @UnidadesEnExistencia,
        UnidadesEnPedido = @UnidadesEnPedido,
        NivelDeReorden = @NivelDeReorden,
        Descontinuado = @Descontinuado
    WHERE ProductoID = @ProductoID
      AND Activo = 1;
END;
GO

CREATE OR ALTER PROCEDURE sp_Productos_Desactivar
    @ProductoID INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Productos
    SET Activo = 0
    WHERE ProductoID = @ProductoID;
END;
GO
