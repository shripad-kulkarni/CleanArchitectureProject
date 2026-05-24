using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Infrastructure.Options
{
    public sealed class FileStorageOptions
    {
        public const string SectionName = "FileStorage";

        public string BasePath { get; init; } = string.Empty;
        public string BaseUrl { get; init; } = string.Empty;
        public long MaxFileSizeInBytes { get; init; } = 5 * 1024 * 1024; // 5MB
    }
}
