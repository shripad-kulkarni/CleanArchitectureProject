using Project.Domain.Enums;
using Project.Domain.Primitives;

namespace Project.Domain.Aggregates.UserAggregate
{
    public sealed class UserDocument : Entity
    {
        private UserDocument() { }

        public int UserId { get; private set; }
        public DocumentType DocumentType { get; private set; }
        public string FileName { get; private set; } = string.Empty;
        public string FilePath { get; private set; } = string.Empty;
        public long FileSizeInBytes { get; private set; }

        public static UserDocument Create(
            int userId,
            DocumentType documentType,
            string fileName,
            string filePath,
            long fileSizeInBytes)
        {
            if (string.IsNullOrWhiteSpace(fileName)) throw new ArgumentException("File name is required.");
            if (string.IsNullOrWhiteSpace(filePath)) throw new ArgumentException("File path is required.");
            if (fileSizeInBytes <= 0) throw new ArgumentException("File size must be greater than zero.");

            return new UserDocument
            {
                UserId = userId,
                DocumentType = documentType,
                FileName = fileName,
                FilePath = filePath,
                FileSizeInBytes = fileSizeInBytes
            };
        }
    }
}
