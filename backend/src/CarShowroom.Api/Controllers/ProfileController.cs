using CarShowroom.Application.Common.Localization;
using CarShowroom.Application.Common.Models;
using CarShowroom.Application.Features.Profile;
using CarShowroom.Application.Features.Profile.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarShowroom.Api.Controllers;

[Authorize]
public class ProfileController(IProfileService profileService) : BaseApiController
{
    [HttpGet]
    [HttpGet("user-profile")]
    public async Task<ActionResult<CustomApiResponse<UserProfileDto>>> GetProfile(CancellationToken cancellationToken)
    {
        var result = await profileService.GetProfileAsync(cancellationToken);
        return CustomResult(result);
    }

    [HttpGet("user-settings")]
    public ActionResult<CustomApiResponse<object>> GetUserSettings()
    {
        return CustomResult(CustomApiResponse<object>.Success(new
        {
            emailNotification = true,
            pushNotification = true,
            monthlyReport = true
        }));
    }

    [HttpPut]
    public async Task<ActionResult<CustomApiResponse<UserProfileDto>>> UpdateProfile(
        [FromBody] UpdateProfileRequest request, CancellationToken cancellationToken)
    {
        var result = await profileService.UpdateProfileAsync(request, cancellationToken);
        return CustomResult(result);
    }

    [HttpPost("change-password")]
    public async Task<ActionResult<CustomApiResponse<Nothing>>> ChangePassword(
        [FromBody] ChangePasswordRequest request, CancellationToken cancellationToken)
    {
        var result = await profileService.ChangePasswordAsync(request, cancellationToken);
        return CustomResult(result);
    }

    [HttpPost("avatar")]
    [RequestSizeLimit(5 * 1024 * 1024)] // 5 MB max
    public async Task<ActionResult<CustomApiResponse<string>>> UploadAvatar(
        IFormFile file, CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0)
        {
            return CustomResult(CustomApiResponse<string>.BadRequest(Msg.InvalidImage));
        }

        var ext = Path.GetExtension(file.FileName);
        await using var stream = file.OpenReadStream();
        var result = await profileService.UploadAvatarAsync(stream, ext, cancellationToken);
        return CustomResult(result);
    }

    [HttpDelete("avatar")]
    public async Task<ActionResult<CustomApiResponse<Nothing>>> DeleteAvatar(CancellationToken cancellationToken)
    {
        var result = await profileService.DeleteAvatarAsync(cancellationToken);
        return CustomResult(result);
    }

    [HttpPost("attachments")]
    [RequestSizeLimit(10 * 1024 * 1024)] // 10 MB max
    public async Task<ActionResult<CustomApiResponse<UserAttachmentDto>>> UploadAttachment(
        IFormFile file, [FromForm] string? documentType, CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0)
        {
            return CustomResult(CustomApiResponse<UserAttachmentDto>.BadRequest(Msg.InvalidDocument));
        }

        await using var stream = file.OpenReadStream();
        var result = await profileService.UploadAttachmentAsync(
            stream, file.FileName, file.ContentType, file.Length, documentType, cancellationToken);
        return CustomResult(result);
    }

    [HttpDelete("attachments/{id:int}")]
    public async Task<ActionResult<CustomApiResponse<Nothing>>> DeleteAttachment(
        int id, CancellationToken cancellationToken)
    {
        var result = await profileService.DeleteAttachmentAsync(id, cancellationToken);
        return CustomResult(result);
    }
}
