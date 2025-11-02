using Microsoft.AspNetCore.Mvc;
using OrderService.Application.DTOs;
using OrderService.Application.Interfaces;

namespace OrderService.API.Controllers
{
    [ApiController]
    [Route("orderservice/api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            return Ok(categories);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            return category == null ? NotFound() : Ok(category);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CategoryDto dto)
        {
            var id = await _categoryService.CreateCategoryAsync(dto.Name, dto.Description);
            return Ok(new { CategoryId = id });
        }

        [HttpPut]
        public async Task<IActionResult> Update(CategoryDto dto)
        {
            await _categoryService.UpdateCategoryAsync(dto.Id, dto.Name, dto.Description);
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _categoryService.DeleteCategoryAsync(id);
            return NoContent();
        }
    }
}
