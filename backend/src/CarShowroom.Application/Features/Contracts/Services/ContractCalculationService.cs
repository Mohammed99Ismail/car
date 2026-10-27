using CarShowroom.Application.Features.Contracts.Dtos;

namespace CarShowroom.Application.Features.Contracts.Services;

/// <summary>
/// خدمة الحسابات المالية والإهلاكية لعقود السيارات
/// تتولى حساب القيمة الدفترية، نسب الإهلاك الأربعة، الإيرادات والأرباح التقديرية، والجدول السنوي
/// </summary>
public class ContractCalculationService : IContractCalculationService
{
    /// <summary>
    /// حساب القيمة الدفترية ومجمع الإهلاك والعمر الإنتاجي للسيارة
    /// </summary>
    /// <param name="purchasePrice">القيمة الشرائية للسيارة</param>
    /// <param name="purchaseDate">تاريخ شراء السيارة</param>
    /// <param name="evaluationDate">تاريخ التقييم (عادة تاريخ بداية العقد)</param>
    /// <returns>
    /// BookValue: القيمة الدفترية للسيارة
    /// AccumulatedDepreciation: مجمع الإهلاك
    /// ProductiveAge: عمر السيارة الإنتاجي بالسنوات
    /// </returns>
    public (decimal BookValue, decimal AccumulatedDepreciation, decimal ProductiveAge) CalculateCarBookValue(
        decimal purchasePrice,
        DateTime purchaseDate,
        DateTime? evaluationDate = null)
    {
        var evalDate = evaluationDate ?? DateTime.UtcNow;
        if (purchasePrice <= 0)
        {
            return (0.00m, 0.00m, 0.00m);
        }

        var totalDays = (evalDate.Date - purchaseDate.Date).TotalDays;
        if (totalDays <= 0)
        {
            return (purchasePrice, 0.00m, 0.00m);
        }

        // حساب عمر السيارة الإنتاجي بالسنوات
        var ageInYears = (decimal)totalDays / 365.0m;
        var productiveAge = Math.Round(ageInYears, 2);

        // بعد 4 سنوات، تستهلك السيارة بالكامل وتصل لقيمة التخريد 1 ريال
        if (ageInYears >= 4.0m)
        {
            var bookVal = 1.00m;
            var accumDepr = Math.Max(0m, purchasePrice - bookVal);
            return (bookVal, accumDepr, productiveAge);
        }

        // إهلاك سنوي بمعدل 25% من القيمة الشرائية
        var deprRatio = ageInYears * 0.25m;
        var accumulated = Math.Round(purchasePrice * deprRatio, 2);
        var bookValue = Math.Max(1.00m, purchasePrice - accumulated);

        return (bookValue, accumulated, productiveAge);
    }

