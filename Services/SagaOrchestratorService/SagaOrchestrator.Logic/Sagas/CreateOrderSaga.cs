using CoreLib.Messages.Commands;
using CoreLib.Messages.Events;
using MassTransit;

namespace SagaOrchestratorService.Logic.Sagas;

public class CreateOrderSaga : MassTransitStateMachine<CreateOrderSagaState>
{
    public State ValidatingUser { get; private set; } = null!;
    public State ReservingProduct { get; private set; } = null!;
    public State CreatingOrder { get; private set; } = null!;
    public State Completed { get; private set; } = null!;
    public State Failed { get; private set; } = null!;

    public Event<CreateOrderCommand> OrderRequested { get; private set; } = null!;
    public Event<UserValidatedEvent> UserValidated { get; private set; } = null!;
    public Event<ProductReservedEvent> ProductReserved { get; private set; } = null!;
    public Event<OrderCreatedEvent> OrderCreated { get; private set; } = null!;

    public CreateOrderSaga()
    {
        InstanceState(x => x.CurrentState);

        Event(() => OrderRequested, x => x.CorrelateById(context => context.Message.CorrelationId));
        Event(() => UserValidated, x => x.CorrelateById(context => context.Message.CorrelationId));
        Event(() => ProductReserved, x => x.CorrelateById(context => context.Message.CorrelationId));
        Event(() => OrderCreated, x => x.CorrelateById(context => context.Message.CorrelationId));

        Initially(
            When(OrderRequested)
                .Then(context =>
                {
                    context.Saga.UserId = context.Message.UserId;
                    context.Saga.ProductId = context.Message.ProductId;
                    context.Saga.Quantity = context.Message.Quantity;
                    //log
                })
                .Send(context => new ValidateUserCommand
                {
                    CorrelationId = context.Saga.CorrelationId,
                    UserId = context.Message.UserId
                })
                .TransitionTo(ValidatingUser)
        );

        During(ValidatingUser,
            When(UserValidated)
                .IfElse(context => context.Message.IsValid,
                    valid => valid
                        .Then(context =>
                        {
                            context.Saga.UserValidated = true;
                            //log
                        })
                        .Send(context => new ReserveProductCommand
                        {
                            CorrelationId = context.Saga.CorrelationId,
                            ProductId = context.Saga.ProductId,
                            Quantity = context.Saga.Quantity
                        })
                        .TransitionTo(ReservingProduct),
                    invalid => invalid
                        .Then(context =>
                        {
                            context.Saga.ErrorMessage = context.Message.ErrorMessage;
                            //log
                        })
                        .PublishAsync(context => context.Init<OrderFailedEvent>(new
                        {
                            context.Saga.CorrelationId,
                            Reason = context.Message.ErrorMessage ?? "User validation failed",
                            FailedStep = "UserValidation"
                        }))
                        .TransitionTo(Failed)
                        .Finalize()
                )
        );

        During(ReservingProduct,
            When(ProductReserved)
                .IfElse(context => context.Message.IsReserved,
                    reserved => reserved
                        .Then(context =>
                        {
                            context.Saga.ProductReserved = true;
                            context.Saga.TotalPrice = context.Message.Price * context.Saga.Quantity;
                            //log
                        })
                        .Send(context => new CoreLib.Messages.Commands.CreateOrderCommand
                        {
                            CorrelationId = context.Saga.CorrelationId,
                            UserId = context.Saga.UserId,
                            ProductId = context.Saga.ProductId,
                            Quantity = context.Saga.Quantity,
                            TotalPrice = context.Saga.TotalPrice
                        })
                        .TransitionTo(CreatingOrder),
                    notReserved => notReserved
                        .Then(context =>
                        {
                            context.Saga.ErrorMessage = context.Message.ErrorMessage;
                            //log
                        })
                        .PublishAsync(context => context.Init<OrderFailedEvent>(new
                        {
                            context.Saga.CorrelationId,
                            Reason = context.Message.ErrorMessage ?? "Product reservation failed",
                            FailedStep = "ProductReservation"
                        }))
                        .TransitionTo(Failed)
                        .Finalize()
                )
        );

        During(CreatingOrder,
            When(OrderCreated)
                .Then(context =>
                {
                    context.Saga.OrderId = context.Message.OrderId;
                    //log
                })
                .TransitionTo(Completed)
                .Finalize()
        );

        SetCompletedWhenFinalized();
    }
}