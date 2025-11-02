using CoreLib.Messages.Commands;
using CoreLib.Messages.Events;
using MassTransit;
using OrderService.Application.Interfaces;

namespace OrderService.Api.Consumers;

public class ReleaseProductCommandConsumer : IConsumer<ReleaseProductCommand>
{
    private readonly IProductService _productService;
    public ReleaseProductCommandConsumer(IProductService productService)
    {
        _productService = productService;
    }
    
    public async Task Consume(ConsumeContext<ReleaseProductCommand> context)
    {
        try
        {
            var product = await _productService.ReleaseProductAsync(context.Message.ProductId, context.Message.Quantity);

            await context.Publish(new ProductReleasedEvent
            {
                CorrelationId = context.Message.CorrelationId,
                ProductId = context.Message.ProductId,
                Quantity = context.Message.Quantity,
                IsReleased = true,
            });
        }
        catch (Exception ex)
        {
            await context.Publish(new ProductReleasedEvent
            {
                CorrelationId = context.Message.CorrelationId,
                ProductId = context.Message.ProductId,
                Quantity = context.Message.Quantity,
                IsReleased = false,
                ErrorMessage = ex.Message
            });
        }
    }
}