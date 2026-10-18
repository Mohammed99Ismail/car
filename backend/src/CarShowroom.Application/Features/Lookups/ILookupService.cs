using CarShowroom.Application.Common.Models;

namespace CarShowroom.Application.Features.Lookups;

public interface ILookupService
{
    Task<CustomApiResponse<IReadOnlyList<LookupItemDto>>> GetMakesAsync(string? search, CancellationToken cancellationToken = default);
    Task<CustomApiResponse<IReadOnlyList<LookupItemDto>>> GetModelsAsync(int makeId, string? search, CancellationToken cancellationToken = default);
    CustomApiResponse<IReadOnlyList<int>> GetManufactureYears();
    Task<CustomApiResponse<IReadOnlyList<LookupItemDto>>> GetBranchesLookupAsync(string? search, CancellationToken cancellationToken = default);
    Task<CustomApiResponse<IReadOnlyList<LookupDto>>> GetContractTypesAsync(CancellationToken cancellationToken = default);
    Task<CustomApiResponse<IReadOnlyList<LookupDto>>> GetRolesLookupAsync(CancellationToken cancellationToken = default);
}
