using CarShowroom.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarShowroom.Infrastructure.Persistence.Configurations;

public class ShowroomConfigurations :
    IEntityTypeConfiguration<CarMake>,
    IEntityTypeConfiguration<CarModel>,
    IEntityTypeConfiguration<ContractType>,
    IEntityTypeConfiguration<Branch>,
    IEntityTypeConfiguration<Customer>,
    IEntityTypeConfiguration<Car>,
    IEntityTypeConfiguration<ContractTermsConfig>,
    IEntityTypeConfiguration<Contract>,
    IEntityTypeConfiguration<SystemSetting>
{
    public void Configure(EntityTypeBuilder<CarMake> builder)
    {
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Name).HasMaxLength(100).IsRequired();
        builder.HasIndex(m => m.Name).IsUnique();

        builder.HasMany(m => m.Models)
            .WithOne(mo => mo.Make)
            .HasForeignKey(mo => mo.MakeId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    public void Configure(EntityTypeBuilder<CarModel> builder)
    {
        builder.HasKey(mo => mo.Id);
        builder.Property(mo => mo.Name).HasMaxLength(100).IsRequired();
        builder.HasIndex(mo => new { mo.MakeId, mo.Name }).IsUnique();
    }

    public void Configure(EntityTypeBuilder<ContractType> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.NameAr).HasMaxLength(150).IsRequired();
        builder.Property(c => c.NameEn).HasMaxLength(150).IsRequired();
    }

    public void Configure(EntityTypeBuilder<Branch> builder)
    {
        builder.HasKey(b => b.Id);
        builder.Property(b => b.Name).HasMaxLength(150).IsRequired();
        builder.Property(b => b.Address).HasMaxLength(300).IsRequired();
        builder.Property(b => b.Mobile).HasMaxLength(30).IsRequired();
        builder.HasIndex(b => b.Name);
    }

    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Name).HasMaxLength(150).IsRequired();
        builder.Property(c => c.NationalId).HasMaxLength(50).IsRequired();
        builder.HasIndex(c => c.NationalId).IsUnique();
        builder.Property(c => c.Mobile).HasMaxLength(30).IsRequired();
        builder.Property(c => c.Address).HasMaxLength(300).IsRequired();
        builder.HasIndex(c => c.Mobile);
    }

    public void Configure(EntityTypeBuilder<Car> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Color).HasMaxLength(50).IsRequired();
        builder.Property(c => c.PlateNumber).HasMaxLength(30).IsRequired();
        builder.HasIndex(c => c.PlateNumber).IsUnique();

        builder.Property(c => c.ChassisNumber).HasMaxLength(60).IsRequired();
        builder.HasIndex(c => c.ChassisNumber).IsUnique();

        builder.Property(c => c.EngineNumber).HasMaxLength(60).IsRequired();
        builder.HasIndex(c => c.EngineNumber).IsUnique();

        builder.Property(c => c.PurchasePrice).HasPrecision(18, 2);
        builder.Property(c => c.BookValue).HasPrecision(18, 2);
        builder.Property(c => c.ImagePath).HasMaxLength(500);
        builder.Property(c => c.Notes).HasMaxLength(1000);

        builder.HasOne(c => c.Make)
            .WithMany()
            .HasForeignKey(c => c.MakeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.Model)
            .WithMany()
            .HasForeignKey(c => c.ModelId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.Branch)
            .WithMany()
            .HasForeignKey(c => c.BranchId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    public void Configure(EntityTypeBuilder<ContractTermsConfig> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.InsurancePercentage).HasPrecision(5, 2);
        builder.Property(c => c.MaintenancePercentage).HasPrecision(5, 2);
        builder.Property(c => c.ReplacementCarPercentage).HasPrecision(5, 2);
        builder.Property(c => c.AdminFeesPercentage).HasPrecision(5, 2);
        builder.Property(c => c.AnnualDepreciationPercentage).HasPrecision(5, 2);
    }

    public void Configure(EntityTypeBuilder<Contract> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.ContractNumber).HasMaxLength(50).IsRequired();
        builder.HasIndex(c => c.ContractNumber).IsUnique();

        builder.Property(c => c.Status).HasMaxLength(30).IsRequired();
        builder.Property(c => c.Notes).HasMaxLength(2000);
        builder.Property(c => c.YearlyBreakdownJson).HasColumnType("text");

        // Precisions for financials
        builder.Property(c => c.PeriodMonths).HasPrecision(18, 2);
        builder.Property(c => c.TotalPrice).HasPrecision(18, 2);
        builder.Property(c => c.PaidPrice).HasPrecision(18, 2);
        builder.Property(c => c.RemainingPrice).HasPrecision(18, 2);
        builder.Property(c => c.MonthlyRent).HasPrecision(18, 2);
        builder.Property(c => c.DownPayment).HasPrecision(18, 2);
        builder.Property(c => c.ExcessMileageFee).HasPrecision(18, 2);
        builder.Property(c => c.ExcessMileageCost).HasPrecision(18, 2);

        builder.Property(c => c.InsurancePercentage).HasPrecision(5, 2);
        builder.Property(c => c.MaintenancePercentage).HasPrecision(5, 2);
        builder.Property(c => c.ReplacementCarPercentage).HasPrecision(5, 2);
        builder.Property(c => c.AdminFeesPercentage).HasPrecision(5, 2);
        builder.Property(c => c.AnnualDepreciationPercentage).HasPrecision(5, 2);

        builder.Property(c => c.CarPurchasePriceSnapshot).HasPrecision(18, 2);
        builder.Property(c => c.CarBookValueSnapshot).HasPrecision(18, 2);
        builder.Property(c => c.CarAccumulatedDepreciationSnapshot).HasPrecision(18, 2);
        builder.Property(c => c.CarProductiveAgeSnapshot).HasPrecision(18, 2);

        builder.Property(c => c.TotalEstimatedRevenue).HasPrecision(18, 2);
        builder.Property(c => c.TotalEstimatedCost).HasPrecision(18, 2);
        builder.Property(c => c.ExpectedProfit).HasPrecision(18, 2);
        builder.Property(c => c.MonthlyCompanyProfit).HasPrecision(18, 2);
        builder.Property(c => c.AdjustedMonthlyRent).HasPrecision(18, 2);
        builder.Property(c => c.DownPaymentMonthlyDiscount).HasPrecision(18, 2);

        // Tax specifics
        builder.Property(c => c.IsTaxApplied).HasDefaultValue(true);
        builder.Property(c => c.TaxPercentage).HasPrecision(5, 2);
        builder.Property(c => c.MonthlyTaxAmount).HasPrecision(18, 2);
        builder.Property(c => c.MonthlyRentWithTax).HasPrecision(18, 2);
        builder.Property(c => c.TotalTaxAmount).HasPrecision(18, 2);
        builder.Property(c => c.TotalPriceWithTax).HasPrecision(18, 2);

        builder.HasOne(c => c.ContractType)
            .WithMany()
            .HasForeignKey(c => c.ContractTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.Car)
            .WithMany()
            .HasForeignKey(c => c.CarId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.Branch)
            .WithMany()
            .HasForeignKey(c => c.BranchId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.Customer)
            .WithMany()
            .HasForeignKey(c => c.CustomerId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(c => c.User)
            .WithMany()
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.SetNull);
    }

    public void Configure(EntityTypeBuilder<SystemSetting> builder)
    {
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Key).HasMaxLength(100).IsRequired();
        builder.HasIndex(s => s.Key).IsUnique();
        builder.Property(s => s.NameAr).HasMaxLength(200).IsRequired();
        builder.Property(s => s.NameEn).HasMaxLength(200).IsRequired();
        builder.Property(s => s.Value).HasMaxLength(1000).IsRequired();
        builder.Property(s => s.DataType).HasMaxLength(50).IsRequired();
        builder.Property(s => s.Category).HasMaxLength(100).IsRequired();
        builder.Property(s => s.DescriptionAr).HasMaxLength(500);
        builder.Property(s => s.DescriptionEn).HasMaxLength(500);
    }
}

