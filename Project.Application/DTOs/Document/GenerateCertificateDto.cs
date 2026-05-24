using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Application.DTOs.Document
{
    public record GenerateCertificateDto(
    int StudentId,
    string CertificateType,
    string? Purpose = null,
    string? LeavingReason = null,
    DateOnly? LeavingDate = null);
}
