namespace ClinicAPI.Shared.Constants
{
  /// <summary>
  /// Permission constants for the application.
  /// 
  /// USAGE:
  /// - For authorization attributes: Use the *_KEY constants (e.g., PermissionKeysConstant.Roles.VIEW_ROLES_KEY)
  /// - For database seeding: Use the PermissionInfo objects (e.g., PermissionKeysConstant.Roles.VIEW_ROLES.Id)
  /// 
  /// Example:
  /// [PermissionAuthorize(PermissionKeysConstant.Roles.VIEW_ROLES_KEY)]  // ✅ Compile-time constant
  /// [PermissionAuthorize(PermissionKeysConstant.Roles.VIEW_ROLES.Key)]  // ❌ Runtime property access (won't work)
  /// </summary>
  public static class PermissionKeysConstant
  {
    public const string SUPER_ADMIN_PERMISSION = "super-admin";
    public const string CUSTOMER_SYSTEM_PERMISSION = "customer";

    // Permission structure to hold both Id and Key
    public class PermissionInfo
    {
      public int Id { get; set; }
      public string Key { get; set; }

      public PermissionInfo(int id, string key)
      {
        Id = id;
        Key = key;
      }
    }

    public static class SuperAdmin
    {
      public const string SUPER_ADMIN_KEY = "super-admin";
      public static readonly PermissionInfo SUPER_ADMIN = new(1, SUPER_ADMIN_KEY);
    }

    public static class Users
    {
      public const string VIEW_USERS_KEY = "view-users";
      public const string EDIT_USERS_KEY = "edit-users";
      public static readonly PermissionInfo VIEW_USERS = new(10, VIEW_USERS_KEY);
      public static readonly PermissionInfo EDIT_USERS = new(11, EDIT_USERS_KEY);
    }

    public static class Patients
    {
      public const string VIEW_PATIENTS_KEY = "view-patients";
      public const string EDIT_PATIENTS_KEY = "edit-patients";
      public static readonly PermissionInfo VIEW_PATIENTS = new(20, VIEW_PATIENTS_KEY);
      public static readonly PermissionInfo EDIT_PATIENTS = new(21, EDIT_PATIENTS_KEY);
    }

    public static class Roles
    {
      public const string VIEW_ROLES_KEY = "view-role";
      public const string EDIT_ROLES_KEY = "edit-roles";
      public static readonly PermissionInfo VIEW_ROLES = new(30, VIEW_ROLES_KEY);
      public static readonly PermissionInfo EDIT_ROLES = new(31, EDIT_ROLES_KEY);
    }

    public static class AcademicDegrees
    {
      public const string VIEW_ACADEMIC_DEGREES_KEY = "view-academic-degrees";
      public const string EDIT_ACADEMIC_DEGREES_KEY = "edit-academic-degrees";
      public static readonly PermissionInfo VIEW_ACADEMIC_DEGREES = new(40, VIEW_ACADEMIC_DEGREES_KEY);
      public static readonly PermissionInfo EDIT_ACADEMIC_DEGREES = new(41, EDIT_ACADEMIC_DEGREES_KEY);
    }

    public static class ExpenseTypes
    {
      public const string VIEW_EXPENSE_TYPES_KEY = "view-expense-types";
      public const string EDIT_EXPENSE_TYPES_KEY = "edit-expense-types";
      public static readonly PermissionInfo VIEW_EXPENSE_TYPES = new(50, VIEW_EXPENSE_TYPES_KEY);
      public static readonly PermissionInfo EDIT_EXPENSE_TYPES = new(51, EDIT_EXPENSE_TYPES_KEY);
    }

    public static class ExpenseCategories
    {
      public const string VIEW_EXPENSE_CATEGORIES_KEY = "view-expense-categories";
      public const string EDIT_EXPENSE_CATEGORIES_KEY = "edit-expense-categories";
      public static readonly PermissionInfo VIEW_EXPENSE_CATEGORIES = new(60, VIEW_EXPENSE_CATEGORIES_KEY);
      public static readonly PermissionInfo EDIT_EXPENSE_CATEGORIES = new(61, EDIT_EXPENSE_CATEGORIES_KEY);
    }



    public static class Nationalities
    {
      public const string VIEW_NATIONALITIES_KEY = "view-nationalities";
      public const string EDIT_NATIONALITIES_KEY = "edit-nationalities";
      public static readonly PermissionInfo VIEW_NATIONALITIES = new(70, VIEW_NATIONALITIES_KEY);
      public static readonly PermissionInfo EDIT_NATIONALITIES = new(71, EDIT_NATIONALITIES_KEY);
    }

