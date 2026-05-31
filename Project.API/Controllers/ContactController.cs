using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project.API.Controllers.Base;
using Project.API.CustomResults;
using Project.Application.Abstractions.Services;
using Project.Application.DTOs.Contact;

namespace Project.API.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/contact")]
    [AllowAnonymous]
    public sealed class ContactController : ApiControllerBase
    {
        private readonly IContactService _contactService;

        public ContactController(IContactService contactService)
        {
            _contactService = contactService;
        }

        [HttpPost]
        public async Task<IActionResult> Submit([FromBody] CreateContactMessageDto dto, CancellationToken ct)
        {
            var result = await _contactService.SubmitAsync(dto, ct);
            if (result.IsFailure) return ToErrorResponse(result.Error);
            return Ok(ApiResponse.Success("Message sent successfully."));
        }
    }
}
