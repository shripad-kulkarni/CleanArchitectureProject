using Project.Application.DTOs.User;

namespace Project.Application.Abstractions.ExternalServices
{
    public interface IPdfGeneratorService
    {
        byte[] GenerateUserProfileReport(UserDto user);
    }
}
