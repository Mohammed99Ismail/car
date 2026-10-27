using CarShowroom.Application.Common.Models;

namespace CarShowroom.Application.Features.Dashboard;

public interface IDashboardService
{
    Task<CustomApiResponse<DashboardStatsDto>> GetStatsAsync(CancellationToken cancellationToken = default);
}
