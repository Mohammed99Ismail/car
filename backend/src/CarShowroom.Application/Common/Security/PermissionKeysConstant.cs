namespace CarShowroom.Application.Common.Security;

/// <summary>
/// Permission constants for the application (pattern adapted from Shared/Constants/PermissionKeysConstant).
///
/// USAGE:
/// - For authorization attributes: Use the *_KEY constants (e.g., PermissionKeysConstant.Cars.VIEW_KEY)
/// - For database seeding: Use <see cref="All"/> (PermissionInfo objects with stable Ids)
/// </summary>
public static class PermissionKeysConstant
{
    public const string ClaimType = "permissions";

    public sealed record PermissionInfo(int Id, string Key, string Module, string NameAr, string NameEn);

    public static class SuperAdmin
    {
        public const string SUPER_ADMIN_KEY = "super-admin";
        public static readonly PermissionInfo SUPER_ADMIN = new(1, SUPER_ADMIN_KEY, "system", "مدير النظام (كل الصلاحيات)", "Super admin (all permissions)");
    }

    public static class Users
    {
        public const string VIEW_KEY = "view-users";
        public const string CREATE_KEY = "create-users";
        public const string UPDATE_KEY = "update-users";
        public const string DELETE_KEY = "delete-users";
    }

    public static class Roles
    {
        public const string VIEW_KEY = "view-roles";
        public const string CREATE_KEY = "create-roles";
        public const string UPDATE_KEY = "update-roles";
        public const string DELETE_KEY = "delete-roles";
    }

    public static class Branches
    {
        public const string VIEW_KEY = "view-branches";
        public const string CREATE_KEY = "create-branches";
        public const string UPDATE_KEY = "update-branches";
        public const string DELETE_KEY = "delete-branches";
    }

    public static class Customers
    {
        public const string VIEW_KEY = "view-customers";
        public const string CREATE_KEY = "create-customers";
        public const string UPDATE_KEY = "update-customers";
        public const string DELETE_KEY = "delete-customers";
    }

    public static class Cars
    {
        public const string VIEW_KEY = "view-cars";
        public const string CREATE_KEY = "create-cars";
        public const string UPDATE_KEY = "update-cars";
        public const string DELETE_KEY = "delete-cars";
    }

    public static class ContractTypes
    {
        public const string VIEW_KEY = "view-contract-types";
    }

    /// <summary>All permissions with stable Ids — used for seeding the permissions table.</summary>
    public static readonly IReadOnlyList<PermissionInfo> All =
    [
        SuperAdmin.SUPER_ADMIN,
        .. Crud(10, "users", "المستخدمين", "Users", Users.VIEW_KEY, Users.CREATE_KEY, Users.UPDATE_KEY, Users.DELETE_KEY),
        .. Crud(20, "roles", "الأدوار", "Roles", Roles.VIEW_KEY, Roles.CREATE_KEY, Roles.UPDATE_KEY, Roles.DELETE_KEY),
        .. Crud(30, "branches", "الفروع", "Branches", Branches.VIEW_KEY, Branches.CREATE_KEY, Branches.UPDATE_KEY, Branches.DELETE_KEY),
        .. Crud(40, "customers", "العملاء", "Customers", Customers.VIEW_KEY, Customers.CREATE_KEY, Customers.UPDATE_KEY, Customers.DELETE_KEY),
        .. Crud(50, "cars", "السيارات", "Cars", Cars.VIEW_KEY, Cars.CREATE_KEY, Cars.UPDATE_KEY, Cars.DELETE_KEY),
        new(60, ContractTypes.VIEW_KEY, "contract-types", "عرض أنواع العقود", "View contract types"),
    ];

    private static PermissionInfo[] Crud(int baseId, string module, string ar, string en,
        string view, string create, string update, string delete) =>
    [
        new(baseId, view, module, $"عرض {ar}", $"View {en.ToLowerInvariant()}"),
        new(baseId + 1, create, module, $"إضافة {ar}", $"Create {en.ToLowerInvariant()}"),
        new(baseId + 2, update, module, $"تعديل {ar}", $"Update {en.ToLowerInvariant()}"),
        new(baseId + 3, delete, module, $"حذف {ar}", $"Delete {en.ToLowerInvariant()}"),
    ];
}

/// <summary>Custom JWT claim types (adapted from Shared/Constants/CustomClaimTypes).</summary>
public static class CustomClaimTypes
{
    public const string Id = "id";
    public const string Name = "name";
    public const string Email = "email";
    public const string RoleId = "roleId";
    public const string Permissions = PermissionKeysConstant.ClaimType;
}

/// <summary>Well-known seeded role ids.</summary>
public static class SystemRoles
{
    public const int SuperAdminId = 1;
    public const int UserId = 2;
}
