namespace Core.Interfaces
{
    public interface IEmailServiceProvider
    {
        Task<bool> SendNoReplyEmailAsync(string recipient, string subject, string content);

    }
}
