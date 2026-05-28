using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Project.Application.Abstractions.Persistence;
using Project.Application.Specifications;
using Project.Domain.Primitives;

namespace Project.Infrastructure.Persistence.Repositories
{
    public class GenericRepository<T> : IRepository<T> where T : AggregateRoot
    {
        protected readonly AppDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<T?> GetByIdAsync(int id, CancellationToken ct = default)
            => await _dbSet.FindAsync([id], ct);

        public async Task<T?> FirstOrDefaultAsync(BaseSpecification<T> spec, CancellationToken ct = default)
            => await SpecificationEvaluator<T>
                .GetQuery(_dbSet.AsQueryable(), spec)
                .FirstOrDefaultAsync(ct);

        public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default)
            => await _dbSet.FirstOrDefaultAsync(predicate, ct);

        public async Task<List<T>> ListAsync(BaseSpecification<T> spec, CancellationToken ct = default)
            => await SpecificationEvaluator<T>
                .GetQuery(_dbSet.AsQueryable(), spec)
                .ToListAsync(ct);

        public async Task<int> CountAsync(BaseSpecification<T> spec, CancellationToken ct = default)
            => await SpecificationEvaluator<T>
                .GetQueryWithoutPaging(_dbSet.AsQueryable(), spec)
                .CountAsync(ct);

        public async Task<bool> ExistsAsync(BaseSpecification<T> spec, CancellationToken ct = default)
            => await SpecificationEvaluator<T>
                .GetQuery(_dbSet.AsQueryable(), spec)
                .AnyAsync(ct);

        public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default)
            => await _dbSet.AnyAsync(predicate, ct);

        public async Task<bool> ExistsIgnoringFiltersAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default)
            => await _dbSet.IgnoreQueryFilters().AnyAsync(predicate, ct);

        public async Task AddAsync(T entity, CancellationToken ct = default)
            => await _dbSet.AddAsync(entity, ct);

        public void Update(T entity)
            => _dbSet.Update(entity);

        public void Delete(T entity)
            => _dbSet.Remove(entity);
    }
}
