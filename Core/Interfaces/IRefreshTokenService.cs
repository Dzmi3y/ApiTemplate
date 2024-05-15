using Core.Models;
using Database.Entities;

namespace Core.Interfaces
{
    public interface IRefreshTokenService
    {
        Task<RefreshTokenInfo> GetAsync(Guid token);
        Task SetAsUsedAsync(Guid token);
        Task SetAsInvalidatedAsync(Guid token);
        Task<Guid> CreateAsync(RefreshTokenInfo tokenInfo);
        Task<User?> GetUserByRefreshTokenAsync(Guid refreshToken);
    }

}
