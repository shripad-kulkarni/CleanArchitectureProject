using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Application.Abstractions.ExternalServices
{
    public interface IEmailService
    {
        Task SendAsync(string to, string subject, string htmlBody, CancellationToken ct = default);
        Task SendWithAttachmentAsync(string to, string subject, string htmlBody, Stream attachment, string attachmentName, CancellationToken ct = default);
    }
}
