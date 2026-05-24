using Project.Domain.Enums;
using Project.Domain.Primitives;
using Project.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Domain.Aggregates.FeeAggregate
{
    public sealed class FeeInstallment : Entity
    {
        private FeeInstallment() { }

        public int FeeId { get; private set; }
        public int InstallmentNumber { get; private set; }
        public Money Amount { get; private set; } = null!;
        public DateOnly DueDate { get; private set; }
        public FeeStatus Status { get; private set; }

        public static FeeInstallment Create(
            int feeId,
            int installmentNumber,
            Money amount,
            DateOnly dueDate)
            => new()
            {
                FeeId = feeId,
                InstallmentNumber = installmentNumber,
                Amount = amount,
                DueDate = dueDate,
                Status = FeeStatus.Pending
            };

        public void MarkPaid()
            => Status = FeeStatus.Paid;

        public void MarkOverdue()
        {
            if (Status != FeeStatus.Paid)
                Status = FeeStatus.Overdue;
        }
    }
}