    public static class LaserDeviceTypes
    {
      public const string VIEW_LASER_DEVICE_TYPES_KEY = "view-laser-device-types";
      public const string EDIT_LASER_DEVICE_TYPES_KEY = "edit-laser-device-types";
      public static readonly PermissionInfo VIEW_LASER_DEVICE_TYPES = new(80, VIEW_LASER_DEVICE_TYPES_KEY);
      public static readonly PermissionInfo EDIT_LASER_DEVICE_TYPES = new(81, EDIT_LASER_DEVICE_TYPES_KEY);
    }

    public static class LaserDeviceTypeModels
    {
      public const string VIEW_LASER_DEVICE_TYPE_MODELS_KEY = "view-laser-device-type-models";
      public const string EDIT_LASER_DEVICE_TYPE_MODELS_KEY = "edit-laser-device-type-models";
      public static readonly PermissionInfo VIEW_LASER_DEVICE_TYPE_MODELS = new(90, VIEW_LASER_DEVICE_TYPE_MODELS_KEY);
      public static readonly PermissionInfo EDIT_LASER_DEVICE_TYPE_MODELS = new(91, EDIT_LASER_DEVICE_TYPE_MODELS_KEY);
    }

    public static class ServiceCategoryTypes
    {
      public const string VIEW_SERVICE_CATEGORY_TYPES_KEY = "view-service-category-types";
      public const string EDIT_SERVICE_CATEGORY_TYPES_KEY = "edit-service-category-types";
      public static readonly PermissionInfo VIEW_SERVICE_CATEGORY_TYPES = new(100, VIEW_SERVICE_CATEGORY_TYPES_KEY);
      public static readonly PermissionInfo EDIT_SERVICE_CATEGORY_TYPES = new(101, EDIT_SERVICE_CATEGORY_TYPES_KEY);
    }

    public static class LaserDevices
    {
      public const string VIEW_LASER_DEVICES_KEY = "view-laser-devices";
      public const string EDIT_LASER_DEVICES_KEY = "edit-laser-devices";
      public static readonly PermissionInfo VIEW_LASER_DEVICES = new(110, VIEW_LASER_DEVICES_KEY);
      public static readonly PermissionInfo EDIT_LASER_DEVICES = new(111, EDIT_LASER_DEVICES_KEY);
    }

    public static class MedicalSupplies
    {
      public const string VIEW_MEDICAL_SUPPLIES_KEY = "view-medical-supplies";
      public const string EDIT_MEDICAL_SUPPLIES_KEY = "edit-medical-supplies";
      public static readonly PermissionInfo VIEW_MEDICAL_SUPPLIES = new(120, VIEW_MEDICAL_SUPPLIES_KEY);
      public static readonly PermissionInfo EDIT_MEDICAL_SUPPLIES = new(121, EDIT_MEDICAL_SUPPLIES_KEY);
    }

    public static class Services
    {
      public const string VIEW_SERVICES_KEY = "view-services";
      public const string EDIT_SERVICES_KEY = "edit-services";
      public static readonly PermissionInfo VIEW_SERVICES = new(130, VIEW_SERVICES_KEY);
      public static readonly PermissionInfo EDIT_SERVICES = new(131, EDIT_SERVICES_KEY);
    }



    public static class Coupons
    {
      public const string VIEW_COUPONS_KEY = "view-coupons";
      public const string EDIT_COUPONS_KEY = "edit-coupons";
      public static readonly PermissionInfo VIEW_COUPONS = new(150, VIEW_COUPONS_KEY);
      public static readonly PermissionInfo EDIT_COUPONS = new(151, EDIT_COUPONS_KEY);
    }

    public static class Appointments
    {
      public const string VIEW_APPOINTMENTS_KEY = "view-appointments";
      public const string EDIT_APPOINTMENTS_KEY = "edit-appointments";
      public static readonly PermissionInfo VIEW_APPOINTMENTS = new(160, VIEW_APPOINTMENTS_KEY);
      public static readonly PermissionInfo EDIT_APPOINTMENTS = new(161, EDIT_APPOINTMENTS_KEY);
    }

    public static class DoctorAppointmentActions
    {
      public const string VIEW_DOCTOR_APPOINTMENT_ACTION_KEY = "view-doctor-appointment-action";
      public const string EDIT_DOCTOR_APPOINTMENT_ACTION_KEY = "edit-doctor-appointment-action";
      public static readonly PermissionInfo VIEW_DOCTOR_APPOINTMENT_ACTION = new(162, VIEW_DOCTOR_APPOINTMENT_ACTION_KEY);
      public static readonly PermissionInfo EDIT_DOCTOR_APPOINTMENT_ACTION = new(163, EDIT_DOCTOR_APPOINTMENT_ACTION_KEY);
    }

    public static class DoctorServicePercentage
    {
      public const string VIEW_DOCTOR_SERVICE_PERCENTAGE_KEY = "view-doctor-service-percentage";
      public const string EDIT_DOCTOR_SERVICE_PERCENTAGE_KEY = "edit-doctor-service-percentage";
      public static readonly PermissionInfo VIEW_DOCTOR_SERVICE_PERCENTAGE = new(170, VIEW_DOCTOR_SERVICE_PERCENTAGE_KEY);
      public static readonly PermissionInfo EDIT_DOCTOR_SERVICE_PERCENTAGE = new(171, EDIT_DOCTOR_SERVICE_PERCENTAGE_KEY);
    }

