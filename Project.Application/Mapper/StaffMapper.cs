using Project.Application.DTOs.Staff;
using Project.Domain.Aggregates.StaffAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Application.Mapper
{
    public static class StaffMapper
    {
        public static StaffDto ToDto(Staff staff) => new(
            Id: staff.Id,
            FirstName: staff.FirstName,
            LastName: staff.LastName,
            Email: staff.Email.Value,
            Phone: staff.Phone.Value,
            DateOfBirth: staff.DateOfBirth,
            Gender: staff.Gender.ToString(),
            EmployeeCode: staff.EmployeeCode,
            Role: staff.Role.ToString(),
            JoiningDate: staff.JoiningDate,
            BasicSalary: staff.BasicSalary.Amount,
            Street: staff.Address.Street,
            City: staff.Address.City,
            State: staff.Address.State,
            PinCode: staff.Address.PinCode);
    }
}
