using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Application.DTOs.Student
{
    public record StudentFilterDto(
    string? SearchTerm = null,
    string? Gender = null,
    bool? IsActive = null,
    int PageNumber = 1,
    int PageSize = 10);
}
