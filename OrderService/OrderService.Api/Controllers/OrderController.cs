using Microsoft.AspNetCore.Mvc;
using OrderService.Application.Interfaces;
using OrderService.Application.DTOs;

namespace Marketplace.API.Controllers
{
    [ApiController]
    [Route("orderservice/api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder(Guid userId, Guid productId, int quantity)
        {
            var id = await _orderService.CreateOrderAsync(userId, productId, quantity);
            return Ok(new { OrderId = id });
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetOrderById(Guid id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            return order == null ? NotFound() : Ok(order);
        }

        [HttpGet("user/{userId:guid}")]
        public async Task<IActionResult> GetOrdersByUser(Guid userId)
        {
            var orders = await _orderService.GetOrdersByUserAsync(userId);
            return Ok(orders);
        }

        [HttpPatch("{id:guid}/status")]
        public async Task<IActionResult> UpdateStatus(Guid id, OrderStatusDto status)
        {
            await _orderService.UpdateOrderStatusAsync(id, status);
            return NoContent();
        }
    }
}