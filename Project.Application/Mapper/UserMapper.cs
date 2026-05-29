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
            Email: user.Email.Value,
            Phone: user.Phone.Value,
            DateOfBirth: user.DateOfBirth,
            Gender: user.Gender.ToString(),
            Street: user.Address.Street,
            City: user.Address.City,
            State: user.Address.State,
            PinCode: user.Address.PinCode,
            BloodGroup: user.BloodGroup,
            EmergencyContact: user.EmergencyContact);

        public static UserDocumentDto ToDocumentDto(UserDocument doc) => new(
            Id: doc.Id,
            UserId: doc.UserId,
            DocumentType: doc.DocumentType.ToString(),
            FileName: doc.FileName,
            FilePath: doc.FilePath,
            FileSizeInBytes: doc.FileSizeInBytes);
    }
}
