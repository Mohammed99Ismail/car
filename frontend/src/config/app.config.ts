/**
 * ==============================================================================
 * محرك إعدادات النظام والهوية البصرية (App Branding Engine)
 * ==============================================================================
 *
 * لتغيير اسم المعرض أو الشعار بسهولة تامة وبدون كتابة أي كود برمجي:
 * فقط افتح ملف: app.config.json
 *
 * 1. appName: اسم النظام في السايدبار وأعلى الصفحة.
 * 2. companyName: اسم المعرض/الشركة في ترويسة التقارير.
 * 3. subTitle: السجل التجاري / الوصف بالتقارير.
 * 4. logo: اسم صورة الشعار داخل مجلد src/assets (مثال: "logo.png" أو "my-logo.svg").
 *    فقط ضع صورتك داخل مجلد assets واكتب اسمها في ملف app.config.json!
 */

import rawConfig from './app.config.json'

// استيراد كافة ملفات الصور من مجلد assets تلقائياً لدعم التغيير الفوري بمجرد كتابة اسم أي صورة
const assetImages = import.meta.glob('../assets/**/*.{png,jpg,jpeg,svg,webp,ico,gif}', {
  eager: true,
  import: 'default',
}) as Record<string, string>

/**
 * دالة البحث وحل مسار الشعار تلقائياً من مجلد assets
 */
function resolveLogoPath(logoInput?: string): string {
  if (!logoInput || typeof logoInput !== 'string') {
    return findDefaultLogo()
  }

  const clean = logoInput
    .trim()
    .replace(/^[@~]\/assets\//, '')
    .replace(/^\/assets\//, '')
    .replace(/^\/+/, '')

  // 1. إذا كان رابطاً مباشراً أو Base64
  if (
    clean.startsWith('http://') ||
    clean.startsWith('https://') ||
    clean.startsWith('data:')
  ) {
    return clean
  }

  // 2. البحث الدقيق داخل مجلد assets
  for (const [key, resolvedUrl] of Object.entries(assetImages)) {
    // تطابق المسار النسبي (مثلاً: images/logo.png)
    if (key.endsWith('/' + clean) || key.endsWith(clean)) {
      return resolvedUrl
    }
    // تطابق اسم الملف فقط (مثلاً: logo.png)
    const filename = key.split('/').pop()
    if (filename && filename.toLowerCase() === clean.toLowerCase()) {
      return resolvedUrl
    }
  }

  // 3. مسار مباشر من مجلد public
  if (logoInput.startsWith('/')) {
    return logoInput
  }

  return findDefaultLogo()
}

function findDefaultLogo(): string {
  for (const [key, url] of Object.entries(assetImages)) {
    if (key.includes('logo.png')) return url
  }
  return Object.values(assetImages)[0] || ''
}

export interface AppBrandingConfig {
  appName: string
  companyName: string
  subTitle: string
  logoPath: string
  logoAlt: string
  constraints: {
    sidebarLogoMaxW: number
    sidebarLogoMaxH: number
    reportLogoMaxW: number
    reportLogoMaxH: number
    maxTitleChars: number
  }
}

export const appConfig: AppBrandingConfig = {
  appName: rawConfig.appName || 'إدارة معرض وتأجير السيارات',
  companyName: rawConfig.companyName || 'شركة معرض وتأجير السيارات',
  subTitle: rawConfig.subTitle || 'المملكة العربية السعودية • سجل تجاري معتمد • شبكة الفروع وخدمات الأسطول',
  logoPath: resolveLogoPath(rawConfig.logo),
  logoAlt: rawConfig.appName || 'شعار معرض وتأجير السيارات',
  constraints: {
    sidebarLogoMaxW: 42,
    sidebarLogoMaxH: 42,
    reportLogoMaxW: 160,
    reportLogoMaxH: 56,
    maxTitleChars: 50,
  },
}

export default appConfig