    /// <summary>
    /// الحساب اللحظي الشامل لمعادلات وتكاليف وأرباح العقد
    /// </summary>
    public ContractCalculationResultDto Calculate(
        ContractCalculationRequest request,
        decimal defaultInsurance = 4.00m,
        decimal defaultMaintenance = 5.00m,
        decimal defaultReplacement = 1.00m,
        decimal defaultAdminFees = 1.00m,
        decimal defaultDepreciation = 25.00m,
        decimal defaultVat = 15.00m)
    {
        var warnings = new List<string>();

        // 1. تقييم السيارة (القيمة الدفترية وعمر السيارة الإنتاجي)
        var purchasePrice = request.CarPurchasePrice ?? 0m;
        var purchaseDate = request.CarPurchaseDate ?? request.StartDate;
        var (calcBookValue, calcAccumulatedDepr, calcProductiveAge) = CalculateCarBookValue(purchasePrice, purchaseDate, request.StartDate);

        decimal bookValue;
        decimal accumulatedDepr;
        decimal productiveAge;

        if (request.CarBookValue.HasValue && request.CarBookValue.Value > 0)
        {
            bookValue = request.CarBookValue.Value;
            accumulatedDepr = Math.Max(0m, purchasePrice - bookValue);
            productiveAge = (purchasePrice > 0) ? Math.Round((accumulatedDepr / purchasePrice) / 0.25m, 2) : calcProductiveAge;
        }
        else
        {
            bookValue = calcBookValue;
            accumulatedDepr = calcAccumulatedDepr;
            productiveAge = calcProductiveAge;
        }

        // قيمة السيارة المعتمدة في حسابات العقد هي القيمة الدفترية الحالية للسيارة
        var carValueUsed = bookValue;

        // 2. معالجة تواريخ العقد والمدة بالأيام والشهور
        var start = request.StartDate.Date;
        var end = request.EndDate.Date;

        // معالجة فارق التوقيت الزمني (UTC Offset) لضمان دقة التاريخ المحلي
        if (request.StartDate.TimeOfDay.TotalHours >= 20)
            start = start.AddDays(1);
        if (request.EndDate.TimeOfDay.TotalHours >= 20)
            end = end.AddDays(1);

        if (end <= start)
        {
            end = start.AddMonths(12);
            warnings.Add("تاريخ نهاية العقد يجب أن يكون بعد تاريخ البداية. تم الضبط الافتراضي على 12 شهراً.");
        }

        // عقد الإيجار قصير الأجل أو حسب الطلب لا يتجاوز سنة واحدة (12 شهراً)
        if (request.ContractTypeId == 2)
        {
            var maxAllowedEnd = start.AddYears(1);
            if (end > maxAllowedEnd)
            {
                end = maxAllowedEnd;
                warnings.Add("عقد الإيجار قصير الأجل أو حسب الطلب لا يمكن أن يتجاوز سنة واحدة (12 شهراً). تم ضبط نهاية العقد على سنة كحد أقصى.");
            }
        }

        // حساب أيام المدة:
        // في عقد الإيجار القصير: لا نحسب يوم البداية -> rentDays = (end - start).TotalDays
        // في عقد الإيجار الطويل: المدة شاملة البداية والنهاية -> (end - start).TotalDays + 1
        int totalDays;
        int rentDays;
        if (request.ContractTypeId == 2)
        {
            rentDays = (int)(end - start).TotalDays;
            if (rentDays <= 0) rentDays = 1;
            totalDays = rentDays;
        }
        else
        {
            totalDays = (int)(end - start).TotalDays + 1;
            rentDays = totalDays;
        }

        // حساب المدة بالشهور بدقة مع إهمال كسور الثواني والتوقيت الزمني
        decimal periodMonths;
        var rawDayDiff = (int)(end - start).TotalDays;
        var monthDiff = ((end.Year - start.Year) * 12) + end.Month - start.Month;

        // مطابقة سنة كاملة (12 شهراً)
        if ((end.Year - start.Year == 1 && end.Month == start.Month && Math.Abs(end.Day - start.Day) <= 1) || rawDayDiff == 365 || rawDayDiff == 366)
        {
            periodMonths = 12m;
        }
        // مطابقة سنتين (24 شهراً)
        else if ((end.Year - start.Year == 2 && end.Month == start.Month && Math.Abs(end.Day - start.Day) <= 1) || rawDayDiff == 730 || rawDayDiff == 731 || rawDayDiff == 732)
        {
            periodMonths = 24m;
        }
        // مطابقة ثلاث سنوات (36 شهراً)
        else if ((end.Year - start.Year == 3 && end.Month == start.Month && Math.Abs(end.Day - start.Day) <= 1) || rawDayDiff == 1095 || rawDayDiff == 1096)
        {
            periodMonths = 36m;
        }
        // مطابقة سنة ونصف (18 شهراً)
        else if ((monthDiff == 18 && Math.Abs(end.Day - start.Day) <= 1) || rawDayDiff == 548 || rawDayDiff == 549)
        {
            periodMonths = 18m;
        }
        // مطابقة نصف سنة (6 شهور)
        else if ((monthDiff == 6 && Math.Abs(end.Day - start.Day) <= 1) || rawDayDiff == 182 || rawDayDiff == 183)
        {
            periodMonths = 6m;
        }
        // مطابقة نفس اليوم من الشهر
        else if (end.Day == start.Day && monthDiff > 0)
        {
            periodMonths = monthDiff;
        }
        // مضاعفات 30 يوماً
        else if (rentDays % 30 == 0)
        {
            periodMonths = rentDays / 30m;
        }
        else
        {
            // الشهر عبارة عن 30 يوم
            periodMonths = Math.Round((decimal)rentDays / 30m, 2);
            if (periodMonths <= 0) periodMonths = 1m;
            if (Math.Abs(periodMonths - Math.Round(periodMonths)) < 0.05m)
            {
                periodMonths = Math.Round(periodMonths);
            }
        }

        // سنوات العقد لحساب جدول الإهلاك
        var contractYears = (int)Math.Ceiling(periodMonths / 12m);
        if (contractYears < 1) contractYears = 1;
        if (request.ContractTypeId == 2 && contractYears > 1)
        {
            contractYears = 1;
        }

        // تنبيهات قيود مدة العقود
        if (request.ContractTypeId == 2)
        {
            if (request.DownPayment.HasValue && request.DownPayment.Value > 0)
            {
                warnings.Add("عقد الإيجار قصير الأجل أو حسب الطلب لا يحتوي على دفعة مقدمة.");
            }
        }
        else if (request.ContractTypeId == 3)
        {
            if (totalDays < 360 && periodMonths < 11.5m)
            {
                warnings.Add("عقد الإيجار طويل الأجل يجب أن يكون سنة واحدة (12 شهراً) على الأقل.");
            }
        }

        // 3. نسب البنود الأربعة ونسبة الإهلاك السنوي
        var insurancePercent = request.InsurancePercentage ?? defaultInsurance;
        var maintenancePercent = request.MaintenancePercentage ?? defaultMaintenance;
        var replacementPercent = request.ReplacementCarPercentage ?? defaultReplacement;
        var adminFeesPercent = request.AdminFeesPercentage ?? defaultAdminFees;
        var annualDeprPercent = request.AnnualDepreciationPercentage ?? defaultDepreciation;

        // 4. بناء جدول الإهلاك والتكاليف السنوي (Yearly Schedule)
        var hasTerms = request.ContractTypeId == 2 || request.ContractTypeId == 3 || request.ContractTypeId == 4;
        var isLeaseToOwn = request.ContractTypeId == 4;
        var yearlySchedule = new List<ContractYearlyScheduleItem>();

        if (hasTerms)
        {
            for (int y = 1; y <= contractYears; y++)
            {
                decimal cumulativeDeprCost;
                decimal netCarValue;
                decimal insCost;
                decimal maintCost;
                decimal repCost;
                decimal admCost;

                if (isLeaseToOwn)
                {
                    // في عقد الإيجار المنتهي بالتمليك:
                    // يوجد نوعان فقط من التكاليف:
                    // 1) تقدير تأمين شامل للسيارة = (insurancePercent / 100) * قيمة السيارة * عدد السنوات y
                    // 2) قيمة إهلاك السيارة نهاية مدة العقد = القيمة الدفترية بالكامل (تنتقل ملكية السيارة للعميل في نهاية العقد)
                    // وصافي قيمة السيارة في نهاية العقد = 0 (Net Car Value = 0)
                    // وباقي البنود (صيانة، سيارة بديلة، مصاريف إدارية) = 0
                    cumulativeDeprCost = contractYears > 0 
                        ? Math.Round(carValueUsed * (y / (decimal)contractYears), 2)
                        : carValueUsed;
                    if (y == contractYears) cumulativeDeprCost = carValueUsed;
                    netCarValue = Math.Round(carValueUsed - cumulativeDeprCost, 2);

                    insCost = Math.Round((insurancePercent / 100m) * carValueUsed * y, 2);
                    maintCost = 0m;
                    repCost = 0m;
                    admCost = 0m;
                }
                else
                {
                    // في عقود الإيجار التشغيلي (قصير وطويل الأجل):
                    // تطبق البنود الأربعة ونسبة الإهلاك السنوية (25% سنواً):
                    // - قيمة إهلاك السيارة: (نسبة الإهلاك * y) * قيمة السيارة
                    // - صافي قيمة السيارة: قيمة السيارة - قيمة إهلاك السيارة
                    // - تقدير تأمين شامل للسيارة: (نسبة التأمين / 100) * قيمة السيارة * y
                    // - تقدير تكلفة الصيانة: (نسبة الصيانة / 100) * قيمة السيارة * y
                    // - تقدير تكلفة سيارة بديلة: (نسبة البديلة / 100) * قيمة السيارة * y
                    // - تقدير تكلفة المصاريف الإدارية: (نسبة الإدارية / 100) * قيمة السيارة * y
                    var cumulativeDeprRatio = Math.Min(1.0m, y * (annualDeprPercent / 100m));
                    cumulativeDeprCost = Math.Round(carValueUsed * cumulativeDeprRatio, 2);
                    netCarValue = Math.Round(carValueUsed - cumulativeDeprCost, 2);

                    insCost = Math.Round((insurancePercent / 100m) * carValueUsed * y, 2);
                    maintCost = Math.Round((maintenancePercent / 100m) * carValueUsed * y, 2);
                    repCost = Math.Round((replacementPercent / 100m) * carValueUsed * y, 2);
                    admCost = Math.Round((adminFeesPercent / 100m) * carValueUsed * y, 2);
                }

                // إجمالي التكلفة التقديرية لهذه السنة
                var totalYearCost = insCost + maintCost + repCost + admCost + cumulativeDeprCost;
                // قيمة تكلفة للسيارة (شهرياً)
                var monthlyCost = y > 0 ? Math.Round(totalYearCost / (y * 12m), 2) : 0m;

                yearlySchedule.Add(new ContractYearlyScheduleItem(
                    Year: y,
                    NetCarValue: netCarValue,
                    DepreciationCost: cumulativeDeprCost,
                    InsuranceCost: insCost,
                    MaintenanceCost: maintCost,
                    ReplacementCarCost: repCost,
                    AdminFeesCost: admCost,
                    TotalEstimatedCost: totalYearCost,
                    MonthlyCost: monthlyCost));
            }
        }

        // 5. الملخص المالي والإيرادات والأرباح
        var rawMonthlyRent = request.MonthlyRent ?? 0m;
        var downPayment = request.ContractTypeId == 2 ? 0m : (request.DownPayment ?? 0m);

        // 6. حساب ضريبة القيمة المضافة (VAT)
        // في عقود الخدمة (ContractTypeId == 1)، لا توجد ضريبة لأن السيارة مخصصة لموظف المعرض
        var isTaxApplied = request.ContractTypeId != 1 && request.ApplyTax;
        var taxPercentage = isTaxApplied ? (request.TaxPercentage ?? defaultVat) : 0m;
        if (taxPercentage < 0m) taxPercentage = 0m;

        // الضريبة تطبق على قيمة الإيجار الأساسي مباشرة (وليس على الصافي بعد الدفعة المقدمة):
        // مثال: إذا كان قسط الإيجار الشهري 1000 والضريبة 15%، تصبح قيمة الضريبة 150
        // ويتم التعامل مع الإيجار الشهري على أساس 1150، ثم تتبع باقي الحسابات كما هي.
        decimal monthlyTaxAmount = 0m;
        decimal monthlyRent = rawMonthlyRent;

        if (isTaxApplied && taxPercentage > 0m)
        {
            monthlyTaxAmount = Math.Round(rawMonthlyRent * (taxPercentage / 100m), 2);
            monthlyRent = Math.Round(rawMonthlyRent + monthlyTaxAmount, 2);
        }

        // الإيجار اليومي التقديري: قيمة الإيجار الشهري مقسومة على 30 يوماً
        var dailyRent = monthlyRent > 0 ? Math.Round(monthlyRent / 30m, 2) : 0m;
        decimal totalEstimatedRevenue;

        if (request.ContractTypeId == 2)
        {
            // في العقد قصير الأجل أو حسب الطلب:
            // إجمالي الإيرادات = الإيجار اليومي التقديري * أيام الإيجار المستحق (rentDays)
            totalEstimatedRevenue = Math.Round((monthlyRent / 30m) * rentDays, 2);
        }
        else
        {
            // في العقد طويل الأجل أو المنتهي بالتمليك:
            // إجمالي الإيرادات = قسط الإيجار الشهري * عدد الشهور المحسوبة
            totalEstimatedRevenue = Math.Round(monthlyRent * periodMonths, 2);
        }

        var totalEstimatedCost = 0m;
        var totalDepreciationCost = 0m;
        var netCarValueAtEnd = carValueUsed;
        var totalInsuranceCost = 0m;
        var totalMaintenanceCost = 0m;
        var totalReplacementCarCost = 0m;
        var totalAdminFeesCost = 0m;
        var monthlyCarCost = 0m;

        if (hasTerms && yearlySchedule.Count > 0)
        {
            var lastSchedule = yearlySchedule[^1];
            totalEstimatedCost = lastSchedule.TotalEstimatedCost;
            totalDepreciationCost = lastSchedule.DepreciationCost;
            netCarValueAtEnd = lastSchedule.NetCarValue;
            totalInsuranceCost = lastSchedule.InsuranceCost;
            totalMaintenanceCost = lastSchedule.MaintenanceCost;
            totalReplacementCarCost = lastSchedule.ReplacementCarCost;
            totalAdminFeesCost = lastSchedule.AdminFeesCost;
            monthlyCarCost = periodMonths > 0
                ? Math.Round(totalEstimatedCost / periodMonths, 2)
                : lastSchedule.MonthlyCost;
        }

        // إجمالي الأرباح التقديرية حسب مدة العقد = إجمالي الإيرادات التقديرية - إجمالي التكلفة التقديرية
        var expectedProfit = Math.Round(totalEstimatedRevenue - totalEstimatedCost, 2);
        // قيمة الفائدة للشركة من تشغيل السيارة (شهرياً)
        var monthlyCompanyProfit = periodMonths > 0 ? Math.Round(expectedProfit / periodMonths, 2) : 0m;

        // تعديل الإيجار الشهري بعد احتساب خصم الدفعة المقدمة
        var adjustedMonthlyRent = monthlyRent;
        var downPaymentMonthlyDiscount = 0m;

        if (periodMonths > 0 && downPayment > 0 && request.ContractTypeId != 2)
        {
            // قيمة الإيجار التقديري المقترح (بعد خصم الدفعة المقدمة):
            // (إجمالي الإيرادات - الدفعة المقدمة) / عدد الشهور
            adjustedMonthlyRent = Math.Round(((monthlyRent * periodMonths) - downPayment) / periodMonths, 2);
            // خصم قيمة الدفعة المقدمة من قيمة قسط الإيجار الشهري المقترح
            downPaymentMonthlyDiscount = Math.Round(Math.Abs(monthlyRent - adjustedMonthlyRent), 2);
        }

        // إجمالي قيمة الضريبة على كامل مدة العقد
        decimal totalTaxAmount = 0m;
        if (isTaxApplied && monthlyTaxAmount > 0m)
        {
            totalTaxAmount = request.ContractTypeId == 2
                ? Math.Round((monthlyTaxAmount / 30m) * rentDays, 2)
                : Math.Round(monthlyTaxAmount * periodMonths, 2);
        }

        decimal monthlyRentWithTax = adjustedMonthlyRent;
        decimal totalPriceWithTax = totalEstimatedRevenue;

        return new ContractCalculationResultDto(
            CarPurchasePrice: purchasePrice,
            CarPurchaseDate: purchaseDate,
            CarProductiveAge: productiveAge,
            CarBookValue: bookValue,
            CarAccumulatedDepreciation: accumulatedDepr,
            CarValueUsed: carValueUsed,
            PeriodDays: totalDays,
            PeriodMonths: periodMonths,
            ContractYears: contractYears,
            YearlySchedule: yearlySchedule,
            MonthlyRentBeforeDownPayment: monthlyRent,
            DownPayment: downPayment,
            AdjustedMonthlyRent: adjustedMonthlyRent,
            DownPaymentMonthlyDiscount: downPaymentMonthlyDiscount,
            TotalEstimatedRevenue: totalEstimatedRevenue,
            TotalEstimatedCost: totalEstimatedCost,
            ExpectedProfit: expectedProfit,
            MonthlyCompanyProfit: monthlyCompanyProfit,
            MileageLimit: request.MileageLimit,
            ExcessMileageFee: request.ExcessMileageFee,
            Warnings: warnings,
            TotalDepreciationCost: totalDepreciationCost,
            NetCarValueAtEnd: netCarValueAtEnd,
            TotalInsuranceCost: totalInsuranceCost,
            TotalMaintenanceCost: totalMaintenanceCost,
            TotalReplacementCarCost: totalReplacementCarCost,
            TotalAdminFeesCost: totalAdminFeesCost,
            MonthlyCarCost: monthlyCarCost,
            DailyRent: dailyRent,
            RentDays: rentDays,
            IsTaxApplied: isTaxApplied,
            TaxPercentage: taxPercentage,
            MonthlyTaxAmount: monthlyTaxAmount,
            MonthlyRentWithTax: monthlyRentWithTax,
            TotalTaxAmount: totalTaxAmount,
            TotalPriceWithTax: totalPriceWithTax);
    }
}
