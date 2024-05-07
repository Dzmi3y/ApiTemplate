using Database.Entities;
using Core.Models;

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
