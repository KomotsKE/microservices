using CoreLib.Messages.Commands;
using CoreLib.Messages.Events;
using MassTransit;

namespace OrderService.API.Sagas;
public class OrderUpdateSaga : MassTransitStateMachine<OrderUpdateSagaState>
{
    public State ValidatingUser { get; private set; } = null!;
    public State CheckingPermission { get; private set; } = null!;
    public State UpdatingOrder {get; private set; } = null!;


    public Event<OrderUpdateCommand> UpdateRequested { get; private set; } = null!;
    public Event<UserValidatedEvent> UserValidated { get; private set; } = null!;
    public Event<UserPermissionEvent> UserHasPermission { get; private set; } = null!;
    public Event<OrderUpdatedEvent> OrderUpdated { get; private set; } = null!;
    public Event<OrderFailedEvent> OrderFailed { get; private set; } = null!;
   
    public OrderUpdateSaga()
    {
        InstanceState(x => x.CurrentState);

        Event(() => UpdateRequested, x => x.CorrelateById(context => context.Message.CorrelationId));
        Event(() => UserValidated, x => x.CorrelateById(context => context.Message.CorrelationId));
        Event(() => UserHasPermission, x => x.CorrelateById(context => context.Message.CorrelationId));
        Event(() => OrderFailed, x => x.CorrelateById(context => context.Message.CorrelationId));

        Initially(
            When(UpdateRequested)
                .Then(context =>
                {
                    context.Saga.CorrelationId = context.Message.CorrelationId;
                    context.Saga.OrderId = context.Message.OrderId;
                    context.Saga.UserId = context.Message.UserId;
                    context.Saga.NewStatus = context.Message.NewStatus;
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
                .Send(ctx => new CheckUserPermissionCommand
                {
                    CorrelationId = ctx.Saga.CorrelationId,
                    OrderId = ctx.Saga.OrderId,
                    UserId = ctx.Saga.UserId
                })
                .TransitionTo(CheckingPermission),
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

        During(CheckingPermission,
            When(UserHasPermission)
                .IfElse(context => context.Message.HasPermission,
                binder => binder
                .Send(ctx => new OrderUpdatedEvent
                {
                    CorrelationId = ctx.Saga.CorrelationId,
                    OrderId = ctx.Saga.OrderId,
                    UserId = ctx.Saga.UserId,
                    Status = ctx.Saga.NewStatus,
                })
                .TransitionTo(UpdatingOrder),

                binder => binder
                .Publish(ctx => new OrderFailedEvent
                {
                    CorrelationId = ctx.Saga.CorrelationId,
                    FailedStep = "CheckingPermission",
                })
                .Finalize()
            )
        );

        During(UpdatingOrder,
            When(OrderUpdated)
                .Finalize()
        );
    }
}
