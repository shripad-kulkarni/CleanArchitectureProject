using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Application.DTOs.Document
{
    public record UploadDocumentDto(
    int EntityId,
    string EntityType,
    string DocumentType,
    string FileName,
    Stream FileStream);
}
