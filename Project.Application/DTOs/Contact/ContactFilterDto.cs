namespace Project.Application.DTOs.Contact
{
    public record ContactFilterDto
    {
        public string? Search { get; init; }
        public bool? IsRead { get; init; }
        public DateOnly? DateFrom { get; init; }
        public DateOnly? DateTo { get; init; }
        public int PageNumber { get; init; } = 1;
        public int PageSize { get; init; } = 10;
    }
}
