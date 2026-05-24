using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Application.Abstractions.ExternalServices
{
    public interface IFileStorageService
    {
        Task<string> UploadAsync(Stream fileStream, string fileName, string folder, CancellationToken ct = default);
        Task<Stream?> ReadAsync(string filePath, CancellationToken ct = default);
        Task DeleteAsync(string filePath, CancellationToken ct = default);
        string GetFileUrl(string filePath);
    }
}
