using CoreLib.Interfaces;
using OrderService.Domain.Entities;
using OrderService.Application.DTOs;
using OrderService.Application.Interfaces;
namespace OrderService.Application.Services;

public class ProductService : IProductService
{
     private readonly IRepository<Product> _productRepo;
    private readonly IRepository<Category> _categoryRepo;

    public ProductService(IRepository<Product> productRepo, IRepository<Category> categoryRepo)
    {
        _productRepo = productRepo;
        _categoryRepo = categoryRepo;
    }

    public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
    {
        var products = await _productRepo.GetAllAsync();
        var categories = await _categoryRepo.GetAllAsync();
        return products.Select(p =>
        {
            var c = categories.FirstOrDefault(cat => cat.Id == p.CategoryId);
            return new ProductDto(p.Id, p.Name, p.Description, p.Price, p.Stock, c?.Name ?? "Unknown");
        });
    }

    public async Task<ProductDto?> GetProductByIdAsync(Guid productId)
    {
        var p = await _productRepo.GetByIdAsync(productId);
        if (p == null) return null;
        var c = await _categoryRepo.GetByIdAsync(p.CategoryId);
        return new ProductDto(p.Id, p.Name, p.Description, p.Price, p.Stock, c?.Name ?? "Unknown");
    }

    public async Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(Guid categoryId)
    {
        var products = await _productRepo.GetAllAsync();
        var filtered = products.Where(p => p.CategoryId == categoryId);
        var category = await _categoryRepo.GetByIdAsync(categoryId);
        return filtered.Select(p => new ProductDto(p.Id, p.Name, p.Description, p.Price, p.Stock, category?.Name ?? "Unknown"));
    }
}