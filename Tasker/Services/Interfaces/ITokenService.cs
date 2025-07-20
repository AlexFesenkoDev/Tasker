using Tasker.Models;

namespace Tasker.Services.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(User user);
    }
}
