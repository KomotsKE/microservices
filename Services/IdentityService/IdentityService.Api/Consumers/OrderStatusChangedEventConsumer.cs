using CoreLib.Messages.Events;
using MassTransit;

namespace IdentityService.Api.Consumers;

/// <summary>
/// Choreography: Реагирует на изменение статуса заказа
/// </summary>
public class OrderStatusChangedEventConsumer : IConsumer<OrderStatusChangedEvent>
{
    // private readonly ILogger<OrderStatusChangedEventConsumer> _logger;

    public OrderStatusChangedEventConsumer()
    {
        // _logger = logger;
    }

    public async Task Consume(ConsumeContext<OrderStatusChangedEvent> context)
    {
        var message = context.Message;
        
        // _logger.LogInformation(
        //     "[IDENTITY] Order status changed - OrderId: {OrderId}, UserId: {UserId}, {OldStatus} -> {NewStatus}",
        //     message.OrderId,
        //     message.UserId,
        //     message.OldStatus,
        //     message.NewStatus
        // );

        // TODO: добавить логику обновления истории заказов пользователя

        await Task.CompletedTask;
    }
}