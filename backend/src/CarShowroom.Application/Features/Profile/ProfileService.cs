using CarShowroom.Application.Common.Interfaces;
using CarShowroom.Application.Common.Localization;
using CarShowroom.Application.Common.Models;
using CarShowroom.Application.Features.Profile.Dtos;
using CarShowroom.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CarShowroom.Application.Features.Profile;

public class ProfileService(
    IApplicationDbContext context,
    ICurrentUserService currentUserService,
    IPasswordHasher passwordHasher,
    IFileStorage fileStorage) : IProfileService
{
    private static readonly HashSet<string> AllowedAvatarExtensions = [".jpg", ".jpeg", ".png", ".webp"];
    private static readonly HashSet<string> AllowedAttachmentExtensions =
        [".jpg", ".jpeg", ".png", ".webp", ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".txt"];

    public async Task<CustomApiResponse<UserProfileDto>> GetProfileAsync(CancellationToken cancellationToken = default)
    {
        if (currentUserService.UserId is null)
        {
            return CustomApiResponse<UserProfileDto>.Unauthorized(Msg.Unauthenticated);
        }

        var userId = currentUserService.UserId.Value;
        var user = await context.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .Include(u => u.Attachments)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user is null)
        {
            return CustomApiResponse<UserProfileDto>.NotFound(Msg.NotFound(Msg.User));
        }

        var dto = MapToDto(user);
        return CustomApiResponse<UserProfileDto>.Success(dto);
    }

    public async Task<CustomApiResponse<UserProfileDto>> UpdateProfileAsync(
        UpdateProfileRequest request, CancellationToken cancellationToken = default)
    {
        if (currentUserService.UserId is null)
        {
            return CustomApiResponse<UserProfileDto>.Unauthorized(Msg.Unauthenticated);
        }

        if (string.IsNullOrWhiteSpace(request.FullName))
        {
            return CustomApiResponse<UserProfileDto>.BadRequest(Msg.ValidationFailed);
        }

        var userId = currentUserService.UserId.Value;
        var user = await context.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .Include(u => u.Attachments)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user is null)
        {
            return CustomApiResponse<UserProfileDto>.NotFound(Msg.NotFound(Msg.User));
        }

        user.FullName = request.FullName.Trim();
        user.PhoneNumber = request.PhoneNumber?.Trim();

        await context.SaveChangesAsync(cancellationToken);

        var dto = MapToDto(user);
        return CustomApiResponse<UserProfileDto>.Success(dto, Msg.ProfileUpdated);
    }

    public async Task<CustomApiResponse<Nothing>> ChangePasswordAsync(
        ChangePasswordRequest request, CancellationToken cancellationToken = default)
    {
        if (currentUserService.UserId is null)
        {
            return CustomApiResponse<Nothing>.Unauthorized(Msg.Unauthenticated);
        }

        if (string.IsNullOrWhiteSpace(request.CurrentPassword) || string.IsNullOrWhiteSpace(request.NewPassword))
        {
            return CustomApiResponse<Nothing>.BadRequest(Msg.ValidationFailed);
        }

        if (request.NewPassword.Length < 6)
        {
            return CustomApiResponse<Nothing>.BadRequest(Msg.ValidationFailed);
        }

        var userId = currentUserService.UserId.Value;
        var user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        if (user is null)
        {
            return CustomApiResponse<Nothing>.NotFound(Msg.NotFound(Msg.User));
        }

        if (!passwordHasher.Verify(user.PasswordHash, request.CurrentPassword))
        {
            return CustomApiResponse<Nothing>.BadRequest(Msg.InvalidCurrentPassword);
        }

        user.PasswordHash = passwordHasher.Hash(request.NewPassword);
        await context.SaveChangesAsync(cancellationToken);

        return CustomApiResponse<Nothing>.Success(Nothing.Value, Msg.PasswordChanged);
    }

    public async Task<CustomApiResponse<string>> UploadAvatarAsync(
        Stream fileStream, string extension, CancellationToken cancellationToken = default)
    {
        if (currentUserService.UserId is null)
        {
            return CustomApiResponse<string>.Unauthorized(Msg.Unauthenticated);
        }

        var ext = extension.ToLowerInvariant();
        if (!AllowedAvatarExtensions.Contains(ext))
        {
            return CustomApiResponse<string>.BadRequest(Msg.InvalidImage);
        }

        var userId = currentUserService.UserId.Value;
        var user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        if (user is null)
        {
            return CustomApiResponse<string>.NotFound(Msg.NotFound(Msg.User));
        }

        if (!string.IsNullOrWhiteSpace(user.AvatarUrl) && user.AvatarUrl.StartsWith("/uploads/avatars/"))
        {
            fileStorage.Delete(user.AvatarUrl);
        }

        var webPath = await fileStorage.SaveAsync(fileStream, ext, "avatars", cancellationToken);
        user.AvatarUrl = webPath;
        await context.SaveChangesAsync(cancellationToken);

        return CustomApiResponse<string>.Success(webPath, Msg.Saved);
    }

    public async Task<CustomApiResponse<Nothing>> DeleteAvatarAsync(CancellationToken cancellationToken = default)
    {
        if (currentUserService.UserId is null)
        {
            return CustomApiResponse<Nothing>.Unauthorized(Msg.Unauthenticated);
        }

        var userId = currentUserService.UserId.Value;
        var user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        if (user is null)
        {
            return CustomApiResponse<Nothing>.NotFound(Msg.NotFound(Msg.User));
        }

        if (!string.IsNullOrWhiteSpace(user.AvatarUrl) && user.AvatarUrl.StartsWith("/uploads/avatars/"))
        {
            fileStorage.Delete(user.AvatarUrl);
        }

        user.AvatarUrl = null;
        await context.SaveChangesAsync(cancellationToken);

        return CustomApiResponse<Nothing>.Success(Nothing.Value, Msg.Deleted);
    }

    public async Task<CustomApiResponse<UserAttachmentDto>> UploadAttachmentAsync(
        Stream fileStream, string fileName, string contentType, long fileSizeBytes, string? documentType, CancellationToken cancellationToken = default)
    {
        if (currentUserService.UserId is null)
        {
            return CustomApiResponse<UserAttachmentDto>.Unauthorized(Msg.Unauthenticated);
        }

        var ext = Path.GetExtension(fileName).ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(ext) || !AllowedAttachmentExtensions.Contains(ext))
        {
            return CustomApiResponse<UserAttachmentDto>.BadRequest(Msg.InvalidDocument);
        }

        var userId = currentUserService.UserId.Value;
        var webPath = await fileStorage.SaveAsync(fileStream, ext, "user-documents", cancellationToken);

        var attachment = new UserAttachment
        {
            UserId = userId,
            FileName = Path.GetFileName(fileName),
            FilePath = webPath,
            ContentType = string.IsNullOrWhiteSpace(contentType) ? "application/octet-stream" : contentType,
            FileSizeBytes = fileSizeBytes,
            DocumentType = documentType?.Trim()
        };

        context.UserAttachments.Add(attachment);
        await context.SaveChangesAsync(cancellationToken);

        var dto = new UserAttachmentDto(
            attachment.Id,
            attachment.FileName,
            attachment.FilePath,
            attachment.ContentType,
            attachment.FileSizeBytes,
            attachment.DocumentType,
            attachment.CreatedDate);

        return CustomApiResponse<UserAttachmentDto>.Success(dto, Msg.AttachmentUploaded);
    }

    public async Task<CustomApiResponse<Nothing>> DeleteAttachmentAsync(
        int attachmentId, CancellationToken cancellationToken = default)
    {
        if (currentUserService.UserId is null)
        {
            return CustomApiResponse<Nothing>.Unauthorized(Msg.Unauthenticated);
        }

        var userId = currentUserService.UserId.Value;
        var attachment = await context.UserAttachments
            .FirstOrDefaultAsync(a => a.Id == attachmentId && a.UserId == userId, cancellationToken);

        if (attachment is null)
        {
            return CustomApiResponse<Nothing>.NotFound(Msg.NotFound("الملف"));
        }

        fileStorage.Delete(attachment.FilePath);
        context.UserAttachments.Remove(attachment);
        await context.SaveChangesAsync(cancellationToken);

        return CustomApiResponse<Nothing>.Success(Nothing.Value, Msg.Deleted);
    }

    private UserProfileDto MapToDto(User user)
    {
        var roles = user.UserRoles.Select(ur => ur.Role.NameAr).ToList();
        var permissions = currentUserService.Permissions ?? [];

        var attachments = user.Attachments
            .OrderByDescending(a => a.CreatedDate)
            .Select(a => new UserAttachmentDto(
                a.Id,
                a.FileName,
                a.FilePath,
                a.ContentType,
                a.FileSizeBytes,
                a.DocumentType,
                a.CreatedDate))
            .ToList();

        return new UserProfileDto(
            user.Id,
            user.FullName,
            user.Email,
            user.PhoneNumber,
            user.AvatarUrl,
            user.IsActive,
            user.CreatedDate,
            roles,
            permissions,
            attachments);
    }
}
