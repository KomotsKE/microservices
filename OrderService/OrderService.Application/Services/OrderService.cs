using CoreLib.Interfaces;
using OrderService.Domain.Entities;
using OrderService.Application.DTOs;
using OrderService.Domain.Enums;
using OrderService.Application.Interfaces;

namespace OrderService.Application.Services;

public class OrderService: IOrderService
{
    private readonly IRepository<Order> _orderRepo;
    private readonly IRepository<Product> _productRepo;

    public OrderService(IRepository<Order> orderRepo, IRepository<Product> productRepo)
    {
        _orderRepo = orderRepo;
        _productRepo = productRepo;
    }

    public async Task<OrderDto> CreateOrderAsync(Guid userId, Guid productId, int quantity)
    {
        var product = await _productRepo.GetByIdAsync(productId);
        if (product == null) throw new Exception("Product not found");
        if (product.Stock < quantity) throw new Exception("Not enough stock");

        product.Stock -= quantity;
        await _productRepo.UpdateAsync(product);

        var order = new Order
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            ProductId = productId,
            Quantity = quantity,
            Price = product.Price * quantity,
            Status = OrderStatus.Created,
            CreatedAt = DateTime.UtcNow
        };
        await _orderRepo.AddAsync(order);

        return MapToDto(order, product.Name);
    }

    public async Task<OrderDto?> GetOrderByIdAsync(Guid orderId)
    {
        var order = await _orderRepo.GetByIdAsync(orderId);
        if (order == null) return null;

        var product = await _productRepo.GetByIdAsync(order.ProductId);
        return MapToDto(order, product?.Name ?? "Unknown");
    }

    public async Task<IEnumerable<OrderDto>> GetOrdersByUserAsync(Guid userId)
    {
        var orders = await _orderRepo.GetAllAsync();
        var userOrders = orders.Where(o => o.UserId == userId).ToList();
        var productIds = userOrders.Select(o => o.ProductId).Distinct().ToList();
        var products = await _productRepo.GetAllAsync();
        return userOrders.Select(o =>
        {
            var p = products.FirstOrDefault(x => x.Id == o.ProductId);
            return MapToDto(o, p?.Name ?? "Unknown");
        });
    }

    public async Task UpdateOrderStatusAsync(Guid orderId, OrderStatusDto newStatus)
    {
        var order = await _orderRepo.GetByIdAsync(orderId);
        if (order == null) throw new Exception("Order not found");
        order.Status = (OrderStatus)newStatus;
        await _orderRepo.UpdateAsync(order);
    }

    private OrderDto MapToDto(Order order, string productName)
    {
        return new OrderDto(
            order.Id,
            order.UserId,
            order.ProductId,
            order.Quantity,
            order.Price,
            order.Status.ToString(),
            order.CreatedAt
        );
    }
}