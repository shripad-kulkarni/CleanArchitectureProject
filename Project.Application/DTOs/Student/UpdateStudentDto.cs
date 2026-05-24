namespace Project.Application.DTOs.Student
{
    public record UpdateStudentDto(
        string FirstName,
        string LastName,
        string Phone,
        string Street,
        string City,
        string State,
        string PinCode,
        string RollNumber,
        string ClassName,
        string AcademicYear);
}
