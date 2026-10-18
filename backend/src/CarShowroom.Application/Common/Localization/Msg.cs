using System.Globalization;

namespace CarShowroom.Application.Common.Localization;

/// <summary>
/// Bilingual (ar/en) user-facing messages. The culture is resolved per request
/// from the Accept-Language header by the RequestLocalization middleware.
/// </summary>
public static class Msg
{
    private static bool IsArabic => CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "ar";

    private static string T(string ar, string en) => IsArabic ? ar : en;

    // General
    public static string Saved => T("تم الحفظ بنجاح", "Saved successfully");
    public static string Deleted => T("تم الحذف بنجاح", "Deleted successfully");
    public static string ValidationFailed => T("البيانات المدخلة غير صحيحة", "One or more validation errors occurred");
    public static string UnexpectedError => T("حدث خطأ غير متوقع", "An unexpected error occurred");
    public static string Forbidden => T("ليس لديك صلاحية لتنفيذ هذا الإجراء", "You do not have permission to perform this action");
    public static string Unauthenticated => T("يجب تسجيل الدخول", "Authentication is required");
    public static string TooManyRequests => T("محاولات كثيرة، حاول لاحقاً", "Too many requests, please try again later");
    public static string NotFound(string entity) => T($"{entity} غير موجود", $"{entity} was not found");
    public static string AlreadyExists(string field) => T($"{field} مستخدم من قبل", $"{field} is already in use");
    public static string InUse(string entity) => T($"لا يمكن حذف {entity} لارتباطه ببيانات أخرى", $"{entity} cannot be deleted because it is in use");

    // Entities
    public static string Car => T("السيارة", "Car");
    public static string Customer => T("العميل", "Customer");
    public static string Branch => T("الفرع", "Branch");
    public static string User => T("المستخدم", "User");
    public static string Role => T("الدور", "Role");
    public static string CarMake => T("نوع السيارة", "Car make");
    public static string CarModel => T("موديل السيارة", "Car model");
    public static string PlateNumber => T("رقم اللوحة", "Plate number");
    public static string ChassisNumber => T("رقم الشاسيه", "Chassis number");
    public static string EngineNumber => T("رقم المحرك", "Engine number");
    public static string NationalId => T("رقم الهوية", "National ID");
    public static string Email => T("البريد الإلكتروني", "Email");
    public static string RoleName => T("اسم الدور", "Role name");

    // Auth
    public static string InvalidCredentials => T("البريد الإلكتروني أو كلمة المرور غير صحيحة", "Invalid email or password");
    public static string AccountDisabled => T("الحساب معطل", "Account is disabled");
    public static string InvalidRefreshToken => T("جلسة الدخول منتهية، يرجى تسجيل الدخول مجدداً", "Session expired, please sign in again");
    public static string Registered => T("تم إنشاء الحساب بنجاح", "Account created successfully");
    public static string LoggedOut => T("تم تسجيل الخروج", "Logged out");
    public static string ResetLinkSent => T("إذا كان البريد مسجلاً فستصلك رسالة لإعادة تعيين كلمة المرور", "If the email is registered, a reset link has been sent");
    public static string InvalidResetToken => T("رابط إعادة التعيين غير صالح أو منتهي", "The reset link is invalid or expired");
    public static string PasswordReset => T("تم تغيير كلمة المرور بنجاح", "Password has been reset successfully");
    public static string ResetEmailSubject => T("إعادة تعيين كلمة المرور", "Reset your password");
    public static string ResetEmailBody(string name, string link) => T(
        $"<p>مرحباً {name}،</p><p>لإعادة تعيين كلمة المرور اضغط على الرابط التالي (صالح لمدة ساعة):</p><p><a href=\"{link}\">{link}</a></p>",
        $"<p>Hello {name},</p><p>Click the link below to reset your password (valid for one hour):</p><p><a href=\"{link}\">{link}</a></p>");

    // Roles / users
    public static string SystemRoleProtected => T("لا يمكن حذف أو تعديل صلاحيات دور النظام", "System roles cannot be deleted or have their permissions changed");
    public static string CannotModifySelf => T("لا يمكنك تعطيل حسابك أو تغيير أدوارك بنفسك", "You cannot disable your own account or change your own roles");
    public static string InvalidRoles => T("دور أو أكثر غير موجود", "One or more roles do not exist");
    public static string InvalidPermissions => T("صلاحية أو أكثر غير موجودة", "One or more permissions do not exist");

    // Cars
    public static string ModelNotOfMake => T("الموديل المختار لا يتبع نوع السيارة", "The selected model does not belong to the selected make");
    public static string InvalidImage => T("الصورة يجب أن تكون jpg أو png أو webp وبحجم أقصى 5 ميجابايت", "Image must be jpg, png or webp and at most 5 MB");
}
