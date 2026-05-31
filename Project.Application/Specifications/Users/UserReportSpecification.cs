using Project.Application.DTOs.Report;
using Project.Domain.Aggregates.UserAggregate;
using Project.Domain.Enums;

namespace Project.Application.Specifications.Users
{
    public sealed class UserReportSpecification : BaseSpecification<User>
    {
        public UserReportSpecification(UserReportQueryDto query)
        {
            var hasSearch = !string.IsNullOrWhiteSpace(query.SearchTerm);
            var hasGender = !string.IsNullOrWhiteSpace(query.Gender)
                && Enum.TryParse<Gender>(query.Gender, true, out _);

            if (hasSearch && hasGender)
            {
                Enum.TryParse<Gender>(query.Gender, true, out var genderValue);
                var term = query.SearchTerm!.ToLower();
                AddCriteria(u => !u.IsDeleted
                    && u.Gender == genderValue
                    && (u.FirstName.ToLower().Contains(term)
                        || u.LastName.ToLower().Contains(term)
                        || u.Email.ToLower().Contains(term)));
            }
            else if (hasSearch)
            {
                var term = query.SearchTerm!.ToLower();
                AddCriteria(u => !u.IsDeleted
                    && (u.FirstName.ToLower().Contains(term)
                        || u.LastName.ToLower().Contains(term)
                        || u.Email.ToLower().Contains(term)));
            }
            else if (hasGender)
            {
                Enum.TryParse<Gender>(query.Gender, true, out var genderValue);
                AddCriteria(u => !u.IsDeleted && u.Gender == genderValue);
            }
            else
            {
                AddCriteria(u => !u.IsDeleted);
            }

            AddOrderBy(u => u.LastName);
        }
    }
}
