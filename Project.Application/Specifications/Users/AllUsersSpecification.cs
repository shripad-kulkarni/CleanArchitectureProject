using Project.Domain.Aggregates.UserAggregate;

namespace Project.Application.Specifications.Users
{
    public sealed class AllUsersSpecification : BaseSpecification<User>
    {
        public AllUsersSpecification()
        {
            AddOrderBy(u => u.FirstName);
        }
    }
}
