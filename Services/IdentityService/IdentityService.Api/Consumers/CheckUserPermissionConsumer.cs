using CoreLib.Messages.Commands;
using CoreLib.Messages.Events;
using IdentityService.Logic.Interfaces;
using MassTransit;

public class CheckUserPermissionConsumer : IConsumer<CheckUserPermissionCommand>
{
    private readonly IUserRoleService _userRoleService;

    public CheckUserPermissionConsumer(IUserRoleService userRoleService)
    {
        _userRoleService = userRoleService;
    }

    public async Task Consume(ConsumeContext<CheckUserPermissionCommand> context)
    {
        if(await _userRoleService.UserHasRoleAsync(context.Message.UserId, "ADMIN"))
        {
            await context.Publish(new UserPermissionEvent
            {
                UserId = context.Message.UserId,
                CorrelationId = context.Message.CorrelationId,
                HasPermission = true,
            });
        }
        else
        {
            await context.Publish(new UserPermissionEvent
            {
                UserId = context.Message.UserId,
                CorrelationId = context.Message.CorrelationId,
                HasPermission = false,
            });
        }
    }
}
