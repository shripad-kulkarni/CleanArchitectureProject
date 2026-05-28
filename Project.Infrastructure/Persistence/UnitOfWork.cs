using Project.Application.Abstractions.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Infrastructure.Persistence
{
    public sealed class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int> SaveChangesAsync(CancellationToken ct = default)
            => await _context.SaveChangesAsync(ct);
    }
}
