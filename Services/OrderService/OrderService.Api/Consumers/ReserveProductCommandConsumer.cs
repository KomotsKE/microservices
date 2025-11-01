using CoreLib.Messages.Commands;
using CoreLib.Messages.Events;
using MassTransit;
using OrderService.Domain.Interfaces;

namespace OrderService.Api.Consumers;

/// <summary>
/// Orchestrator: Резервирует товар на складе
/// </summary>
public class ReserveProductCommandConsumer : IConsumer<ReserveProductCommand>
{
    private readonly IProductRepository _productRepository;
    // private readonly ILogger<ReserveProductCommandConsumer> _logger;

    public ReserveProductCommandConsumer(IProductRepository productRepository)
    {
        _productRepository = productRepository;
        // _logger = logger;
    }

    public async Task Consume(ConsumeContext<ReserveProductCommand> context)
    {
        // _logger.LogInformation(
        //     "[ORDER] Reserving product: {ProductId}, Quantity: {Quantity}",
        //     context.Message.ProductId,
        //     context.Message.Quantity
        // );

        try
        {
            var product = await _productRepository.GetByIdAsync(context.Message.ProductId);

            if (product == null)
            {
                // _logger.LogWarning("[ORDER] Product not found: {ProductId}", context.Message.ProductId);

                await context.Publish(new ProductReservedEvent
                {
                    CorrelationId = context.Message.CorrelationId,
                    ProductId = context.Message.ProductId,
                    Quantity = context.Message.Quantity,
                    Price = 0,
                    IsReserved = false,
                    ErrorMessage = $"Product with ID {context.Message.ProductId} not found"
                });
                return;
            }

            if (product.Stock < context.Message.Quantity)
            {
                // _logger.LogWarning(
                //     "[ORDER] Insufficient stock for product: {ProductId}. Available: {Stock}, Requested: {Quantity}",
                //     context.Message.ProductId,
                //     product.Stock,
                //     context.Message.Quantity
                // );

                await context.Publish(new ProductReservedEvent
                {
                    CorrelationId = context.Message.CorrelationId,
                    ProductId = context.Message.ProductId,
                    Quantity = context.Message.Quantity,
                    Price = product.Price,
                    IsReserved = false,
                    ErrorMessage = $"Insufficient stock. Available: {product.Stock}, Requested: {context.Message.Quantity}"
                });
                return;
            }

            // Резервируем товар (уменьшаем количество на складе)
            product.Stock -= context.Message.Quantity;
            await _productRepository.UpdateAsync(product);

            // _logger.LogInformation(
            //     "[ORDER] Product reserved successfully: {ProductId}, Remaining stock: {Stock}",
            //     product.Id,
            //     product.Stock
            // );

            await context.Publish(new ProductReservedEvent
            {
                CorrelationId = context.Message.CorrelationId,
                ProductId = product.Id,
                Quantity = context.Message.Quantity,
                Price = product.Price,
                IsReserved = true,
                ErrorMessage = null
            });
        }
        catch (Exception ex)
        {
            // _logger.LogError(ex, "[ORDER] Error reserving product: {ProductId}", context.Message.ProductId);

            await context.Publish(new ProductReservedEvent
            {
                CorrelationId = context.Message.CorrelationId,
                ProductId = context.Message.ProductId,
                Quantity = context.Message.Quantity,
                Price = 0,
                IsReserved = false,
                ErrorMessage = $"Internal error: {ex.Message}"
            });
        }
    }
}