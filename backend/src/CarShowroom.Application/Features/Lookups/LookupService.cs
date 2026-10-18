using CarShowroom.Application.Common.Caching;
using CarShowroom.Application.Common.Interfaces;
using CarShowroom.Application.Common.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;

namespace CarShowroom.Application.Features.Lookups;

public class LookupService(
    IApplicationDbContext context,
    HybridCache cache,
    TimeProvider timeProvider) : ILookupService
{
    private static readonly HybridCacheEntryOptions LongCacheOptions = new()
    {
        Expiration = TimeSpan.FromHours(2),
        LocalCacheExpiration = TimeSpan.FromHours(1)
    };

    public async Task<CustomApiResponse<IReadOnlyList<LookupItemDto>>> GetMakesAsync(
        string? search, CancellationToken cancellationToken = default)
    {
        var allMakes = await cache.GetOrCreateAsync(
            CacheKey.CarMakes,
            async token => await context.CarMakes
                .AsNoTracking()
                .OrderBy(m => m.Name)
                .Select(m => new LookupItemDto(m.Id, m.Name))
                .ToListAsync(token),
            LongCacheOptions,
            cancellationToken: cancellationToken);

        if (string.IsNullOrWhiteSpace(search))
        {
            return CustomApiResponse<IReadOnlyList<LookupItemDto>>.Success(allMakes);
        }

        var filtered = allMakes
            .Where(m => m.Name.Contains(search.Trim(), StringComparison.OrdinalIgnoreCase))
            .ToList();

        return CustomApiResponse<IReadOnlyList<LookupItemDto>>.Success(filtered);
    }

    public async Task<CustomApiResponse<IReadOnlyList<LookupItemDto>>> GetModelsAsync(
        int makeId, string? search, CancellationToken cancellationToken = default)
    {
        var cacheKey = CacheKey.CarModels(makeId);

        var allModels = await cache.GetOrCreateAsync(
            cacheKey,
            async token => await context.CarModels
                .AsNoTracking()
                .Where(m => m.MakeId == makeId)
                .OrderBy(m => m.Name)
                .Select(m => new LookupItemDto(m.Id, m.Name))
                .ToListAsync(token),
            LongCacheOptions,
            cancellationToken: cancellationToken);

        if (string.IsNullOrWhiteSpace(search))
        {
            return CustomApiResponse<IReadOnlyList<LookupItemDto>>.Success(allModels);
        }

        var filtered = allModels
            .Where(m => m.Name.Contains(search.Trim(), StringComparison.OrdinalIgnoreCase))
            .ToList();

        return CustomApiResponse<IReadOnlyList<LookupItemDto>>.Success(filtered);
    }

    public CustomApiResponse<IReadOnlyList<int>> GetManufactureYears()
    {
        var currentYear = timeProvider.GetUtcNow().Year;
        var maxYear = currentYear + 1;
        const int minYear = 1980;

        var years = Enumerable.Range(minYear, maxYear - minYear + 1)
            .OrderByDescending(y => y)
            .ToList();

        return CustomApiResponse<IReadOnlyList<int>>.Success(years);
    }

    public async Task<CustomApiResponse<IReadOnlyList<LookupItemDto>>> GetBranchesLookupAsync(
        string? search, CancellationToken cancellationToken = default)
    {
        var source = context.Branches.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim().ToLower();
            source = source.Where(b => b.Name.ToLower().Contains(s));
        }

        var branches = await source
            .OrderBy(b => b.Name)
            .Select(b => new LookupItemDto(b.Id, b.Name))
            .ToListAsync(cancellationToken);

        return CustomApiResponse<IReadOnlyList<LookupItemDto>>.Success(branches);
    }

    public async Task<CustomApiResponse<IReadOnlyList<LookupDto>>> GetContractTypesAsync(
        CancellationToken cancellationToken = default)
    {
        var list = await cache.GetOrCreateAsync(
            CacheKey.ContractTypes,
            async token => await context.ContractTypes
                .AsNoTracking()
                .OrderBy(c => c.Id)
                .Select(c => new LookupDto(c.Id, c.NameAr, c.NameEn))
                .ToListAsync(token),
            LongCacheOptions,
            cancellationToken: cancellationToken);

        return CustomApiResponse<IReadOnlyList<LookupDto>>.Success(list);
    }

    public async Task<CustomApiResponse<IReadOnlyList<LookupDto>>> GetRolesLookupAsync(
        CancellationToken cancellationToken = default)
    {
        var roles = await context.Roles
            .AsNoTracking()
            .OrderBy(r => r.Id)
            .Select(r => new LookupDto(r.Id, r.NameAr, r.NameEn))
            .ToListAsync(cancellationToken);

        return CustomApiResponse<IReadOnlyList<LookupDto>>.Success(roles);
    }
}
