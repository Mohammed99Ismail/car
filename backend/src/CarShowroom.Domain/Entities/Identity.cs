using CarShowroom.Domain.Common;

namespace CarShowroom.Domain.Entities;

public class User : AuditedEntityBase
{
    public required string FullName { get; set; }
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public bool IsActive { get; set; } = true;
    public string? PhoneNumber { get; set; }
    public string? AvatarUrl { get; set; }

    public ICollection<UserRole> UserRoles { get; set; } = [];
    public ICollection<RefreshToken> RefreshTokens { get; set; } = [];
    public ICollection<UserAttachment> Attachments { get; set; } = [];
}

public class UserAttachment : AuditedEntityBase
{
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public required string FileName { get; set; }
    public required string FilePath { get; set; }
    public required string ContentType { get; set; }
    public long FileSizeBytes { get; set; }
    public string? DocumentType { get; set; }
}

public class Role : AuditedEntityBase
{
    public required string NameAr { get; set; }
    public required string NameEn { get; set; }

    /// <summary>System roles (SuperAdmin, User) cannot be deleted.</summary>
    public bool IsSystem { get; set; }

    public ICollection<UserRole> UserRoles { get; set; } = [];
    public ICollection<RolePermission> RolePermissions { get; set; } = [];
}

public class Permission : BaseEntity
{
    public required string Key { get; set; }
    public required string Module { get; set; }
    public required string NameAr { get; set; }
    public required string NameEn { get; set; }

    public ICollection<RolePermission> RolePermissions { get; set; } = [];
}

public class UserRole
{
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public int RoleId { get; set; }
    public Role Role { get; set; } = null!;
}

public class RolePermission
{
    public int RoleId { get; set; }
    public Role Role { get; set; } = null!;
    public int PermissionId { get; set; }
    public Permission Permission { get; set; } = null!;
}

public class RefreshToken : BaseEntity
{
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public required string TokenHash { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime? RevokedAt { get; set; }
    public string? ReplacedByTokenHash { get; set; }

    public bool IsActive(DateTime utcNow) => RevokedAt is null && ExpiresAt > utcNow;
}

public class PasswordResetToken : BaseEntity
{
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public required string TokenHash { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime? UsedAt { get; set; }
}
