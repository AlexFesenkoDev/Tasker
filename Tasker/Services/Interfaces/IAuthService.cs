using Tasker.Models.Dtos;

namespace Tasker.Services.Interfaces
{
    public interface IAuthService
    {
        Task<(bool IsSuccess, string? ErrorMessage)> RegisterAsync(RegisterDto dto);
        Task<(bool IsSuccess, string? Token, string? ErrorMessage)> LoginAsync(LoginDto dto);
    }
}
