namespace CoreLib.Messages.Events;

public record UserPermissionEvent
{
    public Guid CorrelationId { get; init; }
    public Guid UserId { get; init; }
    public bool HasPermission { get; init; }
}