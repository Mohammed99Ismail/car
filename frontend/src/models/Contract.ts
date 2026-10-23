/**
/// بيانات العقد الكاملة للعرض
 */
export interface ContractDto {
  /** المعرف الفريد للعقد */
  id: number
  /** رقم العقد المرجعي */
  contractNumber: string
  /** معرف نوع العقد */
  contractTypeId: number
  /** اسم نوع العقد باللغة العربية */
  contractTypeNameAr: string
  /** اسم نوع العقد باللغة الإنجليزية */
  contractTypeNameEn: string
  /** معرف السيارة موضوع العقد */
  carId: number
  /** ماركة السيارة (الشركة المصنعة) */
  carMakeName: string
  /** موديل السيارة */
  carModelName: string
  /** رقم لوحة السيارة */
  carPlateNumber: string
  /** سنة الصنع / الموديل */
  carManufactureYear: number
  /** لون السيارة */
  carColor: string
  /** مسار صورة السيارة */
  carImagePath: string | null
  /** رقم الشاسيه (الهيكل) للسيارة */
  carChassisNumber?: string | null
  /** معرف فرع المعرض */
  branchId: number
  /** اسم فرع المعرض */
  branchName: string
  /** معرف العميل المستفيد */
  customerId: number | null
  /** اسم العميل المستفيد */
  customerName: string | null
  /** رقم جوال العميل */
  customerMobile: string | null
  /** رقم الهوية الوطنية / الإقامة للعميل */
  customerNationalId: string | null
  /** معرف الموظف المستفيد (لعقود الخدمة) */
  userId: number | null
  /** اسم الموظف المستفيد */
  userFullName: string | null
  /** البريد الإلكتروني للموظف */
  userEmail: string | null
  /** تاريخ بداية العقد */
  startDate: string
  /** تاريخ نهاية العقد */
  endDate: string
  /** أيام الإيجار المستحق / مدة العقد بالأيام */
  periodDays: number
  /** المدة المحسوبة بالشهور */
  periodMonths: number
  /** إجمالي قيمة العقد / الإيرادات التقديرية حسب مدة العقد */
  totalPrice: number
  /** المبلغ المدفوع من قيمة العقد */
  paidPrice: number
  /** المبلغ المتبقي المستحق من العقد */
  remainingPrice: number
  /** قيمة قسط الإيجار التقديري المقترح قبل الدفعة المقدمة (شهرياً) */
  monthlyRent: number | null
  /** قيمة الدفعة المقدمة */
  downPayment: number | null
  /** الحد الأقصى للمسافة المسموحة (كم) */
  mileageLimit: number | null
  /** رسوم كل كيلومتر زائد عن الحد */
  excessMileageFee: number | null
  /** قراءة العداد الفعلية عند نهاية العقد */
  actualEndMileage: number | null
  /** تكلفة المسافة الزائدة الإجمالية */
  excessMileageCost: number | null
  /** نسبة تقدير تأمين شامل للسيارة المعتمدة في العقد (%) */
  insurancePercentage: number | null
  /** نسبة تقدير تكلفة الصيانة خلال فترة العقد المعتمدة في العقد (%) */
  maintenancePercentage: number | null
  /** نسبة تقدير تكلفة سيارة بديلة خلال فترة العقد المعتمدة في العقد (%) */
  replacementCarPercentage: number | null
  /** نسبة تقدير تكلفة المصاريف الإدارية خلال فترة العقد المعتمدة في العقد (%) */
  adminFeesPercentage: number | null
  /** نسبة الإهلاك السنوي المعتمدة في العقد (%) */
  annualDepreciationPercentage: number | null
  /** لقطة القيمة الشرائية للسيارة وقت إنشاء العقد */
  carPurchasePriceSnapshot: number
  /** لقطة قيمة السيارة / القيمة الدفترية للسيارة وقت إنشاء العقد */
  carBookValueSnapshot: number
  /** لقطة مجمع إهلاك السيارة وقت إنشاء العقد */
  carAccumulatedDepreciationSnapshot: number
  /** لقطة عمر السيارة الإنتاجي بالسنوات وقت إنشاء العقد */
  carProductiveAgeSnapshot: number
  /** إجمالي الإيرادات التقديرية حسب مدة العقد */
  totalEstimatedRevenue: number
  /** إجمالي التكلفة التقديرية (تشمل الإهلاك والتأمين والصيانة والبديلة والإدارية) */
  totalEstimatedCost: number
  /** إجمالي الأرباح التقديرية حسب مدة العقد */
  expectedProfit: number
  /** قيمة الفائدة للشركة من تشغيل السيارة (شهرياً) */
  monthlyCompanyProfit: number
  /** قيمة الإيجار التقديري المقترح (بعد خصم الدفعة المقدمة) */
  adjustedMonthlyRent: number
  /** خصم قيمة الدفعة المقدمة من قيمة قسط الإيجار الشهري المقترح */
  downPaymentMonthlyDiscount: number
  /** هل تطبق ضريبة القيمة المضافة على عقد الإيجار أم لا */
  isTaxApplied?: boolean
  /** نسبة ضريبة القيمة المضافة المعتمدة في العقد (%) */
  taxPercentage?: number
  /** قيمة الضريبة المضافة شهرياً على قسط الإيجار */
  monthlyTaxAmount?: number
  /** قيمة الإيجار الشهري المستحق على العميل شاملاً الضريبة */
  monthlyRentWithTax?: number
  /** إجمالي قيمة الضريبة المضافة على كامل مدة العقد */
  totalTaxAmount?: number
  /** إجمالي قيمة العقد النهائي شاملاً الضريبة */
  totalPriceWithTax?: number
  /** حالة العقد (Active, Completed, Cancelled) */
  status: string
  /** ملاحظات وشروط إضافية على العقد */
  notes: string | null
  /** تاريخ إنشاء العقد في النظام */
  createdDate: string
  /** جدول الإهلاك والتكاليف السنوي بالتفصيل بصيغة JSON */
  yearlyBreakdownJson: string | null
}

