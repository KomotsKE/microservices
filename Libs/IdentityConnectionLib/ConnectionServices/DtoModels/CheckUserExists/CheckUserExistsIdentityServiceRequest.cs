namespace IdentityConnectionLib.ConnectionServices.DtoMidels.CheckUserExists;

public record CheckUserExistIdentityServiceRequest
{
    public required Guid userId { get; set; }
}