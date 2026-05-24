namespace Project.API.CustomResults
{
    public class PaginatedResponse<T>
    {
        public bool IsSuccess { get; init; }
        public string Message { get; init; } = string.Empty;
        public IEnumerable<T> Data { get; init; } = [];
        public int PageNumber { get; init; }
        public int PageSize { get; init; }
        public int TotalCount { get; init; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;

        public static PaginatedResponse<T> Success(
            IEnumerable<T> data,
            int pageNumber,
            int pageSize,
            int totalCount,
            string message = "Request successful.")
            => new()
            {
                IsSuccess = true,
                Message = message,
                Data = data,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
    }
}
