using CarShowroom.Domain.Common;

namespace CarShowroom.Domain.Entities;

/// <summary>نوع / ماركة السيارة (مثل Honda, Toyota ...)</summary>
public class CarMake : BaseEntity
{
    /// <summary>اسم الشركة المصنعة / الماركة</summary>
    public required string Name { get; set; }

    /// <summary>موديلات السيارات التابعة للشركة المصنعة</summary>
    public ICollection<CarModel> Models { get; set; } = [];
}

/// <summary>موديل / طراز السيارة (مثل Accord, Camry ...)</summary>
public class CarModel : BaseEntity
{
    /// <summary>معرف الشركة المصنعة</summary>
    public int MakeId { get; set; }

    /// <summary>الشركة المصنعة</summary>
    public CarMake Make { get; set; } = null!;

    /// <summary>اسم الموديل</summary>
    public required string Name { get; set; }
}

/// <summary>أنواع العقود — lookup مرجعي ثابت (خدمة، إيجار قصير الأجل، إيجار طويل الأجل، منتهي بالتمليك)</summary>
public class ContractType : BaseEntity
{
    /// <summary>اسم نوع العقد باللغة العربية</summary>
    public required string NameAr { get; set; }

    /// <summary>اسم نوع العقد باللغة الإنجليزية</summary>
    public required string NameEn { get; set; }
}

/// <summary>فروع ومعارض السيارات</summary>
public class Branch : AuditedEntityBase
{
    /// <summary>اسم الفرع أو المعرض</summary>
    public required string Name { get; set; }

    /// <summary>عنوان الفرع</summary>
    public required string Address { get; set; }

    /// <summary>رقم جوال / هاتف الفرع</summary>
    public required string Mobile { get; set; }
}

/// <summary>بيانات العملاء (المستأجرين / المستفيدين)</summary>
public class Customer : AuditedEntityBase
{
    /// <summary>اسم العميل بالكامل</summary>
    public required string Name { get; set; }

    /// <summary>رقم الهوية الوطنية أو الإقامة</summary>
    public required string NationalId { get; set; }

    /// <summary>رقم جوال العميل</summary>
    public required string Mobile { get; set; }

    /// <summary>عنوان العميل</summary>
    public required string Address { get; set; }
}

/// <summary>بيانات السيارة وأصولها المالية في المعرض</summary>
public class Car : AuditedEntityBase
{
    /// <summary>معرف الشركة المصنعة (الماركة)</summary>
    public int MakeId { get; set; }

    /// <summary>الشركة المصنعة</summary>
    public CarMake Make { get; set; } = null!;

    /// <summary>معرف موديل السيارة</summary>
    public int ModelId { get; set; }

    /// <summary>موديل السيارة</summary>
    public CarModel Model { get; set; } = null!;

    /// <summary>سنة الصنع / الموديل</summary>
    public int ManufactureYear { get; set; }

    /// <summary>لون السيارة</summary>
    public required string Color { get; set; }

    /// <summary>رقم اللوحة</summary>
    public required string PlateNumber { get; set; }

    /// <summary>معرف الفرع المتواجدة به السيارة</summary>
    public int BranchId { get; set; }

    /// <summary>الفرع المتواجدة به السيارة</summary>
    public Branch Branch { get; set; } = null!;

    /// <summary>تاريخ شراء السيارة</summary>
    public DateTime PurchaseDate { get; set; }

    /// <summary>القيمة الشرائية للسيارة</summary>
    public decimal PurchasePrice { get; set; }

    /// <summary>قيمة السيارة / القيمة الدفترية للسيارة (The book value of the car)</summary>
    public decimal BookValue { get; set; }

    /// <summary>مسار صورة السيارة (Relative path under wwwroot, e.g. /uploads/cars/{guid}.jpg)</summary>
    public string? ImagePath { get; set; }

    /// <summary>رقم الهيكل (الشاسيه)</summary>
    public required string ChassisNumber { get; set; }

    /// <summary>رقم المحرك</summary>
    public required string EngineNumber { get; set; }

    /// <summary>ملاحظات إضافية عن السيارة</summary>
    public string? Notes { get; set; }
}

/// <summary>الإعدادات العامة لنسب البنود الأربعة للإهلاك والعقود</summary>
public class ContractTermsConfig : BaseEntity
{
    /// <summary>نسبة تقدير تأمين شامل للسيارة السنوية (%)</summary>
    public decimal InsurancePercentage { get; set; } = 4.00m;

    /// <summary>نسبة تقدير تكلفة الصيانة خلال فترة العقد السنوية (%)</summary>
    public decimal MaintenancePercentage { get; set; } = 5.00m;

