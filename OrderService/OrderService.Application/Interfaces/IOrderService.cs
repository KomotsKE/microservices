using OrderService.Domain.Enums;
using OrderService.Application.DTOs;
namespace OrderService.Application.Interfaces;

public interface IOrderService
{
    Task<OrderDto> CreateOrderAsync(Guid userId, Guid productId, int quantity);
    Task<OrderDto?> GetOrderByIdAsync(Guid orderId);
    Task<IEnumerable<OrderDto>> GetOrdersByUserAsync(Guid userId);
    Task UpdateOrderStatusAsync(Guid orderId, OrderStatus newStatus);
}