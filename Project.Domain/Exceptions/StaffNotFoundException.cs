using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Domain.Exceptions
{
    public sealed class StaffNotFoundException : DomainException
    {
        public StaffNotFoundException(Guid id)
            : base($"Staff with Id '{id}' was not found.")
        {
        }

        public StaffNotFoundException(string employeeCode)
            : base($"Staff with Employee Code '{employeeCode}' was not found.")
        {
        }
    }
}
