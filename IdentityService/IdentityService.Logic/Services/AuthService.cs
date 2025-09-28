using CoreLib.DTOs;
using CoreLib.Interfaces;

namespace AuthService.Logic;

public class AuthService : IAuthService
{
    public Task DeleteUserAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<UserDto>> GetAllUsersAsync()
    {
        throw new NotImplementedException();
    }

    public Task<UserDto?> GetUserByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<AuthResponse> RefreshTokenAsync(string refreshToken)
    {
        throw new NotImplementedException();
    }

    public Task<UserDto> RegisterAsync(RegisterRequest request)
    {
        throw new NotImplementedException();
    }

    public Task UpdateUserAsync(UserDto dto)
    {
        throw new NotImplementedException();
    }
}
