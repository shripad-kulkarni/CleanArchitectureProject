using Project.Application.Common.Result;
using Project.Application.DTOs.Contact;

namespace Project.Application.Abstractions.Services
{
    public interface IContactService
    {
        Task<Result> SubmitAsync(CreateContactMessageDto dto, CancellationToken ct = default);
    }
}
