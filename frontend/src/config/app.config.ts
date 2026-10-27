/**
 * ==============================================================================
 * محرك إعدادات النظام والهوية البصرية المعرب والمترجم (App Branding Engine)
 * ==============================================================================
 *
 * لتغيير اسم المعرض أو الشعار أو العناوين بالعربية والإنجليزية بسهولة تامة:
 * فقط افتح ملف: app.config.json
 *
 * يدعم كلاً من التنسيقين:
 * 1. تنسيق الكائن ثنائي اللغة (الأفضل والأشمل):
 *    "appName": { "ar": "اسم بالعربي", "en": "Name in English" }
 * 2. التنسيق المنفصل:
 *    "appNameAr": "...", "appNameEn": "..."
 * 3. التنسيق البسيط (قيمة واحدة لكلا اللغتين):
 *    "appName": "..."
 */

import { reactive } from 'vue'
import rawConfig from './app.config.json'
import i18n from '~/modules/i18n'

// استيراد كافة ملفات الصور من مجلد assets تلقائياً لدعم التغيير الفوري بمجرد كتابة اسم أي صورة
const assetImages = import.meta.glob('../assets/**/*.{png,jpg,jpeg,svg,webp,ico,gif}', {
  eager: true,
  import: 'default',
}) as Record<string, string>

/**
 * استخراج اللغة الحالية النشطة
 */
function getCurrentLocale(): string {
  try {
    if (i18n?.global?.locale) {
      const loc = i18n.global.locale
      const val = typeof loc === 'object' && 'value' in loc ? (loc as any).value : loc
      if (typeof val === 'string') return val
    }
  } catch {}

  if (typeof localStorage !== 'undefined') {
    try {
      const stored = localStorage.getItem('layout')
      if (stored) {
        const parsed = JSON.parse(stored)
        if (parsed?.activeLanguage) return parsed.activeLanguage
      }
    } catch {}
  }

  return 'ar'
}

/**
 * دالة مساعدة لاستخراج النص حسب اللغة المحددة مع دعم جميع الهياكل
 */
function resolveLocalized(
  value: any,
  lang?: string,
  fallbackAr?: string,
  fallbackEn?: string,
  defaultAr = '',
  defaultEn = '',
): string {
  const targetLang = (lang || getCurrentLocale()).toLowerCase()
  const isEn = targetLang.startsWith('en')

  // 1. إذا كانت القيمة كائناً يحتوي على ar و en
  if (value && typeof value === 'object' && !Array.isArray(value)) {
    if (isEn && value.en) return String(value.en)
    if (!isEn && value.ar) return String(value.ar)
    return String(value.ar || value.en || (isEn ? defaultEn : defaultAr))
  }

  // 2. إذا كانت هناك حقول مسطحة منفصلة (Ar و En)
  if (isEn && fallbackEn) return String(fallbackEn)
  if (!isEn && fallbackAr) return String(fallbackAr)

  // 3. إذا كانت القيمة نصاً مفرداً
  if (typeof value === 'string' && value.trim()) {
    return value
  }

  // 4. القيمة الافتراضية
  return isEn ? (defaultEn || defaultAr) : (defaultAr || defaultEn)
}

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
    if (key.endsWith('/' + clean) || key.endsWith(clean)) {
      return resolvedUrl
    }
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
  appNameAr: string
  appNameEn: string
  companyNameAr: string
  companyNameEn: string
  subTitleAr: string
  subTitleEn: string
  getAppName: (lang?: string) => string
  getCompanyName: (lang?: string) => string
  getSubTitle: (lang?: string) => string
  constraints: {
    sidebarLogoMaxW: number
    sidebarLogoMaxH: number
    reportLogoMaxW: number
    reportLogoMaxH: number
    maxTitleChars: number
  }
}

const raw: any = rawConfig

export const appConfig: AppBrandingConfig = reactive({
  appNameAr: resolveLocalized(raw.appName, 'ar', raw.appNameAr, raw.appNameEn, 'إدارة معرض وتأجير السيارات', 'Car Showroom & Rental Management'),
  appNameEn: resolveLocalized(raw.appName, 'en', raw.appNameAr, raw.appNameEn, 'إدارة معرض وتأجير السيارات', 'Car Showroom & Rental Management'),

  companyNameAr: resolveLocalized(raw.companyName, 'ar', raw.companyNameAr, raw.companyNameEn, 'شركة معرض وتأجير السيارات', 'Car Showroom & Rental Company'),
  companyNameEn: resolveLocalized(raw.companyName, 'en', raw.companyNameAr, raw.companyNameEn, 'شركة معرض وتأجير السيارات', 'Car Showroom & Rental Company'),

  subTitleAr: resolveLocalized(raw.subTitle, 'ar', raw.subTitleAr, raw.subTitleEn, 'المملكة العربية السعودية • سجل تجاري معتمد • شبكة الفروع وخدمات الأسطول', 'Kingdom of Saudi Arabia • Certified Commercial Registration • Fleet & Branch Network'),
  subTitleEn: resolveLocalized(raw.subTitle, 'en', raw.subTitleAr, raw.subTitleEn, 'المملكة العربية السعودية • سجل تجاري معتمد • شبكة الفروع وخدمات الأسطول', 'Kingdom of Saudi Arabia • Certified Commercial Registration • Fleet & Branch Network'),

  getAppName(lang?: string) {
    return resolveLocalized(raw.appName, lang, raw.appNameAr, raw.appNameEn, this.appNameAr, this.appNameEn)
  },
  getCompanyName(lang?: string) {
    return resolveLocalized(raw.companyName, lang, raw.companyNameAr, raw.companyNameEn, this.companyNameAr, this.companyNameEn)
  },
  getSubTitle(lang?: string) {
    return resolveLocalized(raw.subTitle, lang, raw.subTitleAr, raw.subTitleEn, this.subTitleAr, this.subTitleEn)
  },

  get appName() {
    return this.getAppName()
  },
  get companyName() {
    return this.getCompanyName()
  },
  get subTitle() {
    return this.getSubTitle()
  },
  get logoAlt() {
    return this.appName
  },

  logoPath: resolveLogoPath(raw.logo),
  constraints: {
    sidebarLogoMaxW: 42,
    sidebarLogoMaxH: 42,
    reportLogoMaxW: 160,
    reportLogoMaxH: 56,
    maxTitleChars: 50,
  },
})

export default appConfig
