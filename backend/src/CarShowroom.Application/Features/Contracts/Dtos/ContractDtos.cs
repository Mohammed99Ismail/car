using CarShowroom.Application.Common.Localization;
using CarShowroom.Application.Common.Models;
using FluentValidation;

namespace CarShowroom.Application.Features.Contracts.Dtos;

/// <summary>
/// بيانات العقد الكاملة للعرض
/// </summary>
public record ContractDto(
    /// <summary>المعرف الفريد للعقد</summary>
    int Id,
    /// <summary>رقم العقد المرجعي</summary>
    string ContractNumber,
    /// <summary>معرف نوع العقد</summary>
    int ContractTypeId,
    /// <summary>اسم نوع العقد باللغة العربية</summary>
    string ContractTypeNameAr,
    /// <summary>اسم نوع العقد باللغة الإنجليزية</summary>
    string ContractTypeNameEn,
    /// <summary>معرف السيارة موضوع العقد</summary>
    int CarId,
    /// <summary>ماركة السيارة (الشركة المصنعة)</summary>
    string CarMakeName,
    /// <summary>موديل السيارة</summary>
    string CarModelName,
    /// <summary>رقم لوحة السيارة</summary>
    string CarPlateNumber,
    /// <summary>سنة الصنع / الموديل</summary>
    int CarManufactureYear,
    /// <summary>لون السيارة</summary>
    string CarColor,
    /// <summary>مسار صورة السيارة</summary>
    string? CarImagePath,
    /// <summary>رقم الشاسيه (الهيكل) للسيارة</summary>
    string? CarChassisNumber,
    /// <summary>معرف فرع المعرض</summary>
    int BranchId,
    /// <summary>اسم فرع المعرض</summary>
    string BranchName,
    /// <summary>معرف العميل المستفيد</summary>
    int? CustomerId,
    /// <summary>اسم العميل المستفيد</summary>
    string? CustomerName,
    /// <summary>رقم جوال العميل</summary>
    string? CustomerMobile,
    /// <summary>رقم الهوية الوطنية / الإقامة للعميل</summary>
    string? CustomerNationalId,
    /// <summary>معرف الموظف المستفيد (لعقود الخدمة)</summary>
    int? UserId,
    /// <summary>اسم الموظف المستفيد</summary>
    string? UserFullName,
    /// <summary>البريد الإلكتروني للموظف</summary>
    string? UserEmail,
    /// <summary>تاريخ بداية العقد</summary>
    DateTime StartDate,
    /// <summary>تاريخ نهاية العقد</summary>
    DateTime EndDate,
    /// <summary>أيام الإيجار المستحق / مدة العقد بالأيام</summary>
    int PeriodDays,
    /// <summary>المدة المحسوبة بالشهور</summary>
    decimal PeriodMonths,
    /// <summary>إجمالي قيمة العقد / الإيرادات التقديرية حسب مدة العقد</summary>
    decimal TotalPrice,
    /// <summary>المبلغ المدفوع</summary>
    decimal PaidPrice,
    /// <summary>المبلغ المتبقي</summary>
    decimal RemainingPrice,
    /// <summary>قيمة قسط الإيجار التقديري المقترح قبل الدفعة المقدمة (شهرياً)</summary>
    decimal? MonthlyRent,
    /// <summary>قيمة الدفعة المقدمة</summary>
    decimal? DownPayment,
    /// <summary>الحد الأقصى للمسافة المسموحة (كم)</summary>
    int? MileageLimit,
    /// <summary>رسوم كل كيلومتر زائد عن الحد</summary>
    decimal? ExcessMileageFee,
    /// <summary>قراءة العداد الفعلية عند نهاية العقد</summary>
    int? ActualEndMileage,
    /// <summary>تكلفة المسافة الزائدة الإجمالية</summary>
    decimal? ExcessMileageCost,
    /// <summary>نسبة تقدير تأمين شامل للسيارة المعتمدة في العقد (%)</summary>
    decimal? InsurancePercentage,
    /// <summary>نسبة تقدير تكلفة الصيانة خلال فترة العقد المعتمدة في العقد (%)</summary>
    decimal? MaintenancePercentage,
    /// <summary>نسبة تقدير تكلفة سيارة بديلة خلال فترة العقد المعتمدة في العقد (%)</summary>
    decimal? ReplacementCarPercentage,
    /// <summary>نسبة تقدير تكلفة المصاريف الإدارية خلال فترة العقد المعتمدة في العقد (%)</summary>
    decimal? AdminFeesPercentage,
    /// <summary>نسبة الإهلاك السنوي المعتمدة في العقد (%)</summary>
    decimal? AnnualDepreciationPercentage,
    /// <summary>لقطة القيمة الشرائية للسيارة وقت إنشاء العقد</summary>
    decimal CarPurchasePriceSnapshot,
    /// <summary>لقطة قيمة السيارة / القيمة الدفترية للسيارة وقت إنشاء العقد</summary>
    decimal CarBookValueSnapshot,
    /// <summary>لقطة مجمع إهلاك السيارة وقت إنشاء العقد</summary>
    decimal CarAccumulatedDepreciationSnapshot,
    /// <summary>لقطة عمر السيارة الإنتاجي بالسنوات وقت إنشاء العقد</summary>
    decimal CarProductiveAgeSnapshot,
    /// <summary>إجمالي الإيرادات التقديرية حسب مدة العقد</summary>
    decimal TotalEstimatedRevenue,
    /// <summary>إجمالي التكلفة التقديرية</summary>
    decimal TotalEstimatedCost,
    /// <summary>إجمالي الأرباح التقديرية حسب مدة العقد</summary>
    decimal ExpectedProfit,
    /// <summary>قيمة الفائدة للشركة من تشغيل السيارة (شهرياً)</summary>
    decimal MonthlyCompanyProfit,
    /// <summary>قيمة الإيجار التقديري المقترح (بعد خصم الدفعة المقدمة)</summary>
    decimal AdjustedMonthlyRent,
    /// <summary>خصم قيمة الدفعة المقدمة من قيمة قسط الإيجار الشهري المقترح</summary>
    decimal DownPaymentMonthlyDiscount,
    /// <summary>هل تطبق ضريبة القيمة المضافة على عقد الإيجار أم لا</summary>
    bool IsTaxApplied,
    /// <summary>نسبة ضريبة القيمة المضافة المعتمدة في العقد (%)</summary>
    decimal TaxPercentage,
    /// <summary>قيمة الضريبة المضافة شهرياً على قسط الإيجار</summary>
    decimal MonthlyTaxAmount,
    /// <summary>قيمة الإيجار الشهري المستحق على العميل شاملاً الضريبة</summary>
    decimal MonthlyRentWithTax,
    /// <summary>إجمالي قيمة الضريبة المضافة على كامل مدة العقد</summary>
    decimal TotalTaxAmount,
    /// <summary>إجمالي قيمة العقد النهائي شاملاً الضريبة</summary>
    decimal TotalPriceWithTax,
    /// <summary>حالة العقد (Active, Completed, Cancelled)</summary>
    string Status,
    /// <summary>ملاحظات وشروط إضافية على العقد</summary>
    string? Notes,
    /// <summary>تاريخ إنشاء العقد في النظام</summary>
    DateTime CreatedDate,
    /// <summary>جدول الإهلاك والتكاليف السنوي بالتفصيل بصيغة JSON (Yearly Schedule)</summary>
    string? YearlyBreakdownJson);

