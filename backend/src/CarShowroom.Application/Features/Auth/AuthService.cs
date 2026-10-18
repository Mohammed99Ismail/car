using CarShowroom.Application.Common.Interfaces;
using CarShowroom.Application.Common.Localization;
using CarShowroom.Application.Common.Models;
using CarShowroom.Application.Common.Security;
using CarShowroom.Application.Features.Auth.Dtos;
using CarShowroom.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CarShowroom.Application.Features.Auth;

public class AuthService(
    IApplicationDbContext context,
    IPasswordHasher passwordHasher,
    ITokenService tokenService,
    IEmailSender emailSender,
    ICurrentUserService currentUserService,
    TimeProvider timeProvider) : IAuthService
{
    public async Task<CustomApiResponse<AuthResponse>> RegisterAsync(
        RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();

        var exists = await context.Users
            .AnyAsync(u => u.Email.ToLower() == normalizedEmail, cancellationToken);

        if (exists)
        {
            return CustomApiResponse<AuthResponse>.Conflict(Msg.AlreadyExists(Msg.Email));
        }

        var user = new User
        {
            FullName = request.FullName.Trim(),
            Email = normalizedEmail,
            PasswordHash = passwordHasher.Hash(request.Password),
            IsActive = true
        };

        // Assign default User role
        user.UserRoles.Add(new UserRole { RoleId = SystemRoles.UserId });

        await context.Users.AddAsync(user, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return await GenerateAuthResponseAsync(user, cancellationToken);
    }

    public async Task<CustomApiResponse<AuthResponse>> LoginAsync(
        LoginRequest request, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();

        var user = await context.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                    .ThenInclude(r => r.RolePermissions)
                        .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(u => u.Email.ToLower() == normalizedEmail, cancellationToken);

        if (user is null || !passwordHasher.Verify(user.PasswordHash, request.Password))
        {
            return CustomApiResponse<AuthResponse>.BadRequest(Msg.InvalidCredentials);
        }

        if (!user.IsActive)
        {
            return CustomApiResponse<AuthResponse>.Fail(ResultStatusEnum.Forbidden, Msg.AccountDisabled);
        }

        return await GenerateAuthResponseAsync(user, cancellationToken);
    }

    public async Task<CustomApiResponse<AuthResponse>> RefreshTokenAsync(
        RefreshTokenRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            return CustomApiResponse<AuthResponse>.Unauthorized(Msg.InvalidRefreshToken);
        }

        var tokenHash = tokenService.HashToken(request.RefreshToken);
        var now = timeProvider.GetUtcNow().UtcDateTime;

        var storedToken = await context.RefreshTokens
            .Include(t => t.User)
                .ThenInclude(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                        .ThenInclude(r => r.RolePermissions)
                            .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(t => t.TokenHash == tokenHash, cancellationToken);

        if (storedToken is null || !storedToken.IsActive(now))
        {
            return CustomApiResponse<AuthResponse>.Unauthorized(Msg.InvalidRefreshToken);
        }

        if (!storedToken.User.IsActive)
        {
            return CustomApiResponse<AuthResponse>.Fail(ResultStatusEnum.Forbidden, Msg.AccountDisabled);
        }

        // Revoke the old token (rotation pattern)
        var newRawRefreshToken = tokenService.GenerateSecureToken();
        var newRefreshTokenHash = tokenService.HashToken(newRawRefreshToken);

        storedToken.RevokedAt = now;
        storedToken.ReplacedByTokenHash = newRefreshTokenHash;

        var newRefreshToken = new RefreshToken
        {
            UserId = storedToken.UserId,
            TokenHash = newRefreshTokenHash,
            CreatedAt = now,
            ExpiresAt = now.Add(tokenService.RefreshTokenLifetime)
        };

        await context.RefreshTokens.AddAsync(newRefreshToken, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        var (roleIds, permissions) = ResolveUserRolesAndPermissions(storedToken.User);
        var accessToken = tokenService.CreateAccessToken(storedToken.User, roleIds, permissions);

        var response = new AuthResponse(
            accessToken.Token,
            accessToken.ExpiresAt,
            newRawRefreshToken,
            new CurrentUserDto(
                storedToken.User.Id,
                storedToken.User.FullName,
                storedToken.User.Email,
                storedToken.User.UserRoles.Select(ur => ur.Role.NameEn).ToList(),
                permissions));

        return CustomApiResponse<AuthResponse>.Success(response);
    }

    public async Task<CustomApiResponse<Nothing>> LogoutAsync(
        string? refreshToken, CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrWhiteSpace(refreshToken))
        {
            var hash = tokenService.HashToken(refreshToken);
            var token = await context.RefreshTokens.FirstOrDefaultAsync(t => t.TokenHash == hash, cancellationToken);
            if (token is not null && token.RevokedAt is null)
            {
                token.RevokedAt = timeProvider.GetUtcNow().UtcDateTime;
                await context.SaveChangesAsync(cancellationToken);
            }
        }

        return CustomApiResponse<Nothing>.Success(Nothing.Value, Msg.LoggedOut);
    }

    public async Task<CustomApiResponse<Nothing>> ForgotPasswordAsync(
        ForgotPasswordRequest request, string clientOrigin, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var user = await context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == normalizedEmail, cancellationToken);

        // Security: always return success so attackers cannot enumerate valid emails
        if (user is null || !user.IsActive)
        {
            return CustomApiResponse<Nothing>.Success(Nothing.Value, Msg.ResetLinkSent);
        }

        var rawToken = tokenService.GenerateSecureToken();
        var tokenHash = tokenService.HashToken(rawToken);
        var now = timeProvider.GetUtcNow().UtcDateTime;

        var resetToken = new PasswordResetToken
        {
            UserId = user.Id,
            TokenHash = tokenHash,
            ExpiresAt = now.AddHours(1)
        };

        await context.PasswordResetTokens.AddAsync(resetToken, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        var baseUrl = string.IsNullOrWhiteSpace(clientOrigin) ? "http://localhost:5173" : clientOrigin.TrimEnd('/');
        var resetLink = $"{baseUrl}/reset-password?token={Uri.EscapeDataString(rawToken)}";

        await emailSender.SendAsync(
            user.Email,
            Msg.ResetEmailSubject,
            Msg.ResetEmailBody(user.FullName, resetLink),
            cancellationToken);

        return CustomApiResponse<Nothing>.Success(Nothing.Value, Msg.ResetLinkSent);
    }

    public async Task<CustomApiResponse<Nothing>> ResetPasswordAsync(
        ResetPasswordRequest request, CancellationToken cancellationToken = default)
    {
        var tokenHash = tokenService.HashToken(request.Token);
        var now = timeProvider.GetUtcNow().UtcDateTime;

        var resetToken = await context.PasswordResetTokens
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.TokenHash == tokenHash, cancellationToken);

        if (resetToken is null || resetToken.UsedAt is not null || resetToken.ExpiresAt < now)
        {
            return CustomApiResponse<Nothing>.BadRequest(Msg.InvalidResetToken);
        }

        resetToken.UsedAt = now;
        resetToken.User.PasswordHash = passwordHasher.Hash(request.NewPassword);

        // Revoke all active refresh tokens for this user
        var activeTokens = await context.RefreshTokens
            .Where(t => t.UserId == resetToken.UserId && t.RevokedAt == null && t.ExpiresAt > now)
            .ToListAsync(cancellationToken);

        foreach (var t in activeTokens)
        {
            t.RevokedAt = now;
        }

        await context.SaveChangesAsync(cancellationToken);

        return CustomApiResponse<Nothing>.Success(Nothing.Value, Msg.PasswordReset);
    }

    public async Task<CustomApiResponse<CurrentUserDto>> GetCurrentUserAsync(CancellationToken cancellationToken = default)
    {
        var userId = currentUserService.UserId;
        if (userId is null)
        {
            return CustomApiResponse<CurrentUserDto>.Unauthorized(Msg.Unauthenticated);
        }

        var user = await context.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                    .ThenInclude(r => r.RolePermissions)
                        .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(u => u.Id == userId.Value, cancellationToken);

        if (user is null || !user.IsActive)
        {
            return CustomApiResponse<CurrentUserDto>.Unauthorized(Msg.Unauthenticated);
        }

        var (_, permissions) = ResolveUserRolesAndPermissions(user);

        var dto = new CurrentUserDto(
            user.Id,
            user.FullName,
            user.Email,
            user.UserRoles.Select(ur => ur.Role.NameEn).ToList(),
            permissions);

        return CustomApiResponse<CurrentUserDto>.Success(dto);
    }

    private async Task<CustomApiResponse<AuthResponse>> GenerateAuthResponseAsync(
        User user, CancellationToken cancellationToken)
    {
        var (roleIds, permissions) = ResolveUserRolesAndPermissions(user);
        var accessToken = tokenService.CreateAccessToken(user, roleIds, permissions);

        var rawRefreshToken = tokenService.GenerateSecureToken();
        var refreshTokenHash = tokenService.HashToken(rawRefreshToken);
        var now = timeProvider.GetUtcNow().UtcDateTime;

        var refreshToken = new RefreshToken
        {
            UserId = user.Id,
            TokenHash = refreshTokenHash,
            CreatedAt = now,
            ExpiresAt = now.Add(tokenService.RefreshTokenLifetime)
        };

        await context.RefreshTokens.AddAsync(refreshToken, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        var authResponse = new AuthResponse(
            accessToken.Token,
            accessToken.ExpiresAt,
            rawRefreshToken,
            new CurrentUserDto(
                user.Id,
                user.FullName,
                user.Email,
                user.UserRoles.Select(ur => ur.Role.NameEn).ToList(),
                permissions));

        return CustomApiResponse<AuthResponse>.Success(authResponse, Msg.Registered);
    }

    private static (List<int> RoleIds, List<string> Permissions) ResolveUserRolesAndPermissions(User user)
    {
        var roleIds = user.UserRoles.Select(ur => ur.RoleId).ToList();

        // If SuperAdmin, include all permissions
        var isSuperAdmin = roleIds.Contains(SystemRoles.SuperAdminId);

        var permissions = isSuperAdmin
            ? PermissionKeysConstant.All.Select(p => p.Key).Distinct().ToList()
            : user.UserRoles
                .SelectMany(ur => ur.Role.RolePermissions)
                .Select(rp => rp.Permission.Key)
                .Distinct()
                .ToList();

        return (roleIds, permissions);
    }
}
