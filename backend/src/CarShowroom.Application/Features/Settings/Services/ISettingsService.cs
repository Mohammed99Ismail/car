using CarShowroom.Application.Common.Models;
using CarShowroom.Application.Features.Settings.Dtos;

namespace CarShowroom.Application.Features.Settings.Services;

public interface ISettingsService
{
    Task<CustomApiResponse<IReadOnlyList<SystemSettingDto>>> GetAllSettingsAsync(CancellationToken cancellationToken = default);
    Task<CustomApiResponse<SystemSettingDto>> GetSettingByKeyAsync(string key, CancellationToken cancellationToken = default);
    Task<CustomApiResponse<SystemSettingDto>> UpdateSettingAsync(string key, UpdateSystemSettingRequest request, CancellationToken cancellationToken = default);
}