/// <summary>
/// طلب إنشاء عقد جديد
/// </summary>
public record CreateContractRequest(
    /// <summary>معرف نوع العقد (1: خدمة، 2: قصير الأجل، 3: طويل الأجل، 4: منتهي بالتمليك)</summary>
    int ContractTypeId,
    /// <summary>معرف السيارة موضوع العقد</summary>
    int CarId,
    /// <summary>معرف الفرع التابع له العقد</summary>
    int BranchId,
    /// <summary>معرف العميل المستفيد (مطلوب لعقود الإيجار والتمليك)</summary>
    int? CustomerId,
    /// <summary>معرف الموظف المستفيد (مطلوب لعقود الخدمة)</summary>
    int? UserId,
    /// <summary>تاريخ بداية العقد</summary>
    DateTime StartDate,
    /// <summary>تاريخ نهاية العقد</summary>
    DateTime EndDate,
    /// <summary>قيمة قسط الإيجار التقديري المقترح قبل الدفعة المقدمة (شهرياً)</summary>
    decimal? MonthlyRent,
    /// <summary>قيمة الدفعة المقدمة</summary>
    decimal? DownPayment,
    /// <summary>الحد الأقصى للمسافة المسموحة (كم)</summary>
    int? MileageLimit,
    /// <summary>رسوم كل كيلومتر زائد عن الحد</summary>
    decimal? ExcessMileageFee,
    /// <summary>نسبة تقدير تأمين شامل للسيارة المعتمدة في العقد (%) - اختياري للتخصيص</summary>
    decimal? InsurancePercentage = null,
    /// <summary>نسبة تقدير تكلفة الصيانة خلال فترة العقد المعتمدة في العقد (%) - اختياري للتخصيص</summary>
    decimal? MaintenancePercentage = null,
    /// <summary>نسبة تقدير تكلفة سيارة بديلة خلال فترة العقد المعتمدة في العقد (%) - اختياري للتخصيص</summary>
    decimal? ReplacementCarPercentage = null,
    /// <summary>نسبة تقدير تكلفة المصاريف الإدارية خلال فترة العقد المعتمدة في العقد (%) - اختياري للتخصيص</summary>
    decimal? AdminFeesPercentage = null,
    /// <summary>نسبة الإهلاك السنوي المعتمدة في العقد (%) - اختياري للتخصيص</summary>
    decimal? AnnualDepreciationPercentage = null,
    /// <summary>هل تطبق ضريبة القيمة المضافة على عقد الإيجار أم لا (الافتراضي: نعم)</summary>
    bool ApplyTax = true,
    /// <summary>نسبة ضريبة القيمة المضافة المخصصة (%) - اختياري، الافتراضي 15% من النظام</summary>
    decimal? TaxPercentage = null,
    /// <summary>ملاحظات وشروط إضافية على العقد</summary>
    string? Notes = null);

