/**
 * بيانات السيارة الكاملة وأصولها المالية للعرض
 */
export interface CarDto {
  /** المعرف الفريد للسيارة */
  id: number
  /** معرف الشركة المصنعة (الماركة) */
  makeId: number
  /** اسم ماركة السيارة */
  makeName: string
  /** معرف موديل السيارة */
  modelId: number
  /** اسم موديل السيارة */
  modelName: string
  /** سنة الصنع / الموديل */
  manufactureYear: number
  /** لون السيارة */
  color: string
  /** رقم لوحة السيارة */
  plateNumber: string
  /** معرف الفرع التابعة له السيارة */
  branchId: number
  /** اسم الفرع أو المعرض */
  branchName: string
  /** تاريخ شراء السيارة */
  purchaseDate: string
  /** القيمة الشرائية للسيارة */
  purchasePrice: number
  /** قيمة السيارة / القيمة الدفترية للسيارة (Book Value) */
  bookValue: number
  /** مجمع الإهلاك للسيارة */
  accumulatedDepreciation: number
  /** عمر السيارة / العمر الإنتاجي للسيارة (بالسنوات) */
  productiveAge: number
  /** مسار صورة السيارة */
  imagePath: string | null
  /** رقم الهيكل (الشاسيه) */
  chassisNumber: string
  /** رقم المحرك */
  engineNumber: string
  /** ملاحظات إضافية عن السيارة */
  notes: string | null
  /** تاريخ إضافة السيارة للنظام */
  createdDate: string
  /** معرف لون السيارة */
  colorId?: number | null
  /** كود اللون السداسي Hex */
  colorHex?: string | null
}

/**
 * بيانات إنشاء أو تعديل سيارة
 */
export interface SaveCarDto {
  /** معرف الشركة المصنعة (الماركة) */
  makeId: number
  /** معرف موديل السيارة */
  modelId: number
  /** سنة الصنع / الموديل */
  manufactureYear: number
  /** لون السيارة */
  color: string
  /** معرف لون السيارة */
  colorId?: number | null
  /** رقم لوحة السيارة */
  plateNumber: string
  /** معرف الفرع */
  branchId: number
  /** تاريخ شراء السيارة */
  purchaseDate: string
  /** القيمة الشرائية للسيارة */
  purchasePrice: number
  /** قيمة السيارة / القيمة الدفترية المحدثة للسيارة */
  bookValue?: number | null
  /** رقم الهيكل (الشاسيه) */
  chassisNumber: string
  /** رقم المحرك */
  engineNumber: string
  /** ملاحظات إضافية عن السيارة */
  notes?: string | null
}

/**
 * معايير البحث والفلترة في السيارات
 */
export interface CarQueryParameters {
  /** رقم الصفحة الحالية */
  pageIndex?: number
  /** عدد العناصر في الصفحة */
  pageSize?: number
  /** كلمة البحث */
  search?: string
  /** حقل الترتيب */
  sortBy?: string
  /** ترتيب تنازلي */
  isDescending?: boolean
  /** تصفية حسب الماركة */
  makeId?: number | null
  /** تصفية حسب الموديل */
  modelId?: number | null
  /** تصفية حسب الفرع */
  branchId?: number | null
  /** تصفية حسب سنة الصنع */
  manufactureYear?: number | null
  /** تصفية حسب لون السيارة */
  colorId?: number | null
}
