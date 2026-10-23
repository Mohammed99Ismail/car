<script setup lang="ts">
import {
  ArrowLeft24Regular as BackIcon,
  Print24Regular as PrintIcon,
  VehicleCar24Regular as CarIcon,
  DocumentText24Regular as ContractIcon,
  Money24Regular as MoneyIcon,
  People24Regular as CustomerIcon,
  Building24Regular as BranchIcon,
  Shield24Regular as ShieldIcon,
  Wrench24Regular as WrenchIcon,
  Calendar24Regular as CalendarIcon,
} from '@vicons/fluent'
import {
  NAlert,
  NButton,
  NDivider,
  NIcon,
  NImage,
  NSpin,
  NTag,
} from 'naive-ui'
import { isApiSuccess } from '~/models/ApiResponse'
import type { ContractReportDto } from '~/models/Contract'
import { useContractService } from '~/services/contract.service'

const route = useRoute()
const router = useRouter()
const message = useMessage()
const contractService = useContractService()

const contractId = computed(() => Number(route.params.id))
const reportData = ref<ContractReportDto | null>(null)
const loading = ref(true)

async function loadReport() {
  if (!contractId.value) return
  loading.value = true
  try {
    const res = await contractService.getReport(contractId.value)
    if (isApiSuccess(res) && res.data) {
      reportData.value = res.data
    } else {
      message.error(res.message || 'فشل تحميل تقرير العقد')
    }
  } catch (err) {
    message.error('حدث خطأ أثناء تحميل التقرير')
  } finally {
    loading.value = false
  }
}

function handlePrint() {
  const reportElement = document.getElementById('printable-report')
  if (!reportElement) {
    window.print()
    return
  }

  // Create isolated invisible iframe for dedicated report printing
  const iframe = document.createElement('iframe')
  iframe.id = 'report-print-frame'
  iframe.style.position = 'fixed'
  iframe.style.right = '0'
  iframe.style.bottom = '0'
  iframe.style.width = '0'
  iframe.style.height = '0'
  iframe.style.border = 'none'
  iframe.style.opacity = '0'
  iframe.style.pointerEvents = 'none'
  document.body.appendChild(iframe)

  const iframeDoc = iframe.contentWindow?.document
  if (!iframeDoc) {
    window.print()
    return
  }

  // Extract all stylesheets and style tags from current page
  let stylesHtml = ''
  document.querySelectorAll('link[rel="stylesheet"], style').forEach((node) => {
    stylesHtml += node.outerHTML
  })

  // Additional isolated print styling
  const printStyles = `
    <style>
      @page {
        size: A4 portrait;
        margin: 10mm 12mm;
      }
      *, *::before, *::after {
        -webkit-print-color-adjust: exact !important;
        print-color-adjust: exact !important;
        box-sizing: border-box;
      }
      html, body {
        background-color: #ffffff !important;
        color: #0f172a !important;
        font-family: 'Cairo', system-ui, -apple-system, sans-serif !important;
        direction: rtl;
        margin: 0 !important;
        padding: 0 !important;
        width: 100% !important;
        height: auto !important;
        overflow: visible !important;
      }
      #printable-report {
        display: block !important;
        width: 100% !important;
        max-width: 100% !important;
        padding: 0 !important;
        margin: 0 !important;
        border: none !important;
        box-shadow: none !important;
        background: #ffffff !important;
      }
      .break-inside-avoid,
      .avoid-break {
        break-inside: avoid !important;
        page-break-inside: avoid !important;
      }
      img {
        max-width: 100% !important;
        height: auto !important;
      }
    </style>
  `

  const contractNum = reportData.value?.contract.contractNumber || ''
  iframeDoc.open()
  iframeDoc.write(`<!DOCTYPE html>
<html lang="ar" dir="rtl">
  <head>
    <meta charset="UTF-8">
    <title>تقرير عقد رقم ${contractNum}</title>
    ${stylesHtml}
    ${printStyles}
  </head>
  <body>
    <div id="printable-report">
      ${reportElement.innerHTML}
    </div>
  </body>
</html>`)
  iframeDoc.close()

  // Wait for styles & images to settle, then open print dialog
  setTimeout(() => {
    try {
      iframe.contentWindow?.focus()
      iframe.contentWindow?.print()
    } catch (e) {
      window.print()
    } finally {
      setTimeout(() => {
        try {
          document.body.removeChild(iframe)
        } catch (_) {}
      }, 2000)
    }
  }, 350)
}

function formatMoney(val: number | null | undefined): string {
  if (val === null || val === undefined) return '0.00'
  return Number(val).toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 })
}

onMounted(() => {
  loadReport()
})
</script>

