/**
 * بيانات إعداد النظام المرجعي
 */
export interface SystemSettingDto {
  /** المعرف الفريد للإعداد */
  id: number
  /** الرمز البرمجي الفريد للإعداد (مثال: VAT_PERCENTAGE) */
  key: string
  /** اسم الإعداد باللغة العربية */
  nameAr: string
  /** اسم الإعداد باللغة الإنجليزية */
  nameEn: string
  /** قيمة الإعداد */
  value: string
  /** نوع البيانات (Percentage, Number, Text, Boolean, Currency) */
  dataType: string
  /** فئة / تصنيف الإعداد */
  category: string
  /** هل الإعداد للقراءة فقط؟ */
  isReadOnly: boolean
  /** وصف الإعداد باللغة العربية */
  descriptionAr: string | null
  /** وصف الإعداد باللغة الإنجليزية */
  descriptionEn: string | null
  /** تاريخ آخر تعديل */
  lastModifiedDate: string | null
  /** تاريخ إنشاء الإعداد */
  createdDate: string
}

/**
 * طلب تعديل قيمة إعداد من إعدادات النظام
 */
export interface UpdateSystemSettingRequest {
  value: string
}
