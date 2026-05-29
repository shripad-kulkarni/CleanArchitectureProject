using Microsoft.AspNetCore.Mvc;
using Project.API.CustomResults;
using Project.Application.Common.Errors;

namespace Project.API.Controllers.Base
{
    [ApiController]
    public abstract class ApiControllerBase : ControllerBase
    {
        protected IActionResult ToErrorResponse(Error error) => error.Type switch
        {
            ErrorType.NotFound     => NotFound(ApiResponse.Failure(error.Message)),
            ErrorType.Conflict     => Conflict(ApiResponse.Failure(error.Message)),
            ErrorType.Validation   => BadRequest(ApiResponse.Failure(error.Message)),
            ErrorType.Unauthorized => Unauthorized(ApiResponse.Failure(error.Message)),
            _                      => StatusCode(StatusCodes.Status500InternalServerError, ApiResponse.Failure(error.Message))
        };
    }
}
