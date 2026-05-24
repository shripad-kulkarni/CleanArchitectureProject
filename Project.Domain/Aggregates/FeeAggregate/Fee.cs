using Project.Domain.Constants;
using Project.Domain.Enums;
using Project.Domain.Exceptions;
using Project.Domain.Primitives;
using Project.Domain.ValueObjects;

namespace Project.Domain.Aggregates.FeeAggregate
{
    public sealed class Fee : AggregateRoot
    {
        private readonly List<FeeInstallment> _installments = [];

        private Fee() { }

        public int StudentId { get; private set; }
        public string FeeName { get; private set; } = string.Empty;
        public Money TotalAmount { get; private set; } = null!;
        public Money PaidAmount { get; private set; } = Money.Create(0);
        public Money RemainingAmount => TotalAmount.Subtract(PaidAmount);
        public DateOnly DueDate { get; private set; }
        public int TotalInstallments { get; private set; }
        public FeeStatus Status { get; private set; }

        public string? PaymentMode { get; private set; }
        public string? TransactionReference { get; private set; }
        public DateOnly? PaymentDate { get; private set; }

        public IReadOnlyCollection<FeeInstallment> Installments => _installments.AsReadOnly();

        public static Fee Create(
            int studentId,
            string feeName,
            Money totalAmount,
            DateOnly dueDate,
            int totalInstallments = 1)
        {
            if (string.IsNullOrWhiteSpace(feeName))
                throw new InvalidFeeOperationException("Fee name is required.");

            if (totalInstallments < 1 || totalInstallments > FeeConstants.MaxInstallments)
                throw new InvalidFeeOperationException($"Installments must be between 1 and {FeeConstants.MaxInstallments}.");

            return new Fee
            {
                StudentId = studentId,
                FeeName = feeName,
                TotalAmount = totalAmount,
                DueDate = dueDate,
                TotalInstallments = totalInstallments,
                Status = FeeStatus.Pending
            };
        }

        public void AddInstallment(FeeInstallment installment)
        {
            if (_installments.Count >= TotalInstallments)
                throw new InvalidFeeOperationException("Cannot exceed total installments limit.");

            _installments.Add(installment);
        }

        public void RecordPayment(Money amountPaid, string paymentMode, string? transactionReference, DateOnly paymentDate)
        {
            if (string.IsNullOrWhiteSpace(paymentMode))
                throw new InvalidFeeOperationException("Payment mode is required.");

            PaidAmount = PaidAmount.Add(amountPaid);
            PaymentMode = paymentMode;
            TransactionReference = transactionReference;
            PaymentDate = paymentDate;

            Status = PaidAmount.Amount >= TotalAmount.Amount
                ? FeeStatus.Paid
                : FeeStatus.PartiallyPaid;
        }

        public void MarkOverdue()
        {
            if (Status == FeeStatus.Paid) return;
            Status = FeeStatus.Overdue;
        }

        public void Waive()
        {
            if (Status == FeeStatus.Paid)
                throw new InvalidFeeOperationException("Cannot waive an already paid fee.");

            Status = FeeStatus.Waived;
        }
    }
}