/**
 * طلب إنشاء عقد جديد
 */
export interface CreateContractRequest {
  /** معرف نوع العقد (1: خدمة، 2: قصير الأجل، 3: طويل الأجل، 4: منتهي بالتمليك) */
  contractTypeId: number
  /** معرف السيارة موضوع العقد */
  carId: number
  /** معرف فرع المعرض */
  branchId: number
  /** معرف العميل المستفيد (لعقود الإيجار والتمليك) */
  customerId?: number | null
  /** معرف الموظف المستفيد (لعقود الخدمة) */
  userId?: number | null
  /** تاريخ بداية العقد */
  startDate: string
  /** تاريخ نهاية العقد */
  endDate: string
  /** قيمة قسط الإيجار التقديري المقترح قبل الدفعة المقدمة (شهرياً) */
  monthlyRent?: number | null
  /** قيمة الدفعة المقدمة */
  downPayment?: number | null
  /** الحد الأقصى للمسافة المسموحة (كم) */
  mileageLimit?: number | null
  /** رسوم كل كيلومتر زائد عن الحد */
  excessMileageFee?: number | null
  /** نسبة تقدير تأمين شامل للسيارة المعتمدة في العقد (%) */
  insurancePercentage?: number | null
  /** نسبة تقدير تكلفة الصيانة خلال فترة العقد المعتمدة في العقد (%) */
  maintenancePercentage?: number | null
  /** نسبة تقدير تكلفة سيارة بديلة خلال فترة العقد المعتمدة في العقد (%) */
  replacementCarPercentage?: number | null
  /** نسبة تقدير تكلفة المصاريف الإدارية خلال فترة العقد المعتمدة في العقد (%) */
  adminFeesPercentage?: number | null
  /** نسبة الإهلاك السنوي المعتمدة في العقد (%) */
  annualDepreciationPercentage?: number | null
  /** هل تطبق ضريبة القيمة المضافة على عقد الإيجار أم لا (الافتراضي: true) */
  applyTax?: boolean
  /** نسبة ضريبة القيمة المضافة المخصصة (%) - اختياري، الافتراضي 15% */
  taxPercentage?: number | null
  /** ملاحظات وشروط إضافية على العقد */
  notes?: string | null
}

