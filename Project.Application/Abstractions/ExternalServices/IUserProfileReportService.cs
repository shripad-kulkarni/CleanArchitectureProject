using Project.Application.DTOs.User;

namespace Project.Application.Abstractions.ExternalServices
{
    public interface IUserProfileReportService
    {
        Task<byte[]> GenerateAsync(UserDto user, CancellationToken ct = default);
    }
}
