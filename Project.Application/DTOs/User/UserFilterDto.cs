namespace Project.Application.DTOs.User
{
    public record UserFilterDto(
        string? SearchTerm = null,
        string? Gender = null,
        bool? IsActive = null,
        int PageNumber = 1,
        int PageSize = 10);
}
