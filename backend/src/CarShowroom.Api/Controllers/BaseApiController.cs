using System.Diagnostics;
using CarShowroom.Application.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace CarShowroom.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public abstract class BaseApiController : ControllerBase
{
    [DebuggerStepThrough]
    protected ActionResult<CustomApiResponse<T>> CustomResult<T>(CustomApiResponse<T> response)
    {
        return new ObjectResult(response)
        {
            StatusCode = (int)response.ResultStatus
        };
    }
}
