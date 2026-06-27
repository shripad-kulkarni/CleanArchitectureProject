using System.Linq.Expressions;
using FluentValidation;
using Project.Application.Abstractions.ExternalServices;
using Project.Application.Abstractions.Persistence;
using Project.Application.Abstractions.Services;
using Project.Application.Common.Errors;
using Project.Application.Common.Result;
using Project.Application.DTOs.Contact;
using Project.Application.Mapper;
using Project.Application.Pagination;
using Project.Domain.Entities;

namespace Project.Application.Services
{
    public sealed class ContactService : IContactService
    {
        private readonly IRepository<ContactMessage> _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<CreateContactMessageDto> _validator;
        private readonly IEmailService _emailService;
        private readonly IInfoSettingsService _settingsService;

        public ContactService(
            IRepository<ContactMessage> repository,
            IUnitOfWork unitOfWork,
            IValidator<CreateContactMessageDto> validator,
            IEmailService emailService,
            IInfoSettingsService settingsService)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _validator = validator;
            _emailService = emailService;
            _settingsService = settingsService;
        }

        public async Task<Result<PagedList<ContactMessageDto>>> GetAllAsync(ContactFilterDto filter, CancellationToken ct = default)
        {
            var predicates = new List<Expression<Func<ContactMessage, bool>>>();

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                var search = filter.Search.ToLower();
                predicates.Add(m => m.Name.ToLower().Contains(search)
                                 || m.Email.ToLower().Contains(search)
                                 || m.Subject.ToLower().Contains(search));
            }

            if (filter.IsRead.HasValue)
                predicates.Add(m => m.IsRead == filter.IsRead.Value);

            if (filter.DateFrom.HasValue)
                predicates.Add(m => DateOnly.FromDateTime(m.CreatedAt) >= filter.DateFrom.Value);

            if (filter.DateTo.HasValue)
                predicates.Add(m => DateOnly.FromDateTime(m.CreatedAt) <= filter.DateTo.Value);

            var (items, totalCount) = await _repository.ListPagedAsync(
                predicates,
                m => m.CreatedAt,
                filter.PageNumber,
                filter.PageSize,
                ct);

            var dtos = items.Select(ContactMapper.ToDto).ToList();
            return Result<PagedList<ContactMessageDto>>.Success(
                new PagedList<ContactMessageDto>(dtos, totalCount, filter.PageNumber, filter.PageSize));
        }

        public async Task<Result<ContactMessageDto>> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var message = await _repository.GetByIdAsync(id, ct);
            if (message is null)
                return Result<ContactMessageDto>.Failure(Error.NotFound("Contact.NotFound", $"Contact message {id} not found."));
            return Result<ContactMessageDto>.Success(ContactMapper.ToDto(message));
        }

        public async Task<Result> SubmitAsync(CreateContactMessageDto dto, CancellationToken ct = default)
        {
            var validation = await _validator.ValidateAsync(dto, ct);
            if (!validation.IsValid)
                return Result.Failure(
                    Error.ValidationErrors("Contact.Validation", validation.Errors.Select(e => e.ErrorMessage)));

            var message = ContactMessage.Create(dto.Name, dto.Email, dto.Phone, dto.Subject, dto.Message);
            await _repository.AddAsync(message, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            // Send email notification to the organisation — fire-and-forget, don't fail the request
            _ = NotifyAsync(dto, ct);

            return Result.Success();
        }

        private async Task NotifyAsync(CreateContactMessageDto dto, CancellationToken ct)
        {
            try
            {
                var settings = await _settingsService.GetAsync(ct);
                if (string.IsNullOrWhiteSpace(settings.Email)) return;

                var html = $"""
                    <h2 style="color:#1e40af">New Contact Message</h2>
                    <table style="font-family:Arial,sans-serif;font-size:14px;border-collapse:collapse">
                      <tr><td style="padding:6px 12px;font-weight:bold">Name</td><td style="padding:6px 12px">{System.Net.WebUtility.HtmlEncode(dto.Name)}</td></tr>
                      <tr><td style="padding:6px 12px;font-weight:bold">Email</td><td style="padding:6px 12px">{System.Net.WebUtility.HtmlEncode(dto.Email)}</td></tr>
                      {(dto.Phone is not null ? $"<tr><td style='padding:6px 12px;font-weight:bold'>Phone</td><td style='padding:6px 12px'>{System.Net.WebUtility.HtmlEncode(dto.Phone)}</td></tr>" : "")}
                      <tr><td style="padding:6px 12px;font-weight:bold">Subject</td><td style="padding:6px 12px">{System.Net.WebUtility.HtmlEncode(dto.Subject)}</td></tr>
                      <tr><td style="padding:6px 12px;font-weight:bold">Message</td><td style="padding:6px 12px">{System.Net.WebUtility.HtmlEncode(dto.Message)}</td></tr>
                    </table>
                    """;

                await _emailService.SendAsync(
                    settings.Email,
                    $"[Contact] {dto.Subject}",
                    html,
                    ct);
            }
            catch
            {
                // Email errors must never fail the submission
            }
        }
    }
}