/**
 * طلب تعديل بيانات عقد مسجل
 */
export interface UpdateContractRequest {
  /** معرف نوع العقد */
  contractTypeId: number
  /** معرف السيارة موضوع العقد */
  carId: number
  /** معرف الفرع */
  branchId: number
  /** معرف العميل المستفيد */
  customerId?: number | null
  /** معرف الموظف المستفيد */
  userId?: number | null
  /** تاريخ بداية العقد */
  startDate: string
  /** تاريخ نهاية العقد */
  endDate: string
  /** قيمة قسط الإيجار التقديري المقترح قبل الدفعة المقدمة (شهرياً) */
  monthlyRent?: number | null
  /** قيمة الدفعة المقدمة */
  downPayment?: number | null
  /** المبلغ المدفوع من قيمة العقد */
  paidPrice: number
  /** الحد الأقصى للمسافة المسموحة (كم) */
  mileageLimit?: number | null
  /** رسوم كل كيلومتر زائد عن الحد */
  excessMileageFee?: number | null
  /** قراءة العداد الفعلية عند نهاية العقد وتسليم السيارة */
  actualEndMileage?: number | null
  /** نسبة تقدير تأمين شامل للسيارة المعتمدة في العقد (%) */
  insurancePercentage?: number | null
  /** نسبة تقدير تكلفة الصيانة خلال فترة العقد المعتمدة في العقد (%) */
  maintenancePercentage?: number | null
  /** نسبة تقدير تكلفة سيارة بديلة خلال فترة العقد المعتمدة في العقد (%) */
  replacementCarPercentage?: number | null
  /** نسبة تقدير تكلفة المصاريف الإدارية خلال فترة العقد المعتمدة في العقد (%) */
  adminFeesPercentage?: number | null
  /** نسبة الإهلاك السنوي المعتمدة في العقد (%) */
  annualDepreciationPercentage?: number | null
  /** هل تطبق ضريبة القيمة المضافة على عقد الإيجار أم لا */
  applyTax?: boolean
  /** نسبة ضريبة القيمة المضافة المخصصة (%) */
  taxPercentage?: number | null
  /** حالة العقد (Active, Completed, Cancelled) */
  status?: string
  /** ملاحظات وشروط إضافية على العقد */
  notes?: string | null
}

/**
 * معايير البحث والفلترة في العقود
 */
export interface ContractQueryParameters {
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
  /** تصفية حسب نوع العقد */
  contractTypeId?: number | null
  /** تصفية حسب السيارة */
  carId?: number | null
  /** تصفية حسب الفرع */
  branchId?: number | null
  /** تصفية حسب العميل */
  customerId?: number | null
  /** تصفية حسب الموظف المستفيد */
  userId?: number | null
  /** تصفية حسب حالة العقد */
  status?: string | null
  /** تصفية حسب تاريخ البداية (من) */
  startDateFrom?: string | null
  /** تصفية حسب تاريخ البداية (إلى) */
  startDateTo?: string | null
}

/**
 * إعدادات النسب المئوية الأربعة المعتمدة للإهلاك وتكاليف العقود
 */
export interface ContractTermsConfigDto {
  /** نسبة تقدير تأمين شامل للسيارة السنوية (%) */
  insurancePercentage: number
  /** نسبة تقدير تكلفة الصيانة خلال فترة العقد السنوية (%) */
  maintenancePercentage: number
  /** نسبة تقدير تكلفة سيارة بديلة خلال فترة العقد السنوية (%) */
  replacementCarPercentage: number
  /** نسبة تقدير تكلفة المصاريف الإدارية خلال فترة العقد السنوية (%) */
  adminFeesPercentage: number
  /** نسبة الإهلاك السنوي للسيارة (%) */
  annualDepreciationPercentage: number
  /** تاريخ آخر تعديل للإعدادات */
  lastModifiedDate: string
}

/**
 * طلب تحديث الإعدادات العامة لنسب العقود
 */
