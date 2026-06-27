using System.Linq.Expressions;
using Project.Application.Specifications;
using Project.Domain.Primitives;

namespace Project.Application.Abstractions.Persistence
{
    public interface IRepository<T> where T : Entity
    {
        Task<T?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default);
        Task<List<T>> ListAsync(CancellationToken ct = default);
        Task<(List<T> Items, int TotalCount)> ListPagedAsync(IEnumerable<Expression<Func<T, bool>>>? predicates, Expression<Func<T, object>> orderByDescending, int pageNumber, int pageSize, CancellationToken ct = default);
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default);
        Task<bool> ExistsIgnoringFiltersAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default);
        Task AddAsync(T entity, CancellationToken ct = default);
        void Update(T entity);
        void Delete(T entity);

        // Specification-based methods
        Task<T?> FirstOrDefaultAsync(BaseSpecification<T> spec, CancellationToken ct = default);
        Task<List<T>> ListAsync(BaseSpecification<T> spec, CancellationToken ct = default);
        Task<int> CountAsync(BaseSpecification<T> spec, CancellationToken ct = default);
        Task<bool> ExistsAsync(BaseSpecification<T> spec, CancellationToken ct = default);
    }
}
