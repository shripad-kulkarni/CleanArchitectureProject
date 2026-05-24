using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Infrastructure.Options
{
    public sealed class EmailOptions
    {
        public const string SectionName = "Email";

        public string Host { get; init; } = string.Empty;
        public int Port { get; init; } = 587;
        public string UserName { get; init; } = string.Empty;
        public string Password { get; init; } = string.Empty;
        public string FromEmail { get; init; } = string.Empty;
        public string FromName { get; init; } = string.Empty;
        public bool EnableSsl { get; init; } = true;
    }
}
