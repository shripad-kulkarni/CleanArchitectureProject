using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project.API.CustomResults;
using Project.Application.Abstractions.Services;
using Project.Application.DTOs.Settings;

namespace Project.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/school-settings")]
    [Authorize]
    public sealed class SchoolSettingsController : ControllerBase
    {
        private readonly ISchoolSettingsService _service;

        public SchoolSettingsController(ISchoolSettingsService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Get(CancellationToken ct)
        {
            var dto = await _service.GetAsync(ct);
            return Ok(ApiResponse<SchoolSettingsDto>.Success(dto));
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update([FromBody] UpdateSchoolSettingsDto dto, CancellationToken ct)
        {
            await _service.UpdateAsync(dto, ct);
            return Ok(ApiResponse.Success("Settings updated."));
        }

        [HttpPost("logo")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UploadLogo(IFormFile? file, CancellationToken ct)
        {
            if (file is null || file.Length == 0)
                return BadRequest(ApiResponse.Failure("No file provided."));

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (ext is not (".jpg" or ".jpeg" or ".png" or ".gif" or ".webp"))
                return BadRequest(ApiResponse.Failure("Only image files are allowed (jpg, png, gif, webp)."));

            using var stream = file.OpenReadStream();
            var logoPath = await _service.UploadLogoAsync(stream, file.FileName, ct);

            return Ok(ApiResponse<object>.Success(new { logoPath }));
        }
    }
}
