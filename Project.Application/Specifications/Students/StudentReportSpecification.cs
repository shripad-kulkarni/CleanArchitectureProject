using Project.Application.DTOs.Report;
using Project.Domain.Aggregates.StudentAggregate;

namespace Project.Application.Specifications.Students
{
    public sealed class StudentReportSpecification : BaseSpecification<Student>
    {
        public StudentReportSpecification(StudentReportQueryDto query)
        {
            var hasClass = !string.IsNullOrWhiteSpace(query.ClassName);
            var hasYear = !string.IsNullOrWhiteSpace(query.AcademicYear);

            if (hasClass && hasYear)
                AddCriteria(s => !s.IsDeleted
                    && s.ClassName == query.ClassName
                    && s.AcademicYear == query.AcademicYear);
            else if (hasClass)
                AddCriteria(s => !s.IsDeleted && s.ClassName == query.ClassName);
            else if (hasYear)
                AddCriteria(s => !s.IsDeleted && s.AcademicYear == query.AcademicYear);
            else
                AddCriteria(s => !s.IsDeleted);

            AddOrderBy(s => s.ClassName);
        }
    }
}
