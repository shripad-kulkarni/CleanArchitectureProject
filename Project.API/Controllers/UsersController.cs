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
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create([FromForm] CreateUserRequest request, CancellationToken ct)
        {
            if (!DateOnly.TryParse(request.DateOfBirth, out var dob))
                return BadRequest(ApiResponse.Failure("Invalid date of birth format. Use YYYY-MM-DD."));

            if (request.ProfilePhoto is { Length: > 0 })
            {
                if (!request.ProfilePhoto.ContentType.StartsWith("image/"))
                    return BadRequest(ApiResponse.Failure("Profile photo must be an image file."));
                if (request.ProfilePhoto.Length > 5 * 1024 * 1024)
                    return BadRequest(ApiResponse.Failure("Profile photo must not exceed 5 MB."));
            }

            if (request.IntroVideo is { Length: > 0 })
            {
                if (!request.IntroVideo.ContentType.StartsWith("video/"))
                    return BadRequest(ApiResponse.Failure("Intro video must be a video file."));
                if (request.IntroVideo.Length > 100 * 1024 * 1024)
                    return BadRequest(ApiResponse.Failure("Intro video must not exceed 100 MB."));
            }

            var dto = new CreateUserDto(
                request.FirstName, request.LastName, request.Email,
                request.PhoneNumber, dob, request.Gender,
                request.Street, request.City, request.State, request.PinCode,
                request.BloodGroup, request.EmergencyContact, request.Description);

            Stream? photoStream = null;
            Stream? videoStream = null;
            try
            {
                if (request.ProfilePhoto is { Length: > 0 })
                    photoStream = request.ProfilePhoto.OpenReadStream();
                if (request.IntroVideo is { Length: > 0 })
                    videoStream = request.IntroVideo.OpenReadStream();

                var result = await _userService.CreateAsync(dto,
                    photoStream, request.ProfilePhoto?.FileName,
                    videoStream, request.IntroVideo?.FileName,
                    ct);

                if (result.IsFailure) return ToErrorResponse(result.Error);
                return CreatedAtAction(nameof(GetById), new { id = result.Value.Id },
                    ApiResponse<UserDto>.Success(result.Value, "User created successfully."));
            }
            finally
            {
                if (photoStream != null) await photoStream.DisposeAsync();
                if (videoStream != null) await videoStream.DisposeAsync();
            }
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id, CancellationToken ct)
        {
            var result = await _userService.GetByIdAsync(id, ct);
            if (result.IsFailure) return ToErrorResponse(result.Error);
            return Ok(ApiResponse<UserDto>.Success(result.Value));
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll([FromQuery] UserFilterDto filter, CancellationToken ct)
        {
            var result = await _userService.GetAllAsync(filter, ct);
            if (result.IsFailure) return ToErrorResponse(result.Error);
            var paged = result.Value;
            return Ok(PaginatedResponse<UserDto>.Success(paged.Items, paged.PageNumber, paged.PageSize, paged.TotalCount));
        }

        [HttpPut("{id:int}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Update(int id, [FromForm] UpdateUserRequest request, CancellationToken ct)
        {
            if (request.ProfilePhoto is { Length: > 0 })
            {
                if (!request.ProfilePhoto.ContentType.StartsWith("image/"))
                    return BadRequest(ApiResponse.Failure("Profile photo must be an image file."));
                if (request.ProfilePhoto.Length > 5 * 1024 * 1024)
                    return BadRequest(ApiResponse.Failure("Profile photo must not exceed 5 MB."));
            }

            if (request.IntroVideo is { Length: > 0 })
            {
                if (!request.IntroVideo.ContentType.StartsWith("video/"))
                    return BadRequest(ApiResponse.Failure("Intro video must be a video file."));
                if (request.IntroVideo.Length > 100 * 1024 * 1024)
                    return BadRequest(ApiResponse.Failure("Intro video must not exceed 100 MB."));
            }

            var dto = new UpdateUserDto(
                request.FirstName, request.LastName, request.Phone,
                request.Street, request.City, request.State, request.PinCode,
                request.Description);

            Stream? photoStream = null;
            Stream? videoStream = null;
            try
            {
                if (request.ProfilePhoto is { Length: > 0 })
                    photoStream = request.ProfilePhoto.OpenReadStream();
                if (request.IntroVideo is { Length: > 0 })
                    videoStream = request.IntroVideo.OpenReadStream();

                var result = await _userService.UpdateAsync(id, dto,
                    photoStream, request.ProfilePhoto?.FileName,
                    videoStream, request.IntroVideo?.FileName,
                    ct);

                if (result.IsFailure) return ToErrorResponse(result.Error);
                return Ok(ApiResponse<UserDto>.Success(result.Value, "User updated successfully."));
            }
            finally
            {
                if (photoStream != null) await photoStream.DisposeAsync();
                if (videoStream != null) await videoStream.DisposeAsync();
            }
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
        [AllowAnonymous]
        public async Task<IActionResult> GenerateDocument(int id, string documentType, CancellationToken ct)
        {
            var result = await _userService.GenerateDocumentAsync(id, documentType, ct);
            if (result.IsFailure) return ToErrorResponse(result.Error);
            return File(result.Value.Content, result.Value.ContentType, result.Value.FileName);
        }
    }

    public sealed class CreateUserRequest
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string DateOfBirth { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string PinCode { get; set; } = string.Empty;
        public string? BloodGroup { get; set; }
        public string? EmergencyContact { get; set; }
        public string? Description { get; set; }
        public IFormFile? ProfilePhoto { get; set; }
        public IFormFile? IntroVideo { get; set; }
    }

    public sealed class UpdateUserRequest
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string PinCode { get; set; } = string.Empty;
        public string? Description { get; set; }
        public IFormFile? ProfilePhoto { get; set; }
        public IFormFile? IntroVideo { get; set; }
    }

    public sealed class UploadDocumentRequest
    {
        public IFormFile File { get; set; } = null!;
        public string DocumentType { get; set; } = string.Empty;
    }
}
