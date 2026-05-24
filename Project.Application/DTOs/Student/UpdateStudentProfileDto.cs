using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Application.DTOs.Student
{
    public record UpdateStudentProfileDto(
    string? BloodGroup,
    string? ParentName,
    string? ParentPhone,
    string? ParentEmail,
    string? EmergencyContact);
}
