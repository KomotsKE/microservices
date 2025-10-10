using OrderService.Application.DTOs;
namespace OrderService.Application.Interfaces;

public interface IProductService
{
    Task<IEnumerable<ProductDto>> GetAllProductsAsync();
    Task<ProductDto?> GetProductByIdAsync(Guid productId);
    Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(Guid categoryId);
}