/// <summary>
/// طلب تعديل بيانات عقد موجود
/// </summary>
public record UpdateContractRequest(
    /// <summary>معرف نوع العقد</summary>
    int ContractTypeId,
    /// <summary>معرف السيارة موضوع العقد</summary>
    int CarId,
    /// <summary>معرف الفرع</summary>
    int BranchId,
    /// <summary>معرف العميل المستفيد</summary>
    int? CustomerId,
    /// <summary>معرف الموظف المستفيد</summary>
    int? UserId,
    /// <summary>تاريخ بداية العقد</summary>
    DateTime StartDate,
    /// <summary>تاريخ نهاية العقد</summary>
    DateTime EndDate,
    /// <summary>قيمة قسط الإيجار التقديري المقترح قبل الدفعة المقدمة (شهرياً)</summary>
    decimal? MonthlyRent,
    /// <summary>قيمة الدفعة المقدمة</summary>
    decimal? DownPayment,
    /// <summary>المبلغ المدفوع من قيمة العقد</summary>
    decimal PaidPrice,
    /// <summary>الحد الأقصى للمسافة المسموحة (كم)</summary>
    int? MileageLimit,
    /// <summary>رسوم كل كيلومتر زائد عن الحد</summary>
    decimal? ExcessMileageFee,
    /// <summary>قراءة العداد الفعلية عند نهاية العقد وتسليم السيارة</summary>
    int? ActualEndMileage,
    /// <summary>نسبة تقدير تأمين شامل للسيارة المعتمدة في العقد (%)</summary>
    decimal? InsurancePercentage = null,
    /// <summary>نسبة تقدير تكلفة الصيانة خلال فترة العقد المعتمدة في العقد (%)</summary>
    decimal? MaintenancePercentage = null,
    /// <summary>نسبة تقدير تكلفة سيارة بديلة خلال فترة العقد المعتمدة في العقد (%)</summary>
    decimal? ReplacementCarPercentage = null,
    /// <summary>نسبة تقدير تكلفة المصاريف الإدارية خلال فترة العقد المعتمدة في العقد (%)</summary>
    decimal? AdminFeesPercentage = null,
    /// <summary>نسبة الإهلاك السنوي المعتمدة في العقد (%)</summary>
    decimal? AnnualDepreciationPercentage = null,
    /// <summary>هل تطبق ضريبة القيمة المضافة على عقد الإيجار أم لا</summary>
    bool? ApplyTax = null,
    /// <summary>نسبة ضريبة القيمة المضافة المخصصة (%)</summary>
    decimal? TaxPercentage = null,
    /// <summary>حالة العقد (Active, Completed, Cancelled)</summary>
    string? Status = null,
    /// <summary>ملاحظات وشروط إضافية على العقد</summary>
    string? Notes = null);

