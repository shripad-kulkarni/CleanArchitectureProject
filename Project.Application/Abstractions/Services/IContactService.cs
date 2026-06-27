using Project.Application.Common.Result;
using Project.Application.DTOs.Contact;
using Project.Application.Pagination;

namespace Project.Application.Abstractions.Services
{
    public interface IContactService
    {
        Task<Result> SubmitAsync(CreateContactMessageDto dto, CancellationToken ct = default);
        Task<Result<PagedList<ContactMessageDto>>> GetAllAsync(ContactFilterDto filter, CancellationToken ct = default);
        Task<Result<ContactMessageDto>> GetByIdAsync(int id, CancellationToken ct = default);
    }
}
