using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Domain.Exceptions
{
    public sealed class StudentNotFoundException : DomainException
    {
        public StudentNotFoundException(int id)
            : base($"Student with Id '{id}' was not found.")
        {
        }

        public StudentNotFoundException(string admissionNumber)
            : base($"Student with Admission Number '{admissionNumber}' was not found.")
        {
        }
    }
}
