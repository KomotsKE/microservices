using OrderService.Application.DTOs;
namespace OrderService.Application.Interfaces;

public interface ICategoryService
{
    Task<CategoryDto> CreateCategoryAsync(string name, string description);
    Task<CategoryDto> GetCategoryByIdAsync(Guid categoryId);
    Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync();
    Task UpdateCategoryAsync(Guid categoryId, string name, string description);
    Task DeleteCategoryAsync(Guid categoryId);
}