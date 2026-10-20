namespace CarShowroom.Application.Features.Profile.Dtos;

public record UserProfileDto(
    int Id,
    string FullName,
    string Email,
    string? PhoneNumber,
    string? AvatarUrl,
    bool IsActive,
    DateTime CreatedDate,
    IReadOnlyList<string> Roles,
    IReadOnlyList<string> Permissions,
    IReadOnlyList<UserAttachmentDto> Attachments);

public record UserAttachmentDto(
    int Id,
    string FileName,
    string FilePath,
    string ContentType,
    long FileSizeBytes,
    string? DocumentType,
    DateTime CreatedDate);

public record UpdateProfileRequest(
    string FullName,
    string? PhoneNumber);

public record ChangePasswordRequest(
    string CurrentPassword,
    string NewPassword);
