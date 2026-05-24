using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Domain.Constants
{
    public static class DocumentConstants
    {
        public const int MaxFileSizeInMb = 5;
        public const long MaxFileSizeInBytes = MaxFileSizeInMb * 1024 * 1024;

        public static readonly string[] AllowedExtensions = [".pdf", ".jpg", ".jpeg", ".png"];
        public static readonly string[] AllowedMimeTypes = ["application/pdf", "image/jpeg", "image/png"];
    }
}
