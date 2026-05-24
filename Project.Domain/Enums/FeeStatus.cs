using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Domain.Enums
{
    public enum FeeStatus
    {
        Pending = 1,
        PartiallyPaid = 2,
        Paid = 3,
        Overdue = 4,
        Waived = 5
    }
}
