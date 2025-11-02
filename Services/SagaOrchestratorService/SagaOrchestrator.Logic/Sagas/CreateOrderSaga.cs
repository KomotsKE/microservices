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
    public Event<OrderFailedEvent> OrderFailed { get; private set; } = null!;

    public CreateOrderSaga()
    {
        InstanceState(x => x.CurrentState);

        Event(() => OrderRequested, x => x.CorrelateById(context => context.Message.CorrelationId));
        Event(() => UserValidated, x => x.CorrelateById(context => context.Message.CorrelationId));
        Event(() => ProductReserved, x => x.CorrelateById(context => context.Message.CorrelationId));
        Event(() => OrderCreated, x => x.CorrelateById(context => context.Message.CorrelationId));
        Event(() => OrderFailed, x => x.CorrelateById(context => context.Message.CorrelationId));

        Initially(
            When(OrderRequested)
                .Then(context =>
                {
                    context.Saga.CorrelationId = context.Message.CorrelationId;
                    context.Saga.UserId = context.Message.UserId;
                    context.Saga.ProductId = context.Message.ProductId;
                    context.Saga.Quantity = context.Message.Quantity;
                    context.Saga.CreatedAt = DateTime.UtcNow;
                    context.Saga.UpdatedAt = DateTime.UtcNow;
                })
                .Send(context => new ValidateUserCommand
                {
                    CorrelationId = context.Saga.CorrelationId,
                    UserId = context.Saga.UserId
                })
                .TransitionTo(ValidatingUser)
        );

        During(ValidatingUser,
            When(UserValidated)
                .IfElse(context => context.Message.IsValid,
                    binder => binder
                        .Then(ctx => ctx.Saga.UpdatedAt = DateTime.UtcNow)
                        .Send(ctx => new ReserveProductCommand
                        {
                            CorrelationId = ctx.Saga.CorrelationId,
                            ProductId = ctx.Saga.ProductId,
                            Quantity = ctx.Saga.Quantity
                        })
                        .TransitionTo(ReservingProduct),
                    binder => binder
                        .Publish(ctx => new OrderFailedEvent
                        {
                            CorrelationId = ctx.Saga.CorrelationId,
                            FailedStep = "UserValidation",
                            ErrorMessage = ctx.Message.ErrorMessage ?? "User not valid"
                        })
                        .Finalize()
                )
        );

        During(ReservingProduct,
            When(ProductReserved)
                .IfElse(context => context.Message.IsReserved,
                    binder => binder
                        .Then(ctx => {
                            ctx.Saga.UpdatedAt = DateTime.UtcNow;
                            ctx.Saga.TotalPrice = ctx.Message.Price * ctx.Saga.Quantity;
                            })
                        .Send(ctx => new CreateOrderCommand
                        {
                            CorrelationId = ctx.Saga.CorrelationId,
                            UserId = ctx.Saga.UserId,
                            ProductId = ctx.Saga.ProductId,
                            Quantity = ctx.Saga.Quantity,
                            TotalPrice = ctx.Message.Price * ctx.Saga.Quantity
                        })
                        .TransitionTo(CreatingOrder),
                    binder => binder
                        .Send(ctx => new ReleaseProductCommand { CorrelationId = ctx.Saga.CorrelationId })
                        .Publish(ctx => new OrderFailedEvent
                        {
                            CorrelationId = ctx.Saga.CorrelationId,
                            FailedStep = "ReserveProduct",
                            ErrorMessage = ctx.Message.ErrorMessage ?? "Cannot reserve product"
                        })
                        .TransitionTo(Failed)
                )
        );

        During(CreatingOrder,
            When(OrderCreated)
                .Then(ctx =>
                {
                    ctx.Saga.OrderId = ctx.Message.OrderId;
                    ctx.Saga.UpdatedAt = DateTime.UtcNow;
                })
                .Publish(ctx => new OrderCreatedEvent
                {
                    CorrelationId = ctx.Saga.CorrelationId,
                    OrderId = ctx.Saga.OrderId,
                    UserId = ctx.Saga.UserId,
                    ProductId = ctx.Saga.ProductId,
                    Quantity = ctx.Saga.Quantity,
                    TotalPrice = ctx.Saga.TotalPrice,
                    CreatedAt = DateTime.UtcNow,
                    Status = ctx.Message.Status
                })
                .Finalize(),

            When(OrderFailed)
                .Then(ctx => ctx.Saga.UpdatedAt = DateTime.UtcNow)
                .Send(ctx => new ReleaseProductCommand { CorrelationId = ctx.Saga.CorrelationId })
                .Publish(ctx => new OrderFailedEvent
                {
                    CorrelationId = ctx.Saga.CorrelationId,
                    FailedStep = ctx.Message.FailedStep,
                    ErrorMessage = ctx.Message.ErrorMessage
                })
                .Finalize()
        );
        
        SetCompletedWhenFinalized();
    }
}
