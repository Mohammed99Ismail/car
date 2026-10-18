namespace CarShowroom.Application.Features.Users.Dtos;

public record UserSummaryDto(
    int Id,
    string FullName,
    string Email,
    bool IsActive,
    DateTime CreatedDate,
    IReadOnlyList<string> Roles);

public record UserDetailDto(
    int Id,
    string FullName,
    string Email,
    bool IsActive,
    DateTime CreatedDate,
    IReadOnlyList<int> RoleIds);

public record CreateUserRequest(
    string FullName,
    string Email,
    string Password,
    IReadOnlyList<int>? RoleIds);

public record UpdateUserRolesRequest(
    IReadOnlyList<int> RoleIds);

public record ToggleUserStatusRequest(
    bool IsActive);
