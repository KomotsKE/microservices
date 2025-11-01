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
    // private readonly ILogger<ValidateUserCommandConsumer> _logger;

    public ValidateUserCommandConsumer(IUserService userService)
    {
        _userService = userService;
        // _logger = logger;
    }

    public async Task Consume(ConsumeContext<ValidateUserCommand> context)
    {
        // _logger.LogInformation("[IDENTITY] Validating user: {UserId}", context.Message.UserId);

        try
        {
            var user = await _userService.GetUserByIdAsync(context.Message.UserId);

            if (user != null)
            {
                // _logger.LogInformation("[IDENTITY] User found: {UserId}", context.Message.UserId);

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
                // _logger.LogWarning("[IDENTITY] User not found: {UserId}", context.Message.UserId);

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
            // _logger.LogError(ex, "[IDENTITY] Error validating user: {UserId}", context.Message.UserId);

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