using Project.Domain.Primitives;
using Project.Domain.ValueObjects;

namespace Project.Domain.Aggregates.ExpenseAggregate
{
    public sealed class Expense : AggregateRoot
    {
        private Expense() { }

        public string Title { get; private set; } = string.Empty;
        public string Category { get; private set; } = string.Empty;
        public Money Amount { get; private set; } = null!;
        public DateOnly ExpenseDate { get; private set; }
        public string? Description { get; private set; }

        public string? ReceiptFileName { get; private set; }
        public string? ReceiptFilePath { get; private set; }

        public static Expense Create(
            string title,
            string category,
            Money amount,
            DateOnly expenseDate,
            string? description = null)
        {
            if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Title is required.");
            if (string.IsNullOrWhiteSpace(category)) throw new ArgumentException("Category is required.");

            return new Expense
            {
                Title = title,
                Category = category,
                Amount = amount,
                ExpenseDate = expenseDate,
                Description = description
            };
        }

        public void Update(string title, string category, Money amount, string? description)
        {
            if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Title is required.");
            if (string.IsNullOrWhiteSpace(category)) throw new ArgumentException("Category is required.");

            Title = title;
            Category = category;
            Amount = amount;
            Description = description;
        }

        public void AttachReceipt(string fileName, string filePath)
        {
            if (string.IsNullOrWhiteSpace(fileName)) throw new ArgumentException("File name is required.");
            if (string.IsNullOrWhiteSpace(filePath)) throw new ArgumentException("File path is required.");

            ReceiptFileName = fileName;
            ReceiptFilePath = filePath;
        }

        public void RemoveReceipt()
        {
            ReceiptFileName = null;
            ReceiptFilePath = null;
        }
    }
}
