using CarShowroom.Application.Common.Localization;
using CarShowroom.Application.Common.Models;
using FluentValidation;

namespace CarShowroom.Application.Features.Cars.Dtos;

/// <summary>
/// بيانات السيارة الكاملة وأصولها المالية للعرض
/// </summary>
public record CarDto(
    /// <summary>المعرف الفريد للسيارة</summary>
    int Id,
    /// <summary>معرف الشركة المصنعة (الماركة)</summary>
    int MakeId,
    /// <summary>اسم ماركة السيارة (الشركة المصنعة)</summary>
    string MakeName,
    /// <summary>معرف موديل السيارة</summary>
    int ModelId,
    /// <summary>اسم موديل السيارة</summary>
    string ModelName,
    /// <summary>سنة الصنع / الموديل</summary>
    int ManufactureYear,
    /// <summary>لون السيارة</summary>
    string Color,
    /// <summary>رقم لوحة السيارة</summary>
    string PlateNumber,
    /// <summary>معرف الفرع التابعة له السيارة</summary>
    int BranchId,
    /// <summary>اسم الفرع أو المعرض</summary>
    string BranchName,
    /// <summary>تاريخ شراء السيارة</summary>
    DateTime PurchaseDate,
    /// <summary>القيمة الشرائية للسيارة</summary>
    decimal PurchasePrice,
    /// <summary>قيمة السيارة / القيمة الدفترية للسيارة (Book Value)</summary>
    decimal BookValue,
    /// <summary>مجمع الإهلاك للسيارة</summary>
    decimal AccumulatedDepreciation,
    /// <summary>عمر السيارة / العمر الإنتاجي للسيارة (بالسنوات)</summary>
    decimal ProductiveAge,
    /// <summary>مسار صورة السيارة</summary>
    string? ImagePath,
    /// <summary>رقم الهيكل (الشاسيه)</summary>
    string ChassisNumber,
    /// <summary>رقم المحرك</summary>
    string EngineNumber,
    /// <summary>ملاحظات إضافية عن السيارة</summary>
    string? Notes,
    /// <summary>تاريخ إضافة السيارة للنظام</summary>
    DateTime CreatedDate);

/// <summary>
/// طلب إضافة سيارة جديدة في المعرض
/// </summary>
public record CreateCarRequest(
    /// <summary>معرف الشركة المصنعة (الماركة)</summary>
    int MakeId,
    /// <summary>معرف موديل السيارة</summary>
    int ModelId,
    /// <summary>سنة الصنع / الموديل</summary>
    int ManufactureYear,
    /// <summary>لون السيارة</summary>
    string Color,
    /// <summary>رقم لوحة السيارة</summary>
    string PlateNumber,
    /// <summary>معرف الفرع</summary>
    int BranchId,
    /// <summary>تاريخ شراء السيارة</summary>
    DateTime PurchaseDate,
    /// <summary>القيمة الشرائية للسيارة</summary>
    decimal PurchasePrice,
    /// <summary>رقم الهيكل (الشاسيه)</summary>
    string ChassisNumber,
    /// <summary>رقم المحرك</summary>
    string EngineNumber,
    /// <summary>ملاحظات إضافية عن السيارة</summary>
    string? Notes,
    /// <summary>مسار صورة السيارة</summary>
    string? ImagePath = null,
    /// <summary>القيمة الدفترية الأولية للسيارة (اختياري، في حال عدم التحديد تحسب تلقائياً من تاريخ الشراء)</summary>
    decimal? BookValue = null);

/// <summary>
/// طلب تحديث بيانات سيارة موجودة
/// </summary>
public record UpdateCarRequest(
    /// <summary>معرف الشركة المصنعة (الماركة)</summary>
    int MakeId,
    /// <summary>معرف موديل السيارة</summary>
    int ModelId,
    /// <summary>سنة الصنع / الموديل</summary>
    int ManufactureYear,
    /// <summary>لون السيارة</summary>
    string Color,
    /// <summary>رقم لوحة السيارة</summary>
    string PlateNumber,
    /// <summary>معرف الفرع</summary>
    int BranchId,
    /// <summary>تاريخ شراء السيارة</summary>
    DateTime PurchaseDate,
    /// <summary>القيمة الشرائية للسيارة</summary>
    decimal PurchasePrice,
    /// <summary>رقم الهيكل (الشاسيه)</summary>
    string ChassisNumber,
    /// <summary>رقم المحرك</summary>
    string EngineNumber,
    /// <summary>ملاحظات إضافية عن السيارة</summary>
    string? Notes,
    /// <summary>مسار صورة السيارة</summary>
    string? ImagePath = null,
    /// <summary>القيمة الدفترية المحدثة للسيارة</summary>
    decimal? BookValue = null);

