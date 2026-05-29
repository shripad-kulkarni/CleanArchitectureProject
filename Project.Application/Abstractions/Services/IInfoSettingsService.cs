using Project.Application.DTOs.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Application.Abstractions.Services
{
    public interface IInfoSettingsService
    {
        Task<InfoSettingsDto> GetAsync(CancellationToken ct = default);
        Task UpdateAsync(UpdateInfoSettingsDto dto, CancellationToken ct = default);
        Task<string> UploadLogoAsync(Stream stream, string fileName, CancellationToken ct = default);
    }
}