/// <summary>
/// معايير تصفية والبحث في العقود
/// </summary>
public class ContractQueryParameters : QueryParameters
{
    /// <summary>تصفية حسب نوع العقد</summary>
    public int? ContractTypeId { get; set; }
    /// <summary>تصفية حسب السيارة</summary>
    public int? CarId { get; set; }
    /// <summary>تصفية حسب الفرع</summary>
    public int? BranchId { get; set; }
    /// <summary>تصفية حسب العميل</summary>
    public int? CustomerId { get; set; }
    /// <summary>تصفية حسب الموظف</summary>
    public int? UserId { get; set; }
    /// <summary>تصفية حسب حالة العقد</summary>
    public string? Status { get; set; }
    /// <summary>تصفية حسب تاريخ البداية (من)</summary>
    public DateTime? StartDateFrom { get; set; }
    /// <summary>تصفية حسب تاريخ البداية (إلى)</summary>
    public DateTime? StartDateTo { get; set; }
}

/// <summary>
/// إعدادات النسب المئوية الأربعة المعتمدة للإهلاك وتكاليف العقود
/// </summary>
public record ContractTermsConfigDto(
    /// <summary>نسبة تقدير تأمين شامل للسيارة السنوية (%)</summary>
    decimal InsurancePercentage,
    /// <summary>نسبة تقدير تكلفة الصيانة خلال فترة العقد السنوية (%)</summary>
    decimal MaintenancePercentage,
    /// <summary>نسبة تقدير تكلفة سيارة بديلة خلال فترة العقد السنوية (%)</summary>
    decimal ReplacementCarPercentage,
    /// <summary>نسبة تقدير تكلفة المصاريف الإدارية خلال فترة العقد السنوية (%)</summary>
    decimal AdminFeesPercentage,
    /// <summary>نسبة الإهلاك السنوي للسيارة (%)</summary>
    decimal AnnualDepreciationPercentage,
    /// <summary>تاريخ آخر تعديل للإعدادات</summary>
    DateTime LastModifiedDate);

/// <summary>
/// طلب تحديث الإعدادات العامة لنسب العقود
/// </summary>
public record UpdateContractTermsConfigRequest(
    /// <summary>نسبة تقدير تأمين شامل للسيارة السنوية (%)</summary>
    decimal InsurancePercentage,
    /// <summary>نسبة تقدير تكلفة الصيانة خلال فترة العقد السنوية (%)</summary>
    decimal MaintenancePercentage,
    /// <summary>نسبة تقدير تكلفة سيارة بديلة خلال فترة العقد السنوية (%)</summary>
    decimal ReplacementCarPercentage,
    /// <summary>نسبة تقدير تكلفة المصاريف الإدارية خلال فترة العقد السنوية (%)</summary>
    decimal AdminFeesPercentage,
    /// <summary>نسبة الإهلاك السنوي للسيارة (%)</summary>
    decimal AnnualDepreciationPercentage);

/// <summary>
/// طلب الحساب اللحظي لمعادلات وتكاليف وأرباح العقد
/// </summary>
public record ContractCalculationRequest(
    /// <summary>معرف نوع العقد (1: خدمة، 2: قصير، 3: طويل، 4: تمليك)</summary>
    int ContractTypeId,
    /// <summary>معرف السيارة (في حال تم اختيار سيارة مسجلة)</summary>
    int? CarId,
    /// <summary>القيمة الشرائية للسيارة</summary>
    decimal? CarPurchasePrice,
    /// <summary>تاريخ شراء السيارة</summary>
    DateTime? CarPurchaseDate,
    /// <summary>تاريخ بداية العقد</summary>
    DateTime StartDate,
    /// <summary>تاريخ نهاية العقد</summary>
    DateTime EndDate,
    /// <summary>قيمة قسط الإيجار التقديري المقترح قبل الدفعة المقدمة (شهرياً)</summary>
    decimal? MonthlyRent,
    /// <summary>قيمة الدفعة المقدمة</summary>
    decimal? DownPayment,
    /// <summary>الحد الأقصى للمسافة المسموحة (كم)</summary>
    int? MileageLimit,
    /// <summary>رسوم كل كيلومتر زائد عن الحد</summary>
    decimal? ExcessMileageFee,
    /// <summary>نسبة تقدير تأمين شامل للسيارة السنوية (%) - اختياري</summary>
    decimal? InsurancePercentage = null,
    /// <summary>نسبة تقدير تكلفة الصيانة خلال فترة العقد السنوية (%) - اختياري</summary>
    decimal? MaintenancePercentage = null,
    /// <summary>نسبة تقدير تكلفة سيارة بديلة خلال فترة العقد السنوية (%) - اختياري</summary>
    decimal? ReplacementCarPercentage = null,
    /// <summary>نسبة تقدير تكلفة المصاريف الإدارية خلال فترة العقد السنوية (%) - اختياري</summary>
    decimal? AdminFeesPercentage = null,
    /// <summary>نسبة الإهلاك السنوي للسيارة (%) - اختياري</summary>
    decimal? AnnualDepreciationPercentage = null,
    /// <summary>قيمة السيارة / القيمة الدفترية للسيارة (Book Value)</summary>
    decimal? CarBookValue = null,
    /// <summary>هل تطبق ضريبة القيمة المضافة على عقد الإيجار (الافتراضي: نعم)</summary>
    bool ApplyTax = true,
    /// <summary>نسبة ضريبة القيمة المضافة (%) - اختياري، الافتراضي 15% من النظام</summary>
    decimal? TaxPercentage = null);

