using CarShowroom.Application.Common.Models;
using CarShowroom.Application.Features.Roles.Dtos;

namespace CarShowroom.Application.Features.Roles;

public interface IRoleService
{
    Task<CustomApiResponse<PagedListModel<RoleSummaryDto>>> GetRolesAsync(QueryParameters query, CancellationToken cancellationToken = default);
    Task<CustomApiResponse<RoleDetailDto>> GetRoleByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<CustomApiResponse<RoleDetailDto>> CreateRoleAsync(CreateRoleRequest request, CancellationToken cancellationToken = default);
    Task<CustomApiResponse<RoleDetailDto>> UpdateRoleAsync(int id, UpdateRoleRequest request, CancellationToken cancellationToken = default);
    Task<CustomApiResponse<Nothing>> DeleteRoleAsync(int id, CancellationToken cancellationToken = default);
    Task<CustomApiResponse<IReadOnlyList<ModulePermissionsDto>>> GetAllPermissionsAsync(CancellationToken cancellationToken = default);
}
