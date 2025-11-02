namespace CoreLib.Messages.Events;
public class ProductReleasedEvent
{
    public Guid CorrelationId { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public bool IsReleased { get; set; }
    public string? ErrorMessage { get; set; }
}