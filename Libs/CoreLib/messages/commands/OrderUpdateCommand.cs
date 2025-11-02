namespace CoreLib.Messages.Commands;

public record OrderUpdateCommand
{
    public Guid CorrelationId { get; init; }
    public Guid OrderId { get; set; }
    public Guid UserId { get; set; }
    public required string NewStatus { get; set; }
}