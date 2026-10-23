using CarShowroom.Api.Auth;
using CarShowroom.Application.Common.Models;
using CarShowroom.Application.Common.Security;
using CarShowroom.Application.Features.Settings.Dtos;
using CarShowroom.Application.Features.Settings.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarShowroom.Api.Controllers;

[Authorize]
public class SettingsController(ISettingsService settingsService) : BaseApiController
{
    [HttpGet]
    [PermissionAuthorize(PermissionKeysConstant.Settings.VIEW_KEY)]
    public async Task<ActionResult<CustomApiResponse<IReadOnlyList<SystemSettingDto>>>> GetAll(
        CancellationToken cancellationToken)
    {
        var result = await settingsService.GetAllSettingsAsync(cancellationToken);
        return CustomResult(result);
    }

    [HttpGet("{key}")]
    [PermissionAuthorize(PermissionKeysConstant.Settings.VIEW_KEY)]
    public async Task<ActionResult<CustomApiResponse<SystemSettingDto>>> GetByKey(
        string key, CancellationToken cancellationToken)
    {
        var result = await settingsService.GetSettingByKeyAsync(key, cancellationToken);
        return CustomResult(result);
    }

    [HttpPut("{key}")]
    [PermissionAuthorize(PermissionKeysConstant.Settings.UPDATE_KEY)]
    public async Task<ActionResult<CustomApiResponse<SystemSettingDto>>> Update(
        string key, [FromBody] UpdateSystemSettingRequest request, CancellationToken cancellationToken)
    {
        var result = await settingsService.UpdateSettingAsync(key, request, cancellationToken);
        return CustomResult(result);
    }
}