    public static class PulsePricing
    {
      public const string VIEW_PULSE_PRICING_KEY = "view-pulse-pricing";
      public const string EDIT_PULSE_PRICING_KEY = "edit-pulse-pricing";
      public static readonly PermissionInfo VIEW_PULSE_PRICING = new(180, VIEW_PULSE_PRICING_KEY);
      public static readonly PermissionInfo EDIT_PULSE_PRICING = new(181, EDIT_PULSE_PRICING_KEY);
    }

    public static class Reports
    {
      public const string VIEW_REPORTS_KEY = "view-reports";
      public const string EDIT_REPORTS_KEY = "edit-reports";
      public const string EXPORT_PATIENTS_ANALYSIS_REPORT_KEY = "export-patients-analysis-report";
      public const string EXPORT_SERVICES_REPORT_KEY = "export-services-report";
      public static readonly PermissionInfo VIEW_REPORTS = new(190, VIEW_REPORTS_KEY);
      public static readonly PermissionInfo EDIT_REPORTS = new(191, EDIT_REPORTS_KEY);
      public static readonly PermissionInfo EXPORT_PATIENTS_ANALYSIS_REPORT = new(360, EXPORT_PATIENTS_ANALYSIS_REPORT_KEY);
      public static readonly PermissionInfo EXPORT_SERVICES_REPORT = new(361, EXPORT_SERVICES_REPORT_KEY);
    }

    public static class Complaints
    {
      public const string VIEW_COMPLAINTS_KEY = "view-complaints";
      public const string EDIT_COMPLAINTS_KEY = "edit-complaints";
      public const string EXPORT_COMPLAINTS_KEY = "export-complaints";
      public static readonly PermissionInfo VIEW_COMPLAINTS = new(200, VIEW_COMPLAINTS_KEY);
      public static readonly PermissionInfo EDIT_COMPLAINTS = new(201, EDIT_COMPLAINTS_KEY);
      public static readonly PermissionInfo EXPORT_COMPLAINTS = new(362, EXPORT_COMPLAINTS_KEY);
    }

    public static class Permissions
    {
      public const string VIEW_PERMISSIONS_KEY = "view-permissions";
      public const string EDIT_PERMISSIONS_KEY = "edit-permissions";
      public static readonly PermissionInfo VIEW_PERMISSIONS = new(210, VIEW_PERMISSIONS_KEY);
      public static readonly PermissionInfo EDIT_PERMISSIONS = new(211, EDIT_PERMISSIONS_KEY);
    }

    public static class Branches
    {
      public const string VIEW_BRANCHES_KEY = "view-branches";
      public const string EDIT_BRANCHES_KEY = "edit-branches";
      public static readonly PermissionInfo VIEW_BRANCHES = new(220, VIEW_BRANCHES_KEY);
      public static readonly PermissionInfo EDIT_BRANCHES = new(221, EDIT_BRANCHES_KEY);
    }
    public static class ServiceTypes
    {
      public const string VIEW_SERVICE_TYPES_KEY = "view-service-types";
      public const string EDIT_SERVICE_TYPES_KEY = "edit-service-types";
      public static readonly PermissionInfo VIEW_SERVICE_TYPES = new(222, VIEW_SERVICE_TYPES_KEY);
      public static readonly PermissionInfo EDIT_SERVICE_TYPES = new(223, EDIT_SERVICE_TYPES_KEY);
    }
    public static class KnowledgSource
    {
      public const string VIEW_KNOWLEDGE_SOURCE_KEY = "view-knowledge-source";
      public const string EDIT_KNOWLEDGE_SOURCE_KEY = "edit-knowledge-source";

