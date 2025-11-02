using OrderService.Application.DTOs;
namespace OrderService.Application.Interfaces;

public interface IOrderService
{
    Task<OrderDto> CreateOrderAsync(Guid userId, Guid productId, int quantity);
    Task<OrderDto?> GetOrderByIdAsync(Guid orderId);
    Task<IEnumerable<OrderDto>> GetOrdersByUserIdAsync(Guid userId);
    Task<IEnumerable<OrderDto>> GetOrdersByStatusAsync(OrderStatusDto orderStatusDto);
    Task UpdateOrderStatusAsync(Guid orderId, OrderStatusDto newStatus);
    Task<OrderDto> CreateOrderFromSagaAsync(Guid userId, Guid productId, int quantity, decimal totalPrice);
}