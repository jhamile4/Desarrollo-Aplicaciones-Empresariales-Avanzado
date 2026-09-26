USE NeptunoDB;
GO

CREATE OR ALTER PROCEDURE sp_Proveedores_Listar
    @NombreContacto NVARCHAR(30) = NULL,
    @Ciudad NVARCHAR(15) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        ProveedorID,
        CompaniaNombre,
        NombreContacto,
        CargoContacto,
        Direccion,
        Ciudad,
        CodigoPostal,
        Pais,
        Telefono,
        Fax,
        Activo
    FROM Proveedores
    WHERE Activo = 1
      AND
      (
          @NombreContacto IS NULL
          OR NombreContacto LIKE '%' + @NombreContacto + '%'
      )
      AND
      (
          @Ciudad IS NULL
          OR Ciudad LIKE '%' + @Ciudad + '%'
      )
    ORDER BY NombreContacto;
END;
GO

CREATE OR ALTER PROCEDURE sp_Proveedores_Desactivar
    @ProveedorID INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Proveedores
    SET Activo = 0
    WHERE ProveedorID = @ProveedorID;
END;
GO