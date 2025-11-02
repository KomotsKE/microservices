namespace CoreLib.Messages.Events;

public record OrderFailedEvent
{
    public Guid CorrelationId { get; init; }
    public string FailedStep { get; init; } = string.Empty;
    public string? ErrorMessage { get; set; }
}