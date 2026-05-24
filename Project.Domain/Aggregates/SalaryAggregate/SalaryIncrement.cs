using Project.Domain.Primitives;
using Project.Domain.ValueObjects;

namespace Project.Domain.Aggregates.SalaryAggregate
{
    public sealed class SalaryIncrement : Entity
    {
        private SalaryIncrement() { }

        public int StaffId { get; private set; }
        public Money PreviousSalary { get; private set; } = null!;
        public Money NewSalary { get; private set; } = null!;
        public string Reason { get; private set; } = string.Empty;
        public DateOnly EffectiveDate { get; private set; }

        public static SalaryIncrement Create(
            int staffId,
            Money previousSalary,
            Money newSalary,
            string reason,
            DateOnly effectiveDate)
        {
            if (string.IsNullOrWhiteSpace(reason))
                throw new ArgumentException("Reason is required.");

            if (newSalary.Amount <= previousSalary.Amount)
                throw new ArgumentException("New salary must be greater than previous salary.");

            return new SalaryIncrement
            {
                StaffId = staffId,
                PreviousSalary = previousSalary,
                NewSalary = newSalary,
                Reason = reason,
                EffectiveDate = effectiveDate
            };
        }
    }
}
