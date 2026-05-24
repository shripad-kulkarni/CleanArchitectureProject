using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Domain.ValueObjects
{
    public sealed class DateRange
    {
        public DateOnly StartDate { get; }
        public DateOnly EndDate { get; }
        public int TotalDays => EndDate.DayNumber - StartDate.DayNumber + 1;

        private DateRange(DateOnly startDate, DateOnly endDate)
        {
            StartDate = startDate;
            EndDate = endDate;
        }

        public static DateRange Create(DateOnly startDate, DateOnly endDate)
        {
            if (endDate < startDate)
                throw new ArgumentException("End date cannot be before start date.");

            return new DateRange(startDate, endDate);
        }

        public bool Overlaps(DateRange other)
            => StartDate <= other.EndDate && EndDate >= other.StartDate;

        public bool Contains(DateOnly date)
            => date >= StartDate && date <= EndDate;

        public override string ToString()
            => $"{StartDate:dd MMM yyyy} to {EndDate:dd MMM yyyy} ({TotalDays} days)";
    }
}
