using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project.API.CustomResults;
using Project.Domain.Constants;
using Project.Application.Abstractions.Identity;
using System.Security.Claims;

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
                request.Phone,
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

        [HttpPost("change-password")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request, CancellationToken ct)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null)
                return Unauthorized(ApiResponse.Failure("Unauthorized."));

            var result = await _authService.ChangePasswordAsync(userId, request.CurrentPassword, request.NewPassword, ct);
            if (!result.IsSuccess)
                return BadRequest(ApiResponse.Failure(result.Error ?? "Failed to change password."));

            return Ok(ApiResponse.Success("Password changed successfully."));
        }

        [HttpGet("verify-email")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> VerifyEmail([FromQuery] string email, [FromQuery] string token, CancellationToken ct)
        {
            var result = await _authService.VerifyEmailAsync(email, token, ct);
            if (!result.IsSuccess)
                return BadRequest(ApiResponse.Failure(result.Error ?? "Verification failed."));

            return Ok(ApiResponse.Success("Email verified successfully. You can now sign in."));
        }

        [HttpPost("forgot-password")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request, CancellationToken ct)
        {
            await _authService.ForgotPasswordAsync(request.Email, ct);
            return Ok(ApiResponse.Success("If that email is registered, a reset link has been sent."));
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
    public record RegisterRequest(string Email, string Password, string FirstName, string LastName, string Role, string? Phone = null);
    public record InviteUserRequest(string Email, string FirstName, string LastName, string Role);
    public record ChangePasswordRequest(string CurrentPassword, string NewPassword);
    public record ForgotPasswordRequest(string Email);
    public record SetPasswordRequest(string Email, string Token, string NewPassword);
    public record RefreshTokenRequest(string Token);
}

