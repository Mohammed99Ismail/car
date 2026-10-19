using CarShowroom.Application.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarShowroom.Api.Controllers;

[Route("api/notification")]
public class NotificationController : BaseApiController
{
    [HttpGet]
    [AllowAnonymous]
    public ActionResult<CustomApiResponse<object>> GetNotifications()
    {
        return CustomResult(CustomApiResponse<object>.Success(new
        {
            items = Array.Empty<object>(),
            totalCount = 0
        }));
    }
}