export interface UpdateContractTermsConfigRequest {
  /** نسبة تقدير تأمين شامل للسيارة السنوية (%) */
  insurancePercentage: number
  /** نسبة تقدير تكلفة الصيانة خلال فترة العقد السنوية (%) */
  maintenancePercentage: number
  /** نسبة تقدير تكلفة سيارة بديلة خلال فترة العقد السنوية (%) */
  replacementCarPercentage: number
  /** نسبة تقدير تكلفة المصاريف الإدارية خلال فترة العقد السنوية (%) */
  adminFeesPercentage: number
  /** نسبة الإهلاك السنوي للسيارة (%) */
  annualDepreciationPercentage: number
}

/**
 * مدخلات طلب الحساب اللحظي المالي والإهلاكي للعقد
 */
export interface ContractCalculationRequest {
  /** معرف نوع العقد (1: خدمة، 2: قصير، 3: طويل، 4: تمليك) */
  contractTypeId: number
  /** معرف السيارة موضوع العقد */
  carId?: number | null
  /** القيمة الشرائية للسيارة */
  carPurchasePrice?: number | null
  /** تاريخ شراء السيارة */
  carPurchaseDate?: string | null
  /** تاريخ بداية العقد */
  startDate: string
  /** تاريخ نهاية العقد */
  endDate: string
  /** قيمة قسط الإيجار التقديري المقترح قبل الدفعة المقدمة (شهرياً) */
  monthlyRent?: number | null
  /** قيمة الدفعة المقدمة */
  downPayment?: number | null
  /** الحد الأقصى للمسافة المسموحة (كم) */
  mileageLimit?: number | null
  /** رسوم كل كيلومتر زائد عن الحد */
  excessMileageFee?: number | null
  /** نسبة تقدير تأمين شامل للسيارة السنوية (%) */
  insurancePercentage?: number | null
  /** نسبة تقدير تكلفة الصيانة خلال فترة العقد السنوية (%) */
  maintenancePercentage?: number | null
  /** نسبة تقدير تكلفة سيارة بديلة خلال فترة العقد السنوية (%) */
  replacementCarPercentage?: number | null
  /** نسبة تقدير تكلفة المصاريف الإدارية خلال فترة العقد السنوية (%) */
  adminFeesPercentage?: number | null
  /** نسبة الإهلاك السنوي للسيارة (%) */
  annualDepreciationPercentage?: number | null
  /** قيمة السيارة / القيمة الدفترية للسيارة (Book Value) */
  carBookValue?: number | null
  /** هل تطبق ضريبة القيمة المضافة على عقد الإيجار (الافتراضي: true) */
  applyTax?: boolean
  /** نسبة ضريبة القيمة المضافة (%) - اختياري، الافتراضي 15% */
  taxPercentage?: number | null
}

/**
 * تفاصيل سنة تعاقدية واحدة في جدول الإهلاك والتكاليف السنوي
 */
export interface ContractYearlyScheduleItem {
  /** رقم السنة التعاقدية (1، 2، ...) */
  year: number
  /** صافي قيمة السيارة خلال مدة العقد (القيمة الدفترية في نهاية السنة) */
  netCarValue: number
  /** قيمة إهلاك السيارة خلال هذه السنة */
  depreciationCost: number
  /** تقدير تأمين شامل للسيارة لهذه السنة */
  insuranceCost: number
  /** تقدير تكلفة الصيانة خلال هذه السنة */
  maintenanceCost: number
  /** تقدير تكلفة سيارة بديلة خلال هذه السنة */
  replacementCarCost: number
  /** تقدير تكلفة المصاريف الإدارية خلال هذه السنة */
  adminFeesCost: number
  /** إجمالي التكلفة التقديرية لهذه السنة */
  totalEstimatedCost: number
  /** قيمة تكلفة للسيارة (شهرياً) لهذه السنة */
  monthlyCost: number
}

/**
 * النتيجة الشاملة للحساب اللحظي المالي والإهلاكي للعقد
 */
