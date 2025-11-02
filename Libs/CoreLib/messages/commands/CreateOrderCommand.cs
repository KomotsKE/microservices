namespace CoreLib.Messages.Commands;

public record CreateOrderCommand
{
    public Guid CorrelationId { get; init; }
    public Guid UserId { get; init; }
    public Guid ProductId { get; init; }
    public int Quantity { get; init; }
    public decimal TotalPrice { get; init; }
}