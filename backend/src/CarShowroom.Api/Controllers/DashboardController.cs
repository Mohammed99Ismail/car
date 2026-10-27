using CarShowroom.Application.Common.Models;
using CarShowroom.Application.Features.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarShowroom.Api.Controllers;

/// <summary>
/// Dashboard aggregates. Any authenticated user can see the home-page counts,
/// so this is gated by [Authorize] rather than a specific module permission.
/// </summary>
[Route("api/dashboard")]
[ApiController]
public class DashboardController(IDashboardService dashboardService) : BaseApiController
{
    [HttpGet("stats")]
    [Authorize]
    public async Task<ActionResult<CustomApiResponse<DashboardStatsDto>>> GetStats(
        CancellationToken cancellationToken)
    {
        var result = await dashboardService.GetStatsAsync(cancellationToken);
        return CustomResult(result);
    }
}
