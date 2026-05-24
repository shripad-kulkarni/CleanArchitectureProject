using Project.Application.DTOs.Student;
using Project.Domain.Aggregates.StudentAggregate;
using Project.Domain.Enums;

namespace Project.Application.Specifications.Students
{
    // Mirrors StudentFilterSpecification criteria without paging — used for total count.
    public sealed class StudentCountSpecification : BaseSpecification<Student>
    {
        public StudentCountSpecification(StudentFilterDto filter)
        {
            var hasSearch = !string.IsNullOrWhiteSpace(filter.SearchTerm);
            var hasGender = !string.IsNullOrWhiteSpace(filter.Gender)
                && Enum.TryParse<Gender>(filter.Gender, true, out _);

            if (hasSearch && hasGender)
            {
                Enum.TryParse<Gender>(filter.Gender, true, out var genderValue);
                var term = filter.SearchTerm!.ToLower();
                AddCriteria(s => !s.IsDeleted
                    && s.Gender == genderValue
                    && (s.FirstName.ToLower().Contains(term)
                        || s.LastName.ToLower().Contains(term)
                        || s.AdmissionNumber.ToLower().Contains(term)
                        || s.RollNumber.ToLower().Contains(term)));
            }
            else if (hasSearch)
            {
                var term = filter.SearchTerm!.ToLower();
                AddCriteria(s => !s.IsDeleted
                    && (s.FirstName.ToLower().Contains(term)
                        || s.LastName.ToLower().Contains(term)
                        || s.AdmissionNumber.ToLower().Contains(term)
                        || s.RollNumber.ToLower().Contains(term)));
            }
            else if (hasGender)
            {
                Enum.TryParse<Gender>(filter.Gender, true, out var genderValue);
                AddCriteria(s => !s.IsDeleted && s.Gender == genderValue);
            }
            else
            {
                AddCriteria(s => !s.IsDeleted);
            }
        }
    }
}
