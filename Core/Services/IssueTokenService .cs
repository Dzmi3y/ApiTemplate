using Core.IdentityConfig;
using Core.Interfaces;
using Core.Models;
using Database.Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Core.Services
{
    public class IssueTokenService : IIssueTokenService
    {
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly JwtSettings _jwtSettings;

        public IssueTokenService(JwtSettings jwtSettings, IRefreshTokenService refreshTokenService)
        {
            _jwtSettings = jwtSettings;
            _refreshTokenService = refreshTokenService;
        }

        public async Task<AuthenticationResult> GenerateAuthenticationResult(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtSettings.Secret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                }),
                Expires = DateTime.UtcNow.Add(_jwtSettings.AccessTokenLifetime),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            tokenDescriptor.Subject.AddClaim(new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()));

            // custom claims to be populated here...
            tokenDescriptor.Subject.AddClaim(new Claim("name", user.Name));
            tokenDescriptor.Subject.AddClaim(new Claim(JwtRegisteredClaimNames.Email, user.Email));
            //

            var accessToken = tokenHandler.CreateToken(tokenDescriptor);
            var refreshToken = await _refreshTokenService.CreateAsync(new RefreshTokenInfo
            {
                UserId = user.Id,
                JwtId = accessToken.Id,
                ExpiryDateUtc = DateTime.UtcNow.Add(_jwtSettings.RefreshTokenLifetime)
            });

            return new AuthenticationResult
            {
                AccessToken = tokenHandler.WriteToken(accessToken),
                ExpiresIn = (int)_jwtSettings.AccessTokenLifetime.TotalSeconds,
                RefreshToken = refreshToken
            };
        }

        public async Task<AuthenticationResult> RefreshTokenAsync(Guid refreshToken)
        {
            var storedRefreshToken = await _refreshTokenService.GetAsync(refreshToken);

            if (storedRefreshToken == null)
                return new AuthenticationResult { Error = IdentityErrorCode.RefreshTokenNotExists };

            if (DateTime.UtcNow > storedRefreshToken.ExpiryDateUtc)
                return new AuthenticationResult { Error = IdentityErrorCode.RefreshTokenExpired };

            if (storedRefreshToken.Invalidated)
                return new AuthenticationResult { Error = IdentityErrorCode.RefreshTokenInvalidated };

            if (storedRefreshToken.Used)
                return new AuthenticationResult { Error = IdentityErrorCode.RefreshTokenUsed };

            var user = await _refreshTokenService.GetUserByRefreshTokenAsync(refreshToken);
            if (user == null)
                return new AuthenticationResult { Error = IdentityErrorCode.NoAssociatedUser };

            await _refreshTokenService.SetAsUsedAsync(storedRefreshToken.Token);
            return await GenerateAuthenticationResult(user);
        }
    }

}
