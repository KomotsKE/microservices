using CoreLib.Messages.Commands;
using MassTransit;
using OrderService.Domain.Enums;
using OrderService.Domain.Interfaces;

public class ApplyOrderUpdateConsumer : IConsumer<OrderUpdateCommand>
{
    private readonly IOrderRepository _repo;

    public ApplyOrderUpdateConsumer(IOrderRepository repo)
    {
        _repo = repo;
    }

    public async Task Consume(ConsumeContext<OrderUpdateCommand> context)
    {
        var order = await _repo.GetByIdAsync(context.Message.OrderId);
        if(order != null)
        {
            order.Status = Enum.Parse<OrderStatus>(context.Message.NewStatus);
            await _repo.UpdateAsync(order);
            Console.WriteLine($"Order {order.Id} updated to {order.Status}");
        }
    }
}
