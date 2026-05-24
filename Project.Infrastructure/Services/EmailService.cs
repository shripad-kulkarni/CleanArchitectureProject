using Microsoft.Extensions.Options;
using Project.Application.Abstractions.ExternalServices;
using Project.Infrastructure.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Project.Infrastructure.Services
{
    public sealed class EmailService : IEmailService
    {
        private readonly EmailOptions _options;

        public EmailService(IOptions<EmailOptions> options)
        {
            _options = options.Value;
        }

        public async Task SendAsync(
            string to, string subject, string htmlBody,
            CancellationToken ct = default)
        {
            using var client = CreateSmtpClient();
            using var message = CreateMessage(to, subject, htmlBody);
            await client.SendMailAsync(message, ct);
        }

        public async Task SendWithAttachmentAsync(
            string to, string subject, string htmlBody,
            Stream attachment, string attachmentName,
            CancellationToken ct = default)
        {
            using var client = CreateSmtpClient();
            using var message = CreateMessage(to, subject, htmlBody);
            message.Attachments.Add(new Attachment(attachment, attachmentName));
            await client.SendMailAsync(message, ct);
        }

        private SmtpClient CreateSmtpClient() => new(_options.Host, _options.Port)
        {
            Credentials = new NetworkCredential(_options.UserName, _options.Password),
            EnableSsl = _options.EnableSsl
        };

        private MailMessage CreateMessage(string to, string subject, string htmlBody) => new()
        {
            From = new MailAddress(_options.FromEmail, _options.FromName),
            Subject = subject,
            Body = htmlBody,
            IsBodyHtml = true,
            To = { to }
        };
    }
}
