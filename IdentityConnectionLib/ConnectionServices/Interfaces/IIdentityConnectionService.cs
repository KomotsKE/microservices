namespace IdentityConnectionLib.ConnectionServices.interfaces;

public interface IIdentityConnectionService
{
    Task<bool> CheckUserExistsAsync(Guid userId, CancellationToken cancellationToken = default);
}