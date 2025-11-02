using CoreLib.Messages.Commands;
using CoreLib.Messages.Events;
using MassTransit;
using OrderService.Application.Interfaces;
using OrderService.Domain.Entities;
using OrderService.Domain.Enums;
using OrderService.Domain.Interfaces;

namespace OrderService.Api.Consumers;

/// <summary>
/// Orchestrator: Создаёт заказ в базе данных
/// </summary>
public class CreateOrderCommandConsumer : IConsumer<CreateOrderCommand>
{
    private readonly IOrderService _orderService;

    public CreateOrderCommandConsumer(
        IOrderService orderService)
    {
        _orderService = orderService;
    }

    public async Task Consume(ConsumeContext<CreateOrderCommand> context)
    {
        try
        {
            var order = await _orderService.CreateOrderFromSagaAsync(
                context.Message.UserId, context.Message.ProductId,
                context.Message.Quantity, context.Message.TotalPrice
            );
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
            await context.Publish(new OrderFailedEvent
            {
                CorrelationId = context.Message.CorrelationId,
                ErrorMessage = ex.Message,
                FailedStep = "OrderCreation"
            });
        }
    }
}