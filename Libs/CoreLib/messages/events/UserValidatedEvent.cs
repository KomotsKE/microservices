namespace CoreLib.Messages.Events;

public record UserValidatedEvent
{
    public Guid CorrelationId { get; init; }
    public Guid UserId { get; init; }
    public bool IsValid { get; init; }
    public string? ErrorMessage { get; init; }
}