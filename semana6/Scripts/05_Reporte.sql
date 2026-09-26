USE NeptunoDB;
GO

CREATE OR ALTER PROCEDURE sp_Reporte_Pedidos
    @FechaInicio DATE,
    @FechaFin DATE
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        p.PedidoID,
        p.FechaPedido,
        p.Destinatario,
        p.CiudadDestino,
        p.PaisDestino,
        pr.NombreProducto,
        d.Cantidad,
        d.PrecioUnidad,
        d.Descuento
    FROM Pedidos p
    INNER JOIN DetallePedidos d
        ON p.PedidoID = d.PedidoID
    INNER JOIN Productos pr
        ON d.ProductoID = pr.ProductoID
    WHERE p.Activo = 1
      AND p.FechaPedido >= @FechaInicio
      AND p.FechaPedido < DATEADD(DAY, 1, @FechaFin)
    ORDER BY p.FechaPedido, p.PedidoID;
END;
GO