USE NeptunoDB;
GO

CREATE OR ALTER PROCEDURE sp_Pedidos_Listar
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        PedidoID,
        ClienteID,
        EmpleadoID,
        FechaPedido,
        FechaRequerida,
        FechaEnvio,
        TransportistaID,
        Destinatario,
        CiudadDestino,
        PaisDestino,
        Activo
    FROM Pedidos
    WHERE Activo = 1
    ORDER BY PedidoID DESC;
END;
GO

CREATE OR ALTER PROCEDURE sp_Pedidos_Desactivar
    @PedidoID INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Pedidos
    SET Activo = 0
    WHERE PedidoID = @PedidoID;
END;
GO