using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Application.DTOs.Document
{
    public record DocumentDto(
    int Id,
    string DocumentType,
    string FileName,
    string FileUrl,
    DateTime UploadedAt);
}
