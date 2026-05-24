using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Application.DTOs.Student
{
    public record StudentDocumentDto(
    int Id,
    int StudentId,
    string DocumentType,
    string FileName,
    string FilePath,
    long FileSizeInBytes);
}
