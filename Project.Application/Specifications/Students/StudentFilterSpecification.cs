using Project.Domain.Aggregates.StudentAggregate;
using Project.Domain.Enums;
using Project.Application.DTOs.Student;

namespace Project.Application.Specifications.Students
{
    public sealed class StudentFilterSpecification : BaseSpecification<Student>
    {
        public StudentFilterSpecification(StudentFilterDto filter)
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

            AddOrderBy(s => s.FirstName);

            var skip = (filter.PageNumber - 1) * filter.PageSize;
            EnablePaging(skip, filter.PageSize);
        }
    }
}
