using CarShowroom.Application.Common.Interfaces;
using CarShowroom.Application.Common.Security;
using CarShowroom.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CarShowroom.Infrastructure.Persistence;

public static class DbInitializer
{
    public static async Task InitializeAsync(
        ApplicationDbContext context,
        IPasswordHasher passwordHasher,
        ILogger logger,
        CancellationToken cancellationToken = default)
    {
        // 1. Seed Permissions
        var existingPermKeys = await context.Permissions
            .Select(p => p.Key)
            .ToHashSetAsync(cancellationToken);

        var newPermissions = PermissionKeysConstant.All
            .Where(p => !existingPermKeys.Contains(p.Key))
            .Select(p => new Permission
            {
                Id = p.Id,
                Key = p.Key,
                Module = p.Module,
                NameAr = p.NameAr,
                NameEn = p.NameEn
            })
            .ToList();

        if (newPermissions.Count > 0)
        {
            await context.Permissions.AddRangeAsync(newPermissions, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Seeded {Count} new permissions.", newPermissions.Count);
        }

        // 2. Seed System Roles
        var superAdminRole = await context.Roles.FirstOrDefaultAsync(r => r.Id == SystemRoles.SuperAdminId, cancellationToken);
        if (superAdminRole is null)
        {
            superAdminRole = new Role
            {
                Id = SystemRoles.SuperAdminId,
                NameAr = "مدير النظام",
                NameEn = "SuperAdmin",
                IsSystem = true,
                CreatedDate = DateTime.UtcNow
            };
            await context.Roles.AddAsync(superAdminRole, cancellationToken);
        }

        var userRole = await context.Roles.FirstOrDefaultAsync(r => r.Id == SystemRoles.UserId, cancellationToken);
        if (userRole is null)
        {
            userRole = new Role
            {
                Id = SystemRoles.UserId,
                NameAr = "مستخدم",
                NameEn = "User",
                IsSystem = true,
                CreatedDate = DateTime.UtcNow
            };
            await context.Roles.AddAsync(userRole, cancellationToken);
        }
        await context.SaveChangesAsync(cancellationToken);

        // 3. Ensure SuperAdmin role has all permissions
        var allPermIds = await context.Permissions.Select(p => p.Id).ToListAsync(cancellationToken);
        var existingRolePerms = await context.RolePermissions
            .Where(rp => rp.RoleId == SystemRoles.SuperAdminId)
            .Select(rp => rp.PermissionId)
            .ToHashSetAsync(cancellationToken);

        var missingPerms = allPermIds
            .Where(id => !existingRolePerms.Contains(id))
            .Select(id => new RolePermission { RoleId = SystemRoles.SuperAdminId, PermissionId = id })
            .ToList();

        if (missingPerms.Count > 0)
        {
            await context.RolePermissions.AddRangeAsync(missingPerms, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }

        // 4. Seed Default Admin User
        var adminUser = await context.Users
            .Include(u => u.UserRoles)
            .FirstOrDefaultAsync(u => u.Email == "admin@carshowroom.com", cancellationToken);

        if (adminUser is null)
        {
            adminUser = new User
            {
                FullName = "مدير النظام",
                Email = "admin@carshowroom.com",
                PasswordHash = passwordHasher.Hash("Admin@123456"),
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            };
            adminUser.UserRoles.Add(new UserRole { RoleId = SystemRoles.SuperAdminId });
            await context.Users.AddAsync(adminUser, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Seeded default SuperAdmin user: admin@carshowroom.com / Admin@123456");
        }

        // 5. Seed Contract Types (Lookup readonly without any CRUD)
        if (!await context.ContractTypes.AnyAsync(cancellationToken))
        {
            var contractTypes = new List<ContractType>
            {
                new() { Id = 1, NameAr = "إيجار خدمة", NameEn = "Service Rental" },
                new() { Id = 2, NameAr = "إيجار قصير الأجل أو حسب الطلب (سنة أو أقل)", NameEn = "Short-term / On-demand Rental" },
                new() { Id = 3, NameAr = "إيجار طويل الأجل (سنة فأكثر)", NameEn = "Long-term Rental" },
                new() { Id = 4, NameAr = "إيجار منتهي بالتمليك", NameEn = "Rent to Own / Lease to Own" }
            };
            await context.ContractTypes.AddRangeAsync(contractTypes, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Seeded contract types.");
        }

        // 6. Seed Car Makes & Models
        if (!await context.CarMakes.AnyAsync(cancellationToken))
        {
            var makes = new List<CarMake>
            {
                new()
                {
                    Name = "Honda",
                    Models = [new() { Name = "Accord" }, new() { Name = "Civic" }, new() { Name = "CR-V" }, new() { Name = "Pilot" }]
                },
                new()
                {
                    Name = "Toyota",
                    Models = [new() { Name = "Camry" }, new() { Name = "Corolla" }, new() { Name = "Land Cruiser" }, new() { Name = "RAV4" }, new() { Name = "Hilux" }]
                },
                new()
                {
                    Name = "Hyundai",
                    Models = [new() { Name = "Elantra" }, new() { Name = "Sonata" }, new() { Name = "Tucson" }, new() { Name = "Santa Fe" }]
                },
                new()
                {
                    Name = "Nissan",
                    Models = [new() { Name = "Altima" }, new() { Name = "Patrol" }, new() { Name = "Sunny" }, new() { Name = "X-Trail" }]
                },
                new()
                {
                    Name = "Mercedes-Benz",
                    Models = [new() { Name = "C-Class" }, new() { Name = "E-Class" }, new() { Name = "S-Class" }, new() { Name = "G-Class" }]
                },
                new()
                {
                    Name = "BMW",
                    Models = [new() { Name = "3 Series" }, new() { Name = "5 Series" }, new() { Name = "7 Series" }, new() { Name = "X5" }]
                },
                new()
                {
                    Name = "Ford",
                    Models = [new() { Name = "Taurus" }, new() { Name = "Explorer" }, new() { Name = "F-150" }]
                }
            };
            await context.CarMakes.AddRangeAsync(makes, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Seeded car makes and models.");
        }

        // 7. Seed Sample Branches
        if (!await context.Branches.AnyAsync(cancellationToken))
        {
            var branches = new List<Branch>
            {
                new() { Name = "فرع الرياض الرئيسي", Address = "الرياض - طريق الملك فهد", Mobile = "0501112233", CreatedDate = DateTime.UtcNow },
                new() { Name = "فرع جدة", Address = "جدة - طريق المدينة المنورة", Mobile = "0502223344", CreatedDate = DateTime.UtcNow },
                new() { Name = "فرع الدمام", Address = "الدمام - طريق الملك عبد العزيز", Mobile = "0503334455", CreatedDate = DateTime.UtcNow }
            };
            await context.Branches.AddRangeAsync(branches, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Seeded sample branches.");
        }

        // 8. Seed Sample Customers
        if (!await context.Customers.AnyAsync(cancellationToken))
        {
            var customers = new List<Customer>
            {
                new() { Name = "محمد إسماعيل القحطاني", Mobile = "0501234567", NationalId = "1087452391", Address = "الرياض - حي النخيل الغربي", CreatedDate = DateTime.UtcNow },
                new() { Name = "عبدالله بن خالد الشمري", Mobile = "0558765432", NationalId = "1045982173", Address = "جدة - حي الروضة", CreatedDate = DateTime.UtcNow },
                new() { Name = "سلطان فهد العتيبي", Mobile = "0541122334", NationalId = "1098234176", Address = "الدمام - حي الشاطئ الشرقي", CreatedDate = DateTime.UtcNow },
                new() { Name = "سارة أحمد الدوسري", Mobile = "0567788990", NationalId = "2048712390", Address = "الرياض - حي الملقا", CreatedDate = DateTime.UtcNow },
                new() { Name = "فيصل عبدالعزيز الغامدي", Mobile = "0533344556", NationalId = "1032890145", Address = "مكة المكرمة - حي العوالي", CreatedDate = DateTime.UtcNow }
            };
            await context.Customers.AddRangeAsync(customers, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Seeded sample customers.");
        }

        // 9. Seed Contract Terms Configuration
        if (!await context.ContractTermsConfigs.AnyAsync(cancellationToken))
        {
            var config = new ContractTermsConfig
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
            logger.LogInformation("Seeded default contract terms configuration.");
        }

        // 10. Seed Sample Cars matching examples if cars table is empty
        if (!await context.Cars.AnyAsync(cancellationToken))
        {
            var branch = await context.Branches.FirstOrDefaultAsync(cancellationToken);
            var toyotaCamry = await context.CarModels.Include(m => m.Make).FirstOrDefaultAsync(m => m.Name == "Camry", cancellationToken);
            var hondaAccord = await context.CarModels.Include(m => m.Make).FirstOrDefaultAsync(m => m.Name == "Accord", cancellationToken);

            if (branch is not null && toyotaCamry is not null && hondaAccord is not null)
            {
                var cars = new List<Car>
                {
                    // Example 1 Car: purchase date 8/31/2022, price 39,900 SAR, book value 1 SAR
                    new()
                    {
                        MakeId = toyotaCamry.MakeId,
                        ModelId = toyotaCamry.Id,
                        ManufactureYear = 2022,
                        Color = "أبيض لؤلؤي",
                        PlateNumber = "أ ب ج 1 2 3 4",
                        BranchId = branch.Id,
                        PurchaseDate = DateTime.SpecifyKind(new DateTime(2022, 8, 31), DateTimeKind.Utc),
                        PurchasePrice = 39900.00m,
                        BookValue = 1.00m,
                        ChassisNumber = "JTDBR32K123456789",
                        EngineNumber = "2AR-FE-987654321",
                        Notes = "سيارة مثال 1 — مشتراة 31/08/2022 وتجاوزت 4 سنوات (القيمة الدفترية = 1 ريال)",
                        CreatedDate = DateTime.UtcNow
                    },
                    // Example 2 Car: purchase date 4/13/2023, price 111,500 SAR, book value 13,288.36 SAR
                    new()
                    {
                        MakeId = hondaAccord.MakeId,
                        ModelId = hondaAccord.Id,
                        ManufactureYear = 2023,
                        Color = "فضي ميتاليك",
                        PlateNumber = "س ل م 2 0 2 6",
                        BranchId = branch.Id,
                        PurchaseDate = DateTime.SpecifyKind(new DateTime(2023, 4, 13), DateTimeKind.Utc),
                        PurchasePrice = 111500.00m,
                        BookValue = 13288.36m,
                        ChassisNumber = "1HGCV1F1234567890",
                        EngineNumber = "L15BE-123456789",
                        Notes = "سيارة مثال 2 — مشتراة 13/04/2023 (القيمة الدفترية = 13,288.36 ريال)",
                        CreatedDate = DateTime.UtcNow
                    }
                };
                await context.Cars.AddRangeAsync(cars, cancellationToken);
                await context.SaveChangesAsync(cancellationToken);
                logger.LogInformation("Seeded sample cars for contract calculation examples.");
            }
        }

        // 7. Seed System Settings
        var existingSettingKeys = await context.SystemSettings
            .Select(s => s.Key)
            .ToHashSetAsync(cancellationToken);

        var initialSettings = new List<SystemSetting>
        {
            new()
            {
                Key = "VAT_PERCENTAGE",
                NameAr = "ضريبة القيمة المضافة على عقود الإيجار",
                NameEn = "Rental Value Added Tax (VAT)",
                Value = "15",
                DataType = "Percentage",
                Category = "الضرائب والمالية",
                IsReadOnly = true,
                DescriptionAr = "نسبة ضريبة القيمة المضافة الإلزامية (15%) المطبقة على أسعار وقيم عقود الإيجار والخدمات الإيجارية.",
                DescriptionEn = "Mandatory 15% VAT rate applied on rental contracts, daily leasing, and rental services.",
                CreatedDate = DateTime.UtcNow
            }
        };

        var missingSettings = initialSettings.Where(s => !existingSettingKeys.Contains(s.Key)).ToList();
        if (missingSettings.Count > 0)
        {
            await context.SystemSettings.AddRangeAsync(missingSettings, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Seeded {Count} new system settings.", missingSettings.Count);
        }
    }
}

