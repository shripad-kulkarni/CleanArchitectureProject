using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Project.Application.Abstractions.Identity;
using Project.Domain.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Infrastructure.Persistence.Interceptors
{
    public sealed class SoftDeleteInterceptor : SaveChangesInterceptor
    {
        private readonly ICurrentUserService _currentUserService;

        public SoftDeleteInterceptor(ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
        }

        public override InterceptionResult<int> SavingChanges(
            DbContextEventData eventData, InterceptionResult<int> result)
        {
            ApplySoftDelete(eventData.Context);
            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData, InterceptionResult<int> result,
            CancellationToken ct = default)
        {
            ApplySoftDelete(eventData.Context);
            return base.SavingChangesAsync(eventData, result, ct);
        }

        private void ApplySoftDelete(DbContext? context)
        {
            if (context is null) return;

            int.TryParse(_currentUserService.UserId, out var userId);

            // Materialize first — mutating entry.State while iterating the live enumerator skips entries
            var deletedEntries = context.ChangeTracker
                .Entries<Entity>()
                .Where(e => e.State == EntityState.Deleted)
                .ToList();

            foreach (var entry in deletedEntries)
            {
                entry.State = EntityState.Modified;
                entry.Entity.IsDeleted = true;
                entry.Entity.DeletedAt = DateTime.UtcNow;
                entry.Entity.DeletedBy = userId;
            }

            // When Remove() is called on a principal with OwnsOne navigation, EF Core also marks
            // the owned-entity entries as Deleted. After the principal transitions to Modified,
            // those owned entries must be reset to Unchanged so EF Core doesn't attempt a
            // separate DELETE on columns that live in the same row as the principal.
            foreach (var entry in context.ChangeTracker
                .Entries()
                .Where(e => e.State == EntityState.Deleted && e.Metadata.IsOwned())
                .ToList())
            {
                entry.State = EntityState.Unchanged;
            }
        }
    }
}
