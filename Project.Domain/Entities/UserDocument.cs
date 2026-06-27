using Project.Domain.Enums;
using Project.Domain.Primitives;

namespace Project.Domain.Entities
{
    public sealed class UserDocument : Entity
    {
        private UserDocument() { }

        public UserDocument(
            int userId,
            DocumentType documentType,
            string fileName,
            string filePath,
            long fileSizeInBytes)
        {
            UserId          = userId;
            DocumentType    = documentType;
            FileName        = fileName;
            FilePath        = filePath;
            FileSizeInBytes = fileSizeInBytes;
        }

        public int UserId { get; private set; }
        public DocumentType DocumentType { get; private set; }
        public string FileName { get; private set; } = string.Empty;
        public string FilePath { get; private set; } = string.Empty;
        public long FileSizeInBytes { get; private set; }
    }
}
