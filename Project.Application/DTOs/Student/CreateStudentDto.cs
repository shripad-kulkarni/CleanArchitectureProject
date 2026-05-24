namespace Project.Application.DTOs.Student
{
    public record CreateStudentDto(
        string FirstName,
        string LastName,
        string Email,
        string PhoneNumber,
        DateOnly DateOfBirth,
        string Gender,
        string AdmissionNumber,
        DateOnly AdmissionDate,
        string Street,
        string City,
        string State,
        string PinCode,
        string RollNumber,
        string ClassName,
        string AcademicYear,
        string ParentName,
        string ParentPhone,
        string? BloodGroup = null,
        string? ParentEmail = null,
        string? EmergencyContact = null);
}
