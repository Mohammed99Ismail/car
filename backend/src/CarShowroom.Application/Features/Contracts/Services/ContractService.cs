using System.Text.Json;
using CarShowroom.Application.Common.Extensions;
using CarShowroom.Application.Common.Interfaces;
using CarShowroom.Application.Common.Localization;
using CarShowroom.Application.Common.Models;
using CarShowroom.Application.Features.Contracts.Dtos;
using CarShowroom.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CarShowroom.Application.Features.Contracts.Services;

public class ContractService(
    IApplicationDbContext context,
    IContractCalculationService calculationService) : IContractService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public async Task<CustomApiResponse<PagedListModel<ContractDto>>> GetContractsAsync(
        ContractQueryParameters query,
        CancellationToken cancellationToken = default)
    {
        var source = context.Contracts
            .Include(c => c.ContractType)
            .Include(c => c.Car).ThenInclude(car => car.Make)
            .Include(c => c.Car).ThenInclude(car => car.Model)
            .Include(c => c.Branch)
            .Include(c => c.Customer)
            .Include(c => c.User)
            .AsNoTracking();

        if (query.ContractTypeId.HasValue)
            source = source.Where(c => c.ContractTypeId == query.ContractTypeId.Value);

        if (query.CarId.HasValue)
            source = source.Where(c => c.CarId == query.CarId.Value);

        if (query.BranchId.HasValue)
            source = source.Where(c => c.BranchId == query.BranchId.Value);

        if (query.CustomerId.HasValue)
            source = source.Where(c => c.CustomerId == query.CustomerId.Value);

        if (query.UserId.HasValue)
            source = source.Where(c => c.UserId == query.UserId.Value);

        if (!string.IsNullOrWhiteSpace(query.Status))
            source = source.Where(c => c.Status.ToLower() == query.Status.Trim().ToLower());

        if (query.StartDateFrom.HasValue)
            source = source.Where(c => c.StartDate >= query.StartDateFrom.Value);

        if (query.StartDateTo.HasValue)
            source = source.Where(c => c.StartDate <= query.StartDateTo.Value);

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim().ToLower();
            source = source.Where(c =>
                c.ContractNumber.ToLower().Contains(search) ||
                c.Car.PlateNumber.ToLower().Contains(search) ||
                (c.Customer != null && (c.Customer.Name.ToLower().Contains(search) || c.Customer.Mobile.Contains(search))) ||
                (c.User != null && c.User.FullName.ToLower().Contains(search)));
        }

        var paged = await source.ToPagedListAsync(
            query,
            nameof(Contract.Id),
            cancellationToken);

        var dtos = paged.Data.Select(MapToDto).ToList();

        return CustomApiResponse<PagedListModel<ContractDto>>.Success(new PagedListModel<ContractDto>
        {
            Data = dtos,
            Length = paged.Length,
            Page = paged.Page,
            Limit = paged.Limit
        });
    }

    public async Task<CustomApiResponse<ContractDto>> GetContractByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var contract = await context.Contracts
            .Include(c => c.ContractType)
            .Include(c => c.Car).ThenInclude(car => car.Make)
            .Include(c => c.Car).ThenInclude(car => car.Model)
            .Include(c => c.Branch)
            .Include(c => c.Customer)
            .Include(c => c.User)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

        if (contract is null)
        {
            return CustomApiResponse<ContractDto>.NotFound("العقد غير موجود");
        }

        return CustomApiResponse<ContractDto>.Success(MapToDto(contract));
    }

    public async Task<CustomApiResponse<ContractReportDto>> GetContractReportAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var contract = await context.Contracts
            .Include(c => c.ContractType)
            .Include(c => c.Car).ThenInclude(car => car.Make)
            .Include(c => c.Car).ThenInclude(car => car.Model)
            .Include(c => c.Branch)
            .Include(c => c.Customer)
            .Include(c => c.User)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

        if (contract is null)
        {
            return CustomApiResponse<ContractReportDto>.NotFound("العقد غير موجود");
        }

        var contractDto = MapToDto(contract);

        // Deserialize or re-calculate schedule
        List<ContractYearlyScheduleItem> yearlyList = [];
        if (!string.IsNullOrWhiteSpace(contract.YearlyBreakdownJson))
        {
            try
            {
                yearlyList = JsonSerializer.Deserialize<List<ContractYearlyScheduleItem>>(
                    contract.YearlyBreakdownJson, JsonOptions) ?? [];
            }
            catch
            {
                // Fallback to calculation
            }
        }

        var totalDepreciationCost = 0m;
        var netCarValueAtEnd = contract.CarBookValueSnapshot;
        var totalInsuranceCost = 0m;
        var totalMaintenanceCost = 0m;
        var totalReplacementCarCost = 0m;
        var totalAdminFeesCost = 0m;
        var monthlyCarCost = 0m;

        if (yearlyList.Count > 0)
        {
            var lastSchedule = yearlyList[^1];
            totalDepreciationCost = lastSchedule.DepreciationCost;
            netCarValueAtEnd = lastSchedule.NetCarValue;
            totalInsuranceCost = lastSchedule.InsuranceCost;
            totalMaintenanceCost = lastSchedule.MaintenanceCost;
            totalReplacementCarCost = lastSchedule.ReplacementCarCost;
            totalAdminFeesCost = lastSchedule.AdminFeesCost;
            monthlyCarCost = contract.PeriodMonths > 0
                ? Math.Round(contract.TotalEstimatedCost / contract.PeriodMonths, 2)
                : lastSchedule.MonthlyCost;
        }

        var calcResult = new ContractCalculationResultDto(
            CarPurchasePrice: contract.CarPurchasePriceSnapshot,
            CarPurchaseDate: contract.Car.PurchaseDate,
            CarProductiveAge: contract.CarProductiveAgeSnapshot,
            CarBookValue: contract.CarBookValueSnapshot,
            CarAccumulatedDepreciation: contract.CarAccumulatedDepreciationSnapshot,
            CarValueUsed: contract.CarBookValueSnapshot,
            PeriodDays: contract.PeriodDays,
            PeriodMonths: contract.PeriodMonths,
            ContractYears: yearlyList.Count > 0 ? yearlyList.Count : (int)Math.Ceiling(contract.PeriodMonths / 12m),
            YearlySchedule: yearlyList,
            MonthlyRentBeforeDownPayment: contract.MonthlyRent ?? 0m,
            DownPayment: contract.DownPayment ?? 0m,
            AdjustedMonthlyRent: contract.AdjustedMonthlyRent,
            DownPaymentMonthlyDiscount: contract.DownPaymentMonthlyDiscount,
            TotalEstimatedRevenue: contract.TotalEstimatedRevenue,
            TotalEstimatedCost: contract.TotalEstimatedCost,
            ExpectedProfit: contract.ExpectedProfit,
            MonthlyCompanyProfit: contract.MonthlyCompanyProfit,
            MileageLimit: contract.MileageLimit,
            ExcessMileageFee: contract.ExcessMileageFee,
            Warnings: [],
            TotalDepreciationCost: totalDepreciationCost,
            NetCarValueAtEnd: netCarValueAtEnd,
            TotalInsuranceCost: totalInsuranceCost,
            TotalMaintenanceCost: totalMaintenanceCost,
            TotalReplacementCarCost: totalReplacementCarCost,
            TotalAdminFeesCost: totalAdminFeesCost,
            MonthlyCarCost: monthlyCarCost,
            DailyRent: contract.MonthlyRent.HasValue ? Math.Round(contract.MonthlyRent.Value / 30m, 2) : 0m,
            RentDays: contract.PeriodDays,
            IsTaxApplied: contractDto.IsTaxApplied,
            TaxPercentage: contractDto.TaxPercentage,
            MonthlyTaxAmount: contractDto.MonthlyTaxAmount,
            MonthlyRentWithTax: contractDto.MonthlyRentWithTax,
            TotalTaxAmount: contractDto.TotalTaxAmount,
            TotalPriceWithTax: contractDto.TotalPriceWithTax);

        var report = new ContractReportDto(
            Contract: contractDto,
            Calculation: calcResult,
            GeneratedAt: DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));

        return CustomApiResponse<ContractReportDto>.Success(report);
    }

    public async Task<CustomApiResponse<ContractCalculationResultDto>> CalculateAsync(
        ContractCalculationRequest request,
        CancellationToken cancellationToken = default)
    {
        var termsConfig = await GetOrCreateTermsConfigAsync(cancellationToken);
        var defaultVat = await GetSystemVatPercentageAsync(cancellationToken);

        var calcRequest = request;
        if (request.CarId.HasValue)
        {
            var car = await context.Cars.AsNoTracking().FirstOrDefaultAsync(c => c.Id == request.CarId.Value, cancellationToken);
            if (car is not null)
            {
                calcRequest = request with
                {
                    CarPurchasePrice = request.CarPurchasePrice ?? car.PurchasePrice,
                    CarPurchaseDate = request.CarPurchaseDate ?? car.PurchaseDate,
                    CarBookValue = request.CarBookValue ?? (car.BookValue > 0 ? car.BookValue : null)
                };
            }
        }

        var result = calculationService.Calculate(
            calcRequest,
            termsConfig.InsurancePercentage,
            termsConfig.MaintenancePercentage,
            termsConfig.ReplacementCarPercentage,
            termsConfig.AdminFeesPercentage,
            termsConfig.AnnualDepreciationPercentage,
            defaultVat);

        return CustomApiResponse<ContractCalculationResultDto>.Success(result);
    }

    public async Task<CustomApiResponse<ContractDto>> CreateContractAsync(
        CreateContractRequest request,
        CancellationToken cancellationToken = default)
    {
        // 1. Verify Car
        var car = await context.Cars.FirstOrDefaultAsync(c => c.Id == request.CarId, cancellationToken);
        if (car is null)
        {
            return CustomApiResponse<ContractDto>.NotFound(Msg.NotFound(Msg.Car));
        }

        // 2. Verify Branch
        var branchExists = await context.Branches.AnyAsync(b => b.Id == request.BranchId, cancellationToken);
        if (!branchExists)
        {
            return CustomApiResponse<ContractDto>.NotFound(Msg.NotFound(Msg.Branch));
        }

        // 3. Verify Beneficiary
        if (request.ContractTypeId == 1) // Service
        {
            if (!request.UserId.HasValue || !await context.Users.AnyAsync(u => u.Id == request.UserId.Value, cancellationToken))
            {
                return CustomApiResponse<ContractDto>.BadRequest("عقد الخدمة يتطلب اختيار مستخدم / موظف نظام صحيح");
            }
        }
        else // Rental & Lease-to-own
        {
            if (!request.CustomerId.HasValue || !await context.Customers.AnyAsync(c => c.Id == request.CustomerId.Value, cancellationToken))
            {
                return CustomApiResponse<ContractDto>.BadRequest("عقد الإيجار يتطلب اختيار عميل صحيح");
            }
        }

        // Validate Short-term / on-demand rental duration <= 1 year
        if (request.ContractTypeId == 2)
        {
            var startD = request.StartDate.Date;
            var endD = request.EndDate.Date;
            if (request.StartDate.TimeOfDay.TotalHours >= 20) startD = startD.AddDays(1);
            if (request.EndDate.TimeOfDay.TotalHours >= 20) endD = endD.AddDays(1);
            if (endD > startD.AddYears(1))
            {
                return CustomApiResponse<ContractDto>.BadRequest("عقد الإيجار قصير الأجل أو حسب الطلب لا يمكن أن يتجاوز سنة واحدة (12 شهراً).");
            }
        }

        // 4. Calculate Financials
        var termsConfig = await GetOrCreateTermsConfigAsync(cancellationToken);
        var defaultVat = await GetSystemVatPercentageAsync(cancellationToken);

        var calcReq = new ContractCalculationRequest(
            ContractTypeId: request.ContractTypeId,
            CarId: car.Id,
            CarPurchasePrice: car.PurchasePrice,
            CarPurchaseDate: car.PurchaseDate,
            StartDate: request.StartDate,
            EndDate: request.EndDate,
            MonthlyRent: request.MonthlyRent,
            DownPayment: request.DownPayment,
            MileageLimit: request.MileageLimit,
            ExcessMileageFee: request.ExcessMileageFee,
            InsurancePercentage: request.InsurancePercentage ?? termsConfig.InsurancePercentage,
            MaintenancePercentage: request.MaintenancePercentage ?? termsConfig.MaintenancePercentage,
            ReplacementCarPercentage: request.ReplacementCarPercentage ?? termsConfig.ReplacementCarPercentage,
            AdminFeesPercentage: request.AdminFeesPercentage ?? termsConfig.AdminFeesPercentage,
            AnnualDepreciationPercentage: request.AnnualDepreciationPercentage ?? termsConfig.AnnualDepreciationPercentage,
            CarBookValue: car.BookValue > 0 ? car.BookValue : null,
            ApplyTax: request.ApplyTax,
            TaxPercentage: request.TaxPercentage ?? defaultVat);

        var calcResult = calculationService.Calculate(
            calcReq,
            termsConfig.InsurancePercentage,
            termsConfig.MaintenancePercentage,
            termsConfig.ReplacementCarPercentage,
            termsConfig.AdminFeesPercentage,
            termsConfig.AnnualDepreciationPercentage,
            defaultVat);

        // Update Car BookValue in DB as well
        car.BookValue = calcResult.CarBookValue;

        // 5. Generate Contract Number: CNT-{Year}-{Count+1}
        var year = DateTime.UtcNow.Year;
        var nextCount = await context.Contracts.CountAsync(cancellationToken) + 1;
        var contractNumber = $"CNT-{year}-{nextCount:D4}";

        var yearlyJson = JsonSerializer.Serialize(calcResult.YearlySchedule, JsonOptions);

        var contract = new Contract
        {
            ContractNumber = contractNumber,
            ContractTypeId = request.ContractTypeId,
            CarId = request.CarId,
            BranchId = request.BranchId,
            CustomerId = request.ContractTypeId == 1 ? null : request.CustomerId,
            UserId = request.ContractTypeId == 1 ? request.UserId : null,
            StartDate = request.StartDate.ToUniversalTime(),
            EndDate = request.EndDate.ToUniversalTime(),
            PeriodDays = calcResult.PeriodDays,
            PeriodMonths = calcResult.PeriodMonths,
            TotalPrice = calcResult.TotalPriceWithTax,
            PaidPrice = calcResult.DownPayment,
            RemainingPrice = Math.Max(0m, calcResult.TotalPriceWithTax - calcResult.DownPayment),
            MonthlyRent = request.MonthlyRent,
            DownPayment = calcResult.DownPayment,
            MileageLimit = request.MileageLimit,
            ExcessMileageFee = request.ExcessMileageFee,
            InsurancePercentage = calcReq.InsurancePercentage,
            MaintenancePercentage = calcReq.MaintenancePercentage,
            ReplacementCarPercentage = calcReq.ReplacementCarPercentage,
            AdminFeesPercentage = calcReq.AdminFeesPercentage,
            AnnualDepreciationPercentage = calcReq.AnnualDepreciationPercentage,
            CarPurchasePriceSnapshot = calcResult.CarPurchasePrice,
            CarBookValueSnapshot = calcResult.CarBookValue,
            CarAccumulatedDepreciationSnapshot = calcResult.CarAccumulatedDepreciation,
            CarProductiveAgeSnapshot = calcResult.CarProductiveAge,
            TotalEstimatedRevenue = calcResult.TotalEstimatedRevenue,
            TotalEstimatedCost = calcResult.TotalEstimatedCost,
            ExpectedProfit = calcResult.ExpectedProfit,
            MonthlyCompanyProfit = calcResult.MonthlyCompanyProfit,
            AdjustedMonthlyRent = calcResult.AdjustedMonthlyRent,
            DownPaymentMonthlyDiscount = calcResult.DownPaymentMonthlyDiscount,
            IsTaxApplied = calcResult.IsTaxApplied,
            TaxPercentage = calcResult.TaxPercentage,
            MonthlyTaxAmount = calcResult.MonthlyTaxAmount,
            MonthlyRentWithTax = calcResult.MonthlyRentWithTax,
            TotalTaxAmount = calcResult.TotalTaxAmount,
            TotalPriceWithTax = calcResult.TotalPriceWithTax,
            YearlyBreakdownJson = yearlyJson,
            Status = "Active",
            Notes = request.Notes?.Trim()
        };

        await context.Contracts.AddAsync(contract, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return await GetContractByIdAsync(contract.Id, cancellationToken);
    }

    public async Task<CustomApiResponse<ContractDto>> UpdateContractAsync(
        int id,
        UpdateContractRequest request,
        CancellationToken cancellationToken = default)
    {
        var contract = await context.Contracts.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        if (contract is null)
        {
            return CustomApiResponse<ContractDto>.NotFound("العقد غير موجود");
        }

        var car = await context.Cars.FirstOrDefaultAsync(c => c.Id == request.CarId, cancellationToken);
        if (car is null)
        {
            return CustomApiResponse<ContractDto>.NotFound(Msg.NotFound(Msg.Car));
        }

        var branchExists = await context.Branches.AnyAsync(b => b.Id == request.BranchId, cancellationToken);
        if (!branchExists)
        {
            return CustomApiResponse<ContractDto>.NotFound(Msg.NotFound(Msg.Branch));
        }

        // Validate Short-term / on-demand rental duration <= 1 year
        if (request.ContractTypeId == 2)
        {
            var startD = request.StartDate.Date;
            var endD = request.EndDate.Date;
            if (request.StartDate.TimeOfDay.TotalHours >= 20) startD = startD.AddDays(1);
            if (request.EndDate.TimeOfDay.TotalHours >= 20) endD = endD.AddDays(1);
            if (endD > startD.AddYears(1))
            {
                return CustomApiResponse<ContractDto>.BadRequest("عقد الإيجار قصير الأجل أو حسب الطلب لا يمكن أن يتجاوز سنة واحدة (12 شهراً).");
            }
        }

        var termsConfig = await GetOrCreateTermsConfigAsync(cancellationToken);
        var defaultVat = await GetSystemVatPercentageAsync(cancellationToken);
        var applyTax = request.ApplyTax ?? contract.IsTaxApplied;
        var taxPercentage = request.TaxPercentage ?? (contract.TaxPercentage > 0 ? contract.TaxPercentage : defaultVat);

        var calcReq = new ContractCalculationRequest(
            ContractTypeId: request.ContractTypeId,
            CarId: car.Id,
            CarPurchasePrice: car.PurchasePrice,
            CarPurchaseDate: car.PurchaseDate,
            StartDate: request.StartDate,
            EndDate: request.EndDate,
            MonthlyRent: request.MonthlyRent,
            DownPayment: request.DownPayment,
            MileageLimit: request.MileageLimit,
            ExcessMileageFee: request.ExcessMileageFee,
            InsurancePercentage: request.InsurancePercentage ?? termsConfig.InsurancePercentage,
            MaintenancePercentage: request.MaintenancePercentage ?? termsConfig.MaintenancePercentage,
            ReplacementCarPercentage: request.ReplacementCarPercentage ?? termsConfig.ReplacementCarPercentage,
            AdminFeesPercentage: request.AdminFeesPercentage ?? termsConfig.AdminFeesPercentage,
            AnnualDepreciationPercentage: request.AnnualDepreciationPercentage ?? termsConfig.AnnualDepreciationPercentage,
            ApplyTax: applyTax,
            TaxPercentage: taxPercentage);

        var calcResult = calculationService.Calculate(
            calcReq,
            termsConfig.InsurancePercentage,
            termsConfig.MaintenancePercentage,
            termsConfig.ReplacementCarPercentage,
            termsConfig.AdminFeesPercentage,
            termsConfig.AnnualDepreciationPercentage,
            defaultVat);

        // Excess mileage calculation if contract is completing
        decimal? excessMileageCost = null;
        if (request.ActualEndMileage.HasValue && request.MileageLimit.HasValue && request.ExcessMileageFee.HasValue)
        {
            if (request.ActualEndMileage.Value > request.MileageLimit.Value)
            {
                var excessKm = request.ActualEndMileage.Value - request.MileageLimit.Value;
                excessMileageCost = Math.Round(excessKm * request.ExcessMileageFee.Value, 2);
            }
            else
            {
                excessMileageCost = 0m;
            }
        }

        var yearlyJson = JsonSerializer.Serialize(calcResult.YearlySchedule, JsonOptions);

        contract.ContractTypeId = request.ContractTypeId;
        contract.CarId = request.CarId;
        contract.BranchId = request.BranchId;
        contract.CustomerId = request.ContractTypeId == 1 ? null : request.CustomerId;
        contract.UserId = request.ContractTypeId == 1 ? request.UserId : null;
        contract.StartDate = request.StartDate.ToUniversalTime();
        contract.EndDate = request.EndDate.ToUniversalTime();
        contract.PeriodDays = calcResult.PeriodDays;
        contract.PeriodMonths = calcResult.PeriodMonths;
        contract.MonthlyRent = request.MonthlyRent;
        contract.DownPayment = calcResult.DownPayment;
        contract.PaidPrice = request.PaidPrice;
        contract.TotalPrice = calcResult.TotalPriceWithTax + (excessMileageCost ?? 0m);
        contract.RemainingPrice = Math.Max(0m, contract.TotalPrice - contract.PaidPrice);
        contract.MileageLimit = request.MileageLimit;
        contract.ExcessMileageFee = request.ExcessMileageFee;
        contract.ActualEndMileage = request.ActualEndMileage;
        contract.ExcessMileageCost = excessMileageCost;
        contract.InsurancePercentage = calcReq.InsurancePercentage;
        contract.MaintenancePercentage = calcReq.MaintenancePercentage;
        contract.ReplacementCarPercentage = calcReq.ReplacementCarPercentage;
        contract.AdminFeesPercentage = calcReq.AdminFeesPercentage;
        contract.AnnualDepreciationPercentage = calcReq.AnnualDepreciationPercentage;
        contract.TotalEstimatedRevenue = calcResult.TotalEstimatedRevenue;
        contract.TotalEstimatedCost = calcResult.TotalEstimatedCost;
        contract.ExpectedProfit = calcResult.ExpectedProfit;
        contract.MonthlyCompanyProfit = calcResult.MonthlyCompanyProfit;
        contract.AdjustedMonthlyRent = calcResult.AdjustedMonthlyRent;
        contract.DownPaymentMonthlyDiscount = calcResult.DownPaymentMonthlyDiscount;
        contract.IsTaxApplied = calcResult.IsTaxApplied;
        contract.TaxPercentage = calcResult.TaxPercentage;
        contract.MonthlyTaxAmount = calcResult.MonthlyTaxAmount;
        contract.MonthlyRentWithTax = calcResult.MonthlyRentWithTax;
        contract.TotalTaxAmount = calcResult.TotalTaxAmount;
        contract.TotalPriceWithTax = calcResult.TotalPriceWithTax;
        contract.YearlyBreakdownJson = yearlyJson;
        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            contract.Status = request.Status.Trim();
        }
        contract.Notes = request.Notes?.Trim();

        await context.SaveChangesAsync(cancellationToken);

        return await GetContractByIdAsync(contract.Id, cancellationToken);
    }

    public async Task<CustomApiResponse<Nothing>> DeleteContractAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var contract = await context.Contracts.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        if (contract is null)
        {
            return CustomApiResponse<Nothing>.NotFound("العقد غير موجود");
        }

        context.Contracts.Remove(contract);
        await context.SaveChangesAsync(cancellationToken);

        return CustomApiResponse<Nothing>.Success(Nothing.Value, Msg.Deleted);
    }

    public async Task<CustomApiResponse<ContractTermsConfigDto>> GetTermsConfigAsync(
        CancellationToken cancellationToken = default)
    {
        var config = await GetOrCreateTermsConfigAsync(cancellationToken);
        var dto = new ContractTermsConfigDto(
            config.InsurancePercentage,
            config.MaintenancePercentage,
            config.ReplacementCarPercentage,
            config.AdminFeesPercentage,
            config.AnnualDepreciationPercentage,
            config.LastModifiedDate);

        return CustomApiResponse<ContractTermsConfigDto>.Success(dto);
    }

    public async Task<CustomApiResponse<ContractTermsConfigDto>> UpdateTermsConfigAsync(
        UpdateContractTermsConfigRequest request,
        CancellationToken cancellationToken = default)
    {
        var config = await context.ContractTermsConfigs.FirstOrDefaultAsync(cancellationToken);
        if (config is null)
        {
            config = new ContractTermsConfig();
            await context.ContractTermsConfigs.AddAsync(config, cancellationToken);
        }

        config.InsurancePercentage = request.InsurancePercentage;
        config.MaintenancePercentage = request.MaintenancePercentage;
        config.ReplacementCarPercentage = request.ReplacementCarPercentage;
        config.AdminFeesPercentage = request.AdminFeesPercentage;
        config.AnnualDepreciationPercentage = request.AnnualDepreciationPercentage;
        config.LastModifiedDate = DateTime.UtcNow;

        await context.SaveChangesAsync(cancellationToken);

        var dto = new ContractTermsConfigDto(
            config.InsurancePercentage,
            config.MaintenancePercentage,
            config.ReplacementCarPercentage,
            config.AdminFeesPercentage,
            config.AnnualDepreciationPercentage,
            config.LastModifiedDate);

        return CustomApiResponse<ContractTermsConfigDto>.Success(dto);
    }

    private async Task<ContractTermsConfig> GetOrCreateTermsConfigAsync(CancellationToken cancellationToken)
    {
        var config = await context.ContractTermsConfigs.FirstOrDefaultAsync(cancellationToken);
        if (config is null)
        {
            config = new ContractTermsConfig
            {
                InsurancePercentage = 4.00m,
                MaintenancePercentage = 5.00m,
                ReplacementCarPercentage = 1.00m,
                AdminFeesPercentage = 1.00m,
                AnnualDepreciationPercentage = 25.00m,
                LastModifiedDate = DateTime.UtcNow
            };
            await context.ContractTermsConfigs.AddAsync(config, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }
        return config;
    }

    private async Task<decimal> GetSystemVatPercentageAsync(CancellationToken cancellationToken)
    {
        var setting = await context.SystemSettings
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Key == "VAT_PERCENTAGE", cancellationToken);

        if (setting != null && decimal.TryParse(setting.Value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var vat))
        {
            return vat;
        }

        return 15.00m;
    }

    private static ContractDto MapToDto(Contract c)
    {
        var isTax = c.IsTaxApplied && c.ContractTypeId != 1;
        var taxPercent = isTax ? (c.TaxPercentage > 0 ? c.TaxPercentage : 15.00m) : 0m;
        var monthlyTax = isTax ? (c.MonthlyTaxAmount > 0 ? c.MonthlyTaxAmount : Math.Round(c.AdjustedMonthlyRent * (taxPercent / 100m), 2)) : 0m;
        var monthlyWithTax = isTax ? (c.MonthlyRentWithTax > 0 ? c.MonthlyRentWithTax : Math.Round(c.AdjustedMonthlyRent + monthlyTax, 2)) : c.AdjustedMonthlyRent;
        var totalTax = isTax ? (c.TotalTaxAmount > 0 ? c.TotalTaxAmount : Math.Round(c.TotalEstimatedRevenue * (taxPercent / 100m), 2)) : 0m;
        var totalWithTax = isTax ? (c.TotalPriceWithTax > 0 ? c.TotalPriceWithTax : Math.Round(c.TotalEstimatedRevenue + totalTax, 2)) : (c.TotalPrice > 0 ? c.TotalPrice : c.TotalEstimatedRevenue);

        return new(
            c.Id,
            c.ContractNumber,
            c.ContractTypeId,
            c.ContractType?.NameAr ?? string.Empty,
            c.ContractType?.NameEn ?? string.Empty,
            c.CarId,
            c.Car?.Make?.Name ?? string.Empty,
            c.Car?.Model?.Name ?? string.Empty,
            c.Car?.PlateNumber ?? string.Empty,
            c.Car?.ManufactureYear ?? 0,
            c.Car?.Color ?? string.Empty,
            c.Car?.ImagePath,
            c.Car?.ChassisNumber,
            c.BranchId,
            c.Branch?.Name ?? string.Empty,
            c.CustomerId,
            c.Customer?.Name,
            c.Customer?.Mobile,
            c.Customer?.NationalId,
            c.UserId,
            c.User?.FullName,
            c.User?.Email,
            c.StartDate,
            c.EndDate,
            c.PeriodDays,
            c.PeriodMonths,
            totalWithTax,
            c.PaidPrice,
            c.RemainingPrice > 0 ? c.RemainingPrice : Math.Max(0m, totalWithTax - c.PaidPrice),
            c.MonthlyRent,
            c.DownPayment,
            c.MileageLimit,
            c.ExcessMileageFee,
            c.ActualEndMileage,
            c.ExcessMileageCost,
            c.InsurancePercentage,
            c.MaintenancePercentage,
            c.ReplacementCarPercentage,
            c.AdminFeesPercentage,
            c.AnnualDepreciationPercentage,
            c.CarPurchasePriceSnapshot,
            c.CarBookValueSnapshot,
            c.CarAccumulatedDepreciationSnapshot,
            c.CarProductiveAgeSnapshot,
            c.TotalEstimatedRevenue,
            c.TotalEstimatedCost,
            c.ExpectedProfit,
            c.MonthlyCompanyProfit,
            c.AdjustedMonthlyRent,
            c.DownPaymentMonthlyDiscount,
            isTax,
            taxPercent,
            monthlyTax,
            monthlyWithTax,
            totalTax,
            totalWithTax,
            c.Status,
            c.Notes,
            c.CreatedDate,
            c.YearlyBreakdownJson);
    }
}
