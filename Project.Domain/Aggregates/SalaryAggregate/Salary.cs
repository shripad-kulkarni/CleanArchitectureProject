using Project.Domain.Enums;
using Project.Domain.Primitives;
using Project.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Domain.Aggregates.SalaryAggregate
{
    public sealed class Salary : AggregateRoot
    {
        private Salary() { }

        public int StaffId { get; private set; }
        public int Month { get; private set; }
        public int Year { get; private set; }
        public Money BasicSalary { get; private set; } = null!;
        public Money Allowances { get; private set; } = null!;
        public Money Deductions { get; private set; } = null!;
        public Money NetSalary { get; private set; } = null!;
        public SalaryStatus Status { get; private set; }

        public static Salary Create(
            int staffId,
            int month,
            int year,
            Money basicSalary,
            Money allowances,
            Money deductions)
        {
            if (month < 1 || month > 12) throw new ArgumentException("Invalid month.");
            if (year < 2000) throw new ArgumentException("Invalid year.");

            return new Salary
            {
                StaffId = staffId,
                Month = month,
                Year = year,
                BasicSalary = basicSalary,
                Allowances = allowances,
                Deductions = deductions,
                NetSalary = Money.Create(basicSalary.Amount + allowances.Amount - deductions.Amount),
                Status = SalaryStatus.Pending
            };
        }

        public void MarkProcessed()
        {
            if (Status != SalaryStatus.Pending)
                throw new InvalidOperationException("Only pending salary can be processed.");

            Status = SalaryStatus.Processed;
        }

        public void MarkPaid()
        {
            if (Status != SalaryStatus.Processed)
                throw new InvalidOperationException("Only processed salary can be marked paid.");

            Status = SalaryStatus.Paid;
        }

        public void PutOnHold()
        {
            if (Status == SalaryStatus.Paid)
                throw new InvalidOperationException("Cannot hold an already paid salary.");

            Status = SalaryStatus.OnHold;
        }
    }
}
