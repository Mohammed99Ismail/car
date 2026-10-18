using CarShowroom.Domain.Common;

namespace CarShowroom.Domain.Entities;

/// <summary>نوع السيارة (Honda, Toyota ...)</summary>
public class CarMake : BaseEntity
{
    public required string Name { get; set; }
    public ICollection<CarModel> Models { get; set; } = [];
}

/// <summary>موديل السيارة (Accord, Camry ...)</summary>
public class CarModel : BaseEntity
{
    public int MakeId { get; set; }
    public CarMake Make { get; set; } = null!;
    public required string Name { get; set; }
}

/// <summary>انواع العقود — lookup ثابت للقراءة فقط</summary>
public class ContractType : BaseEntity
{
    public required string NameAr { get; set; }
    public required string NameEn { get; set; }
}

/// <summary>مواقع المعرض وفروعه</summary>
public class Branch : AuditedEntityBase
{
    public required string Name { get; set; }
    public required string Address { get; set; }
    public required string Mobile { get; set; }
}

/// <summary>العملاء (مستخدمو السيارات)</summary>
public class Customer : AuditedEntityBase
{
    public required string Name { get; set; }
    public required string NationalId { get; set; }
    public required string Mobile { get; set; }
    public required string Address { get; set; }
}

public class Car : AuditedEntityBase
{
    public int MakeId { get; set; }
    public CarMake Make { get; set; } = null!;

    public int ModelId { get; set; }
    public CarModel Model { get; set; } = null!;

    public int ManufactureYear { get; set; }
    public required string Color { get; set; }
    public required string PlateNumber { get; set; }

    public int BranchId { get; set; }
    public Branch Branch { get; set; } = null!;

    public DateTime PurchaseDate { get; set; }
    public decimal PurchasePrice { get; set; }

    /// <summary>Relative path under wwwroot, e.g. /uploads/cars/{guid}.jpg</summary>
    public string? ImagePath { get; set; }

    public required string ChassisNumber { get; set; }
    public required string EngineNumber { get; set; }
    public string? Notes { get; set; }
}
