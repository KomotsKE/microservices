using CoreLib.Interfaces;
using OrderService.Domain.Entities;
using OrderService.Application.DTOs;
using OrderService.Application.Interfaces;
namespace OrderService.Application.Services;

public class CategoryService : ICategoryService
{
    private readonly IRepository<Category> _categoryRepository;
    public CategoryService(IRepository<Category> categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }
    public async Task<CategoryDto> CreateCategoryAsync(string name, string description)
    {
        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = name,
            Description = description
        };
        await _categoryRepository.AddAsync(category);
        return new CategoryDto(category.Id, category.Name, category.Description);
    }

    public async Task DeleteCategoryAsync(Guid categoryId)
    {
        await _categoryRepository.DeleteAsync(categoryId);
    }

    public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
    {
        var categories = await _categoryRepository.GetAllAsync();
        return categories.Select(c => new CategoryDto(c.Id, c.Name, c.Description));
    }

    public async Task<CategoryDto> GetCategoryByIdAsync(Guid categoryId)
    {
        var category = await _categoryRepository.GetByIdAsync(categoryId) ?? throw new Exception("Category not found");
        return new CategoryDto(category.Id, category.Name, category.Description); 
    }

    public async Task UpdateCategoryAsync(Guid categoryId, string name, string description)
    {
        var category = await _categoryRepository.GetByIdAsync(categoryId) ?? throw new Exception("Category not found");
        category.Name = name;
        category.Description = description;
        await _categoryRepository.UpdateAsync(category);
    }
}