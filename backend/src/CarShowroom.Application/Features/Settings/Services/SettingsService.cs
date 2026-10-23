using CarShowroom.Application.Common.Interfaces;
using CarShowroom.Application.Common.Models;
using CarShowroom.Application.Features.Settings.Dtos;
using CarShowroom.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CarShowroom.Application.Features.Settings.Services;

public class SettingsService(IApplicationDbContext context) : ISettingsService
{
    public async Task<CustomApiResponse<IReadOnlyList<SystemSettingDto>>> GetAllSettingsAsync(
        CancellationToken cancellationToken = default)
    {
        var settings = await context.SystemSettings
            .AsNoTracking()
            .OrderBy(s => s.Category)
            .ThenBy(s => s.Id)
            .Select(s => MapToDto(s))
            .ToListAsync(cancellationToken);

        return CustomApiResponse<IReadOnlyList<SystemSettingDto>>.Success(settings);
    }

    public async Task<CustomApiResponse<SystemSettingDto>> GetSettingByKeyAsync(
        string key,
        CancellationToken cancellationToken = default)
    {
        var setting = await context.SystemSettings
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Key == key, cancellationToken);

        if (setting is null)
        {
            return CustomApiResponse<SystemSettingDto>.NotFound($"الإعداد بالرمز '{key}' غير موجود");
        }

        return CustomApiResponse<SystemSettingDto>.Success(MapToDto(setting));
    }

    public async Task<CustomApiResponse<SystemSettingDto>> UpdateSettingAsync(
        string key,
        UpdateSystemSettingRequest request,
        CancellationToken cancellationToken = default)
    {
        var setting = await context.SystemSettings
            .FirstOrDefaultAsync(s => s.Key == key, cancellationToken);

        if (setting is null)
        {
            return CustomApiResponse<SystemSettingDto>.NotFound($"الإعداد بالرمز '{key}' غير موجود");
        }

        if (setting.IsReadOnly)
        {
            return CustomApiResponse<SystemSettingDto>.BadRequest("هذا الإعداد للقراءة فقط ولا يمكن تعديله يدوياً");
        }

        setting.Value = request.Value.Trim();
        setting.LastModifiedDate = DateTime.UtcNow;

        await context.SaveChangesAsync(cancellationToken);

        return CustomApiResponse<SystemSettingDto>.Success(MapToDto(setting), "تم تحديث الإعداد بنجاح");
    }

    private static SystemSettingDto MapToDto(SystemSetting s) =>
        new(
            s.Id,
            s.Key,
            s.NameAr,
            s.NameEn,
            s.Value,
            s.DataType,
            s.Category,
            s.IsReadOnly,
            s.DescriptionAr,
            s.DescriptionEn,
            s.LastModifiedDate,
            s.CreatedDate);
}
