using CoreLib.Messages.Commands;
using CoreLib.Messages.Events;
using IdentityService.Logic.Interfaces;
using MassTransit;

namespace IdentityService.Api.Consumers;

/// <summary>
/// Orchestrator: Проверяет существование пользователя
/// </summary>
public class ValidateUserCommandConsumer : IConsumer<ValidateUserCommand>
{
    private readonly IUserService _userService;

    public ValidateUserCommandConsumer(IUserService userService)
    {
        _userService = userService;
    }

    public async Task Consume(ConsumeContext<ValidateUserCommand> context)
    {
        try
        {
            var user = await _userService.GetUserByIdAsync(context.Message.UserId);

            if (user != null)
            {
                await context.Publish(new UserValidatedEvent
                {
                    CorrelationId = context.Message.CorrelationId,
                    UserId = context.Message.UserId,
                    IsValid = true,
                    ErrorMessage = null
                });
            }
            else
            {
                await context.Publish(new UserValidatedEvent
                {
                    CorrelationId = context.Message.CorrelationId,
                    UserId = context.Message.UserId,
                    IsValid = false,
                    ErrorMessage = $"User with ID {context.Message.UserId} not found"
                });
            }
        }
        catch (Exception ex)
        {
            await context.Publish(new UserValidatedEvent
            {
                CorrelationId = context.Message.CorrelationId,
                UserId = context.Message.UserId,
                IsValid = false,
                ErrorMessage = $"Internal error: {ex.Message}"
            });
        }
    }
}