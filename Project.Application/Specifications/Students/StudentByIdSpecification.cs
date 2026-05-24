using Project.Domain.Aggregates.StudentAggregate;

namespace Project.Application.Specifications.Students
{
    public sealed class StudentByIdSpecification : BaseSpecification<Student>
    {
        public StudentByIdSpecification(int id)
            : base(s => s.Id == id && !s.IsDeleted)
        {
            AddInclude(s => s.Documents);
        }
    }
}
