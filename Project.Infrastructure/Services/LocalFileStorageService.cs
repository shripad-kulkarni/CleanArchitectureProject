using Microsoft.Extensions.Options;
using Project.Application.Abstractions.ExternalServices;
using Project.Infrastructure.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Infrastructure.Services
{
    public sealed class LocalFileStorageService : IFileStorageService
    {
        private readonly FileStorageOptions _options;

        public LocalFileStorageService(IOptions<FileStorageOptions> options)
        {
            _options = options.Value;
        }

        public async Task<string> UploadAsync(
            Stream fileStream, string fileName,
            string folder, CancellationToken ct = default)
        {
            var folderPath = Path.Combine(_options.BasePath, folder);
            Directory.CreateDirectory(folderPath);

            var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";
            var filePath = Path.Combine(folderPath, uniqueFileName);

            await using var stream = File.Create(filePath);
            await fileStream.CopyToAsync(stream, ct);

            return Path.Combine(folder, uniqueFileName);
        }

        public Task<Stream?> ReadAsync(string filePath, CancellationToken ct = default)
        {
            var fullPath = Path.Combine(_options.BasePath, filePath);
            if (!File.Exists(fullPath))
                return Task.FromResult<Stream?>(null);

            Stream stream = File.OpenRead(fullPath);
            return Task.FromResult<Stream?>(stream);
        }

        public Task DeleteAsync(string filePath, CancellationToken ct = default)
        {
            var fullPath = Path.Combine(_options.BasePath, filePath);
            if (File.Exists(fullPath))
                File.Delete(fullPath);

            return Task.CompletedTask;
        }

        public string GetFileUrl(string filePath)
            => $"{_options.BaseUrl.TrimEnd('/')}/{filePath.Replace("\\", "/")}";
    }
}
