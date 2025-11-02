namespace CoreLib.Messages.Commands;
public record CheckUserPermissionCommand
{
    public Guid CorrelationId { get; set; }
    public Guid OrderId { get; set; }
    public Guid UserId { get; set; }
}
