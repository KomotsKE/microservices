namespace IdentityConnectionLib.ConnectionServices.DtoMidels.CheckUserExists;

public record CheckUserExistIdentityServiceRequest
{
    public required Guid UserId { get; set; }
}