    /// <summary>نسبة تقدير تكلفة سيارة بديلة خلال فترة العقد السنوية (%)</summary>
    public decimal ReplacementCarPercentage { get; set; } = 1.00m;

    /// <summary>نسبة تقدير تكلفة المصاريف الإدارية خلال فترة العقد السنوية (%)</summary>
    public decimal AdminFeesPercentage { get; set; } = 1.00m;

    /// <summary>نسبة الإهلاك السنوي للسيارة (%) - افتراضياً 25% لإهلاك على 4 سنوات</summary>
    public decimal AnnualDepreciationPercentage { get; set; } = 25.00m;

    /// <summary>تاريخ آخر تعديل للإعدادات</summary>
    public DateTime LastModifiedDate { get; set; } = DateTime.UtcNow;
}

/// <summary>إعدادات النظام العامة والتهيئة التشغيلية</summary>
public class SystemSetting : AuditedEntityBase
{
    /// <summary>المفتاح الفريد للإعداد (مثال: VAT_PERCENTAGE)</summary>
    public required string Key { get; set; }

    /// <summary>اسم الإعداد بالعربية</summary>
    public required string NameAr { get; set; }

    /// <summary>اسم الإعداد بالإنجليزية</summary>
    public required string NameEn { get; set; }

    /// <summary>قيمة الإعداد</summary>
    public required string Value { get; set; }

    /// <summary>نوع البيانات (Percentage, Number, Text, Boolean, Currency)</summary>
    public string DataType { get; set; } = "Percentage";

    /// <summary>فئة الإعداد (الضرائب والمالية، العقود، عام...)</summary>
    public string Category { get; set; } = "الضرائب والمالية";

    /// <summary>هل الإعداد للقراءة فقط؟</summary>
    public bool IsReadOnly { get; set; } = true;

    /// <summary>وصف الإعداد بالعربية</summary>
    public string? DescriptionAr { get; set; }

    /// <summary>وصف الإعداد بالإنجليزية</summary>
    public string? DescriptionEn { get; set; }
}

/// <summary>عقود السيارات (خدمة، إيجار قصير الأجل، إيجار طويل الأجل، منتهي بالتمليك)</summary>
public class Contract : AuditedEntityBase
{
    /// <summary>رقم العقد المرجعي</summary>
    public required string ContractNumber { get; set; }

    /// <summary>معرف نوع العقد</summary>
    public int ContractTypeId { get; set; }

    /// <summary>نوع العقد</summary>
    public ContractType ContractType { get; set; } = null!;

    /// <summary>معرف السيارة موضوع العقد</summary>
    public int CarId { get; set; }

    /// <summary>السيارة موضوع العقد</summary>
    public Car Car { get; set; } = null!;

    /// <summary>معرف الفرع التابع له العقد</summary>
    public int BranchId { get; set; }

    /// <summary>فرع المعرض</summary>
    public Branch Branch { get; set; } = null!;

    /// <summary>العميل المستفيد (في عقود الإيجار والتمليك)</summary>
    public int? CustomerId { get; set; }

    /// <summary>بيانات العميل المستفيد</summary>
    public Customer? Customer { get; set; }

    /// <summary>الموظف / مستخدم النظام المستفيد (في عقود الخدمة)</summary>
    public int? UserId { get; set; }

    /// <summary>بيانات الموظف المستفيد</summary>
    public User? User { get; set; }

    /// <summary>تاريخ بداية العقد</summary>
    public DateTime StartDate { get; set; }

    /// <summary>تاريخ نهاية العقد</summary>
    public DateTime EndDate { get; set; }

    /// <summary>أيام الإيجار المستحق / مدة العقد بالأيام</summary>
    public int PeriodDays { get; set; }

    /// <summary>المدة المحسوبة بالشهور</summary>
    public decimal PeriodMonths { get; set; }

    // Financial basics
    /// <summary>إجمالي قيمة العقد / الإيرادات التقديرية حسب مدة العقد</summary>
    public decimal TotalPrice { get; set; }

    /// <summary>المبلغ المدفوع من قيمة العقد</summary>
    public decimal PaidPrice { get; set; }

    /// <summary>المبلغ المتبقي المستحق من قيمة العقد</summary>
    public decimal RemainingPrice { get; set; }

    // Rental specifics
    /// <summary>قيمة قسط الإيجار التقديري المقترح قبل الدفعة المقدمة (شهرياً)</summary>
    public decimal? MonthlyRent { get; set; }

    /// <summary>قيمة الدفعة المقدمة</summary>
    public decimal? DownPayment { get; set; }

    /// <summary>الحد الأقصى للمسافة المسموحة (كم)</summary>
    public int? MileageLimit { get; set; }

