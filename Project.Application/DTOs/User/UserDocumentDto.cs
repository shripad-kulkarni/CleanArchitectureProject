namespace Project.Application.DTOs.User
{
    public record UserDocumentDto(
        int Id,
        int UserId,
        string DocumentType,
        string FileName,
        string FilePath,
        long FileSizeInBytes);
}
