using CoreLib.DTOs;
namespace CoreLib.Interfaces;

public interface IUserService
{
    Task<UserDto> CreateUser(CreateUserRequest request);
    Task<UserDto?> GetUserByIdAsync(Guid id);
    Task<IEnumerable<UserDto>> GetAllUsersAsync();
    Task UpdateUserAsync(UserDto dto);
    Task DeleteUserAsync(Guid id);
    Task<UserWithPasswordDto?> GetUserByEmailAsync(string email);
}