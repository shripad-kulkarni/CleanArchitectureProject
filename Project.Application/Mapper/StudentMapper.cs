using Project.Application.DTOs.Student;
using Project.Domain.Aggregates.StudentAggregate;

namespace Project.Application.Mapper
{
    public static class StudentMapper
    {
        public static StudentDto ToDto(Student student) => new(
            Id: student.Id,
            FirstName: student.FirstName,
            LastName: student.LastName,
            Email: student.Email.Value,
            Phone: student.Phone.Value,
            DateOfBirth: student.DateOfBirth,
            Gender: student.Gender.ToString(),
            AdmissionNumber: student.AdmissionNumber,
            AdmissionDate: student.AdmissionDate,
            RollNumber: student.RollNumber,
            ClassName: student.ClassName,
            AcademicYear: student.AcademicYear,
            Street: student.Address.Street,
            City: student.Address.City,
            State: student.Address.State,
            PinCode: student.Address.PinCode,
            BloodGroup: student.BloodGroup,
            ParentName: student.ParentName,
            ParentPhone: student.ParentPhone,
            ParentEmail: student.ParentEmail,
            EmergencyContact: student.EmergencyContact);

        public static StudentDocumentDto ToDocumentDto(StudentDocument doc) => new(
            Id: doc.Id,
            StudentId: doc.StudentId,
            DocumentType: doc.DocumentType.ToString(),
            FileName: doc.FileName,
            FilePath: doc.FilePath,
            FileSizeInBytes: doc.FileSizeInBytes);
    }
}
