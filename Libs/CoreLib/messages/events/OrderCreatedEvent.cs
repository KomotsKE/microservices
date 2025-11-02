namespace CoreLib.Messages.Events;

public record OrderCreatedEvent
{
    public Guid CorrelationId { get; init; }
    public Guid OrderId { get; init; }
    public Guid UserId { get; init; }
    public Guid ProductId { get; init; }
    public int Quantity { get; init; }
    public decimal TotalPrice { get; init; }
    public DateTime CreatedAt { get; init; }
    public string Status { get; set; } = "Created";
}