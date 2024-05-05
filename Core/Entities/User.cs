using Core.Entities.Base;
using System.ComponentModel.DataAnnotations;


namespace Core.Entities
{
    public class User : AuditableEntity
    {
        [Key]
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public bool Confirmed { get; set; }
        public string? ConfirmationToken { get; set; }
        public DateTime? ConfirmationTokenExpiryDate { get; set; }
    }

}
