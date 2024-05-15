namespace API.Contracts.Requests
{
    public class ForgotPasswordConfirmationRequest
    {
        public Guid UserId { get; set; }
        public string Password { get; set; } = string.Empty;
        public string ConfirmationToken { get; set; } = string.Empty;
    }
}
