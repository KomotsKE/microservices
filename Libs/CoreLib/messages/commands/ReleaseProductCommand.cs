namespace CoreLib.Messages.Commands;

public class ReleaseProductCommand
{
    public Guid CorrelationId { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}