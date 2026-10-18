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
    IEntityTypeConfiguration<Car>
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
}
