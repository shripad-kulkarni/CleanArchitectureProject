using Microsoft.AspNetCore.Identity;

namespace Project.Domain.Aggregates
{
    public sealed class ApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
