using Project.Application.Abstractions.ExternalServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Infrastructure.Services
{
    public sealed class IndianDateTimeProvider : IDateTimeProvider
    {
        private static readonly TimeZoneInfo IndiaTimeZone =
            TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");

        public DateTime UtcNow => DateTime.UtcNow;

        public DateTime Now => TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, IndiaTimeZone);

        public DateOnly Today => DateOnly.FromDateTime(Now);
    }
}
