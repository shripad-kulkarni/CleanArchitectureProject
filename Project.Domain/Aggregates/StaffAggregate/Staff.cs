using Project.Domain.Aggregates.SalaryAggregate;
using Project.Domain.Enums;
using Project.Domain.Primitives;
using Project.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Domain.Aggregates.StaffAggregate
{
    public sealed class Staff : AggregateRoot
    {
        private readonly List<SalaryIncrement> _salaryIncrements = [];

        private Staff() { }

        public string FirstName { get; private set; } = string.Empty;
        public string LastName { get; private set; } = string.Empty;
        public string FullName => $"{FirstName} {LastName}";
        public Email Email { get; private set; } = null!;
        public PhoneNumber Phone { get; private set; } = null!;
        public DateOnly DateOfBirth { get; private set; }
        public Gender Gender { get; private set; }
        public Address Address { get; private set; } = null!;
        public string EmployeeCode { get; private set; } = string.Empty;
        public StaffRole Role { get; private set; }
        public DateOnly JoiningDate { get; private set; }
        public Money BasicSalary { get; private set; } = null!;

        public IReadOnlyCollection<SalaryIncrement> SalaryIncrements => _salaryIncrements.AsReadOnly();

        public static Staff Create(
            string firstName,
            string lastName,
            Email email,
            PhoneNumber phone,
            DateOnly dateOfBirth,
            Gender gender,
            Address address,
            string employeeCode,
            StaffRole role,
            DateOnly joiningDate,
            Money basicSalary)
        {
            if (string.IsNullOrWhiteSpace(firstName)) throw new ArgumentException("First name is required.");
            if (string.IsNullOrWhiteSpace(lastName)) throw new ArgumentException("Last name is required.");
            if (string.IsNullOrWhiteSpace(employeeCode)) throw new ArgumentException("Employee code is required.");

            return new Staff
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                Phone = phone,
                DateOfBirth = dateOfBirth,
                Gender = gender,
                Address = address,
                EmployeeCode = employeeCode,
                Role = role,
                JoiningDate = joiningDate,
                BasicSalary = basicSalary
            };
        }

        public void Update(
            string firstName,
            string lastName,
            PhoneNumber phone,
            Address address,
            StaffRole role)
        {
            if (string.IsNullOrWhiteSpace(firstName)) throw new ArgumentException("First name is required.");
            if (string.IsNullOrWhiteSpace(lastName)) throw new ArgumentException("Last name is required.");

            FirstName = firstName;
            LastName = lastName;
            Phone = phone;
            Address = address;
            Role = role;
        }

        public void UpdateSalary(Money newSalary, string reason, DateOnly effectiveDate)
        {
            ArgumentNullException.ThrowIfNull(newSalary);

            var increment = SalaryIncrement.Create(Id, BasicSalary, newSalary, reason, effectiveDate);
            _salaryIncrements.Add(increment);
            BasicSalary = newSalary;
        }
    }
}
