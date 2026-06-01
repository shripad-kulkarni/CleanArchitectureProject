using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project.API.CustomResults;
using Project.Application.Abstractions.Identity;
using Project.Application.Abstractions.Services;

namespace Project.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/chat")]
    [Authorize]
    public sealed class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;
        private readonly ICurrentUserService _currentUserService;

        public ChatController(IChatService chatService, ICurrentUserService currentUserService)
        {
            _chatService = chatService;
            _currentUserService = currentUserService;
        }

        [HttpGet("{otherUserId}")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetConversation(
            string otherUserId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50,
            CancellationToken ct = default)
        {
            var userId = _currentUserService.UserId;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(ApiResponse.Failure("Not authenticated."));

            var result = await _chatService.GetConversationAsync(userId, otherUserId, page, pageSize, ct);
            return Ok(ApiResponse<object>.Success(result.Value));
        }

        [HttpPost("{senderId}/read")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> MarkAsRead(string senderId, CancellationToken ct)
        {
            var receiverId = _currentUserService.UserId;
            if (string.IsNullOrEmpty(receiverId))
                return Unauthorized(ApiResponse.Failure("Not authenticated."));

            await _chatService.MarkAsReadAsync(receiverId, senderId, ct);
            return Ok(ApiResponse.Success("Messages marked as read."));
        }

        [HttpGet("unread-count")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUnreadCount(CancellationToken ct)
        {
            var userId = _currentUserService.UserId;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(ApiResponse.Failure("Not authenticated."));

            var result = await _chatService.GetUnreadCountAsync(userId, ct);
            return Ok(ApiResponse<object>.Success(new { count = result.Value }));
        }

        [HttpGet("users")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetChatUsers(CancellationToken ct)
        {
            var userId = _currentUserService.UserId;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(ApiResponse.Failure("Not authenticated."));

            var result = await _chatService.GetChatUsersAsync(userId, ct);
            return Ok(ApiResponse<object>.Success(result.Value));
        }

        [HttpPost("upload")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UploadFile(IFormFile file, CancellationToken ct)
        {
            if (_currentUserService.UserId is null)
                return Unauthorized(ApiResponse.Failure("Not authenticated."));

            if (file is null || file.Length == 0)
                return BadRequest(ApiResponse.Failure("No file provided."));

            using var stream = file.OpenReadStream();
            var result = await _chatService.UploadFileAsync(stream, file.FileName, file.Length, ct);

            if (result.IsFailure)
                return BadRequest(ApiResponse.Failure(result.Error.Message));

            return Ok(ApiResponse<object>.Success(new { url = result.Value, fileName = file.FileName }));
        }
    }
}
