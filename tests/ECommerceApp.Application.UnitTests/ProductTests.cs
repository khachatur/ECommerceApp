using ECommerceApp.Application.Features.Products.Commands;
using ECommerceApp.Domain.Entities;
using ECommerceApp.Domain.Interfaces;
using Moq;

namespace ECommerceApp.Tests;

public class ProductTests
{
    [Fact]
    public async Task CreateProductCommandHandler_Should_AddProduct()
    {
        // Arrange
        var repoMock = new Mock<IProductRepository>();
        var contextMock = new Mock<IAppDbContext>();
        var handler = new CreateProductCommandHandler(repoMock.Object, contextMock.Object);
        var command = new CreateProductCommand
        {
            Name = "Test",
            Price = 10
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        repoMock.Verify(r => r.AddAsync(It.Is<Product>(p => p.Name == "Test" && p.Price == 10)), Times.Once);
    }
}
