namespace CoreLib.Messages.Commands;

public record ReserveProductCommand
{
    public Guid CorrelationId { get; init; }
    public Guid ProductId { get; init; }
    public int Quantity { get; init; }
}