using CarShowroom.Application.Common.Models;

namespace CarShowroom.Application.Features.Roles.Dtos;

public record RoleSummaryDto(
    int Id,
    string NameAr,
    string NameEn,
    bool IsSystem,
    int PermissionsCount,
    int UsersCount);

public record RoleDetailDto(
    int Id,
    string NameAr,
    string NameEn,
    bool IsSystem,
    IReadOnlyList<int> PermissionIds);

public record CreateRoleRequest(
    string NameAr,
    string NameEn,
    IReadOnlyList<int>? PermissionIds);

public record UpdateRoleRequest(
    string NameAr,
    string NameEn,
    IReadOnlyList<int>? PermissionIds);

public record PermissionDto(
    int Id,
    string Key,
    string Module,
    string NameAr,
    string NameEn);

public record ModulePermissionsDto(
    string Module,
    IReadOnlyList<PermissionDto> Permissions);
