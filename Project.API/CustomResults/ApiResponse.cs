namespace Project.API.CustomResults
{
    public class ApiResponse<T>
    {
        public bool IsSuccess { get; init; }
        public string Message { get; init; } = string.Empty;
        public T? Data { get; init; }
        public IEnumerable<string>? Errors { get; init; }

        public static ApiResponse<T> Success(T data, string message = "Request successful.")
            => new() { IsSuccess = true, Message = message, Data = data };

        public static ApiResponse<T> Failure(string message, IEnumerable<string>? errors = null)
            => new() { IsSuccess = false, Message = message, Errors = errors };
    }

    public class ApiResponse
    {
        public bool IsSuccess { get; init; }
        public string Message { get; init; } = string.Empty;
        public IEnumerable<string>? Errors { get; init; }

        public static ApiResponse Success(string message = "Request successful.")
            => new() { IsSuccess = true, Message = message };

        public static ApiResponse Failure(string message, IEnumerable<string>? errors = null)
            => new() { IsSuccess = false, Message = message, Errors = errors };
    }
}