      public static readonly PermissionInfo VIEW_KNOWLEDGE_SOURCE = new(224, VIEW_KNOWLEDGE_SOURCE_KEY);
      public static readonly PermissionInfo EDIT_KNOWLEDGE_SOURCE = new(225, EDIT_KNOWLEDGE_SOURCE_KEY);
    }
    public static class CanceledReasons
    {
      public const string VIEW_CANCELED_REASONS_KEY = "view-canceled-reasons";
      public const string EDIT_CANCELED_REASONS_KEY = "edit-canceled-reasons";
      public static readonly PermissionInfo VIEW_CANCELED_REASONS = new(226, VIEW_CANCELED_REASONS_KEY);
      public static readonly PermissionInfo EDIT_CANCELED_REASONS = new(227, EDIT_CANCELED_REASONS_KEY);
    }
    public static class PaymentMethods
    {
      public const string VIEW_PAYMENT_METHODS_KEY = "view-payment-methods";
      public const string EDIT_PAYMENT_METHODS_KEY = "edit-payment-methods";
      public static readonly PermissionInfo VIEW_PAYMENT_METHODS = new(228, VIEW_PAYMENT_METHODS_KEY);
      public static readonly PermissionInfo EDIT_PAYMENT_METHODS = new(229, EDIT_PAYMENT_METHODS_KEY);
    }
    public static class Banks
    {
      public const string VIEW_BANKS_KEY = "view-banks";
      public const string EDIT_BANKS_KEY = "edit-banks";
      public static readonly PermissionInfo VIEW_BANKS = new(230, VIEW_BANKS_KEY);
      public static readonly PermissionInfo EDIT_BANKS = new(231, EDIT_BANKS_KEY);
    }
    public static class LaserDeviceModels
    {
      public const string VIEW_LASER_DEVICE_MODELS_KEY = "view-laser-device-models";
      public const string EDIT_LASER_DEVICE_MODELS_KEY = "edit-laser-device-models";
      public static readonly PermissionInfo VIEW_LASER_DEVICE_MODELS = new(232, VIEW_LASER_DEVICE_MODELS_KEY);
      public static readonly PermissionInfo EDIT_LASER_DEVICE_MODELS = new(233, EDIT_LASER_DEVICE_MODELS_KEY);
    }
    public static class MaterialUnits
    {
      public const string VIEW_MATERIAL_UNITS_KEY = "view-material-units";
      public const string EDIT_MATERIAL_UNITS_KEY = "edit-material-units";
      public static readonly PermissionInfo VIEW_MATERIAL_UNITS = new(234, VIEW_MATERIAL_UNITS_KEY);
      public static readonly PermissionInfo EDIT_MATERIAL_UNITS = new(235, EDIT_MATERIAL_UNITS_KEY);
    }
    public static class BodyAreas
    {
      public const string VIEW_BODY_AREAS_KEY = "view-body-areas";
      public const string EDIT_BODY_AREAS_KEY = "edit-body-areas";
      public static readonly PermissionInfo VIEW_BODY_AREAS = new(236, VIEW_BODY_AREAS_KEY);
      public static readonly PermissionInfo EDIT_BODY_AREAS = new(237, EDIT_BODY_AREAS_KEY);
    }
    public static class ComplaintReasons
    {
      public const string VIEW_COMPLAINT_REASONS_KEY = "view-complaint-reasons";
      public const string EDIT_COMPLAINT_REASONS_KEY = "edit-complaint-reasons";
      public static readonly PermissionInfo VIEW_COMPLAINT_REASONS = new(238, VIEW_COMPLAINT_REASONS_KEY);
      public static readonly PermissionInfo EDIT_COMPLAINT_REASONS = new(239, EDIT_COMPLAINT_REASONS_KEY);
    }
    public static class ExternalMaterials
    {
      public const string VIEW_EXTERNAL_MATERIALS_KEY = "view-external-materials";
      public const string EDIT_EXTERNAL_MATERIALS_KEY = "edit-external-materials";
      public static readonly PermissionInfo VIEW_EXTERNAL_MATERIALS = new(240, VIEW_EXTERNAL_MATERIALS_KEY);
      public static readonly PermissionInfo EDIT_EXTERNAL_MATERIALS = new(241, EDIT_EXTERNAL_MATERIALS_KEY);
    }
    public static class ComplaintStatuses
    {
      public const string VIEW_COMPLAINT_STATUSES_KEY = "view-complaint-statuses";
      public const string EDIT_COMPLAINT_STATUSES_KEY = "edit-complaint-statuses";
      public static readonly PermissionInfo VIEW_COMPLAINT_STATUSES = new(242, VIEW_COMPLAINT_STATUSES_KEY);
      public static readonly PermissionInfo EDIT_COMPLAINT_STATUSES = new(243, EDIT_COMPLAINT_STATUSES_KEY);
    }
    public static class Packages
    {
      public const string VIEW_PACKAGES_KEY = "view-packages";
      public const string EDIT_PACKAGES_KEY = "edit-packages";
      public static readonly PermissionInfo VIEW_PACKAGES = new(244, VIEW_PACKAGES_KEY);
      public static readonly PermissionInfo EDIT_PACKAGES = new(245, EDIT_PACKAGES_KEY);
    }
    public static class MaterialShipments
    {
      public const string VIEW_MATERIAL_SHIPMENTS_KEY = "view-material-shipments";
      public const string EDIT_MATERIAL_SHIPMENTS_KEY = "edit-material-shipments";
      public static readonly PermissionInfo VIEW_MATERIAL_SHIPMENTS = new(246, VIEW_MATERIAL_SHIPMENTS_KEY);
      public static readonly PermissionInfo EDIT_MATERIAL_SHIPMENTS = new(247, EDIT_MATERIAL_SHIPMENTS_KEY);
    }




