using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Application.DTOs.Report
{
    public record DashboardSummaryDto(
    int TotalStudents,
    int ActiveStudents,
    int TotalStaff,
    int ActiveStaff,
    decimal TotalFeesCollectedThisMonth,
    decimal PendingFeesTotal,
    decimal TotalExpensesThisMonth,
    int PendingLeaveRequests);
}
