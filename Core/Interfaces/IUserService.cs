using Core.DTOs;
using Database.Entities;

namespace Core.Interfaces
{
    public interface IUserService
    {
        Task<User?> GetUserByEmailAsync(string email);
        Task<Guid> RegisterAccountAsync(AccountUserDto account);
        Task DeleteAccountAsync(Guid userId);
        Task<User?> GetByIdAsync(Guid id);
        Task<bool> ForgotPasswordFinalizeAsync(Guid userId, string passwordHash, string confirmationToken);
        Task<bool> ConfirmAccountRegistrationAsync(Guid userId, string confirmationToken);
    }
}
