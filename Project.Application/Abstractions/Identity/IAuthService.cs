namespace Project.Application.Abstractions.Identity
{
    public interface IAuthService
    {
        Task<AuthResult> LoginAsync(string email, string password, CancellationToken ct = default);
        Task<AuthResult> RegisterAsync(string email, string password, string firstName, string lastName, string role, CancellationToken ct = default);
        Task<AuthResult> InviteUserAsync(string email, string firstName, string lastName, string role, CancellationToken ct = default);
        Task<AuthResult> SetPasswordAsync(string email, string token, string newPassword, CancellationToken ct = default);
        Task<AuthResult> RefreshTokenAsync(string token, CancellationToken ct = default);
    }

    public record AuthResult(
        bool IsSuccess,
        string? Token,
        string? Error,
        string? Id = null,
        string? Name = null,
        string? Email = null);
}
