CREATE PROCEDURE UpdateProductStock
    @ProductId INT,
    @Quantity INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        UPDATE Products
        SET Quantity = Quantity - @Quantity
        WHERE Id = @ProductId AND Quantity >= @Quantity;
        IF @@ROWCOUNT = 0
            THROW 50001, 'Insufficient stock or product not found.', 1;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END
GO