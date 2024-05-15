using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IEmailServiceProvider
    {
        Task<bool> SendNoReplyEmailAsync(string recipient, string subject, string content);

    }
}
