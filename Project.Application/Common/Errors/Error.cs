using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Application.Common.Errors
{
    public sealed record Error(string Code, string Message, ErrorType Type, IReadOnlyList<string>? Errors = null)
    {
        public static readonly Error None = new(string.Empty, string.Empty, ErrorType.Failure);

        public static Error NotFound(string code, string message)
            => new(code, message, ErrorType.NotFound);

        public static Error Validation(string code, string message)
            => new(code, message, ErrorType.Validation);

        public static Error ValidationErrors(string code, IEnumerable<string> errors)
        {
            var list = errors.ToList();
            return new(code, list.Count == 1 ? list[0] : "Validation failed.", ErrorType.Validation, list);
        }

        public static Error Conflict(string code, string message)
            => new(code, message, ErrorType.Conflict);

        public static Error Failure(string code, string message)
            => new(code, message, ErrorType.Failure);

        public static Error Unauthorized(string code, string message)
            => new(code, message, ErrorType.Unauthorized);
    }
}
