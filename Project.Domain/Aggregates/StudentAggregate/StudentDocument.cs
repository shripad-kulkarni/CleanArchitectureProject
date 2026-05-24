using Project.Domain.Enums;
using Project.Domain.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Domain.Aggregates.StudentAggregate
{
    public sealed class StudentDocument : Entity
    {
        private StudentDocument() { }

        public int StudentId { get; private set; }
        public DocumentType DocumentType { get; private set; }
        public string FileName { get; private set; } = string.Empty;
        public string FilePath { get; private set; } = string.Empty;
        public long FileSizeInBytes { get; private set; }

        public static StudentDocument Create(
            int studentId,
            DocumentType documentType,
            string fileName,
            string filePath,
            long fileSizeInBytes)
        {
            if (string.IsNullOrWhiteSpace(fileName)) throw new ArgumentException("File name is required.");
            if (string.IsNullOrWhiteSpace(filePath)) throw new ArgumentException("File path is required.");
            if (fileSizeInBytes <= 0) throw new ArgumentException("File size must be greater than zero.");

            return new StudentDocument
            {
                StudentId = studentId,
                DocumentType = documentType,
                FileName = fileName,
                FilePath = filePath,
                FileSizeInBytes = fileSizeInBytes
            };
        }
    }
}
