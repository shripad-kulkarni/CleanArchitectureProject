using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project.API.Controllers.Base;
using Project.API.CustomResults;
using Project.Application.Abstractions.Services;
using Project.Application.DTOs.Contact;
using Project.Domain.Constants;

namespace Project.API.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/contact")]
    public sealed class ContactController : ApiControllerBase
    {
        private readonly IContactService _contactService;

        public ContactController(IContactService contactService)
        {
            _contactService = contactService;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Submit([FromBody] CreateContactMessageDto dto, CancellationToken ct)
        {
            var result = await _contactService.SubmitAsync(dto, ct);
            if (result.IsFailure) return ToErrorResponse(result.Error);
            return Ok(ApiResponse.Success("Message sent successfully."));
        }

        [HttpGet]
        [Authorize(Roles = RoleConstants.Admin)]
        public async Task<IActionResult> GetAll([FromQuery] ContactFilterDto filter, CancellationToken ct)
        {
            var result = await _contactService.GetAllAsync(filter, ct);
            if (result.IsFailure) return ToErrorResponse(result.Error);
            var paged = result.Value;
            return Ok(PaginatedResponse<ContactMessageDto>.Success(paged.Items, paged.PageNumber, paged.PageSize, paged.TotalCount));
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = RoleConstants.Admin)]
        public async Task<IActionResult> GetById(int id, CancellationToken ct)
        {
            var result = await _contactService.GetByIdAsync(id, ct);
            if (result.IsFailure) return ToErrorResponse(result.Error);
            return Ok(ApiResponse<ContactMessageDto>.Success(result.Value));
        }
    }
}
