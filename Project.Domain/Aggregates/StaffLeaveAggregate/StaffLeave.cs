using Project.Domain.Enums;
using Project.Domain.Exceptions;
using Project.Domain.Primitives;
using Project.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Domain.Aggregates.StaffLeaveAggregate
{
    public sealed class StaffLeave : AggregateRoot
    {
        private StaffLeave() { }

        public int StaffId { get; private set; }
        public LeaveType LeaveType { get; private set; }
        public DateRange DateRange { get; private set; } = null!;
        public string Reason { get; private set; } = string.Empty;
        public LeaveStatus Status { get; private set; }
        public string? RejectionReason { get; private set; }
        public int? ApprovedById { get; private set; }

        public static StaffLeave Create(
            int staffId,
            LeaveType leaveType,
            DateRange dateRange,
            string reason)
        {
            if (string.IsNullOrWhiteSpace(reason))
                throw new InvalidLeaveRequestException("Leave reason is required.");

            return new StaffLeave
            {
                StaffId = staffId,
                LeaveType = leaveType,
                DateRange = dateRange,
                Reason = reason,
                Status = LeaveStatus.Pending
            };
        }

        public void Approve(int approvedById)
        {
            if (Status != LeaveStatus.Pending)
                throw new InvalidLeaveRequestException("Only pending leave requests can be approved.");

            Status = LeaveStatus.Approved;
            ApprovedById = approvedById;
        }

        public void Reject(int rejectedById, string reason)
        {
            if (Status != LeaveStatus.Pending)
                throw new InvalidLeaveRequestException("Only pending leave requests can be rejected.");

            if (string.IsNullOrWhiteSpace(reason))
                throw new InvalidLeaveRequestException("Rejection reason is required.");

            Status = LeaveStatus.Rejected;
            ApprovedById = rejectedById;
            RejectionReason = reason;
        }

        public void Cancel()
        {
            if (Status != LeaveStatus.Pending)
                throw new InvalidLeaveRequestException("Only pending leave requests can be cancelled.");

            Status = LeaveStatus.Cancelled;
        }
    }
}
