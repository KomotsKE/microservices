namespace CoreLib.Messages.Events;

public record OrderUpdatedEvent
{
    public Guid CorrelationId { get; init; }
    public Guid OrderId { get; init; }
    public Guid UserId { get; init; }
    public required string Status { get; set; }
}