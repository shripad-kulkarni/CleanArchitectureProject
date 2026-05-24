using Project.Application.DTOs.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Application.Abstractions.Services
{
    public interface ISchoolSettingsService
    {
        Task<SchoolSettingsDto> GetAsync(CancellationToken ct = default);
        Task UpdateAsync(UpdateSchoolSettingsDto dto, CancellationToken ct = default);
        Task<string> UploadLogoAsync(Stream stream, string fileName, CancellationToken ct = default);
    }
}
