using CoreLib.Interfaces;
using OrderService.Domain.Entities;
using OrderService.Domain.Enums;

namespace OrderService.Domain.Interfaces;

public interface IOrderRepository : IRepository<Order>
{
    Task<IEnumerable<Order>> GetOrdersByUserIdAsync(Guid userId);
    Task<IEnumerable<Order>> GetOrdersByStatusAsync(OrderStatus status);
}