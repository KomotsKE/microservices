namespace CoreLib.Messages.Events;

public record OrderStatusChangedEvent
{
    public Guid OrderId { get; init; }
    public Guid UserId { get; init; }
    public string OldStatus { get; init; } = string.Empty;
    public string NewStatus { get; init; } = string.Empty;
    public DateTime ChangedAt { get; init; }
}