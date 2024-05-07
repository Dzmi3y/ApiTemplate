using System.ComponentModel.DataAnnotations;

namespace API.Contracts.Requests
{
    public class RefreshTokenRequest
    {
        [Required]
        public Guid RefreshToken { get; set; }
    }

}