    public static class LaserDeviceDailyVisits
    {
      public const string VIEW_LASER_DEVICE_DAILY_VISITS_KEY = "view-laser-device-daily-visits";
      public const string EDIT_LASER_DEVICE_DAILY_VISITS_KEY = "edit-laser-device-daily-visits";
      public static readonly PermissionInfo VIEW_LASER_DEVICE_DAILY_VISITS = new(250, VIEW_LASER_DEVICE_DAILY_VISITS_KEY);
      public static readonly PermissionInfo EDIT_LASER_DEVICE_DAILY_VISITS = new(251, EDIT_LASER_DEVICE_DAILY_VISITS_KEY);
    }
    public static class DoctorProfits
    {
      public const string VIEW_DOCTOR_PROFITS_KEY = "view-doctor-profits";
      public static readonly PermissionInfo VIEW_DOCTOR_PROFITS = new(252, VIEW_DOCTOR_PROFITS_KEY);
    }
    public static class MaterialCategories
    {
      public const string VIEW_MATERIAL_CATEGORIES_KEY = "view-material-categories";
      public const string EDIT_MATERIAL_CATEGORIES_KEY = "edit-material-categories";
      public static readonly PermissionInfo VIEW_MATERIAL_CATEGORIES = new(253, VIEW_MATERIAL_CATEGORIES_KEY);
      public static readonly PermissionInfo EDIT_MATERIAL_CATEGORIES = new(254, EDIT_MATERIAL_CATEGORIES_KEY);
    }

    public static class SelledMaterials
    {
      public const string VIEW_SELLED_MATERIALS_KEY = "view-selled-materials";
      public const string EDIT_SELLED_MATERIALS_KEY = "edit-selled-materials";
      public static readonly PermissionInfo VIEW_SELLED_MATERIALS = new(255, VIEW_SELLED_MATERIALS_KEY);
      public static readonly PermissionInfo EDIT_SELLED_MATERIALS = new(256, EDIT_SELLED_MATERIALS_KEY);
    }
    public static class Expenses
    {
      public const string VIEW_EXPENSES_KEY = "view-expenses";
      public const string EDIT_EXPENSES_KEY = "edit-expenses";
      public static readonly PermissionInfo VIEW_EXPENSES = new(257, VIEW_EXPENSES_KEY);
      public static readonly PermissionInfo EDIT_EXPENSES = new(258, EDIT_EXPENSES_KEY);
    }
    public static class DailyUsedMaterials
    {

      public const string VIEW_DAILY_USED_MATERIALS_KEY = "view-daily-used-materials";
      public const string EDIT_DAILY_USED_MATERIALS_KEY = "edit-daily-used-materials";

      public static readonly PermissionInfo VIEW_DAILY_USED_MATERIALS = new(259, VIEW_DAILY_USED_MATERIALS_KEY);
      public static readonly PermissionInfo EDIT_DAILY_USED_MATERIALS = new(260, EDIT_DAILY_USED_MATERIALS_KEY);
    }

    // Financial Material Categories
    public static class FinancialMaterialCategories
    {
      public const string VIEW_FINANCIAL_MATERIAL_CATEGORIES_KEY = "view-financial-material-categories";
      public const string EDIT_FINANCIAL_MATERIAL_CATEGORIES_KEY = "edit-financial-material-categories";
      public static readonly PermissionInfo VIEW_FINANCIAL_MATERIAL_CATEGORIES = new(261, VIEW_FINANCIAL_MATERIAL_CATEGORIES_KEY);
      public static readonly PermissionInfo EDIT_FINANCIAL_MATERIAL_CATEGORIES = new(262, EDIT_FINANCIAL_MATERIAL_CATEGORIES_KEY);
    }

    // Financial Material Units
    public static class FinancialMaterialUnits
    {
      public const string VIEW_FINANCIAL_MATERIAL_UNITS_KEY = "view-financial-material-units";
      public const string EDIT_FINANCIAL_MATERIAL_UNITS_KEY = "edit-financial-material-units";
      public static readonly PermissionInfo VIEW_FINANCIAL_MATERIAL_UNITS = new(263, VIEW_FINANCIAL_MATERIAL_UNITS_KEY);
      public static readonly PermissionInfo EDIT_FINANCIAL_MATERIAL_UNITS = new(264, EDIT_FINANCIAL_MATERIAL_UNITS_KEY);
    }

    // Financial Expense Types
    public static class FinancialExpenseTypes
    {
      public const string VIEW_FINANCIAL_EXPENSE_TYPES_KEY = "view-financial-expense-types";
      public const string EDIT_FINANCIAL_EXPENSE_TYPES_KEY = "edit-financial-expense-types";
      public static readonly PermissionInfo VIEW_FINANCIAL_EXPENSE_TYPES = new(265, VIEW_FINANCIAL_EXPENSE_TYPES_KEY);
      public static readonly PermissionInfo EDIT_FINANCIAL_EXPENSE_TYPES = new(266, EDIT_FINANCIAL_EXPENSE_TYPES_KEY);
    }

