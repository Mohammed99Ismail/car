using CarShowroom.Application.Common.Models;
using CarShowroom.Application.Features.Users.Dtos;

namespace CarShowroom.Application.Features.Users;

public interface IUserService
{
    Task<CustomApiResponse<PagedListModel<UserSummaryDto>>> GetUsersAsync(QueryParameters query, CancellationToken cancellationToken = default);
    Task<CustomApiResponse<UserDetailDto>> GetUserByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<CustomApiResponse<UserDetailDto>> CreateUserAsync(CreateUserRequest request, CancellationToken cancellationToken = default);
    Task<CustomApiResponse<Nothing>> UpdateUserRolesAsync(int id, UpdateUserRolesRequest request, CancellationToken cancellationToken = default);
    Task<CustomApiResponse<Nothing>> ToggleUserStatusAsync(int id, ToggleUserStatusRequest request, CancellationToken cancellationToken = default);
}
