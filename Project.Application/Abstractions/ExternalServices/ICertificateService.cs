using Project.Application.DTOs.User;

namespace Project.Application.Abstractions.ExternalServices
{
    public interface ICertificateService
    {
        Task<byte[]> GenerateAsync(UserDto user, CancellationToken ct = default);
    }
}
