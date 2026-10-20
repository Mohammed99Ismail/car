using CarShowroom.Application.Common.Models;
using CarShowroom.Application.Features.Profile.Dtos;

namespace CarShowroom.Application.Features.Profile;

public interface IProfileService
{
    Task<CustomApiResponse<UserProfileDto>> GetProfileAsync(CancellationToken cancellationToken = default);
    Task<CustomApiResponse<UserProfileDto>> UpdateProfileAsync(UpdateProfileRequest request, CancellationToken cancellationToken = default);
    Task<CustomApiResponse<Nothing>> ChangePasswordAsync(ChangePasswordRequest request, CancellationToken cancellationToken = default);
    Task<CustomApiResponse<string>> UploadAvatarAsync(Stream fileStream, string extension, CancellationToken cancellationToken = default);
    Task<CustomApiResponse<Nothing>> DeleteAvatarAsync(CancellationToken cancellationToken = default);
    Task<CustomApiResponse<UserAttachmentDto>> UploadAttachmentAsync(Stream fileStream, string fileName, string contentType, long fileSizeBytes, string? documentType, CancellationToken cancellationToken = default);
    Task<CustomApiResponse<Nothing>> DeleteAttachmentAsync(int attachmentId, CancellationToken cancellationToken = default);
}