    // Financial Client Types
    public static class FinancialClientTypes
    {
      public const string VIEW_FINANCIAL_CLIENT_TYPES_KEY = "view-financial-client-types";
      public const string EDIT_FINANCIAL_CLIENT_TYPES_KEY = "edit-financial-client-types";
      public static readonly PermissionInfo VIEW_FINANCIAL_CLIENT_TYPES = new(267, VIEW_FINANCIAL_CLIENT_TYPES_KEY);
      public static readonly PermissionInfo EDIT_FINANCIAL_CLIENT_TYPES = new(268, EDIT_FINANCIAL_CLIENT_TYPES_KEY);
    }

    // Finance Expenses
    public static class FinanceExpenses
    {
      public const string VIEW_FINANCE_EXPENSES_KEY = "view-finance-expenses";
      public const string EDIT_FINANCE_EXPENSES_KEY = "edit-finance-expenses";
      public static readonly PermissionInfo VIEW_FINANCE_EXPENSES = new(269, VIEW_FINANCE_EXPENSES_KEY);
      public static readonly PermissionInfo EDIT_FINANCE_EXPENSES = new(270, EDIT_FINANCE_EXPENSES_KEY);
    }

    // Clients
    public static class Clients
    {
      public const string VIEW_CLIENTS_KEY = "view-clients";
      public const string EDIT_CLIENTS_KEY = "edit-clients";
      public static readonly PermissionInfo VIEW_CLIENTS = new(271, VIEW_CLIENTS_KEY);
      public static readonly PermissionInfo EDIT_CLIENTS = new(272, EDIT_CLIENTS_KEY);
    }

    // Finance Materials
    public static class FinanceMaterials
    {
      public const string VIEW_FINANCE_MATERIALS_KEY = "view-finance-materials";
      public const string EDIT_FINANCE_MATERIALS_KEY = "edit-finance-materials";
      public static readonly PermissionInfo VIEW_FINANCE_MATERIALS = new(273, VIEW_FINANCE_MATERIALS_KEY);
      public static readonly PermissionInfo EDIT_FINANCE_MATERIALS = new(274, EDIT_FINANCE_MATERIALS_KEY);
    }

    // Finance Material Shipments
    public static class FinanceMaterialShipments
    {
      public const string VIEW_FINANCE_MATERIAL_SHIPMENTS_KEY = "view-finance-material-shipments";
      public const string EDIT_FINANCE_MATERIAL_SHIPMENTS_KEY = "edit-finance-material-shipments";
      public static readonly PermissionInfo VIEW_FINANCE_MATERIAL_SHIPMENTS = new(275, VIEW_FINANCE_MATERIAL_SHIPMENTS_KEY);
      public static readonly PermissionInfo EDIT_FINANCE_MATERIAL_SHIPMENTS = new(276, EDIT_FINANCE_MATERIAL_SHIPMENTS_KEY);
    }

    // Money Transfers
    public static class MoneyTransfers
    {
      public const string VIEW_MONEY_TRANSFERS_KEY = "view-money-transfers";
      public const string EDIT_MONEY_TRANSFERS_KEY = "edit-money-transfers";
      public static readonly PermissionInfo VIEW_MONEY_TRANSFERS = new(281, VIEW_MONEY_TRANSFERS_KEY);
      public static readonly PermissionInfo EDIT_MONEY_TRANSFERS = new(282, EDIT_MONEY_TRANSFERS_KEY);
    }

    // Branch Transfers (Transfer Between Branches)
    public static class BranchTransfers
    {
      public const string VIEW_BRANCH_TRANSFERS_KEY = "view-branch-transfers";
      public const string EDIT_BRANCH_TRANSFERS_KEY = "edit-branch-transfers";
      public static readonly PermissionInfo VIEW_BRANCH_TRANSFERS = new(277, VIEW_BRANCH_TRANSFERS_KEY);
      public static readonly PermissionInfo EDIT_BRANCH_TRANSFERS = new(278, EDIT_BRANCH_TRANSFERS_KEY);
    }

    // Main Warehouse Transfers (Transfer from Main Warehouse to Branches)
    public static class MainWarehouseTransfers
    {
      public const string VIEW_MAIN_WAREHOUSE_TRANSFERS_KEY = "view-main-warehouse-transfers";
      public const string EDIT_MAIN_WAREHOUSE_TRANSFERS_KEY = "edit-main-warehouse-transfers";
      public static readonly PermissionInfo VIEW_MAIN_WAREHOUSE_TRANSFERS = new(279, VIEW_MAIN_WAREHOUSE_TRANSFERS_KEY);
      public static readonly PermissionInfo EDIT_MAIN_WAREHOUSE_TRANSFERS = new(280, EDIT_MAIN_WAREHOUSE_TRANSFERS_KEY);
    }