/// <summary>
/// تفاصيل سنة تعاقدية واحدة في جدول الإهلاك والتكاليف السنوي
/// </summary>
public record ContractYearlyScheduleItem(
    /// <summary>رقم السنة التعاقدية (1، 2، ...)</summary>
    int Year,
    /// <summary>صافي قيمة السيارة خلال مدة العقد (القيمة الدفترية في نهاية السنة)</summary>
    decimal NetCarValue,
    /// <summary>قيمة إهلاك السيارة خلال هذه السنة</summary>
    decimal DepreciationCost,
    /// <summary>تقدير تأمين شامل للسيارة لهذه السنة</summary>
    decimal InsuranceCost,
    /// <summary>تقدير تكلفة الصيانة خلال هذه السنة</summary>
    decimal MaintenanceCost,
    /// <summary>تقدير تكلفة سيارة بديلة خلال هذه السنة</summary>
    decimal ReplacementCarCost,
    /// <summary>تقدير تكلفة المصاريف الإدارية خلال هذه السنة</summary>
    decimal AdminFeesCost,
    /// <summary>إجمالي التكلفة التقديرية لهذه السنة</summary>
    decimal TotalEstimatedCost,
    /// <summary>قيمة تكلفة للسيارة (شهرياً) لهذه السنة</summary>
    decimal MonthlyCost);

