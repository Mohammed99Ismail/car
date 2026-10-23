namespace CarShowroom.Application.Features.Settings.Dtos;

/// <summary>
/// بيانات إعداد النظام المرجعي للعرض
/// </summary>
public record SystemSettingDto(
    int Id,
    string Key,
    string NameAr,
    string NameEn,
    string Value,
    string DataType,
    string Category,
    bool IsReadOnly,
    string? DescriptionAr,
    string? DescriptionEn,
    DateTime? LastModifiedDate,
    DateTime CreatedDate);

/// <summary>
/// طلب تعديل قيمة إعداد من إعدادات النظام
/// </summary>
public record UpdateSystemSettingRequest(
    string Value);
