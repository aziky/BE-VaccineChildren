using Microsoft.AspNetCore.Mvc;
using VaccineChildren.Core.Base;
using VaccineChildren.Core.Exceptions;

namespace VaccineChildren.API.Controllers
{
    public abstract class BaseController : ControllerBase
    {
        protected IActionResult HandleException(Exception ex, string controllerName)
        {
            switch (ex)
            {
                case KeyNotFoundException _:
                    return NotFound(BaseResponse<string>.NotFoundResponse($"Error at the {controllerName}: {ex.Message}"));

                case UnauthorizedAccessException _:
                    return Unauthorized(BaseResponse<string>.UnauthorizedResponse($"Error at the {controllerName}: {ex.Message}"));

                case ArgumentException _:
                    return BadRequest(BaseResponse<string>.BadRequestResponse($"Error at the {controllerName}: {ex.Message}"));

                case InvalidOperationException _:
                    return StatusCode(500, BaseResponse<string>.InternalErrorResponse($"Error at the {controllerName}: {ex.Message}"));

                case CustomExceptions.NoDataFoundException _:
                    return NotFound(BaseResponse<string>.NotFoundResponse($"Error at the {controllerName}: {ex.Message}"));
                
                default:
                    return StatusCode(500, BaseResponse<string>.InternalErrorResponse($"Error at the {controllerName}: {ex.Message}"));
            }
        }
    }
}
