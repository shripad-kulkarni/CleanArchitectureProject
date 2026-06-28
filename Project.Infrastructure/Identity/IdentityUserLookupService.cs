using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Project.Application.Abstractions.Identity;
using Project.Application.DTOs.Chat;
using Project.Domain.Aggregates;

namespace Project.Infrastructure.Identity
{
    public sealed class IdentityUserLookupService : IIdentityUserLookupService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public IdentityUserLookupService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<Dictionary<string, string>> GetIdentityIdsByEmailsAsync(
            IEnumerable<string> emails,
            CancellationToken ct = default)
        {
            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (var email in emails)
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user?.Id is not null)
                    result[email] = user.Id;
            }

            return result;
        }

        public async Task<Dictionary<string, bool>> GetActiveStatusByEmailsAsync(
            IEnumerable<string> emails,
            CancellationToken ct = default)
        {
            var emailList = emails.Select(e => e.ToLowerInvariant()).ToList();

            return await _userManager.Users
                .Where(u => emailList.Contains(u.NormalizedEmail!.ToLower()))
                .ToDictionaryAsync(
                    u => u.Email!.ToLowerInvariant(),
                    u => u.IsActive,
                    ct);
        }

        public async Task<List<ChatUserDto>> GetAllChatUsersAsync(
            string currentIdentityId,
            CancellationToken ct = default)
        {
            var users = await _userManager.Users
                .Where(u => u.Id != currentIdentityId && u.IsActive)
                .OrderBy(u => u.FirstName)
                .ThenBy(u => u.LastName)
                .ToListAsync(ct);

            return users.Select(u => new ChatUserDto(
                IdentityId: u.Id,
                FullName: $"{u.FirstName} {u.LastName}".Trim(),
                Email: u.Email ?? string.Empty,
                ProfilePhotoUrl: null))
            .ToList();
        }
    }
}
