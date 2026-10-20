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
    public static string ResetEmailBody(string name, string link) => BuildResetEmailHtml(name, link, IsArabic);

    private static string BuildResetEmailHtml(string name, string link, bool isArabic)
    {
        var dir = isArabic ? "rtl" : "ltr";
        var align = isArabic ? "right" : "left";
        var lang = isArabic ? "ar" : "en";
        var borderAlert = isArabic ? "border-right:4px solid #f59e0b;" : "border-left:4px solid #f59e0b;";

        var systemTitle = isArabic ? "نظام إدارة وتأجير السيارات" : "Car Showroom & Rental System";
        var subtitle = isArabic ? "بوابة الأمان والتحكم في الحساب" : "Account Security & Access Portal";
        var greeting = isArabic ? $"مرحباً {name}،" : $"Hello {name},";
        var instruction = isArabic 
            ? "تلقينا طلباً لإعادة تعيين كلمة المرور الخاصة بحسابك في النظام. يمكنك إتمام العملية عبر الضغط على الزر أدناه:"
            : "We received a request to reset your password. You can complete the process by clicking the button below:";
        var buttonText = isArabic ? "إعادة تعيين كلمة المرور" : "Reset Password";
        var validityTitle = isArabic ? "تنبيه الصلاحية" : "Validity Notice";
        var validityDesc = isArabic 
            ? "هذا الرابط مخصص لك وصالح للاستخدام لمدة 60 دقيقة فقط."
            : "This link is intended for you and valid for 60 minutes only.";
        var fallbackText = isArabic 
            ? "إذا كان الزر لا يعمل معك، يمكنك نسخ الرابط التالي ولصقه في متصفحك:"
            : "If the button doesn't work, copy and paste this URL into your browser:";
        var securityNote = isArabic 
            ? "إذا لم تقم بطلب هذا الإجراء، يمكنك تجاهل هذه الرسالة بأمان وسيبقى حسابك محمياً."
            : "If you didn't request this action, you can safely ignore this email and your account remains secure.";
        var rights = isArabic ? "جميع الحقوق محفوظة." : "All rights reserved.";

        return $$"""
<!DOCTYPE html>
<html lang="{{lang}}" dir="{{dir}}">
<head>
  <meta charset="utf-8">
  <meta name="viewport" content="width=device-width, initial-scale=1.0">
</head>
<body style="margin:0;padding:0;background-color:#f1f5f9;font-family:'Segoe UI',Roboto,-apple-system,BlinkMacSystemFont,Arial,sans-serif;color:#1e293b;direction:{{dir}};text-align:{{align}};">
  <table role="presentation" width="100%" cellspacing="0" cellpadding="0" style="background-color:#f1f5f9;padding:40px 15px;">
    <tr>
      <td align="center">
        <table role="presentation" width="100%" style="max-width:580px;background:#ffffff;border-radius:16px;overflow:hidden;box-shadow:0 10px 25px -5px rgba(0,0,0,0.06), 0 8px 10px -6px rgba(0,0,0,0.06);border:1px solid #e2e8f0;" cellspacing="0" cellpadding="0">
          <!-- Header Banner -->
          <tr>
            <td style="background:linear-gradient(135deg, #1e3a8a 0%, #2563eb 100%);padding:36px 30px;text-align:center;">
              <div style="font-size:38px;line-height:1;margin-bottom:10px;">🚗</div>
              <h1 style="margin:0;color:#ffffff;font-size:22px;font-weight:700;letter-spacing:0.3px;">{{systemTitle}}</h1>
              <p style="margin:6px 0 0;color:#bfdbfe;font-size:13px;">{{subtitle}}</p>
            </td>
          </tr>
          <!-- Body Content -->
          <tr>
            <td style="padding:36px 32px;">
              <h2 style="margin:0 0 16px;color:#0f172a;font-size:18px;font-weight:600;">{{greeting}}</h2>
              <p style="margin:0 0 24px;color:#475569;font-size:15px;line-height:1.7;">{{instruction}}</p>
              
              <!-- CTA Button -->
              <div style="text-align:center;margin:32px 0;">
                <a href="{{link}}" style="display:inline-block;background:linear-gradient(135deg, #2563eb 0%, #1d4ed8 100%);color:#ffffff;text-decoration:none;font-weight:700;font-size:16px;padding:14px 38px;border-radius:10px;box-shadow:0 4px 14px rgba(37,99,235,0.4);letter-spacing:0.3px;">
                  {{buttonText}}
                </a>
              </div>

              <!-- Alert Box -->
              <div style="background-color:#f8fafc;{{borderAlert}}border-radius:8px;padding:14px 18px;margin:28px 0;color:#475569;font-size:13px;line-height:1.6;">
                <strong style="color:#b45309;">⏱️ {{validityTitle}}:</strong> {{validityDesc}}
              </div>

              <hr style="border:none;border-top:1px solid #f1f5f9;margin:24px 0;" />

              <!-- Fallback Link -->
              <p style="margin:0 0 8px;color:#94a3b8;font-size:12px;line-height:1.5;">{{fallbackText}}</p>
              <p style="margin:0;word-break:break-all;font-size:12px;">
                <a href="{{link}}" style="color:#2563eb;text-decoration:underline;">{{link}}</a>
              </p>
            </td>
          </tr>
          <!-- Footer -->
          <tr>
            <td style="background-color:#f8fafc;padding:22px 32px;text-align:center;border-top:1px solid #e2e8f0;font-size:12px;color:#94a3b8;">
              <p style="margin:0 0 6px;">{{securityNote}}</p>
              <p style="margin:0;">© 2026 Car Showroom System. {{rights}}</p>
            </td>
          </tr>
        </table>
      </td>
    </tr>
  </table>
</body>
</html>
""";
    }

    // Roles / users
    public static string SystemRoleProtected => T("لا يمكن حذف أو تعديل صلاحيات دور النظام", "System roles cannot be deleted or have their permissions changed");
    public static string CannotModifySelf => T("لا يمكنك تعطيل حسابك أو تغيير أدوارك بنفسك", "You cannot disable your own account or change your own roles");
    public static string InvalidRoles => T("دور أو أكثر غير موجود", "One or more roles do not exist");
    public static string InvalidPermissions => T("صلاحية أو أكثر غير موجودة", "One or more permissions do not exist");

    // Cars
    public static string ModelNotOfMake => T("الموديل المختار لا يتبع نوع السيارة", "The selected model does not belong to the selected make");
    public static string InvalidImage => T("الصورة يجب أن تكون jpg أو png أو webp وبحجم أقصى 5 ميجابايت", "Image must be jpg, png or webp and at most 5 MB");

    // Profile
    public static string InvalidCurrentPassword => T("كلمة المرور الحالية غير صحيحة", "Current password is incorrect");
    public static string PasswordChanged => T("تم تغيير كلمة المرور بنجاح", "Password changed successfully");
    public static string ProfileUpdated => T("تم تحديث البيانات الشخصية بنجاح", "Profile updated successfully");
    public static string AttachmentUploaded => T("تم رفع الملف بنجاح", "Attachment uploaded successfully");
    public static string InvalidDocument => T("الملف غير صالح أو يتجاوز الحجم المسموح (10 ميجابايت)", "Invalid file or exceeds maximum size (10 MB)");
}
