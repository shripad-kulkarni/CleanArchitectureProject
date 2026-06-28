using Project.Application.DTOs.Chat;

namespace Project.Application.Abstractions.Identity
{
    public interface IIdentityUserLookupService
    {
        /// <summary>
        /// Returns a map of email → Identity GUID for the given emails.
        /// </summary>
        Task<Dictionary<string, string>> GetIdentityIdsByEmailsAsync(
            IEnumerable<string> emails,
            CancellationToken ct = default);

        /// <summary>
        /// Returns a map of email → IsActive for the given emails.
        /// </summary>
        Task<Dictionary<string, bool>> GetActiveStatusByEmailsAsync(
            IEnumerable<string> emails,
            CancellationToken ct = default);

        /// <summary>
        /// Returns all active Identity users except the caller — used to populate the chat contact list.
        /// </summary>
        Task<List<ChatUserDto>> GetAllChatUsersAsync(string currentIdentityId, CancellationToken ct = default);
    }
}
