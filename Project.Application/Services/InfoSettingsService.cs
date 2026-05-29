using Project.Application.Abstractions.ExternalServices;
using Project.Application.Abstractions.Persistence;
using Project.Application.Abstractions.Services;
using Project.Application.DTOs.Settings;
using Project.Domain.Entities;

namespace Project.Application.Services
{
    public sealed class InfoSettingsService : IInfoSettingsService
    {
        private readonly IRepository<InfoSetting> _repo;
        private readonly IUnitOfWork _uow;
        private readonly IFileStorageService _fileStorage;

        public InfoSettingsService(
            IRepository<InfoSetting> repo,
            IUnitOfWork uow,
            IFileStorageService fileStorage)
        {
            _repo = repo;
            _uow = uow;
            _fileStorage = fileStorage;
        }

        public async Task<InfoSettingsDto> GetAsync(CancellationToken ct = default)
        {
            var s = await _repo.FirstOrDefaultAsync(x => true, ct) ?? InfoSetting.CreateDefault();
            return ToDto(s);
        }

        public async Task UpdateAsync(UpdateInfoSettingsDto dto, CancellationToken ct = default)
        {
            var s = await _repo.FirstOrDefaultAsync(x => true, ct);
            if (s is null)
            {
                s = InfoSetting.CreateDefault();
                await _repo.AddAsync(s, ct);
            }

            s.Update(dto.SchoolName, dto.Address, dto.PhoneNumber, dto.Email);
            await _uow.SaveChangesAsync(ct);
        }

        public async Task<string> UploadLogoAsync(Stream stream, string fileName, CancellationToken ct = default)
        {
            var relativePath = await _fileStorage.UploadAsync(stream, fileName, "logos", ct);
            var logoUrl = _fileStorage.GetFileUrl(relativePath);

            var s = await _repo.FirstOrDefaultAsync(x => true, ct);
            if (s is null)
            {
                s = InfoSetting.CreateDefault();
                await _repo.AddAsync(s, ct);
            }

            s.SetLogoPath(logoUrl);
            await _uow.SaveChangesAsync(ct);

            return logoUrl;
        }

        private static InfoSettingsDto ToDto(InfoSetting s) =>
            new(s.Name, s.LogoPath, s.Address, s.PhoneNumber, s.Email);
    }
}