/// <summary>
/// النتيجة الشاملة للحساب اللحظي المالي والإهلاكي للعقد
/// </summary>
public record ContractCalculationResultDto(
    /// <summary>القيمة الشرائية للسيارة</summary>
    decimal CarPurchasePrice,
    /// <summary>تاريخ شراء السيارة</summary>
    DateTime CarPurchaseDate,
    /// <summary>عمر السيارة / العمر الإنتاجي للسيارة (بالسنوات)</summary>
    decimal CarProductiveAge,
    /// <summary>القيمة الدفترية للسيارة</summary>
    decimal CarBookValue,
    /// <summary>مجمع الإهلاك للسيارة</summary>
    decimal CarAccumulatedDepreciation,
    /// <summary>قيمة السيارة المعتمدة في الحسابات (القيمة الدفترية)</summary>
    decimal CarValueUsed,
    /// <summary>مدة العقد الإجمالية بالأيام (شاملة البداية والنهاية)</summary>
    int PeriodDays,
    /// <summary>المدة المحسوبة بالشهور</summary>
    decimal PeriodMonths,
    /// <summary>سنوات العقد للإهلاك</summary>
    int ContractYears,
    /// <summary>جدول الإهلاك والتكاليف السنوي (Yearly Schedule)</summary>
    List<ContractYearlyScheduleItem> YearlySchedule,
    /// <summary>قيمة قسط الإيجار التقديري المقترح قبل الدفعة المقدمة (شهرياً)</summary>
    decimal MonthlyRentBeforeDownPayment,
    /// <summary>قيمة الدفعة المقدمة</summary>
    decimal DownPayment,
    /// <summary>قيمة الإيجار التقديري المقترح (بعد خصم الدفعة المقدمة شهرياً)</summary>
    decimal AdjustedMonthlyRent,
    /// <summary>خصم قيمة الدفعة المقدمة من قيمة قسط الإيجار الشهري المقترح</summary>
    decimal DownPaymentMonthlyDiscount,
    /// <summary>إجمالي الإيرادات التقديرية حسب مدة العقد</summary>
    decimal TotalEstimatedRevenue,
    /// <summary>إجمالي التكلفة التقديرية</summary>
    decimal TotalEstimatedCost,
    /// <summary>إجمالي الأرباح التقديرية حسب مدة العقد</summary>
    decimal ExpectedProfit,
    /// <summary>قيمة الفائدة للشركة من تشغيل السيارة (شهرياً)</summary>
    decimal MonthlyCompanyProfit,
    /// <summary>الحد الأقصى للمسافة المسموحة (كم)</summary>
    int? MileageLimit,
    /// <summary>رسوم كل كيلومتر زائد عن الحد</summary>
    decimal? ExcessMileageFee,
    /// <summary>تنبيهات وملاحظات التحقق والحسابات</summary>
    List<string> Warnings,
    /// <summary>قيمة إهلاك السيارة نهاية مدة العقد الإجمالية</summary>
    decimal TotalDepreciationCost = 0m,
    /// <summary>صافي قيمة السيارة خلال مدة العقد (نهاية مدة العقد)</summary>
    decimal NetCarValueAtEnd = 0m,
    /// <summary>تقدير تأمين شامل للسيارة الإجمالي</summary>
    decimal TotalInsuranceCost = 0m,
    /// <summary>تقدير تكلفة الصيانة خلال فترة العقد الإجمالية</summary>
    decimal TotalMaintenanceCost = 0m,
    /// <summary>تقدير تكلفة سيارة بديلة خلال فترة العقد الإجمالية</summary>
    decimal TotalReplacementCarCost = 0m,
    /// <summary>تقدير تكلفة المصاريف الإدارية خلال فترة العقد الإجمالية</summary>
    decimal TotalAdminFeesCost = 0m,
    /// <summary>قيمة تكلفة للسيارة (شهرياً)</summary>
    decimal MonthlyCarCost = 0m,
    /// <summary>الإيجار اليومي التقديري (في العقود القصيرة والأقل من سنة)</summary>
    decimal DailyRent = 0m,
    /// <summary>أيام الإيجار المستحق / الفعلي (بدون احتساب يوم البداية)</summary>
    int RentDays = 0,
    /// <summary>هل ضريبة القيمة المضافة مطبقة على العقد</summary>
    bool IsTaxApplied = true,
    /// <summary>نسبة ضريبة القيمة المضافة المعتمدة في العقد (%)</summary>
    decimal TaxPercentage = 15.00m,
    /// <summary>قيمة الضريبة المضافة شهرياً على قسط الإيجار</summary>
    decimal MonthlyTaxAmount = 0m,
    /// <summary>قيمة الإيجار الشهري المستحق على العميل شاملاً الضريبة</summary>
    decimal MonthlyRentWithTax = 0m,
    /// <summary>إجمالي قيمة الضريبة المضافة على كامل مدة العقد</summary>
    decimal TotalTaxAmount = 0m,
    /// <summary>إجمالي قيمة العقد شاملاً الضريبة</summary>
    decimal TotalPriceWithTax = 0m);

/// <summary>
/// تقرير العقد والعمليات الحسابية الملحقة به
/// </summary>
public record ContractReportDto(
    /// <summary>بيانات العقد</summary>
    ContractDto Contract,
    /// <summary>نتائج الحسابات المالية والإهلاكية للعقد</summary>
    ContractCalculationResultDto Calculation,
    /// <summary>تاريخ ووقت إصدار التقرير</summary>
    string GeneratedAt);

