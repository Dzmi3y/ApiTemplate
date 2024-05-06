using Core.DTOs;
using Database.Entities;

namespace Core.Interfaces
{
    public interface IUserService
    {
        Task<User?> GetUserByEmailAsync(string email);
        Task<Guid> RegisterAccountAsync(AccountUserDto account);
    }
}