export interface ContractCalculationResultDto {
  /** القيمة الشرائية للسيارة */
  carPurchasePrice: number
  /** تاريخ شراء السيارة */
  carPurchaseDate: string
  /** عمر السيارة / العمر الإنتاجي للسيارة (بالسنوات) */
  carProductiveAge: number
  /** القيمة الدفترية للسيارة */
  carBookValue: number
  /** مجمع الإهلاك للسيارة */
  carAccumulatedDepreciation: number
  /** قيمة السيارة المعتمدة في الحسابات (القيمة الدفترية) */
  carValueUsed: number
  /** مدة العقد الإجمالية بالأيام (شاملة البداية والنهاية) */
  periodDays: number
  /** المدة المحسوبة بالشهور */
  periodMonths: number
  /** سنوات العقد للإهلاك */
  contractYears: number
  /** جدول الإهلاك والتكاليف السنوي (Yearly Schedule) */
  yearlySchedule: ContractYearlyScheduleItem[]
  /** قيمة قسط الإيجار التقديري المقترح قبل الدفعة المقدمة (شهرياً) */
  monthlyRentBeforeDownPayment: number
  /** قيمة الدفعة المقدمة */
  downPayment: number
  /** قيمة الإيجار التقديري المقترح (بعد خصم الدفعة المقدمة شهرياً) */
  adjustedMonthlyRent: number
  /** خصم قيمة الدفعة المقدمة من قيمة قسط الإيجار الشهري المقترح */
  downPaymentMonthlyDiscount: number
  /** إجمالي الإيرادات التقديرية حسب مدة العقد */
  totalEstimatedRevenue: number
  /** إجمالي التكلفة التقديرية (تشمل الإهلاك والتأمين والصيانة والبديلة والإدارية) */
  totalEstimatedCost: number
  /** إجمالي الأرباح التقديرية حسب مدة العقد */
  expectedProfit: number
  /** قيمة الفائدة للشركة من تشغيل السيارة (شهرياً) */
  monthlyCompanyProfit: number
  /** الحد الأقصى للمسافة المسموحة (كم) */
  mileageLimit: number | null
  /** رسوم كل كيلومتر زائد عن الحد */
  excessMileageFee: number | null
  /** تنبيهات وملاحظات التحقق والحسابات */
  warnings: string[]
  /** قيمة إهلاك السيارة نهاية مدة العقد الإجمالية */
  totalDepreciationCost: number
  /** صافي قيمة السيارة خلال مدة العقد (نهاية مدة العقد) */
  netCarValueAtEnd: number
  /** تقدير تأمين شامل للسيارة الإجمالي */
  totalInsuranceCost: number
  /** تقدير تكلفة الصيانة خلال فترة العقد الإجمالية */
  totalMaintenanceCost: number
  /** تقدير تكلفة سيارة بديلة خلال فترة العقد الإجمالية */
  totalReplacementCarCost: number
  /** تقدير تكلفة المصاريف الإدارية خلال فترة العقد الإجمالية */
  totalAdminFeesCost: number
  /** قيمة تكلفة للسيارة (شهرياً) */
  monthlyCarCost: number
  /** الإيجار اليومي التقديري (في العقود القصيرة والأقل من سنة) */
  dailyRent?: number
  /** أيام الإيجار المستحق / الفعلي (بدون احتساب يوم البداية) */
  rentDays?: number
  /** هل ضريبة القيمة المضافة مطبقة على العقد */
  isTaxApplied?: boolean
  /** نسبة ضريبة القيمة المضافة المعتمدة في العقد (%) */
  taxPercentage?: number
  /** قيمة الضريبة المضافة شهرياً على قسط الإيجار */
  monthlyTaxAmount?: number
  /** قيمة الإيجار الشهري المستحق على العميل شاملاً الضريبة */
  monthlyRentWithTax?: number
  /** إجمالي قيمة الضريبة المضافة على كامل مدة العقد */
  totalTaxAmount?: number
  /** إجمالي قيمة العقد شاملاً الضريبة */
  totalPriceWithTax?: number
}

/**
 * تقرير العقد والعمليات الحسابية الملحقة به
 */
export interface ContractReportDto {
  /** بيانات العقد */
  contract: ContractDto
  /** نتائج الحسابات المالية والإهلاكية للعقد */
  calculation: ContractCalculationResultDto
  /** تاريخ ووقت إصدار التقرير */
  generatedAt: string
}
