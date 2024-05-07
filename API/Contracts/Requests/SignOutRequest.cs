using System.ComponentModel.DataAnnotations;

namespace API.Contracts.Requests
{
    public class SignOutRequest
    {
        [Required]
        public Guid RefreshToken { get; set; }
    }

}
