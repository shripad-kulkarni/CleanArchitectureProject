using Project.Domain.Aggregates.UserAggregate;

namespace Project.Application.Specifications.Users
{
    public sealed class UserByIdSpecification : BaseSpecification<User>
    {
        public UserByIdSpecification(int id)
            : base(u => u.Id == id && !u.IsDeleted)
        {
            AddInclude(u => u.Documents);
        }
    }
}
