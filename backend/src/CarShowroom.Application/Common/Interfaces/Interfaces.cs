using CarShowroom.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CarShowroom.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
    DbSet<Role> Roles { get; }
    DbSet<Permission> Permissions { get; }
    DbSet<UserRole> UserRoles { get; }
    DbSet<RolePermission> RolePermissions { get; }
    DbSet<RefreshToken> RefreshTokens { get; }
    DbSet<PasswordResetToken> PasswordResetTokens { get; }
    DbSet<CarMake> CarMakes { get; }
    DbSet<CarModel> CarModels { get; }
    DbSet<ContractType> ContractTypes { get; }
    DbSet<Branch> Branches { get; }
    DbSet<Customer> Customers { get; }
    DbSet<Car> Cars { get; }
    DbSet<UserAttachment> UserAttachments { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

/// <summary>Current authenticated user (adapted from Shared/Auth/ICurrentUserService).</summary>
public interface ICurrentUserService
{
    int? UserId { get; }
    string? Email { get; }
    bool IsAuthenticated { get; }
    IReadOnlyList<int> RoleIds { get; }
    IReadOnlyList<string> Permissions { get; }
}

public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string hash, string password);
}

public record AccessToken(string Token, DateTime ExpiresAt);

public interface ITokenService
{
    AccessToken CreateAccessToken(User user, IReadOnlyCollection<int> roleIds, IReadOnlyCollection<string> permissions);

    /// <summary>Creates a cryptographically-random opaque token (used for refresh + password reset).</summary>
    string GenerateSecureToken();

    /// <summary>SHA-256 hash; only hashes are persisted.</summary>
    string HashToken(string token);

    TimeSpan RefreshTokenLifetime { get; }
}

public interface IEmailSender
{
    Task SendAsync(string to, string subject, string htmlBody, CancellationToken cancellationToken = default);
}

public interface IFileStorage
{
    /// <summary>Saves the file under wwwroot/{folder} and returns the relative web path (e.g. /uploads/cars/x.jpg).</summary>
    Task<string> SaveAsync(Stream content, string extension, string folder, CancellationToken cancellationToken = default);

    void Delete(string? relativePath);
}
