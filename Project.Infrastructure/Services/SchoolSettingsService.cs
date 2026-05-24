using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Project.Application.Abstractions.Services;
using Project.Application.DTOs.Settings;
using Project.Domain.Aggregates.SchoolSettingAggregate;
using Project.Infrastructure.Persistence;

namespace Project.Infrastructure.Services
{
    public sealed class SchoolSettingsService : ISchoolSettingsService
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;

        public SchoolSettingsService(AppDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        public async Task<SchoolSettingsDto> GetAsync(CancellationToken ct = default)
        {
            var s = await _db.SchoolSettings.FirstOrDefaultAsync(ct)
                    ?? SchoolSettings.CreateDefault();
            return ToDto(s);
        }

        public async Task UpdateAsync(UpdateSchoolSettingsDto dto, CancellationToken ct = default)
        {
            var s = await _db.SchoolSettings.FirstOrDefaultAsync(ct);
            if (s is null)
            {
                s = SchoolSettings.CreateDefault();
                _db.SchoolSettings.Add(s);
            }
            s.Update(dto.SchoolName, dto.Address, dto.PhoneNumber, dto.Email);
            await _db.SaveChangesAsync(ct);
        }

        public async Task<string> UploadLogoAsync(Stream stream, string fileName, CancellationToken ct = default)
        {
            var uploadsDir = Path.Combine(_env.ContentRootPath, "wwwroot", "uploads");
            Directory.CreateDirectory(uploadsDir);

            var ext = Path.GetExtension(fileName).ToLowerInvariant();
            var dest = Path.Combine(uploadsDir, $"school-logo{ext}");

            using var fs = File.Create(dest);
            await stream.CopyToAsync(fs, ct);

            var logoPath = $"/uploads/school-logo{ext}";

            var s = await _db.SchoolSettings.FirstOrDefaultAsync(ct);
            if (s is null)
            {
                s = SchoolSettings.CreateDefault();
                _db.SchoolSettings.Add(s);
            }
            s.SetLogoPath(logoPath);
            await _db.SaveChangesAsync(ct);

            return logoPath;
        }

        private static SchoolSettingsDto ToDto(SchoolSettings s) =>
            new(s.SchoolName, s.LogoPath, s.Address, s.PhoneNumber, s.Email);
    }
}
