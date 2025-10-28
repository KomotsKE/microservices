namespace CoreLib.Messages.Events;

public record OrderFailedEvent
{
    public Guid CorrelationId { get; init; }
    public string Reason { get; init; } = string.Empty;
    public string FailedStep { get; init; } = string.Empty;
}