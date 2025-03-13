using ECommerceApp.Common.DTOs;

namespace ECommerceApp.WebApp.Services;

public interface IProductService
{
    Task<List<ProductDto>> GetProductsAsync(string token);
    Task<ProductDto> GetProductAsync(int id, string token);
    Task CreateProductAsync(ProductDto product, string token);
    Task UpdateProductAsync(ProductDto product, string token);
    Task DeleteProductAsync(int id, string token);
}