    /// <summary>رسوم كل كيلومتر زائد عن الحد</summary>
    public decimal? ExcessMileageFee { get; set; }

    /// <summary>قراءة العداد الفعلية عند نهاية العقد وتسليم السيارة</summary>
    public int? ActualEndMileage { get; set; }

    /// <summary>تكلفة المسافة الزائدة الإجمالية</summary>
    public decimal? ExcessMileageCost { get; set; }

    // The 4 deduction terms percentages applied to this contract
    /// <summary>نسبة تقدير تأمين شامل للسيارة المعتمدة في العقد (%)</summary>
    public decimal? InsurancePercentage { get; set; }

    /// <summary>نسبة تقدير تكلفة الصيانة خلال فترة العقد المعتمدة في العقد (%)</summary>
    public decimal? MaintenancePercentage { get; set; }

    /// <summary>نسبة تقدير تكلفة سيارة بديلة خلال فترة العقد المعتمدة في العقد (%)</summary>
    public decimal? ReplacementCarPercentage { get; set; }

    /// <summary>نسبة تقدير تكلفة المصاريف الإدارية خلال فترة العقد المعتمدة في العقد (%)</summary>
    public decimal? AdminFeesPercentage { get; set; }

    /// <summary>نسبة الإهلاك السنوي المعتمدة في العقد (%)</summary>
    public decimal? AnnualDepreciationPercentage { get; set; }

    // Tax specifics
    /// <summary>هل تطبق ضريبة القيمة المضافة على عقد الإيجار أم لا</summary>
    public bool IsTaxApplied { get; set; } = true;

    /// <summary>نسبة ضريبة القيمة المضافة المعتمدة في العقد (%)</summary>
    public decimal TaxPercentage { get; set; } = 15.00m;

    /// <summary>قيمة الضريبة المضافة شهرياً على قسط الإيجار</summary>
    public decimal MonthlyTaxAmount { get; set; }

    /// <summary>قيمة الإيجار الشهري المستحق على العميل شاملاً الضريبة</summary>
    public decimal MonthlyRentWithTax { get; set; }

    /// <summary>إجمالي قيمة الضريبة المضافة على كامل مدة العقد</summary>
    public decimal TotalTaxAmount { get; set; }

    /// <summary>إجمالي قيمة العقد النهائي شاملاً الضريبة</summary>
    public decimal TotalPriceWithTax { get; set; }

    // Snapshot at contract creation
    /// <summary>لقطة القيمة الشرائية للسيارة وقت إنشاء العقد</summary>
    public decimal CarPurchasePriceSnapshot { get; set; }

    /// <summary>لقطة قيمة السيارة / القيمة الدفترية للسيارة وقت إنشاء العقد</summary>
    public decimal CarBookValueSnapshot { get; set; }

    /// <summary>لقطة مجمع إهلاك السيارة وقت إنشاء العقد</summary>
    public decimal CarAccumulatedDepreciationSnapshot { get; set; }

    /// <summary>لقطة عمر السيارة الإنتاجي بالسنوات وقت إنشاء العقد</summary>
    public decimal CarProductiveAgeSnapshot { get; set; }

    // Financial calculation results
    /// <summary>إجمالي الإيرادات التقديرية حسب مدة العقد</summary>
    public decimal TotalEstimatedRevenue { get; set; }

    /// <summary>إجمالي التكلفة التقديرية (الإهلاك + التأمين + الصيانة + البديلة + الإدارية)</summary>
    public decimal TotalEstimatedCost { get; set; }

    /// <summary>إجمالي الأرباح التقديرية حسب مدة العقد</summary>
    public decimal ExpectedProfit { get; set; }

    /// <summary>قيمة الفائدة للشركة من تشغيل السيارة (شهرياً)</summary>
    public decimal MonthlyCompanyProfit { get; set; }

    /// <summary>قيمة الإيجار التقديري المقترح (بعد خصم الدفعة المقدمة)</summary>
    public decimal AdjustedMonthlyRent { get; set; }

    /// <summary>خصم قيمة الدفعة المقدمة من قيمة قسط الإيجار الشهري المقترح</summary>
    public decimal DownPaymentMonthlyDiscount { get; set; }

    /// <summary>جدول الإهلاك والتكاليف السنوي بالتفصيل بصيغة JSON (Yearly Schedule)</summary>
    public string? YearlyBreakdownJson { get; set; }

    /// <summary>حالة العقد (Active, Completed, Cancelled)</summary>
    public string Status { get; set; } = "Active";

    /// <summary>ملاحظات وشروط إضافية على العقد</summary>
    public string? Notes { get; set; }
}
