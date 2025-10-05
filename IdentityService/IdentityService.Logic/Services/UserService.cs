using Azure.Core;
using CoreLib.DTOs;
using CoreLib.Entities;
using CoreLib.Interfaces;

namespace IdentityService.Logic;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserDto> CreateUser(CreateUserRequest request)
    {
        var user = new User
        {
            Id = request.Id,
            Name = request.Name,
            Email = request.Email,
            PasswordHash = request.PasswordHash,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        await _userRepository.AddAsync(user);
        return new UserDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
        };
    }

    public async Task DeleteUserAsync(Guid id)
    {
        await _userRepository.DeleteAsync(id);
    }

    public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
    {
        var users = await _userRepository.GetAllAsync();
        return users.Select(user => new UserDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
        });
    }

    public async Task<UserDto?> GetUserByIdAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        return user == null ? null : new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            Name = user.Name,
        };
    }

    public async Task UpdateUserAsync(UserDto dto)
    {
        var user = await _userRepository.GetByIdAsync(dto.Id) ?? throw new Exception("User not found");
        user.Name = dto.Name;
        user.UpdatedAt = DateTime.UtcNow;
        await _userRepository.UpdateAsync(user);
    }

    public async Task<UserWithPasswordDto?> GetUserByEmailAsync(string email)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        return user == null ? null : new UserWithPasswordDto
        {
            Id = user.Id,
            Email = user.Email,
            Name = user.Name,
            PasswordHash = user.PasswordHash
        };
    }
}