
using OrderService.Domain.Entities;
using OrderService.Application.DTOs;
using OrderService.Domain.Enums;
using OrderService.Application.Interfaces;
using OrderService.Domain.Interfaces;
using IdentityConnectionLib.ConnectionServices.DtoMidels.CheckUserExists;
using IdentityConnectionLib.ConnectionServices.interfaces;

namespace OrderService.Application.Services;

public class OrderService: IOrderService
{
    private readonly IOrderRepository _orderRepo;
    private readonly IProductRepository _productRepo;
    private readonly IIdentityConnectionService _identityService;

    public OrderService(IOrderRepository orderRepo, IProductRepository productRepo, IIdentityConnectionService identityService)
    {
        _orderRepo = orderRepo;
        _productRepo = productRepo;
        _identityService = identityService;
    }

    public async Task<OrderDto> CreateOrderAsync(Guid userId, Guid productId, int quantity)
    {
        var checkUserDto = new CheckUserExistIdentityServiceRequest { UserId = userId };
        var userExists = await _identityService.CheckUserExistsAsync(checkUserDto);
        if (!userExists.IsExist)
            throw new Exception("User not found");
            
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
            TotalPrice = product.Price * quantity,
            Status = OrderStatus.Created,
            CreatedAt = DateTime.UtcNow
        };
        await _orderRepo.AddAsync(order);

        return MapToDto(order);
    }

    public async Task<OrderDto?> GetOrderByIdAsync(Guid orderId)
    {
        var order = await _orderRepo.GetByIdAsync(orderId);
        if (order == null) return null;

        return MapToDto(order);
    }

    public async Task<IEnumerable<OrderDto>> GetOrdersByStatusAsync(OrderStatusDto orderStatusDto)
    {
        var orders = await _orderRepo.GetOrdersByStatusAsync((OrderStatus)orderStatusDto);
        return orders.Select(o => MapToDto(o));
    }

    public async Task<IEnumerable<OrderDto>> GetOrdersByUserIdAsync(Guid userId)
    {
        var orders = await _orderRepo.GetOrdersByUserIdAsync(userId);
        return orders.Select(o => MapToDto(o));
    }

    public async Task UpdateOrderStatusAsync(Guid orderId, OrderStatusDto newStatus)
    {
        var order = await _orderRepo.GetByIdAsync(orderId);
        if (order == null) throw new Exception("Order not found");
        order.Status = (OrderStatus)newStatus;
        await _orderRepo.UpdateAsync(order);
    }

    private OrderDto MapToDto(Order order)
    {
        return new OrderDto(
            order.Id,
            order.UserId,
            order.ProductId,
            order.Quantity,
            order.TotalPrice,
            order.Status.ToString(),
            order.CreatedAt
        );
    }
}