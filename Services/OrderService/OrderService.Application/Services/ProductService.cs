using CoreLib.Interfaces;
using OrderService.Domain.Entities;
using OrderService.Application.DTOs;
using OrderService.Application.Interfaces;
using OrderService.Domain.Interfaces;
namespace OrderService.Application.Services;
public class ProductService : IProductService
{
    private readonly IProductRepository _productRepo;

    public ProductService(IProductRepository productRepo)
    {
        _productRepo = productRepo;
    }

    public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
    {
        var products = await _productRepo.GetAllAsync();
        return products.Select(p => MapToDto(p));
    }

    public async Task<ProductDto> GetProductByIdAsync(Guid productId)
    {
        var product = await _productRepo.GetByIdAsync(productId) ?? throw new Exception("Product not found");
        return MapToDto(product);
    }

    public async Task<ProductDto> GetProductByNameAsync(string productName)
    {
        var product = await _productRepo.GetByNameAsync(productName) ?? throw new Exception("Product not found");
        return MapToDto(product);
    }

    public async Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(string categoryName)
    {
        var products = await _productRepo.GetByCategoryAsync(categoryName);
        return products.Select(p => MapToDto(p));
    }

    public async Task<ProductDto> ReleaseProductAsync(Guid productId, int quantity)
    {
        var product = await _productRepo.GetByIdAsync(productId)
            ?? throw new Exception($"Product {productId} not found");
        product.Release(quantity);
        await _productRepo.UpdateAsync(product);
        return MapToDto(product);
    }

    public async Task<ProductDto> ReserveProductAsync(Guid productId, int quantity)
    {
        var product = await _productRepo.GetByIdAsync(productId) 
            ?? throw new Exception($"Product {productId} not found");
        product.Reserve(quantity);
        await _productRepo.UpdateAsync(product);
        return MapToDto(product);
    }

    private ProductDto MapToDto(Product product)
    {
        return new ProductDto(product.Id, product.Name, product.Description, product.Price, product.Stock, product.CategoryId);
    }
}