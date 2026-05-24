namespace Project.Application.DTOs.Document
{
    public record GeneratedDocumentResult(byte[] Content, string FileName, string ContentType);
    public record DownloadDocumentResult(byte[] Content, string FileName, string ContentType);
}
