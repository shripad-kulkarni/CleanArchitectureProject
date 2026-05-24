using Project.Application.DTOs.Student;

namespace Project.Application.Abstractions.ExternalServices
{
    public interface IPdfGeneratorService
    {
        byte[] GenerateBonafideCertificate(StudentDto student);
        byte[] GenerateLeavingCertificate(StudentDto student);
        byte[] GenerateStudentProfileReport(StudentDto student);
    }
}