<template>
  <div class="p-4 md:p-8 max-w-5xl mx-auto space-y-6 print:p-0 print:m-0 print:max-w-none print:w-full print:space-y-4">
    <!-- Non-printable Toolbar -->
    <div class="print:hidden flex items-center justify-between bg-white dark:bg-slate-800 p-4 rounded-2xl shadow-sm border border-slate-100 dark:border-slate-700/60">
      <div class="flex items-center gap-3">
        <NButton
          secondary
          class="rounded-xl"
          @click="router.push('/contracts')"
        >
          <template #icon>
            <NIcon><BackIcon /></NIcon>
          </template>
          العودة لقائمة العقود
        </NButton>
        <span class="text-sm font-bold text-slate-700 dark:text-slate-200">
          تقرير العقد رقم: <span class="font-mono text-blue-600">{{ reportData?.contract.contractNumber }}</span>
        </span>
      </div>

      <div class="flex items-center gap-3">
        <NButton
          type="primary"
          class="font-bold shadow-md shadow-blue-500/20 rounded-xl px-5"
          @click="handlePrint"
        >
          <template #icon>
            <NIcon><PrintIcon /></NIcon>
          </template>
          طباعة التقرير
        </NButton>
      </div>
    </div>

    <!-- Loading State -->
    <div v-if="loading" class="text-center py-20 bg-white dark:bg-slate-800 rounded-3xl shadow-sm">
      <NSpin size="large" />
      <p class="text-sm text-slate-500 mt-4">جاري تجهيز تقرير العقد والحسابات المالية...</p>
    </div>

    <!-- Official Printable Report Document -->
    <div
      v-else-if="reportData"
      id="printable-report"
      class="bg-white text-slate-900 p-8 sm:p-12 rounded-3xl shadow-lg border border-slate-200 space-y-8 print:p-0 print:border-none print:shadow-none print:space-y-6 print:rounded-none"
    >
      <!-- 1. Showroom Official Header -->
      <div class="break-inside-avoid flex flex-col sm:flex-row items-center justify-between gap-6 border-b-2 border-slate-900 pb-6">
        <div class="text-right space-y-1">
          <h1 class="text-2xl sm:text-3xl font-black tracking-tight text-slate-950 flex items-center gap-2">
            <span>🚗</span> شركة معرض وتأجير السيارات
          </h1>
          <p class="text-xs text-slate-600 font-semibold">
            المملكة العربية السعودية • سجل تجاري معتمد • شبكة الفروع وخدمات الأسطول
          </p>
          <p class="text-xs text-slate-500">
            الفرع المصدر: <strong class="text-slate-800">{{ reportData.contract.branchName }}</strong>
          </p>
        </div>

        <!-- Contract Badge Box -->
        <div class="border-2 border-slate-900 rounded-2xl p-4 text-center min-w-[220px] bg-slate-50">
          <span class="text-[11px] font-bold uppercase tracking-wider text-slate-500 block">تقرير عقد معتمد</span>
          <span class="text-xl font-mono font-black text-blue-700 block my-0.5">{{ reportData.contract.contractNumber }}</span>
          <span class="inline-block px-3 py-0.5 rounded-full text-xs font-bold bg-slate-900 text-white">
            {{ reportData.contract.contractTypeNameAr }}
          </span>
        </div>
      </div>

      <!-- 2. Parties Information (Showroom & Beneficiary) -->
      <div class="break-inside-avoid grid grid-cols-1 sm:grid-cols-2 gap-4">
        <!-- Showroom Party -->
        <div class="p-4 rounded-2xl bg-slate-50 border border-slate-200 space-y-2">
          <h3 class="text-xs font-black uppercase tracking-wider text-slate-500 border-b border-slate-200 pb-1.5 flex items-center gap-1.5">
            <NIcon><BranchIcon /></NIcon> الطرف الأول (المؤجر / المعرض)
          </h3>
          <div class="text-xs space-y-1 text-slate-700">
            <p><strong class="text-slate-900">الاسم:</strong> شركة معرض وتأجير السيارات</p>
            <p><strong class="text-slate-900">الفرع:</strong> {{ reportData.contract.branchName }}</p>
            <p><strong class="text-slate-900">حالة العقد:</strong>
              <span class="font-bold text-emerald-700">{{ reportData.contract.status === 'Active' ? 'ساري العمل به' : reportData.contract.status }}</span>
            </p>
            <p><strong class="text-slate-900">تاريخ الإصدار:</strong> {{ new Date(reportData.contract.createdDate).toLocaleDateString('ar-SA') }}</p>
          </div>
        </div>

        <!-- Beneficiary Party -->
        <div class="p-4 rounded-2xl bg-slate-50 border border-slate-200 space-y-2">
          <h3 class="text-xs font-black uppercase tracking-wider text-slate-500 border-b border-slate-200 pb-1.5 flex items-center gap-1.5">
            <NIcon><CustomerIcon /></NIcon> الطرف الثاني (المستفيد)
          </h3>
          <div v-if="reportData.contract.contractTypeId === 1" class="text-xs space-y-1 text-slate-700">
            <p><strong class="text-slate-900">الموظف المسؤول:</strong> {{ reportData.contract.userFullName }}</p>
            <p><strong class="text-slate-900">البريد الإلكتروني:</strong> {{ reportData.contract.userEmail }}</p>
            <p><strong class="text-slate-900">الصفة:</strong> مستخدم نظام / موظف بالمعرض (عقد خدمة)</p>
          </div>
          <div v-else class="text-xs space-y-1 text-slate-700">
            <p><strong class="text-slate-900">اسم العميل:</strong> {{ reportData.contract.customerName || '—' }}</p>
            <p><strong class="text-slate-900">رقم الهوية الوطنية:</strong> {{ reportData.contract.customerNationalId || '—' }}</p>
            <p><strong class="text-slate-900">رقم الجوال:</strong> {{ reportData.contract.customerMobile || '—' }}</p>
          </div>
        </div>
      </div>

      <!-- 3. Vehicle Specifications & Book Value History -->
      <div class="break-inside-avoid space-y-3">
        <h3 class="text-sm font-black text-slate-900 flex items-center gap-2 border-r-4 border-blue-600 pr-2">
          بيانات السيارة والقيمة الدفترية ومجمع الإهلاك
        </h3>

        <div class="p-5 rounded-2xl bg-blue-50/40 border border-blue-200/60 space-y-4">
          <div class="flex flex-col md:flex-row items-start md:items-center justify-between gap-4">
            <!-- Car Image + Details -->
            <div class="flex items-center gap-4 min-w-0">
              <!-- Car Image Box -->
              <div class="relative flex-shrink-0 w-28 h-20 sm:w-36 sm:h-24 rounded-xl overflow-hidden border-2 border-white shadow-md bg-white flex items-center justify-center">
                <n-image
                  v-if="reportData.contract.carImagePath"
                  :src="reportData.contract.carImagePath"
                  :preview-src="reportData.contract.carImagePath"
                  alt="صورة السيارة"
                  object-fit="cover"
                  class="w-full h-full"
                  :img-props="{ class: 'w-full h-full object-cover rounded-xl transition-transform duration-300 hover:scale-105' }"
                />
                <div v-else class="flex flex-col items-center justify-center text-slate-400 gap-1 p-2 text-center w-full h-full bg-slate-100">
                  <n-icon size="28" :component="CarIcon" class="text-slate-400" />
                  <span class="text-[10px] font-medium text-slate-500">لا توجد صورة</span>
                </div>
              </div>

              <!-- Car Details -->
              <div class="space-y-1 min-w-0">
                <h4 class="text-base sm:text-lg font-black text-slate-900 leading-tight">
                  {{ reportData.contract.carMakeName }} {{ reportData.contract.carModelName }}
                  <span class="text-sm font-bold text-blue-700">({{ reportData.contract.carManufactureYear }})</span>
                </h4>
                <div class="text-xs text-slate-600 flex flex-wrap items-center gap-x-3 gap-y-1">
                  <span>اللون: <strong class="text-slate-900">{{ reportData.contract.carColor || '—' }}</strong></span>
                  <span class="text-slate-300">•</span>
                  <span>
                    رقم الشاسيه (الهيكل):
                    <strong class="font-mono text-slate-900 tracking-wider">
                      {{ reportData.contract.carChassisNumber || '—' }}
                    </strong>
                  </span>
                </div>
              </div>
            </div>

            <!-- License Plate Graphic -->
            <div class="flex-shrink-0 self-center md:self-auto px-4 py-2 bg-amber-50 border-2 border-amber-400 rounded-xl text-center shadow-sm">
              <span class="text-[10px] block text-slate-500 font-bold uppercase tracking-wider">KSA • المملكة</span>
              <span class="font-mono font-black text-amber-950 text-base tracking-widest">
                {{ reportData.contract.carPlateNumber }}
              </span>
            </div>
          </div>

          <!-- Valuation Table -->
          <div class="grid grid-cols-2 sm:grid-cols-4 gap-3 text-center text-xs">
            <div class="bg-white p-3 rounded-xl border border-slate-200">
              <span class="text-slate-500 block mb-1">تاريخ شراء السيارة</span>
              <strong class="text-slate-900 block text-sm">
                {{ new Date(reportData.calculation.carPurchaseDate).toLocaleDateString('ar-SA') }}
              </strong>
            </div>

            <div class="bg-white p-3 rounded-xl border border-slate-200">
              <span class="text-slate-500 block mb-1">القيمة الشرائية للسيارة</span>
              <strong class="text-slate-900 block text-sm font-mono">
                {{ reportData.calculation.carPurchasePrice.toLocaleString() }} ر.س
              </strong>
            </div>

            <div class="bg-white p-3 rounded-xl border border-slate-200">
              <span class="text-slate-500 block mb-1">مجمع الإهلاك حتى بداية العقد</span>
              <strong class="text-amber-600 block text-sm font-mono">
                {{ reportData.calculation.carAccumulatedDepreciation.toLocaleString() }} ر.س
              </strong>
            </div>

            <div class="bg-blue-600 text-white p-3 rounded-xl border border-blue-700 shadow-sm">
              <span class="text-blue-100 block mb-1 font-semibold">القيمة الدفترية المعتمدة</span>
              <strong class="block text-base font-black font-mono">
                {{ reportData.calculation.carBookValue.toLocaleString() }} ر.س
              </strong>
            </div>
          </div>

          <div class="text-xs text-slate-600 bg-white/70 p-3 rounded-xl border border-blue-100 flex flex-wrap items-center justify-between gap-2">
            <span>
              • العمر الإنتاجي للسيارة: <strong class="text-slate-900 font-bold">{{ reportData.calculation.carProductiveAge }} سنة</strong>
            </span>
            <span>
              • أساس الإهلاك: <strong>25% سنوياً على 4 سنوات</strong>
            </span>
            <span v-if="reportData.calculation.carProductiveAge >= 4" class="text-amber-700 font-bold">
              • تجاوزت 4 سنوات (القيمة الدفترية = 1 ريال فقط)
            </span>
          </div>
        </div>
      </div>

      <!-- 4. Contract Duration & Mileage Limits -->
      <div class="break-inside-avoid space-y-3">
        <h3 class="text-sm font-black text-slate-900 flex items-center gap-2 border-r-4 border-indigo-600 pr-2">
          فترة العقد وشروط المسافة (الكيلومترات)
        </h3>

        <div class="grid grid-cols-2 sm:grid-cols-4 gap-3 text-center text-xs">
          <div class="bg-slate-50 p-3 rounded-xl border border-slate-200">
            <span class="text-slate-500 block mb-1">تاريخ بداية العقد</span>
            <strong class="text-slate-900 text-sm">
              {{ new Date(reportData.contract.startDate).toLocaleDateString('ar-SA') }}
            </strong>
          </div>

          <div class="bg-slate-50 p-3 rounded-xl border border-slate-200">
            <span class="text-slate-500 block mb-1">تاريخ نهاية العقد</span>
            <strong class="text-slate-900 text-sm">
              {{ new Date(reportData.contract.endDate).toLocaleDateString('ar-SA') }}
            </strong>
          </div>

          <div class="bg-slate-50 p-3 rounded-xl border border-slate-200">
            <span class="text-slate-500 block mb-1">المدة الكلية</span>
            <strong class="text-blue-700 text-sm">
              {{ reportData.calculation.periodMonths }} شهر ({{ reportData.calculation.periodDays }} يوم)
            </strong>
          </div>

          <div class="bg-slate-50 p-3 rounded-xl border border-slate-200">
            <span class="text-slate-500 block mb-1">سنوات العقد للإهلاك</span>
            <strong class="text-purple-700 text-sm">
              {{ reportData.calculation.contractYears }} سنة
            </strong>
          </div>
        </div>

        <!-- Short-term Daily Rent Info -->
        <div v-if="reportData.contract.contractTypeId === 2" class="p-3 bg-amber-50 rounded-xl border border-amber-200 text-xs flex flex-wrap items-center justify-between gap-3">
          <span class="text-amber-900 font-bold">عقد إيجار قصير الأجل / حسب الطلب (حساب يومي):</span>
          <span>الإيجار اليومي: <strong class="text-amber-800 font-mono">{{ formatMoney(reportData.calculation.dailyRent || (reportData.contract.monthlyRent ? reportData.contract.monthlyRent / 30 : 0)) }} ر.س/يوم</strong></span>
          <span>أيام الإيجار الفعلي: <strong class="text-amber-800 font-mono">{{ reportData.calculation.rentDays || reportData.contract.periodDays }} يوم</strong></span>
          <span>إجمالي الإيجار المستحق: <strong class="text-emerald-700 font-mono font-bold">{{ formatMoney(reportData.calculation.totalEstimatedRevenue) }} ر.س</strong></span>
        </div>

        <div v-if="reportData.calculation.mileageLimit" class="grid grid-cols-1 sm:grid-cols-2 gap-3 text-xs">
          <div class="p-3 bg-slate-50 rounded-xl border border-slate-200 flex items-center justify-between">
            <span class="text-slate-600 font-semibold">الحد الأقصى للمسافة المسموحة:</span>
            <strong class="text-slate-900 font-mono text-sm">{{ reportData.calculation.mileageLimit.toLocaleString() }} كم</strong>
          </div>
          <div class="p-3 bg-slate-50 rounded-xl border border-slate-200 flex items-center justify-between">
            <span class="text-slate-600 font-semibold">رسوم كل كيلومتر زائد عن الحد:</span>
            <strong class="text-slate-900 font-mono text-sm">{{ reportData.calculation.excessMileageFee }} ر.س / كم</strong>
          </div>
        </div>
      </div>

      <!-- 5. Yearly Schedule Table (Types 2 & 3) -->
      <div v-if="reportData.calculation.yearlySchedule && reportData.calculation.yearlySchedule.length > 0" class="break-inside-avoid space-y-3">
        <h3 class="text-sm font-black text-slate-900 flex items-center justify-between border-r-4 border-amber-600 pr-2">
          <span v-if="reportData.contract.contractTypeId === 4">جدول تكاليف التأمين الشامل وإهلاك السيارة لعقد الإيجار المنتهي بالتمليك</span>
          <span v-else>جدول بنود الخصم الأربعة وإهلاك السيارة التفصيلي بالسنوات</span>
          <span v-if="reportData.contract.contractTypeId === 4" class="text-xs text-slate-500 font-normal">
            (التأمين: {{ reportData.contract.insurancePercentage }}% • إهلاك كامل القيمة الدفترية للسيارة)
          </span>
          <span v-else class="text-xs text-slate-500 font-normal">
            (التأمين: {{ reportData.contract.insurancePercentage }}% • الصيانة: {{ reportData.contract.maintenancePercentage }}% • البديلة: {{ reportData.contract.replacementCarPercentage }}% • الإدارية: {{ reportData.contract.adminFeesPercentage }}%)
          </span>
        </h3>

        <div class="overflow-x-auto rounded-2xl border-2 border-slate-200">
          <table class="w-full text-center text-xs">
            <thead class="bg-slate-900 text-white font-bold">
              <tr>
                <th class="p-3">السنة</th>
                <th class="p-3">صافي قيمة السيارة خلال مدة العقد</th>
                <th class="p-3">إهلاك السيارة</th>
                <th class="p-3">التأمين الشامل</th>
                <th v-if="reportData.contract.contractTypeId !== 4" class="p-3">الصيانة</th>
                <th v-if="reportData.contract.contractTypeId !== 4" class="p-3">سيارة بديلة</th>
                <th v-if="reportData.contract.contractTypeId !== 4" class="p-3">مصاريف إدارية</th>
                <th class="p-3 bg-red-950/80">إجمالي التكلفة التقديرية</th>
                <th class="p-3 bg-blue-950/80">التكلفة الشهرية للسيارة</th>
              </tr>
            </thead>
            <tbody class="divide-y divide-slate-200 font-mono">
              <tr v-for="item in reportData.calculation.yearlySchedule" :key="item.year" class="hover:bg-slate-50">
                <td class="p-3 font-bold text-slate-900 bg-slate-100 font-sans">السنة {{ item.year }}</td>
                <td class="p-3">
                  {{ formatMoney(item.netCarValue) }} ر.س
                  <span v-if="reportData.contract.contractTypeId === 4 && item.year === reportData.calculation.yearlySchedule.length" class="text-[10px] text-amber-700 block">(تؤول للعميل)</span>
                </td>
                <td class="p-3 text-amber-700 font-bold">{{ formatMoney(item.depreciationCost) }} ر.س</td>
                <td class="p-3">{{ formatMoney(item.insuranceCost) }} ر.س</td>
                <td v-if="reportData.contract.contractTypeId !== 4" class="p-3">{{ formatMoney(item.maintenanceCost) }} ر.س</td>
                <td v-if="reportData.contract.contractTypeId !== 4" class="p-3">{{ formatMoney(item.replacementCarCost) }} ر.س</td>
                <td v-if="reportData.contract.contractTypeId !== 4" class="p-3">{{ formatMoney(item.adminFeesCost) }} ر.س</td>
                <td class="p-3 font-bold text-red-700 bg-red-50/50">{{ formatMoney(item.totalEstimatedCost) }} ر.س</td>
                <td class="p-3 font-black text-blue-800 bg-blue-50/50">{{ formatMoney(item.monthlyCost) }} ر.س</td>
              </tr>
            </tbody>
          </table>
        </div>

        <!-- Terms Summary in Report -->
        <div v-if="reportData.contract.contractTypeId === 4" class="grid grid-cols-1 sm:grid-cols-2 gap-2 text-xs pt-1">
          <div class="p-2.5 bg-slate-50 rounded-xl border border-slate-200">
            <span class="text-slate-500 block text-[11px]">تقدير تأمين شامل للسيارة:</span>
            <strong class="text-slate-900 font-mono">{{ formatMoney(reportData.calculation.totalInsuranceCost || reportData.calculation.yearlySchedule[reportData.calculation.yearlySchedule.length - 1].insuranceCost) }} ر.س</strong>
          </div>
          <div class="p-2.5 bg-slate-50 rounded-xl border border-slate-200">
            <span class="text-slate-500 block text-[11px]">إهلاك السيارة (القيمة الدفترية كاملة):</span>
            <strong class="text-amber-700 font-mono">{{ formatMoney(reportData.calculation.totalDepreciationCost || reportData.calculation.yearlySchedule[reportData.calculation.yearlySchedule.length - 1].depreciationCost) }} ر.س</strong>
          </div>
        </div>

        <div v-else class="grid grid-cols-2 sm:grid-cols-4 gap-2 text-xs pt-1">
          <div class="p-2.5 bg-slate-50 rounded-xl border border-slate-200">
            <span class="text-slate-500 block text-[11px]">تقدير تأمين شامل للسيارة:</span>
            <strong class="text-slate-900 font-mono">{{ formatMoney(reportData.calculation.totalInsuranceCost || reportData.calculation.yearlySchedule[reportData.calculation.yearlySchedule.length - 1].insuranceCost) }} ر.س</strong>
          </div>
          <div class="p-2.5 bg-slate-50 rounded-xl border border-slate-200">
            <span class="text-slate-500 block text-[11px]">تقدير تكلفة الصيانة:</span>
            <strong class="text-slate-900 font-mono">{{ formatMoney(reportData.calculation.totalMaintenanceCost || reportData.calculation.yearlySchedule[reportData.calculation.yearlySchedule.length - 1].maintenanceCost) }} ر.س</strong>
          </div>
          <div class="p-2.5 bg-slate-50 rounded-xl border border-slate-200">
            <span class="text-slate-500 block text-[11px]">تقدير سيارة بديلة:</span>
            <strong class="text-slate-900 font-mono">{{ formatMoney(reportData.calculation.totalReplacementCarCost || reportData.calculation.yearlySchedule[reportData.calculation.yearlySchedule.length - 1].replacementCarCost) }} ر.س</strong>
          </div>
          <div class="p-2.5 bg-slate-50 rounded-xl border border-slate-200">
            <span class="text-slate-500 block text-[11px]">تقدير مصاريف إدارية:</span>
            <strong class="text-slate-900 font-mono">{{ formatMoney(reportData.calculation.totalAdminFeesCost || reportData.calculation.yearlySchedule[reportData.calculation.yearlySchedule.length - 1].adminFeesCost) }} ر.س</strong>
          </div>
        </div>
      </div>

      <!-- 6. Financial Summary & Profitability Breakdown -->
      <div class="break-inside-avoid space-y-3">
        <h3 class="text-sm font-black text-slate-900 flex items-center gap-2 border-r-4 border-emerald-600 pr-2">
          الملخص المالي الشامل وأرباح العقد
        </h3>

        <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
          <!-- Column 1: Rent & Deductions -->
          <div class="bg-slate-50 p-5 rounded-2xl border border-slate-200 space-y-3 text-xs">
            <div class="flex items-center justify-between pb-2 border-b border-slate-200">
              <span class="text-slate-600 font-semibold">قيمة قسط الإيجار الشهري قبل الدفعة المقدمة:</span>
              <strong class="text-slate-900 font-mono text-sm">
                {{ formatMoney(reportData.calculation.monthlyRentBeforeDownPayment) }} ر.س
              </strong>
            </div>

            <div class="flex items-center justify-between pb-2 border-b border-slate-200">
              <span class="text-slate-600 font-semibold">قيمة الدفعة المقدمة:</span>
              <strong class="text-amber-700 font-mono text-sm">
                {{ formatMoney(reportData.calculation.downPayment) }} ر.س
              </strong>
            </div>

            <div class="flex items-center justify-between pb-2 border-b border-slate-200">
              <span class="text-slate-600 font-semibold">خصم الدفعة المقدمة من قسط الإيجار الشهري:</span>
              <strong class="text-amber-700 font-mono text-sm">
                {{ formatMoney(reportData.calculation.downPaymentMonthlyDiscount) }} ر.س/شهر
              </strong>
            </div>

            <div class="flex items-center justify-between pt-1">
              <span class="font-black text-slate-900">قيمة الإيجار الشهري المقترح بعد خصم الدفعة:</span>
              <strong class="text-emerald-700 font-mono text-base font-black">
                {{ formatMoney(reportData.calculation.adjustedMonthlyRent) }} ر.س
              </strong>
            </div>
          </div>

          <!-- Column 2: Total Revenues & Net Profits -->
          <div class="bg-emerald-50/40 p-5 rounded-2xl border border-emerald-200 space-y-3 text-xs">
            <div class="flex items-center justify-between pb-2 border-b border-emerald-200/60">
              <span class="text-slate-700 font-semibold">إجمالي الإيرادات التقديرية حسب مدة العقد:</span>
              <strong class="text-slate-900 font-mono text-sm font-black">
                {{ formatMoney(reportData.calculation.totalEstimatedRevenue) }} ر.س
              </strong>
            </div>

            <div class="flex items-center justify-between pb-2 border-b border-emerald-200/60">
              <span class="text-slate-700 font-semibold">إجمالي التكاليف التقديرية حسب مدة العقد:</span>
              <strong class="text-red-700 font-mono text-sm font-bold">
                {{ formatMoney(reportData.calculation.totalEstimatedCost) }} ر.س
              </strong>
            </div>

            <div class="flex items-center justify-between pb-2 border-b border-emerald-200/60">
              <span class="font-bold text-slate-900">إجمالي الأرباح المتوقعة حسب مدة العقد:</span>
              <strong class="text-emerald-700 font-mono text-base font-black">
                {{ formatMoney(reportData.calculation.expectedProfit) }} ر.س
              </strong>
            </div>

            <div class="flex items-center justify-between pt-1">
              <span class="font-bold text-slate-900">فائدة الشركة الشهرية من تشغيل السيارة:</span>
              <strong class="text-blue-700 font-mono text-sm font-black">
                {{ formatMoney(reportData.calculation.monthlyCompanyProfit) }} ر.س / شهر
              </strong>
            </div>
          </div>
        </div>

        <!-- VAT Tax Breakdown Strip (for Rental Contracts) -->
        <div v-if="reportData.contract.contractTypeId !== 1" class="p-4 rounded-2xl bg-gradient-to-r from-blue-50/80 via-indigo-50/60 to-blue-50/80 border-2 border-blue-200/80 space-y-3 text-xs avoid-break">
          <div class="flex items-center justify-between pb-2 border-b border-blue-200/60">
            <div class="flex items-center gap-2">
              <span class="text-base">🧾</span>
              <strong class="text-slate-900 text-sm">بيان احتساب ضريبة القيمة المضافة (VAT) المعتمدة بالنظام</strong>
            </div>
            <NTag :type="reportData.contract.isTaxApplied ? 'info' : 'default'" size="small" round :bordered="false" class="font-bold">
              {{ reportData.contract.isTaxApplied ? 'ضريبة القيمة المضافة 15% (مطبقة على العميل)' : 'معفى من الضريبة (غير مطبقة)' }}
            </NTag>
          </div>

          <div class="grid grid-cols-1 sm:grid-cols-4 gap-3 text-center">
            <div class="p-2.5 bg-white rounded-xl border border-slate-200">
              <span class="text-slate-500 block text-[11px] mb-0.5">قسط الإيجار قبل الضريبة</span>
              <strong class="text-slate-900 font-mono text-sm">
                {{ formatMoney(reportData.calculation.adjustedMonthlyRent) }} ر.س
              </strong>
            </div>

            <div class="p-2.5 bg-white rounded-xl border border-blue-200">
              <span class="text-blue-700 block text-[11px] font-semibold mb-0.5">
                مبلغ الضريبة {{ reportData.contract.isTaxApplied ? '(15%)' : '(0%)' }}
              </span>
              <strong class="text-blue-700 font-mono text-sm">
                {{ reportData.contract.isTaxApplied ? `+${formatMoney(reportData.contract.monthlyTaxAmount || reportData.calculation.monthlyTaxAmount)} ر.س` : '0.00 ر.س' }}
              </strong>
            </div>

            <div class="p-2.5 bg-gradient-to-br from-emerald-600 to-teal-700 text-white rounded-xl shadow-xs">
              <span class="text-emerald-100 block text-[11px] font-bold mb-0.5">الإيجار الشهري المستحق على العميل</span>
              <strong class="text-white font-mono text-base font-black">
                {{ formatMoney(reportData.contract.monthlyRentWithTax || reportData.calculation.monthlyRentWithTax || reportData.calculation.adjustedMonthlyRent) }} ر.س
              </strong>
            </div>

            <div class="p-2.5 bg-white rounded-xl border border-slate-200">
              <span class="text-slate-500 block text-[11px] mb-0.5">إجمالي قيمة العقد شامل الضريبة</span>
              <strong class="text-slate-900 font-mono text-sm font-black">
                {{ formatMoney(reportData.contract.totalPriceWithTax || reportData.calculation.totalPriceWithTax || reportData.contract.totalPrice) }} ر.س
              </strong>
            </div>
          </div>
        </div>
      </div>

      <!-- 7. Notes & Signatures -->
      <div class="break-inside-avoid space-y-6 pt-4 border-t border-slate-200">
        <div v-if="reportData.contract.notes" class="p-4 rounded-xl bg-slate-50 border border-slate-200 text-xs">
          <strong class="block mb-1 text-slate-800">ملاحظات وشروط خاصة:</strong>
          <p class="text-slate-600 leading-relaxed">{{ reportData.contract.notes }}</p>
        </div>

        <!-- Official Signatures -->
        <div class="grid grid-cols-2 gap-8 text-center pt-8 text-xs font-bold text-slate-800">
          <div class="space-y-12">
            <p>توقيع الطرف الأول (المؤجر / المعرض)</p>
            <div class="border-b border-dashed border-slate-400 w-3/4 mx-auto" />
            <p class="text-[11px] text-slate-400">الختم الرسمي</p>
          </div>

          <div class="space-y-12">
            <p>توقيع الطرف الثاني (المستفيد / العميل)</p>
            <div class="border-b border-dashed border-slate-400 w-3/4 mx-auto" />
            <p class="text-[11px] text-slate-400">التوقيع والاسم</p>
          </div>
        </div>

        <div class="text-center text-[10px] text-slate-400 pt-6">
          تم إصدار هذا التقرير آلياً عبر نظام إدارة معرض وتأجير السيارات بتاريخ: {{ reportData.generatedAt }}
        </div>
      </div>
    </div>
  </div>
