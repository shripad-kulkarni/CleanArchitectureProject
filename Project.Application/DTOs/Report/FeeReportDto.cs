using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Application.DTOs.Report
{
    public record FeeReportDto(
    string AcademicYear,
    decimal TotalFeesDue,
    decimal TotalFeesCollected,
    decimal TotalFeesPending,
    int TotalStudents,
    int FullyPaidStudents,
    int PartiallyPaidStudents,
    int UnpaidStudents);
}
