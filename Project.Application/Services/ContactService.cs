using FluentValidation;
using Project.Application.Abstractions.ExternalServices;
using Project.Application.Abstractions.Persistence;
using Project.Application.Abstractions.Services;
using Project.Application.Common.Errors;
using Project.Application.Common.Result;
using Project.Application.DTOs.Contact;
using Project.Domain.Aggregates.ContactAggregate;

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