</template>

<style lang="scss">
@media print {
  @page {
    size: A4 portrait;
    margin: 8mm 10mm;
  }

  // Hide application navigation, sidebar, and headers
  .n-layout-sider,
  .n-layout-sider-scroll-container,
  aside,
  nav,
  header,
  .navbar,
  .n-page-header,
  .logo-container,
  .print\:hidden,
  .no-print,
  .dark-mode-container,
  #theme-toggle,
  .preloader,
  .n-notification-container,
  .n-message-container,
  .n-modal-container {
    display: none !important;
    visibility: hidden !important;
    width: 0 !important;
    height: 0 !important;
  }

  html,
  body {
    background: #ffffff !important;
    background-color: #ffffff !important;
    color: #0f172a !important;
    height: auto !important;
    min-height: auto !important;
    max-height: none !important;
    width: 100% !important;
    max-width: 100% !important;
    overflow: visible !important;
    margin: 0 !important;
    padding: 0 !important;
    position: static !important;
    -webkit-print-color-adjust: exact !important;
    print-color-adjust: exact !important;
  }

  .n-layout,
  .n-layout--static-positioned,
  .n-layout--absolute-positioned,
  .n-layout-scroll-container,
  .main-content,
  .relative,
  .n-scrollbar,
  .n-scrollbar-container,
  .n-scrollbar-content,
  .md-container {
    position: static !important;
    display: block !important;
    height: auto !important;
    min-height: 0 !important;
    max-height: none !important;
    overflow: visible !important;
    background: transparent !important;
    border: none !important;
    padding: 0 !important;
    margin: 0 !important;
    box-shadow: none !important;
    width: 100% !important;
    max-width: 100% !important;
    transform: none !important;
  }

  #printable-report {
    display: block !important;
    position: static !important;
    width: 100% !important;
    max-width: 100% !important;
    margin: 0 !important;
    padding: 0 !important;
    border: none !important;
    box-shadow: none !important;
    background: #ffffff !important;
    color: #0f172a !important;
  }

  .break-inside-avoid,
  .avoid-break {
    break-inside: avoid !important;
    page-break-inside: avoid !important;
  }

  * {
    -webkit-print-color-adjust: exact !important;
    print-color-adjust: exact !important;
  }
}
</style>
