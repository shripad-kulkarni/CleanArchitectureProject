using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project.API.Controllers.Base;
using Project.API.CustomResults;
using Project.Application.Abstractions.Services;
using Project.Application.DTOs.MenuSetting;
using Project.Domain.Constants;

namespace Project.API.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/menu-settings")]
    [Authorize]
    public sealed class MenuSettingsController : ApiControllerBase
    {
        private readonly IMenuSettingService _service;

        public MenuSettingsController(IMenuSettingService service)
        {
            _service = service;
        }

        [HttpGet]
        [Authorize(Roles = RoleConstants.Admin)]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            var result = await _service.GetAllAsync(ct);
            if (result.IsFailure) return ToErrorResponse(result.Error);
            return Ok(ApiResponse<List<MenuNodeDto>>.Success(result.Value));
        }

        [HttpGet("for-role/{role}")]
        public async Task<IActionResult> GetForRole(string role, CancellationToken ct)
        {
            var result = await _service.GetMenuForRoleAsync(role, ct);
            if (result.IsFailure) return ToErrorResponse(result.Error);
            return Ok(ApiResponse<List<MenuNodeDto>>.Success(result.Value));
        }

        [HttpPut]
        [Authorize(Roles = RoleConstants.Admin)]
        public async Task<IActionResult> Update([FromBody] UpdateMenuSettingsDto dto, CancellationToken ct)
        {
            var result = await _service.UpdateAsync(dto, ct);
            if (result.IsFailure) return ToErrorResponse(result.Error);
            return Ok(ApiResponse.Success("Menu settings updated."));
        }
    }
}
