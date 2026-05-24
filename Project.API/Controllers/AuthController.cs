using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project.API.CustomResults;
using Project.Domain.Constants;
using Project.Infrastructure.Identity;

namespace Project.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/auth")]
    public sealed class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
        {
            var result = await _authService.LoginAsync(request.Email, request.Password, ct);

            if (!result.IsSuccess)
                return Unauthorized(ApiResponse.Failure(result.Error ?? "Invalid credentials."));

            return Ok(ApiResponse<object>.Success(new
            {
                token = result.Token,
                id = result.Id,
                name = result.Name,
                email = result.Email,
            }, "Login successful."));
        }

        [HttpPost("register")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken ct)
        {
            var result = await _authService.RegisterAsync(
                request.Email,
                request.Password,
                request.FirstName,
                request.LastName,
                request.Role,
                ct);

            if (!result.IsSuccess)
                return BadRequest(ApiResponse.Failure(result.Error ?? "Registration failed."));

            return StatusCode(StatusCodes.Status201Created,
                ApiResponse<object>.Success(new { token = result.Token }, "Registration successful."));
        }

        [HttpPost("invite")]
        [Authorize(Roles = RoleConstants.Admin)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> InviteUser([FromBody] InviteUserRequest request, CancellationToken ct)
        {
            var result = await _authService.InviteUserAsync(
                request.Email, request.FirstName, request.LastName, request.Role, ct);

            if (!result.IsSuccess)
                return BadRequest(ApiResponse.Failure(result.Error ?? "Invite failed."));

            return Ok(ApiResponse.Success("Invite sent successfully."));
        }

        [HttpPost("set-password")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SetPassword([FromBody] SetPasswordRequest request, CancellationToken ct)
        {
            var result = await _authService.SetPasswordAsync(request.Email, request.Token, request.NewPassword, ct);

            if (!result.IsSuccess)
                return BadRequest(ApiResponse.Failure(result.Error ?? "Failed to set password."));

            return Ok(ApiResponse.Success("Password set successfully. You can now log in."));
        }

        [HttpPost("refresh")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request, CancellationToken ct)
        {
            var result = await _authService.RefreshTokenAsync(request.Token, ct);

            if (!result.IsSuccess)
                return Unauthorized(ApiResponse.Failure(result.Error ?? "Token refresh failed."));

            return Ok(ApiResponse<object>.Success(new { token = result.Token }, "Token refreshed."));
        }
    }

    public record LoginRequest(string Email, string Password);
    public record RegisterRequest(string Email, string Password, string FirstName, string LastName, string Role);
    public record InviteUserRequest(string Email, string FirstName, string LastName, string Role);
    public record SetPasswordRequest(string Email, string Token, string NewPassword);
    public record RefreshTokenRequest(string Token);
}

