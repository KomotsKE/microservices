namespace CoreLib.Messages.Commands;

public record ValidateUserCommand
{
    public Guid CorrelationId { get; init; }
    public Guid UserId { get; init; }
}