    // HR - Attendance
    public static class Attendance
    {
      public const string VIEW_ATTENDANCE_KEY = "view-attendance";
      public const string EDIT_ATTENDANCE_KEY = "edit-attendance";
      public static readonly PermissionInfo VIEW_ATTENDANCE = new(320, VIEW_ATTENDANCE_KEY);
      public static readonly PermissionInfo EDIT_ATTENDANCE = new(321, EDIT_ATTENDANCE_KEY);
    }

    // HR - Rewards
    public static class Rewards
    {
      public const string VIEW_REWARDS_KEY = "view-rewards";
      public const string EDIT_REWARDS_KEY = "edit-rewards";
      public static readonly PermissionInfo VIEW_REWARDS = new(350, VIEW_REWARDS_KEY);
      public static readonly PermissionInfo EDIT_REWARDS = new(351, EDIT_REWARDS_KEY);
    }

    // HR - Deductions
    public static class Deductions
    {
      public const string VIEW_DEDUCTIONS_KEY = "view-deductions";
      public const string EDIT_DEDUCTIONS_KEY = "edit-deductions";
      public static readonly PermissionInfo VIEW_DEDUCTIONS = new(352, VIEW_DEDUCTIONS_KEY);
      public static readonly PermissionInfo EDIT_DEDUCTIONS = new(353, EDIT_DEDUCTIONS_KEY);
    }

    // HR - Advances
    public static class Advances
    {
      public const string VIEW_ADVANCES_KEY = "view-advances";
      public const string EDIT_ADVANCES_KEY = "edit-advances";
      public static readonly PermissionInfo VIEW_ADVANCES = new(354, VIEW_ADVANCES_KEY);
      public static readonly PermissionInfo EDIT_ADVANCES = new(355, EDIT_ADVANCES_KEY);
    }

    // HR - Holidays
    public static class Holidays
    {
      public const string VIEW_HOLIDAYS_KEY = "view-holidays";
      public const string EDIT_HOLIDAYS_KEY = "edit-holidays";
      public static readonly PermissionInfo VIEW_HOLIDAYS = new(356, VIEW_HOLIDAYS_KEY);
      public static readonly PermissionInfo EDIT_HOLIDAYS = new(357, EDIT_HOLIDAYS_KEY);
    }


    // HR - Working Hours
    public static class WorkingHours
    {
      public const string VIEW_WORKING_HOURS_KEY = "view-working-hours";
      public const string EDIT_WORKING_HOURS_KEY = "edit-working-hours";
      public static readonly PermissionInfo VIEW_WORKING_HOURS = new(358, VIEW_WORKING_HOURS_KEY);
      public static readonly PermissionInfo EDIT_WORKING_HOURS = new(359, EDIT_WORKING_HOURS_KEY);
    }

    // Material Usages in Sessions
    public static class MaterialUsagesInSessions
    {
      public const string VIEW_MATERIAL_USAGES_IN_SESSIONS_KEY = "view-material-usages-in-sessions";
      public static readonly PermissionInfo VIEW_MATERIAL_USAGES_IN_SESSIONS = new(293, VIEW_MATERIAL_USAGES_IN_SESSIONS_KEY);
    }

    // Treasury Accounts
    public static class TreasuryAccounts
    {
      public const string VIEW_TREASURY_ACCOUNTS_KEY = "view-treasury-accounts";
      public const string EDIT_TREASURY_ACCOUNTS_KEY = "edit-treasury-accounts";
      public static readonly PermissionInfo VIEW_TREASURY_ACCOUNTS = new(294, VIEW_TREASURY_ACCOUNTS_KEY);
      public static readonly PermissionInfo EDIT_TREASURY_ACCOUNTS = new(295, EDIT_TREASURY_ACCOUNTS_KEY);
    }

    // Material Items
    public static class MaterialItems
    {
      public const string VIEW_MATERIAL_ITEMS_KEY = "view-material-items";
      public const string EDIT_MATERIAL_ITEMS_KEY = "edit-material-items";
      public static readonly PermissionInfo VIEW_MATERIAL_ITEMS = new(296, VIEW_MATERIAL_ITEMS_KEY);
      public static readonly PermissionInfo EDIT_MATERIAL_ITEMS = new(297, EDIT_MATERIAL_ITEMS_KEY);
    }

    // Finance Material Issues
    public static class FinanceMaterialIssues
    {
      public const string VIEW_FINANCE_MATERIAL_ISSUES_KEY = "view-finance-material-issues";
      public const string EDIT_FINANCE_MATERIAL_ISSUES_KEY = "edit-finance-material-issues";
      public static readonly PermissionInfo VIEW_FINANCE_MATERIAL_ISSUES = new(298, VIEW_FINANCE_MATERIAL_ISSUES_KEY);
      public static readonly PermissionInfo EDIT_FINANCE_MATERIAL_ISSUES = new(299, EDIT_FINANCE_MATERIAL_ISSUES_KEY);
    }

