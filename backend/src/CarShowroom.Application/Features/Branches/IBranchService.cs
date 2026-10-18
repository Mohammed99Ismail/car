using CarShowroom.Application.Common.Models;
using CarShowroom.Application.Features.Branches.Dtos;

namespace CarShowroom.Application.Features.Branches;

public interface IBranchService
{
    Task<CustomApiResponse<PagedListModel<BranchDto>>> GetBranchesAsync(QueryParameters query, CancellationToken cancellationToken = default);
    Task<CustomApiResponse<BranchDto>> GetBranchByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<CustomApiResponse<BranchDto>> CreateBranchAsync(CreateBranchRequest request, CancellationToken cancellationToken = default);
    Task<CustomApiResponse<BranchDto>> UpdateBranchAsync(int id, UpdateBranchRequest request, CancellationToken cancellationToken = default);
    Task<CustomApiResponse<Nothing>> DeleteBranchAsync(int id, CancellationToken cancellationToken = default);
}
