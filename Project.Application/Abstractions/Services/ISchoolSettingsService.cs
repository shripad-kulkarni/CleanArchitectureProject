using Project.Application.DTOs.Settings;

namespace Project.Application.Abstractions.Services
{
    public interface ISchoolSettingsService
    {
        Task<SchoolSettingsDto> GetAsync(CancellationToken ct = default);
        Task UpdateAsync(UpdateSchoolSettingsDto dto, CancellationToken ct = default);
        Task<string> UploadLogoAsync(Stream stream, string fileName, CancellationToken ct = default);
    }
}
