using CarShowroom.Application.Common.Extensions;
using CarShowroom.Application.Common.Interfaces;
using CarShowroom.Application.Common.Localization;
using CarShowroom.Application.Common.Models;
using CarShowroom.Application.Common.Security;
using CarShowroom.Application.Features.Roles.Dtos;
using CarShowroom.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CarShowroom.Application.Features.Roles;

public class RoleService(IApplicationDbContext context) : IRoleService
{
    public async Task<CustomApiResponse<PagedListModel<RoleSummaryDto>>> GetRolesAsync(
        QueryParameters query, CancellationToken cancellationToken = default)
    {
        var source = context.Roles.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim().ToLower();
            source = source.Where(r => r.NameAr.ToLower().Contains(search) || r.NameEn.ToLower().Contains(search));
        }

        var paged = await source.ToPagedListAsync(
            r => new RoleSummaryDto(
                r.Id,
                r.NameAr,
                r.NameEn,
                r.IsSystem,
                r.RolePermissions.Count,
                r.UserRoles.Count),
            query,
            nameof(Role.Id),
            cancellationToken);

        return CustomApiResponse<PagedListModel<RoleSummaryDto>>.Success(paged);
    }

    public async Task<CustomApiResponse<RoleDetailDto>> GetRoleByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var role = await context.Roles
            .Include(r => r.RolePermissions)
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

        if (role is null)
        {
            return CustomApiResponse<RoleDetailDto>.NotFound(Msg.NotFound(Msg.Role));
        }

        var dto = new RoleDetailDto(
            role.Id,
            role.NameAr,
            role.NameEn,
            role.IsSystem,
            role.RolePermissions.Select(rp => rp.PermissionId).ToList());

        return CustomApiResponse<RoleDetailDto>.Success(dto);
    }

    public async Task<CustomApiResponse<RoleDetailDto>> CreateRoleAsync(
        CreateRoleRequest request, CancellationToken cancellationToken = default)
    {
        var nameEn = request.NameEn.Trim();
        var exists = await context.Roles.AnyAsync(r => r.NameEn.ToLower() == nameEn.ToLower(), cancellationToken);
        if (exists)
        {
            return CustomApiResponse<RoleDetailDto>.Conflict(Msg.AlreadyExists(Msg.RoleName));
        }

        var role = new Role
        {
            NameAr = request.NameAr.Trim(),
            NameEn = nameEn,
            IsSystem = false
        };

        if (request.PermissionIds is { Count: > 0 })
        {
            var validPermIds = await context.Permissions
                .Where(p => request.PermissionIds.Contains(p.Id))
                .Select(p => p.Id)
                .ToListAsync(cancellationToken);

            foreach (var permId in validPermIds)
            {
                role.RolePermissions.Add(new RolePermission { PermissionId = permId });
            }
        }

        await context.Roles.AddAsync(role, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        var dto = new RoleDetailDto(
            role.Id,
            role.NameAr,
            role.NameEn,
            role.IsSystem,
            role.RolePermissions.Select(rp => rp.PermissionId).ToList());

        return CustomApiResponse<RoleDetailDto>.Created(dto, Msg.Saved);
    }

    public async Task<CustomApiResponse<RoleDetailDto>> UpdateRoleAsync(
        int id, UpdateRoleRequest request, CancellationToken cancellationToken = default)
    {
        var role = await context.Roles
            .Include(r => r.RolePermissions)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

        if (role is null)
        {
            return CustomApiResponse<RoleDetailDto>.NotFound(Msg.NotFound(Msg.Role));
        }

        if (role.IsSystem && id == SystemRoles.SuperAdminId)
        {
            return CustomApiResponse<RoleDetailDto>.BadRequest(Msg.SystemRoleProtected);
        }

        var nameEn = request.NameEn.Trim();
        var duplicate = await context.Roles
            .AnyAsync(r => r.Id != id && r.NameEn.ToLower() == nameEn.ToLower(), cancellationToken);

        if (duplicate)
        {
            return CustomApiResponse<RoleDetailDto>.Conflict(Msg.AlreadyExists(Msg.RoleName));
        }

        role.NameAr = request.NameAr.Trim();
        role.NameEn = nameEn;

        // Flexible permission updating: completely synchronize role's permissions
        role.RolePermissions.Clear();
        if (request.PermissionIds is { Count: > 0 })
        {
            var validPermIds = await context.Permissions
                .Where(p => request.PermissionIds.Contains(p.Id))
                .Select(p => p.Id)
                .ToListAsync(cancellationToken);

            foreach (var permId in validPermIds)
            {
                role.RolePermissions.Add(new RolePermission { RoleId = role.Id, PermissionId = permId });
            }
        }

        await context.SaveChangesAsync(cancellationToken);

        var dto = new RoleDetailDto(
            role.Id,
            role.NameAr,
            role.NameEn,
            role.IsSystem,
            role.RolePermissions.Select(rp => rp.PermissionId).ToList());

        return CustomApiResponse<RoleDetailDto>.Success(dto, Msg.Saved);
    }

    public async Task<CustomApiResponse<Nothing>> DeleteRoleAsync(int id, CancellationToken cancellationToken = default)
    {
        var role = await context.Roles
            .Include(r => r.UserRoles)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

        if (role is null)
        {
            return CustomApiResponse<Nothing>.NotFound(Msg.NotFound(Msg.Role));
        }

        if (role.IsSystem)
        {
            return CustomApiResponse<Nothing>.BadRequest(Msg.SystemRoleProtected);
        }

        if (role.UserRoles.Count > 0)
        {
            return CustomApiResponse<Nothing>.Conflict(Msg.InUse(Msg.Role));
        }

        context.Roles.Remove(role);
        await context.SaveChangesAsync(cancellationToken);

        return CustomApiResponse<Nothing>.Success(Nothing.Value, Msg.Deleted);
    }

    public async Task<CustomApiResponse<IReadOnlyList<ModulePermissionsDto>>> GetAllPermissionsAsync(
        CancellationToken cancellationToken = default)
    {
        var permissions = await context.Permissions
            .AsNoTracking()
            .OrderBy(p => p.Id)
            .ToListAsync(cancellationToken);

        var grouped = permissions
            .GroupBy(p => p.Module)
            .Select(g => new ModulePermissionsDto(
                g.Key,
                g.Select(p => new PermissionDto(p.Id, p.Key, p.Module, p.NameAr, p.NameEn)).ToList()))
            .ToList();

        return CustomApiResponse<IReadOnlyList<ModulePermissionsDto>>.Success(grouped);
    }
}
