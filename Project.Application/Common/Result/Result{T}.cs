using Project.Application.Common.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Application.Common.Result
{
    public sealed class Result<T> : Result
    {
        private readonly T? _value;

        private Result(T value, Error error, bool isSuccess)
            : base(isSuccess, error)
        {
            _value = value;
        }

        public T Value => IsSuccess
            ? _value!
            : throw new InvalidOperationException("Cannot access value of a failed result.");

        public static Result<T> Success(T value) => new(value, Error.None, true);
        public new static Result<T> Failure(Error error) => new(default!, error, false);
    }
}
