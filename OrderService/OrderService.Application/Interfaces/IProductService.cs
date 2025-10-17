using OrderService.Application.DTOs;
namespace OrderService.Application.Interfaces;

public interface IProductService
{
    Task<IEnumerable<ProductDto>> GetAllProductsAsync();
    Task<ProductDto> GetProductByIdAsync(Guid productId);
    Task<ProductDto> GetProductByNameAsync(string productName);
    Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(string categoryName);
}