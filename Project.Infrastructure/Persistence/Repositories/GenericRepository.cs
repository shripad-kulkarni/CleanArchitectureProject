using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Project.Application.Abstractions.Persistence;
using Project.Application.Specifications;
using Project.Domain.Primitives;

namespace Project.Infrastructure.Persistence.Repositories
{
    public class GenericRepository<T> : IRepository<T> where T : Entity
    {
        protected readonly AppDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<T?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            return await _dbSet.FindAsync([id], ct);
        }

        public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default)
        {
            return await _dbSet.FirstOrDefaultAsync(predicate, ct);
        }

        public async Task<List<T>> ListAsync(CancellationToken ct = default)
        {
            return await _dbSet.ToListAsync(ct);
        }

        public async Task<(List<T> Items, int TotalCount)> ListPagedAsync(
            IEnumerable<Expression<Func<T, bool>>>? predicates,
            Expression<Func<T, object>> orderByDescending,
            int pageNumber,
            int pageSize,
            CancellationToken ct = default)
        {
            var query = _dbSet.AsQueryable();

            if (predicates != null)
            {
                foreach (var predicate in predicates)
                {
                    query = query.Where(predicate);
                }
            }

            var totalCount = await query.CountAsync(ct);

            var items = await query
                .OrderByDescending(orderByDescending)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            return (items, totalCount);
        }

        public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default)
        {
            return await _dbSet.AnyAsync(predicate, ct);
        }

        public async Task<bool> ExistsIgnoringFiltersAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default)
        {
            return await _dbSet.IgnoreQueryFilters().AnyAsync(predicate, ct);
        }

        public async Task AddAsync(T entity, CancellationToken ct = default)
        {
            await _dbSet.AddAsync(entity, ct);
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        // Specification-based methods
        public async Task<T?> FirstOrDefaultAsync(BaseSpecification<T> spec, CancellationToken ct = default)
        {
            return await SpecificationEvaluator<T>.GetQuery(_dbSet.AsQueryable(), spec).FirstOrDefaultAsync(ct);
        }

        public async Task<List<T>> ListAsync(BaseSpecification<T> spec, CancellationToken ct = default)
        {
            return await SpecificationEvaluator<T>.GetQuery(_dbSet.AsQueryable(), spec).ToListAsync(ct);
        }

        public async Task<int> CountAsync(BaseSpecification<T> spec, CancellationToken ct = default)
        {
            return await SpecificationEvaluator<T>.GetQueryWithoutPaging(_dbSet.AsQueryable(), spec).CountAsync(ct);
        }

        public async Task<bool> ExistsAsync(BaseSpecification<T> spec, CancellationToken ct = default)
        {
            return await SpecificationEvaluator<T>.GetQuery(_dbSet.AsQueryable(), spec).AnyAsync(ct);
        }
    }
}
