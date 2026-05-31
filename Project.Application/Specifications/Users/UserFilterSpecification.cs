using Project.Application.DTOs.User;
using Project.Domain.Aggregates.UserAggregate;
using Project.Domain.Enums;

namespace Project.Application.Specifications.Users
{
    public sealed class UserFilterSpecification : BaseSpecification<User>
    {
        public UserFilterSpecification(UserFilterDto filter)
        {
            var hasSearch = !string.IsNullOrWhiteSpace(filter.SearchTerm);
            var hasGender = !string.IsNullOrWhiteSpace(filter.Gender)
                && Enum.TryParse<Gender>(filter.Gender, true, out _);

            if (hasSearch && hasGender)
            {
                Enum.TryParse<Gender>(filter.Gender, true, out var genderValue);
                var term = filter.SearchTerm!.ToLower();
                AddCriteria(u => !u.IsDeleted
                    && u.Gender == genderValue
                    && (u.FirstName.ToLower().Contains(term)
                        || u.LastName.ToLower().Contains(term)
                        || u.Email.ToLower().Contains(term)));
            }
            else if (hasSearch)
            {
                var term = filter.SearchTerm!.ToLower();
                AddCriteria(u => !u.IsDeleted
                    && (u.FirstName.ToLower().Contains(term)
                        || u.LastName.ToLower().Contains(term)
                        || u.Email.ToLower().Contains(term)));
            }
            else if (hasGender)
            {
                Enum.TryParse<Gender>(filter.Gender, true, out var genderValue);
                AddCriteria(u => !u.IsDeleted && u.Gender == genderValue);
            }
            else
            {
                AddCriteria(u => !u.IsDeleted);
            }

            AddOrderBy(u => u.LastName);

            var skip = (filter.PageNumber - 1) * filter.PageSize;
            EnablePaging(skip, filter.PageSize);
        }
    }
}
