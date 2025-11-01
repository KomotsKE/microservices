using CoreLib.Messages.Commands;
using CoreLib.Messages.Events;
using MassTransit;
using OrderService.Domain.Entities;
using OrderService.Domain.Enums;
using OrderService.Domain.Interfaces;

namespace OrderService.Api.Consumers;

/// <summary>
/// Orchestrator: Создаёт заказ в базе данных
/// </summary>
public class CreateOrderCommandConsumer : IConsumer<CoreLib.Messages.Commands.CreateOrderCommand>
{
    private readonly IOrderRepository _orderRepository;
    // private readonly ILogger<CreateOrderCommandConsumer> _logger;

    public CreateOrderCommandConsumer(
        IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
        // _logger = logger;
    }

    public async Task Consume(ConsumeContext<CoreLib.Messages.Commands.CreateOrderCommand> context)
    {
        // _logger.LogInformation(
        //     "[ORDER] Creating order: UserId={UserId}, ProductId={ProductId}, Quantity={Quantity}",
        //     context.Message.UserId,
        //     context.Message.ProductId,
        //     context.Message.Quantity
        // );

        try
        {
            var order = new Order
            {
                Id = Guid.NewGuid(),
                UserId = context.Message.UserId,
                ProductId = context.Message.ProductId,
                Quantity = context.Message.Quantity,
                TotalPrice = context.Message.TotalPrice,
                Status = OrderStatus.Created,
                CreatedAt = DateTime.UtcNow
            };

            await _orderRepository.AddAsync(order);

            // _logger.LogInformation("[ORDER] Order created successfully: {OrderId}", order.Id);

            await context.Publish(new OrderCreatedEvent
            {
                CorrelationId = context.Message.CorrelationId,
                OrderId = order.Id,
                UserId = order.UserId,
                ProductId = order.ProductId,
                Quantity = order.Quantity,
                TotalPrice = order.TotalPrice,
                CreatedAt = order.CreatedAt
            });
        }
        catch (Exception ex)
        {
            // _logger.LogError(ex, "[ORDER] Error creating order");

            await context.Publish(new OrderFailedEvent
            {
                CorrelationId = context.Message.CorrelationId,
                Reason = $"Failed to create order: {ex.Message}",
                FailedStep = "OrderCreation"
            });
        }
    }
}