namespace Project.Application.DTOs.Chat
{
    public sealed record ChatUserDto(
        string IdentityId,
        string FullName,
        string Email,
        string? ProfilePhotoUrl);
}
