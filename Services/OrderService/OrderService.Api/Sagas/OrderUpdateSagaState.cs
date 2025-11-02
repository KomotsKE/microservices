using MassTransit;

namespace OrderService.API.Sagas;
public class OrderUpdateSagaState : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public int CurrentState { get; set; }

    public Guid OrderId { get; set; }
    public Guid UserId { get; set; }


    public required string NewStatus { get; set; }
}