CREATE TYPE dbo.OrderItemType AS TABLE
(
    ProductId INT,
    Quantity INT
);
GO

CREATE PROCEDURE UpdateProductStock
    @OrderItems dbo.OrderItemType READONLY
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        
        UPDATE p
        SET p.Quantity = p.Quantity - oi.Quantity
        FROM Products p
        INNER JOIN @OrderItems oi ON p.Id = oi.ProductId
        WHERE p.Quantity >= oi.Quantity;

        IF @@ROWCOUNT != (SELECT COUNT(*) FROM @OrderItems)
            THROW 50001, 'Insufficient stock for one or more products.', 1;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO