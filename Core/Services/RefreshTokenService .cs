using Core.Interfaces;
using Core.Models;
using Database;
using Database.Entities;
using Database.Entities.Base;
using Microsoft.EntityFrameworkCore;

namespace Core.Services
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly AppDbContext _db;

        public RefreshTokenService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<User?> GetUserByRefreshTokenAsync(Guid refreshToken)
        {
            return (await _db.RefreshTokens.Include(x => x.User).AsNoTracking()
                .SingleOrDefaultAsync(rt => rt.Token == refreshToken))?.User;
        }

        public async Task<RefreshTokenInfo> GetAsync(Guid token)
        {
            var refreshToken = await _db.RefreshTokens.AsNoTracking().SingleOrDefaultAsync(t => t.Token == token);
            if (refreshToken == null) return null;
            return new RefreshTokenInfo
            {
                Token = refreshToken.Token,
                UserId = refreshToken.UserId,
                JwtId = refreshToken.JwtId,
                ExpiryDateUtc = refreshToken.ExpiryDateUtc,
                Invalidated = refreshToken.Invalidated,
                Used = refreshToken.Used
            };
        }

        public async Task SetAsUsedAsync(Guid token)
        {
            var refreshToken = await _db.RefreshTokens.AsNoTracking().SingleOrDefaultAsync(t => t.Token == token);
            refreshToken.Used = true;
            _db.RefreshTokens.Update(refreshToken);
            await _db.SaveChangesAsync();
        }

        public async Task SetAsInvalidatedAsync(Guid token)
        {
            var refreshToken = await _db.RefreshTokens.AsNoTracking().SingleOrDefaultAsync(t => t.Token == token);
            refreshToken.Invalidated = true;
            _db.RefreshTokens.Update(refreshToken);
            await _db.SaveChangesAsync();
        }

        public async Task<Guid> CreateAsync(RefreshTokenInfo tokenInfo)
        {
            var refreshToken = new RefreshToken
            {
                UserId = tokenInfo.UserId,
                JwtId = tokenInfo.JwtId,
                ExpiryDateUtc = tokenInfo.ExpiryDateUtc
            };
            await _db.RefreshTokens.AddAsync(refreshToken);
            await _db.SaveChangesAsync();
            return refreshToken.Token;
        }
    }

}
