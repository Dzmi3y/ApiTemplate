namespace Core.DTOs
{
    public class AccountUserDto
    {
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string ConfirmationToken { get; set; } = string.Empty;
    }
}