    // HR - Asset Custody
    public static class AssetCustody
    {
      public const string VIEW_ASSET_CUSTODY_KEY = "view-asset-custody";
      public const string EDIT_ASSET_CUSTODY_KEY = "edit-asset-custody";
      public static readonly PermissionInfo VIEW_ASSET_CUSTODY = new(300, VIEW_ASSET_CUSTODY_KEY);
      public static readonly PermissionInfo EDIT_ASSET_CUSTODY = new(301, EDIT_ASSET_CUSTODY_KEY);
    }

    // HR - Official Holidays
    public static class OfficialHolidays
    {
      public const string VIEW_OFFICIAL_HOLIDAYS_KEY = "view-official-holidays";
      public const string EDIT_OFFICIAL_HOLIDAYS_KEY = "edit-official-holidays";
      public static readonly PermissionInfo VIEW_OFFICIAL_HOLIDAYS = new(303, VIEW_OFFICIAL_HOLIDAYS_KEY);
      public static readonly PermissionInfo EDIT_OFFICIAL_HOLIDAYS = new(304, EDIT_OFFICIAL_HOLIDAYS_KEY);
    }

    // HR - Paid Salaries
    public static class PaidSalaries
    {
      public const string VIEW_PAID_SALARIES_KEY = "view-paid-salaries";
      public const string EDIT_PAID_SALARIES_KEY = "edit-paid-salaries";
      public static readonly PermissionInfo VIEW_PAID_SALARIES = new(305, VIEW_PAID_SALARIES_KEY);
      public static readonly PermissionInfo EDIT_PAID_SALARIES = new(306, EDIT_PAID_SALARIES_KEY);
    }

    // HR - Vacation Balance
    public static class VacationBalance
    {
      public const string VIEW_VACATION_BALANCE_KEY = "view-vacation-balance";
      public const string EDIT_VACATION_BALANCE_KEY = "edit-vacation-balance";
      public static readonly PermissionInfo VIEW_VACATION_BALANCE = new(307, VIEW_VACATION_BALANCE_KEY);
      public static readonly PermissionInfo EDIT_VACATION_BALANCE = new(308, EDIT_VACATION_BALANCE_KEY);
    }

    // HR - Attendance - Edit Check-out User
    public static class AttendanceCheckoutUser
    {
      public const string EDIT_CHECKOUT_USER_KEY = "edit-checkout-user";
      public static readonly PermissionInfo EDIT_CHECKOUT_USER = new(312, EDIT_CHECKOUT_USER_KEY);
    }

    // HR - Role Vacation Policies
    public static class RoleVacationPolicies
    {
      public const string VIEW_ROLE_VACATION_POLICIES_KEY = "view-role-vacation-policies";
      public const string EDIT_ROLE_VACATION_POLICIES_KEY = "edit-role-vacation-policies";
      public static readonly PermissionInfo VIEW_ROLE_VACATION_POLICIES = new(370, VIEW_ROLE_VACATION_POLICIES_KEY);
      public static readonly PermissionInfo EDIT_ROLE_VACATION_POLICIES = new(371, EDIT_ROLE_VACATION_POLICIES_KEY);
    }

    // HR - Monthly Vacation Accrual
    public static class MonthlyVacationAccrual
    {
      public const string VIEW_MONTHLY_VACATION_ACCRUAL_KEY = "view-monthly-vacation-accrual";
      public const string CONFIRM_MONTHLY_VACATION_ACCRUAL_KEY = "confirm-monthly-vacation-accrual";
      public static readonly PermissionInfo VIEW_MONTHLY_VACATION_ACCRUAL = new(372, VIEW_MONTHLY_VACATION_ACCRUAL_KEY);
      public static readonly PermissionInfo CONFIRM_MONTHLY_VACATION_ACCRUAL = new(373, CONFIRM_MONTHLY_VACATION_ACCRUAL_KEY);
    }

    // HR - Annual Vacation Transactions
    public static class AnnualVacationTransactions
    {
      public const string VIEW_ANNUAL_VACATION_TRANSACTIONS_KEY = "view-annual-vacation-transactions";
      public const string MANAGE_ANNUAL_VACATION_TRANSACTIONS_KEY = "manage-annual-vacation-transactions";
      public static readonly PermissionInfo VIEW_ANNUAL_VACATION_TRANSACTIONS = new(374, VIEW_ANNUAL_VACATION_TRANSACTIONS_KEY);
      public static readonly PermissionInfo MANAGE_ANNUAL_VACATION_TRANSACTIONS = new(375, MANAGE_ANNUAL_VACATION_TRANSACTIONS_KEY);
    }
  }
}

