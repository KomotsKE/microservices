using Microsoft.AspNetCore.Mvc;
using OrderService.Application.Interfaces;

namespace OrderService.API.Controllers
{
    [ApiController]
    [Route("orderservice/api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _productService.GetAllProductsAsync();
            return Ok(products);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            return product == null ? NotFound() : Ok(product);
        }

        [HttpGet("category/{category}")]
        public async Task<IActionResult> GetByCategory(string category)
        {
            var products = await _productService.GetProductsByCategoryAsync(category);
            return Ok(products);
        }

        [HttpGet("{name}")]
        public async Task<IActionResult> GetByName(string name)
        {
            var products = await _productService.GetProductByNameAsync(name);
            return Ok(products);
        }
    }
}
