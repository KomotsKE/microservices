namespace CoreLib.Messages.Events;

public record ProductReservedEvent
{
    public Guid CorrelationId { get; init; }
    public Guid ProductId { get; init; }
    public int Quantity { get; init; }
    public decimal Price { get; init; }
    public bool IsReserved { get; init; }
    public string? ErrorMessage { get; init; }
}