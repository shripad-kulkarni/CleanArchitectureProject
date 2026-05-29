using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project.API.Controllers.Base;
using Project.API.CustomResults;
using Project.Application.Abstractions.Services;
using Project.Application.DTOs.User;

namespace Project.API.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/users")]
    [Authorize]
    public sealed class UsersController : ApiControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserDto dto, CancellationToken ct)
        {
            var result = await _userService.CreateAsync(dto, ct);
            if (result.IsFailure) return ToErrorResponse(result.Error);
            return CreatedAtAction(nameof(GetById), new { id = result.Value.Id },
                ApiResponse<UserDto>.Success(result.Value, "User created successfully."));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id, CancellationToken ct)
        {
            var result = await _userService.GetByIdAsync(id, ct);
            if (result.IsFailure) return ToErrorResponse(result.Error);
            return Ok(ApiResponse<UserDto>.Success(result.Value));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] UserFilterDto filter, CancellationToken ct)
        {
            var result = await _userService.GetAllAsync(filter, ct);
            if (result.IsFailure) return ToErrorResponse(result.Error);
            var paged = result.Value;
            return Ok(PaginatedResponse<UserDto>.Success(paged.Items, paged.PageNumber, paged.PageSize, paged.TotalCount));
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateUserDto dto, CancellationToken ct)
        {
            var result = await _userService.UpdateAsync(id, dto, ct);
            if (result.IsFailure) return ToErrorResponse(result.Error);
            return Ok(ApiResponse<UserDto>.Success(result.Value, "User updated successfully."));
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var result = await _userService.DeleteAsync(id, ct);
            if (result.IsFailure) return ToErrorResponse(result.Error);
            return NoContent();
        }

        [HttpPut("{id:int}/profile")]
        public async Task<IActionResult> UpdateProfile(int id, [FromBody] UpdateUserProfileDto dto, CancellationToken ct)
        {
            var result = await _userService.UpdateProfileAsync(id, dto, ct);
            if (result.IsFailure) return ToErrorResponse(result.Error);
            return Ok(ApiResponse<UserDto>.Success(result.Value, "Profile updated successfully."));
        }

        [HttpGet("{id:int}/documents")]
        public async Task<IActionResult> GetDocuments(int id, CancellationToken ct)
        {
            var result = await _userService.GetDocumentsAsync(id, ct);
            if (result.IsFailure) return ToErrorResponse(result.Error);
            return Ok(ApiResponse<IReadOnlyList<UserDocumentDto>>.Success(result.Value));
        }

        [HttpPost("{id:int}/documents")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadDocument(int id, [FromForm] UploadDocumentRequest request, CancellationToken ct)
        {
            if (request.File is null || request.File.Length == 0)
                return BadRequest(ApiResponse.Failure("No file was provided."));

            await using var stream = request.File.OpenReadStream();
            var result = await _userService.UploadDocumentAsync(
                id, request.DocumentType, request.File.FileName, stream, request.File.Length, ct);

            if (result.IsFailure) return ToErrorResponse(result.Error);
            return StatusCode(StatusCodes.Status201Created,
                ApiResponse<UserDocumentDto>.Success(result.Value, "Document uploaded successfully."));
        }

        [HttpGet("{id:int}/documents/{docId:int}/download")]
        public async Task<IActionResult> DownloadDocument(int id, int docId, CancellationToken ct)
        {
            var result = await _userService.DownloadDocumentAsync(id, docId, ct);
            if (result.IsFailure) return ToErrorResponse(result.Error);
            return File(result.Value.Content, result.Value.ContentType, result.Value.FileName);
        }

        [HttpGet("{id:int}/documents/generate/{documentType}")]
        public async Task<IActionResult> GenerateDocument(int id, string documentType, CancellationToken ct)
        {
            var result = await _userService.GenerateDocumentAsync(id, documentType, ct);
            if (result.IsFailure) return ToErrorResponse(result.Error);
            return File(result.Value.Content, result.Value.ContentType, result.Value.FileName);
        }
    }

    public sealed class UploadDocumentRequest
    {
        public IFormFile File { get; set; } = null!;
        public string DocumentType { get; set; } = string.Empty;
    }
}
