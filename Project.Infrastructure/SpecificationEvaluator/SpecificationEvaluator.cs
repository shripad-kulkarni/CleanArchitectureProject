using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Project.Application.Specifications;
using Project.Domain.Primitives;

namespace Project.Infrastructure.SpecificationEvaluator
{
    public static class SpecificationEvaluator<T> where T : AggregateRoot
    {
        private static readonly MethodInfo _includeMethod =
            typeof(EntityFrameworkQueryableExtensions)
                .GetMethods(BindingFlags.Static | BindingFlags.Public)
                .First(m => m.Name == nameof(EntityFrameworkQueryableExtensions.Include)
                         && m.IsGenericMethod
                         && m.GetGenericArguments().Length == 2);

        public static IQueryable<T> GetQuery(IQueryable<T> query, BaseSpecification<T> spec)
        {
            if (spec.Criteria is not null)
                query = query.Where(spec.Criteria);

            foreach (var include in spec.Includes)
            {
                var method = _includeMethod.MakeGenericMethod(typeof(T), include.ReturnType);
                query = (IQueryable<T>)method.Invoke(null, [query, include])!;
            }

            query = spec.IncludeStrings
                .Aggregate(query, (current, include) => current.Include(include));

            if (spec.OrderBy is not null)
                query = query.OrderBy(spec.OrderBy);
            else if (spec.OrderByDescending is not null)
                query = query.OrderByDescending(spec.OrderByDescending);

            if (spec.IsPagingEnabled)
                query = query.Skip(spec.Skip).Take(spec.Take);

            return query;
        }

        public static IQueryable<T> GetQueryWithoutPaging(IQueryable<T> query, BaseSpecification<T> spec)
        {
            if (spec.Criteria is not null)
                query = query.Where(spec.Criteria);

            return query;
        }
    }
}