public class CreateContractRequestValidator : AbstractValidator<CreateContractRequest>
{
    public CreateContractRequestValidator()
    {
        RuleFor(x => x.ContractTypeId)
            .InclusiveBetween(1, 4)
            .WithMessage("نوع العقد غير صحيح");

        RuleFor(x => x.CarId)
            .GreaterThan(0)
            .WithMessage("السيارة مطلوبة");

        RuleFor(x => x.BranchId)
            .GreaterThan(0)
            .WithMessage("الفرع مطلوب");

        RuleFor(x => x.StartDate)
            .NotEmpty()
            .WithMessage("تاريخ بداية العقد مطلوب");

        RuleFor(x => x.EndDate)
            .NotEmpty()
            .GreaterThan(x => x.StartDate)
            .WithMessage("تاريخ نهاية العقد يجب أن يكون بعد تاريخ البداية");

        // Type 1: Service Contract
        When(x => x.ContractTypeId == 1, () =>
        {
            RuleFor(x => x.UserId)
                .NotNull()
                .GreaterThan(0)
                .WithMessage("عقد الخدمة يتطلب اختيار الموظف / مستخدم النظام المستفيد");
        });

        // Types 2, 3, 4: Rental & Lease-to-own
        When(x => x.ContractTypeId >= 2, () =>
        {
            RuleFor(x => x.CustomerId)
                .NotNull()
                .GreaterThan(0)
                .WithMessage("عقد الإيجار يتطلب اختيار العميل المستفيد");

            RuleFor(x => x.MonthlyRent)
                .NotNull()
                .GreaterThan(0)
                .WithMessage("قيمة الإيجار الشهري مطلوبة وتكون أكبر من الصفر");
        });

        // Type 2: Short-term rental (<= 12 months, no down payment)
        When(x => x.ContractTypeId == 2, () =>
        {
            RuleFor(x => (x.EndDate - x.StartDate).TotalDays)
                .LessThanOrEqualTo(366)
                .WithMessage("عقد الإيجار قصير الأجل يجب ألا يتجاوز سنة واحدة (12 شهراً)");

            RuleFor(x => x.DownPayment)
                .Must(dp => !dp.HasValue || dp.Value == 0)
                .WithMessage("عقد الإيجار قصير الأجل لا يحتوي على دفعة مقدمة");
        });

        // Type 3: Long-term rental (>= 12 months)
        When(x => x.ContractTypeId == 3, () =>
        {
            RuleFor(x => (x.EndDate - x.StartDate).TotalDays)
                .GreaterThanOrEqualTo(360)
                .WithMessage("عقد الإيجار طويل الأجل يجب أن يكون سنة واحدة (12 شهراً) فأكثر");
        });
    }
}

public class UpdateContractRequestValidator : AbstractValidator<UpdateContractRequest>
{
    public UpdateContractRequestValidator()
    {
        RuleFor(x => x.ContractTypeId).InclusiveBetween(1, 4).WithMessage("نوع العقد غير صحيح");
        RuleFor(x => x.CarId).GreaterThan(0).WithMessage("السيارة مطلوبة");
        RuleFor(x => x.BranchId).GreaterThan(0).WithMessage("الفرع مطلوب");
        RuleFor(x => x.StartDate).NotEmpty().WithMessage("تاريخ البداية مطلوب");
        RuleFor(x => x.EndDate).NotEmpty().GreaterThan(x => x.StartDate).WithMessage("تاريخ النهاية يجب أن يكون بعد البداية");

        When(x => x.ContractTypeId == 1, () =>
        {
            RuleFor(x => x.UserId).NotNull().GreaterThan(0).WithMessage("الموظف المستفيد مطلوب");
        });

        When(x => x.ContractTypeId >= 2, () =>
        {
            RuleFor(x => x.CustomerId).NotNull().GreaterThan(0).WithMessage("العميل مطلوب");
            RuleFor(x => x.MonthlyRent).NotNull().GreaterThan(0).WithMessage("قيمة الإيجار الشهري مطلوبة");
        });

        When(x => x.ContractTypeId == 2, () =>
        {
            RuleFor(x => (x.EndDate - x.StartDate).TotalDays)
                .LessThanOrEqualTo(366)
                .WithMessage("عقد الإيجار قصير الأجل يجب ألا يتجاوز سنة واحدة");

            RuleFor(x => x.DownPayment)
                .Must(dp => !dp.HasValue || dp.Value == 0)
                .WithMessage("عقد الإيجار قصير الأجل لا يحتوي على دفعة مقدمة");
        });

        When(x => x.ContractTypeId == 3, () =>
        {
            RuleFor(x => (x.EndDate - x.StartDate).TotalDays)
                .GreaterThanOrEqualTo(360)
                .WithMessage("عقد الإيجار طويل الأجل يجب أن يكون سنة واحدة على الأقل");
        });
    }
}
