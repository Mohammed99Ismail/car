using CarShowroom.Application.Common.Extensions;
using CarShowroom.Application.Common.Interfaces;
using CarShowroom.Application.Common.Localization;
using CarShowroom.Application.Common.Models;
using CarShowroom.Application.Features.Users.Dtos;
using CarShowroom.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CarShowroom.Application.Features.Users;

public class UserService(
    IApplicationDbContext context,
    IPasswordHasher passwordHasher,
    ICurrentUserService currentUserService,
    TimeProvider timeProvider) : IUserService
{
    public async Task<CustomApiResponse<PagedListModel<UserSummaryDto>>> GetUsersAsync(
        QueryParameters query, CancellationToken cancellationToken = default)
    {
        var source = context.Users.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim().ToLower();
            source = source.Where(u => u.FullName.ToLower().Contains(search) || u.Email.ToLower().Contains(search));
        }

        var paged = await source.ToPagedListAsync(
            u => new UserSummaryDto(
                u.Id,
                u.FullName,
                u.Email,
                u.IsActive,
                u.CreatedDate,
                u.UserRoles.Select(ur => ur.Role.NameAr).ToList()),
            query,
            nameof(User.Id),
            cancellationToken);

        return CustomApiResponse<PagedListModel<UserSummaryDto>>.Success(paged);
    }

    public async Task<CustomApiResponse<UserDetailDto>> GetUserByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var user = await context.Users
            .Include(u => u.UserRoles)
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

        if (user is null)
        {
            return CustomApiResponse<UserDetailDto>.NotFound(Msg.NotFound(Msg.User));
        }

        var dto = new UserDetailDto(
            user.Id,
            user.FullName,
            user.Email,
            user.IsActive,
            user.CreatedDate,
            user.UserRoles.Select(ur => ur.RoleId).ToList());

        return CustomApiResponse<UserDetailDto>.Success(dto);
    }

    public async Task<CustomApiResponse<UserDetailDto>> CreateUserAsync(
        CreateUserRequest request, CancellationToken cancellationToken = default)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var exists = await context.Users.AnyAsync(u => u.Email.ToLower() == email, cancellationToken);
        if (exists)
        {
            return CustomApiResponse<UserDetailDto>.Conflict(Msg.AlreadyExists(Msg.Email));
        }

        var user = new User
        {
            FullName = request.FullName.Trim(),
            Email = email,
            PasswordHash = passwordHasher.Hash(request.Password),
            IsActive = true
        };

        if (request.RoleIds is { Count: > 0 })
        {
            var validRoleIds = await context.Roles
                .Where(r => request.RoleIds.Contains(r.Id))
                .Select(r => r.Id)
                .ToListAsync(cancellationToken);

            foreach (var rId in validRoleIds)
            {
                user.UserRoles.Add(new UserRole { RoleId = rId });
            }
        }

        await context.Users.AddAsync(user, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        var dto = new UserDetailDto(
            user.Id,
            user.FullName,
            user.Email,
            user.IsActive,
            user.CreatedDate,
            user.UserRoles.Select(ur => ur.RoleId).ToList());

        return CustomApiResponse<UserDetailDto>.Created(dto, Msg.Saved);
    }

    public async Task<CustomApiResponse<Nothing>> UpdateUserRolesAsync(
        int id, UpdateUserRolesRequest request, CancellationToken cancellationToken = default)
    {
        var user = await context.Users
            .Include(u => u.UserRoles)
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

        if (user is null)
        {
            return CustomApiResponse<Nothing>.NotFound(Msg.NotFound(Msg.User));
        }

        // Prevent modifying current user's own roles to avoid lockout
        if (currentUserService.UserId == id)
        {
            return CustomApiResponse<Nothing>.BadRequest(Msg.CannotModifySelf);
        }

        user.UserRoles.Clear();
        if (request.RoleIds is { Count: > 0 })
        {
            var validRoleIds = await context.Roles
                .Where(r => request.RoleIds.Contains(r.Id))
                .Select(r => r.Id)
                .ToListAsync(cancellationToken);

            foreach (var rId in validRoleIds)
            {
                user.UserRoles.Add(new UserRole { UserId = user.Id, RoleId = rId });
            }
        }

        await context.SaveChangesAsync(cancellationToken);
        return CustomApiResponse<Nothing>.Success(Nothing.Value, Msg.Saved);
    }

    public async Task<CustomApiResponse<Nothing>> ToggleUserStatusAsync(
        int id, ToggleUserStatusRequest request, CancellationToken cancellationToken = default)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        if (user is null)
        {
            return CustomApiResponse<Nothing>.NotFound(Msg.NotFound(Msg.User));
        }

        if (currentUserService.UserId == id)
        {
            return CustomApiResponse<Nothing>.BadRequest(Msg.CannotModifySelf);
        }

        user.IsActive = request.IsActive;

        // If deactivated, revoke all active refresh tokens
        if (!user.IsActive)
        {
            var now = timeProvider.GetUtcNow().UtcDateTime;
            var tokens = await context.RefreshTokens
                .Where(t => t.UserId == id && t.RevokedAt == null && t.ExpiresAt > now)
                .ToListAsync(cancellationToken);

            foreach (var t in tokens)
            {
                t.RevokedAt = now;
            }
        }

        await context.SaveChangesAsync(cancellationToken);
        return CustomApiResponse<Nothing>.Success(Nothing.Value, Msg.Saved);
    }
}