/// <summary>
/// معايير تصفية والبحث في السيارات
/// </summary>
public class CarQueryParameters : QueryParameters
{
    /// <summary>تصفية حسب الفرع</summary>
    public int? BranchId { get; set; }
    /// <summary>تصفية حسب الشركة المصنعة (الماركة)</summary>
    public int? MakeId { get; set; }
    /// <summary>تصفية حسب موديل السيارة</summary>
    public int? ModelId { get; set; }
    /// <summary>تصفية حسب سنة الصنع</summary>
    public int? ManufactureYear { get; set; }
}

public class CreateCarRequestValidator : AbstractValidator<CreateCarRequest>
{
    public CreateCarRequestValidator()
    {
        RuleFor(x => x.MakeId).GreaterThan(0).WithMessage(Msg.ValidationFailed);
        RuleFor(x => x.ModelId).GreaterThan(0).WithMessage(Msg.ValidationFailed);
        RuleFor(x => x.ManufactureYear).InclusiveBetween(1980, 2100).WithMessage(Msg.ValidationFailed);
        RuleFor(x => x.Color).NotEmpty().WithMessage(Msg.ValidationFailed).MaximumLength(50);
        RuleFor(x => x.PlateNumber).NotEmpty().WithMessage(Msg.ValidationFailed).MaximumLength(30);
        RuleFor(x => x.BranchId).GreaterThan(0).WithMessage(Msg.ValidationFailed);
        RuleFor(x => x.PurchasePrice).GreaterThanOrEqualTo(0).WithMessage(Msg.ValidationFailed);
        RuleFor(x => x.BookValue).GreaterThanOrEqualTo(0).When(x => x.BookValue.HasValue).WithMessage(Msg.ValidationFailed);
        RuleFor(x => x.ChassisNumber).NotEmpty().WithMessage(Msg.ValidationFailed).MaximumLength(60);
        RuleFor(x => x.EngineNumber).NotEmpty().WithMessage(Msg.ValidationFailed).MaximumLength(60);
        RuleFor(x => x.Notes).MaximumLength(1000);
    }
}

public class UpdateCarRequestValidator : AbstractValidator<UpdateCarRequest>
{
    public UpdateCarRequestValidator()
    {
        RuleFor(x => x.MakeId).GreaterThan(0).WithMessage(Msg.ValidationFailed);
        RuleFor(x => x.ModelId).GreaterThan(0).WithMessage(Msg.ValidationFailed);
        RuleFor(x => x.ManufactureYear).InclusiveBetween(1980, 2100).WithMessage(Msg.ValidationFailed);
        RuleFor(x => x.Color).NotEmpty().WithMessage(Msg.ValidationFailed).MaximumLength(50);
        RuleFor(x => x.PlateNumber).NotEmpty().WithMessage(Msg.ValidationFailed).MaximumLength(30);
        RuleFor(x => x.BranchId).GreaterThan(0).WithMessage(Msg.ValidationFailed);
        RuleFor(x => x.PurchasePrice).GreaterThanOrEqualTo(0).WithMessage(Msg.ValidationFailed);
        RuleFor(x => x.BookValue).GreaterThanOrEqualTo(0).When(x => x.BookValue.HasValue).WithMessage(Msg.ValidationFailed);
        RuleFor(x => x.ChassisNumber).NotEmpty().WithMessage(Msg.ValidationFailed).MaximumLength(60);
        RuleFor(x => x.EngineNumber).NotEmpty().WithMessage(Msg.ValidationFailed).MaximumLength(60);
        RuleFor(x => x.Notes).MaximumLength(1000);
    }
}
