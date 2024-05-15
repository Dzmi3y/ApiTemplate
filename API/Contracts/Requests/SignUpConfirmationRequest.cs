namespace API.Contracts.Requests
{
    public class SignUpConfirmationRequest
    {
        public Guid UserId { get; set; }
        public string ConfirmationToken { get; set; } = string.Empty;
    }
}
