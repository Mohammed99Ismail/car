using CarShowroom.Application.Common.Caching;
using CarShowroom.Application.Common.Interfaces;
using CarShowroom.Application.Common.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;

namespace CarShowroom.Application.Features.Dashboard;

public class DashboardService(
    IApplicationDbContext context,
    HybridCache cache) : IDashboardService
{
    // Short TTL so the dashboard stays fresh after CRUD while still absorbing
    // repeated navigations/refreshes (the home page is the most-hit route).
    private static readonly HybridCacheEntryOptions StatsCacheOptions = new()
    {
        Expiration = TimeSpan.FromSeconds(30),
        LocalCacheExpiration = TimeSpan.FromSeconds(30)
    };

    public async Task<CustomApiResponse<DashboardStatsDto>> GetStatsAsync(
        CancellationToken cancellationToken = default)
    {
        var stats = await cache.GetOrCreateAsync(
            CacheKey.DashboardStats,
            async token =>
            {
                // Four cheap COUNT(*) queries executed against the same context
                // sequentially (DbContext is not thread-safe for concurrent use).
                var cars = await context.Cars.CountAsync(token);
                var customers = await context.Customers.CountAsync(token);
                var branches = await context.Branches.CountAsync(token);
                var contractTypes = await context.ContractTypes.CountAsync(token);
                return new DashboardStatsDto(cars, customers, branches, contractTypes);
            },
            StatsCacheOptions,
            cancellationToken: cancellationToken);

        return CustomApiResponse<DashboardStatsDto>.Success(stats);
    }
}
