using MassTransit;

namespace SagaOrchestratorService.Logic.Sagas;

public class CreateOrderSagaState : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public string CurrentState { get; set; } = string.Empty;

    public Guid OrderId { get; set; }
    public Guid UserId { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal TotalPrice { get; set; }
    
    

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}