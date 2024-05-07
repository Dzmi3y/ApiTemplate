using Core.Models;
using Database.Entities;

namespace Core.Interfaces
{
    public interface IIssueTokenService
    {
        Task<AuthenticationResult> RefreshTokenAsync(Guid refreshToken);
        Task<AuthenticationResult> GenerateAuthenticationResult(User user);
    }

}
