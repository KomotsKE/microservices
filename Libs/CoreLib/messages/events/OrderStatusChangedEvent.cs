namespace CoreLib.Messages.Events;

public record OrderStatusChangedEvent
{
    public Guid CorrelationId { get; init; }
    public Guid OrderId { get; init; }
    public Guid UserId { get; init; }
    public required string OldStatus { get; set; }
    public required string NewStatus { get; set; }
}
