using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Domain.Exceptions
{
    public sealed class InvalidLeaveRequestException : DomainException
    {
        public InvalidLeaveRequestException(string message)
            : base(message)
        {
        }
    }
}
