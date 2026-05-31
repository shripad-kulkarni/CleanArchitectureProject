using Project.Application.DTOs.User;
using Project.Domain.Aggregates.UserAggregate;

namespace Project.Application.Mapper
{
    public static class UserMapper
    {
        public static UserDto ToDto(User user) => new(
            Id: user.Id,
            FirstName: user.FirstName,
            LastName: user.LastName,
            Email: user.Email,
            Phone: user.Phone,
            DateOfBirth: user.DateOfBirth,
            Gender: user.Gender.ToString(),
            Street: user.Street,
            City: user.City,
            State: user.State,
            PinCode: user.PinCode,
            BloodGroup: user.BloodGroup,
            EmergencyContact: user.EmergencyContact,
            Description: user.Description,
            ProfilePhotoUrl: user.ProfilePhotoUrl,
            IntroVideoUrl: user.IntroVideoUrl);

        public static UserDocumentDto ToDocumentDto(UserDocument doc) => new(
            Id: doc.Id,
            UserId: doc.UserId,
            DocumentType: doc.DocumentType.ToString(),
            FileName: doc.FileName,
            FilePath: doc.FilePath,
            FileSizeInBytes: doc.FileSizeInBytes);
    }
}
