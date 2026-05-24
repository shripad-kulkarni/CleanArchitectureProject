namespace Project.Application.DTOs.Student
{
    public record StudentDto(
        int Id,
        string FirstName,
        string LastName,
        string Email,
        string Phone,
        DateOnly DateOfBirth,
        string Gender,
        string AdmissionNumber,
        DateOnly AdmissionDate,
        string RollNumber,
        string ClassName,
        string AcademicYear,
        string Street,
        string City,
        string State,
        string PinCode,
        string? BloodGroup = null,
        string? ParentName = null,
        string? ParentPhone = null,
        string? ParentEmail = null,
        string? EmergencyContact = null)
    {
        public string FullName => $"{FirstName} {LastName}";
    }
}
