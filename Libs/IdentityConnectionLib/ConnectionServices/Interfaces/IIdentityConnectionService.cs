using IdentityConnectionLib.ConnectionServices.DtoMidels.CheckUserExists;

namespace IdentityConnectionLib.ConnectionServices.interfaces;

public interface IIdentityConnectionService
{
    Task<CheckUserExistIdentityServiceResponce> CheckUserExistsAsync(CheckUserExistIdentityServiceRequest request);
}