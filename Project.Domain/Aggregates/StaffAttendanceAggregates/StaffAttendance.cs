using Project.Domain.Enums;
using Project.Domain.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Domain.Aggregates.StaffAttendanceAggregates
{
    public sealed class StaffAttendance : AggregateRoot
    {
        private StaffAttendance() { }

        public int StaffId { get; private set; }
        public DateOnly Date { get; private set; }
        public AttendanceStatus Status { get; private set; }
        public TimeOnly? CheckIn { get; private set; }
        public TimeOnly? CheckOut { get; private set; }
        public string? Remarks { get; private set; }

        public static StaffAttendance Create(
            int staffId,
            DateOnly date,
            AttendanceStatus status,
            TimeOnly? checkIn = null,
            TimeOnly? checkOut = null,
            string? remarks = null)
            => new()
            {
                StaffId = staffId,
                Date = date,
                Status = status,
                CheckIn = checkIn,
                CheckOut = checkOut,
                Remarks = remarks
            };

        public void UpdateStatus(AttendanceStatus status, string? remarks = null)
        {
            Status = status;
            Remarks = remarks;
        }
    }
}
