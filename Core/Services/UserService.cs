using Core.DTOs;
using Core.Interfaces;
using Database;
using Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Core.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _configuration;

        public UserService(AppDbContext db, IConfiguration configuration)
        {
            _db = db;
            _configuration = configuration;
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            return await _db.Users.AsNoTracking().SingleOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _db.Users.AsNoTracking()
                .SingleOrDefaultAsync(u => u.Email == email);
        }

        public async Task<Guid> RegisterAccountAsync(AccountUserDto user)
        {
            var tokenLifeTime =
                TimeSpan.Parse(_configuration.GetSection("AccountSettings:RegisterConfirmationCodeLifetime")
                    .Value); //todo map thru config class
            var newUser = new User
            {
                Email = user.Email,
                PasswordHash = user.PasswordHash,
                Name = user.Name,
                ConfirmationToken = user.ConfirmationToken,
                ConfirmationTokenExpiryDate = DateTime.UtcNow.Add(tokenLifeTime),
            };
            await _db.Users.AddAsync(newUser);
            await _db.SaveChangesAsync();
            return newUser.Id;
        }

        public async Task DeleteAccountAsync(Guid userId)
        {
            var user = await _db.Users.SingleOrDefaultAsync(u => u.Id == userId);
            if (user == null) return;

            user.IsDeleted = true;
            await _db.SaveChangesAsync();
        }

        public async Task<bool> ForgotPasswordFinalizeAsync(Guid userId, string passwordHash, string confirmationToken)
        {
            var user = await _db.Users.SingleOrDefaultAsync(u =>
                u.Id == userId && u.ConfirmationToken == confirmationToken);
            if (user == null) return false;

            user.PasswordHash = passwordHash;
            user.ConfirmationToken = null;
            user.ConfirmationTokenExpiryDate = null;
            await _db.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ConfirmAccountRegistrationAsync(Guid userId, string confirmationToken)
        {
            var user = await _db.Users.SingleOrDefaultAsync(u =>
                u.Id == userId && u.ConfirmationToken == confirmationToken);
            if (user == null) return false;

            user.Confirmed = true;
            user.ConfirmationToken = null;
            user.ConfirmationTokenExpiryDate = null;
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
