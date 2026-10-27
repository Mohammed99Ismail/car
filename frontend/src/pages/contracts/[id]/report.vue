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
import { appConfig } from '~/config/app.config'
import { useAppConfig } from '~/composables/useAppConfig'

const { t, locale } = useI18n()
const { appName, companyName, subTitle, logoAlt } = useAppConfig()
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
  const isEn = locale.value?.startsWith('en')
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
        direction: ${isEn ? 'ltr' : 'rtl'};
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
<html lang="${isEn ? 'en' : 'ar'}" dir="${isEn ? 'ltr' : 'rtl'}">
  <head>
    <meta charset="UTF-8">
    <title>${t('contracts.reportTitlePrefix', { number: contractNum })}</title>
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

function formatDate(val: string | Date | null | undefined): string {
  if (!val) return '—'
  return new Date(val).toLocaleDateString(locale.value?.startsWith('en') ? 'en-US' : 'ar-SA')
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
          {{ t('contracts.reportBackToList') }}
        </NButton>
        <span class="text-sm font-bold text-slate-700 dark:text-slate-200">
          {{ t('contracts.reportTitlePrefix', { number: reportData?.contract.contractNumber }) }}
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
          {{ t('contracts.printReport') }}
        </NButton>
      </div>
    </div>

    <!-- Loading State -->
    <div v-if="loading" class="text-center py-20 bg-white dark:bg-slate-800 rounded-3xl shadow-sm">
      <NSpin size="large" />
      <p class="text-sm text-slate-500 mt-4">{{ t('contracts.reportPreparing') }}</p>
    </div>

    <!-- Official Printable Report Document -->
    <div
      v-else-if="reportData"
      id="printable-report"
      class="bg-white text-slate-900 p-8 sm:p-12 rounded-3xl shadow-lg border border-slate-200 space-y-8 print:p-0 print:border-none print:shadow-none print:space-y-6 print:rounded-none"
    >
      <!-- 1. Showroom Official Header -->
      <div class="break-inside-avoid flex flex-col sm:flex-row items-center justify-between gap-6 border-b-2 border-slate-900 pb-6">
        <div class="flex items-center gap-4 text-start min-w-0 max-w-full sm:max-w-[70%]">
          <!-- Constrained Official Logo -->
          <div class="report-logo-box flex items-center justify-center shrink-0 w-14 h-14 max-w-[56px] max-h-[56px] rounded-2xl overflow-hidden bg-slate-50 border border-slate-300 p-1.5 shadow-2xs">
            <img
              :src="appConfig.logoPath"
              :alt="logoAlt"
              class="w-full h-full max-w-full max-h-full object-contain shrink-0 block"
            />
          </div>

          <div class="space-y-1 min-w-0">
            <h1
              class="text-xl sm:text-2xl font-black tracking-tight text-slate-950 line-clamp-2 break-words"
              :title="companyName"
            >
              {{ companyName }}
            </h1>
            <p
              class="text-xs text-slate-600 font-semibold line-clamp-1 break-words"
              :title="subTitle"
            >
              {{ subTitle }}
            </p>
            <p class="text-xs text-slate-500">
              {{ t('contracts.reportBranch') }}: <strong class="text-slate-800">{{ reportData.contract.branchName }}</strong>
            </p>
          </div>
        </div>

        <!-- Contract Badge Box -->
        <div class="border-2 border-slate-900 rounded-2xl p-4 text-center min-w-[220px] bg-slate-50">
          <span class="text-[11px] font-bold uppercase tracking-wider text-slate-500 block">{{ t('contracts.reportCertifiedBadge') }}</span>
          <span class="text-xl font-mono font-black text-blue-700 block my-0.5">{{ reportData.contract.contractNumber }}</span>
          <span class="inline-block px-3 py-0.5 rounded-full text-xs font-bold bg-slate-900 text-white">
            {{ locale.startsWith('en') ? (reportData.contract.contractTypeNameEn || reportData.contract.contractTypeNameAr) : reportData.contract.contractTypeNameAr }}
          </span>
        </div>
      </div>

      <!-- 2. Parties Information (Showroom & Beneficiary) -->
      <div class="break-inside-avoid grid grid-cols-1 sm:grid-cols-2 gap-4">
        <!-- Showroom Party -->
        <div class="p-4 rounded-2xl bg-slate-50 border border-slate-200 space-y-2">
          <h3 class="text-xs font-black uppercase tracking-wider text-slate-500 border-b border-slate-200 pb-1.5 flex items-center gap-1.5">
            <NIcon><BranchIcon /></NIcon> {{ t('contracts.reportParty1Title') }}
          </h3>
          <div class="text-xs space-y-1 text-slate-700">
            <p class="truncate" :title="companyName"><strong class="text-slate-900">{{ t('contracts.reportName') }}</strong> {{ companyName }}</p>
            <p><strong class="text-slate-900">{{ t('contracts.reportBranchLabel') }}</strong> {{ reportData.contract.branchName }}</p>
            <p><strong class="text-slate-900">{{ t('contracts.reportStatusLabel') }}</strong>
              <span class="font-bold text-emerald-700">{{ reportData.contract.status === 'Active' ? t('contracts.reportStatusActive') : reportData.contract.status }}</span>
            </p>
            <p><strong class="text-slate-900">{{ t('contracts.reportIssueDate') }}</strong> {{ new Date(reportData.contract.createdDate).toLocaleDateString(locale.startsWith('en') ? 'en-US' : 'ar-SA') }}</p>
          </div>
        </div>

        <!-- Beneficiary Party -->
        <div class="p-4 rounded-2xl bg-slate-50 border border-slate-200 space-y-2">
          <h3 class="text-xs font-black uppercase tracking-wider text-slate-500 border-b border-slate-200 pb-1.5 flex items-center gap-1.5">
            <NIcon><CustomerIcon /></NIcon> {{ t('contracts.reportParty2Title') }}
          </h3>
          <div v-if="reportData.contract.contractTypeId === 1" class="text-xs space-y-1 text-slate-700">
            <p><strong class="text-slate-900">{{ t('contracts.reportEmployee') }}</strong> {{ reportData.contract.userFullName }}</p>
            <p><strong class="text-slate-900">{{ t('contracts.reportEmail') }}</strong> {{ reportData.contract.userEmail }}</p>
            <p><strong class="text-slate-900">{{ t('contracts.reportEmployeeRole') }}</strong> {{ t('contracts.reportEmployeeRoleDesc') }}</p>
          </div>
          <div v-else class="text-xs space-y-1 text-slate-700">
            <p><strong class="text-slate-900">{{ t('contracts.reportCustomerName') }}</strong> {{ reportData.contract.customerName || '—' }}</p>
            <p><strong class="text-slate-900">{{ t('contracts.reportNationalId') }}</strong> {{ reportData.contract.customerNationalId || '—' }}</p>
            <p><strong class="text-slate-900">{{ t('contracts.reportMobile') }}</strong> {{ reportData.contract.customerMobile || '—' }}</p>
          </div>
        </div>
      </div>

      <!-- 3. Vehicle Specifications & Book Value History -->
      <div class="break-inside-avoid space-y-3">
        <h3 class="text-sm font-black text-slate-900 flex items-center gap-2 border-s-4 border-blue-600 ps-2">
          {{ t('contracts.reportVehicleTitle') }}
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
                  :alt="t('cars.imagePath')"
                  object-fit="cover"
                  class="w-full h-full"
                  :img-props="{ class: 'w-full h-full object-cover rounded-xl transition-transform duration-300 hover:scale-105' }"
                />
                <div v-else class="flex flex-col items-center justify-center text-slate-400 gap-1 p-2 text-center w-full h-full bg-slate-100">
                  <n-icon size="28" :component="CarIcon" class="text-slate-400" />
                  <span class="text-[10px] font-medium text-slate-500">{{ t('cars.noImage') }}</span>
                </div>
              </div>

              <!-- Car Details -->
              <div class="space-y-1 min-w-0">
                <h4 class="text-base sm:text-lg font-black text-slate-900 leading-tight">
                  {{ reportData.contract.carMakeName }} {{ reportData.contract.carModelName }}
                  <span class="text-sm font-bold text-blue-700">({{ reportData.contract.carManufactureYear }})</span>
                </h4>
                <div class="text-xs text-slate-600 flex flex-wrap items-center gap-x-3 gap-y-1">
                  <span>{{ t('contracts.reportColor') }} <strong class="text-slate-900">{{ reportData.contract.carColor || '—' }}</strong></span>
                  <span class="text-slate-300">•</span>
                  <span>
                    {{ t('contracts.reportChassis') }}
                    <strong class="font-mono text-slate-900 tracking-wider">
                      {{ reportData.contract.carChassisNumber || '—' }}
                    </strong>
                  </span>
                </div>
              </div>
            </div>

            <!-- License Plate Graphic -->
            <div class="flex-shrink-0 self-center md:self-auto px-4 py-2 bg-amber-50 border-2 border-amber-400 rounded-xl text-center shadow-sm">
              <span class="text-[10px] block text-slate-500 font-bold uppercase tracking-wider">{{ t('contracts.reportPlateCountry') }}</span>
              <span class="font-mono font-black text-amber-950 text-base tracking-widest">
                {{ reportData.contract.carPlateNumber }}
              </span>
            </div>
          </div>

          <!-- Valuation Table -->
          <div class="grid grid-cols-2 sm:grid-cols-4 gap-3 text-center text-xs">
            <div class="bg-white p-3 rounded-xl border border-slate-200">
              <span class="text-slate-500 block mb-1">{{ t('contracts.reportPurchaseDate') }}</span>
              <strong class="text-slate-900 block text-sm">
                {{ formatDate(reportData.calculation.carPurchaseDate) }}
              </strong>
            </div>

            <div class="bg-white p-3 rounded-xl border border-slate-200">
              <span class="text-slate-500 block mb-1">{{ t('contracts.reportPurchasePrice') }}</span>
              <strong class="text-slate-900 block text-sm font-mono">
                {{ formatMoney(reportData.calculation.carPurchasePrice) }} {{ t('contracts.sar') }}
              </strong>
            </div>

            <div class="bg-white p-3 rounded-xl border border-slate-200">
              <span class="text-slate-500 block mb-1">{{ t('contracts.reportAccumulatedDepreciation') }}</span>
              <strong class="text-amber-600 block text-sm font-mono">
                {{ formatMoney(reportData.calculation.carAccumulatedDepreciation) }} {{ t('contracts.sar') }}
              </strong>
            </div>

            <div class="bg-blue-600 text-white p-3 rounded-xl border border-blue-700 shadow-sm">
              <span class="text-blue-100 block mb-1 font-semibold">{{ t('contracts.reportBookValueStart') }}</span>
              <strong class="block text-base font-black font-mono">
                {{ formatMoney(reportData.calculation.carBookValue) }} {{ t('contracts.sar') }}
              </strong>
            </div>
          </div>

          <div class="text-xs text-slate-600 bg-white/70 p-3 rounded-xl border border-blue-100 flex flex-wrap items-center justify-between gap-2">
            <span>
              • {{ t('contracts.reportProductiveAge') }}: <strong class="text-slate-900 font-bold">{{ reportData.calculation.carProductiveAge }} {{ t('contracts.years') }}</strong>
            </span>
            <span>
              {{ t('contracts.reportDepreciationBasis') }}
            </span>
            <span v-if="reportData.calculation.carProductiveAge >= 4" class="text-amber-700 font-bold">
              {{ t('contracts.reportExceededAge') }}
            </span>
          </div>
        </div>
      </div>

      <!-- 4. Contract Duration & Mileage Limits -->
      <div class="break-inside-avoid space-y-3">
        <h3 class="text-sm font-black text-slate-900 flex items-center gap-2 border-s-4 border-indigo-600 ps-2">
          {{ t('contracts.reportPeriodTitle') }}
        </h3>

        <div class="grid grid-cols-2 sm:grid-cols-4 gap-3 text-center text-xs">
          <div class="bg-slate-50 p-3 rounded-xl border border-slate-200">
            <span class="text-slate-500 block mb-1">{{ t('contracts.startDate') }}</span>
            <strong class="text-slate-900 text-sm">
              {{ formatDate(reportData.contract.startDate) }}
            </strong>
          </div>

          <div class="bg-slate-50 p-3 rounded-xl border border-slate-200">
            <span class="text-slate-500 block mb-1">{{ t('contracts.endDate') }}</span>
            <strong class="text-slate-900 text-sm">
              {{ formatDate(reportData.contract.endDate) }}
            </strong>
          </div>

          <div class="bg-slate-50 p-3 rounded-xl border border-slate-200">
            <span class="text-slate-500 block mb-1">{{ t('contracts.reportTotalPeriod') }}</span>
            <strong class="text-blue-700 text-sm">
              {{ reportData.calculation.periodMonths }} {{ t('contracts.months') }} ({{ reportData.calculation.periodDays }} {{ t('contracts.days') }})
            </strong>
          </div>

          <div class="bg-slate-50 p-3 rounded-xl border border-slate-200">
            <span class="text-slate-500 block mb-1">{{ t('contracts.contractYears') }}</span>
            <strong class="text-purple-700 text-sm">
              {{ reportData.calculation.contractYears }} {{ t('contracts.years') }}
            </strong>
          </div>
        </div>

        <!-- Short-term Daily Rent Info -->
        <div v-if="reportData.contract.contractTypeId === 2" class="p-3 bg-amber-50 rounded-xl border border-amber-200 text-xs flex flex-wrap items-center justify-between gap-3">
          <span class="text-amber-900 font-bold">{{ t('contracts.reportShortTermNotice') }}</span>
          <span>{{ t('contracts.reportDailyRent') }} <strong class="text-amber-800 font-mono">{{ formatMoney(reportData.calculation.dailyRent || (reportData.contract.monthlyRent ? reportData.contract.monthlyRent / 30 : 0)) }} {{ t('contracts.sar') }}/{{ t('contracts.days') }}</strong></span>
          <span>{{ t('contracts.reportActualRentDays') }} <strong class="text-amber-800 font-mono">{{ reportData.calculation.rentDays || reportData.contract.periodDays }} {{ t('contracts.days') }}</strong></span>
          <span>{{ t('contracts.reportTotalRentDue') }} <strong class="text-emerald-700 font-mono font-bold">{{ formatMoney(reportData.calculation.totalEstimatedRevenue) }} {{ t('contracts.sar') }}</strong></span>
        </div>

        <div v-if="reportData.calculation.mileageLimit" class="grid grid-cols-1 sm:grid-cols-2 gap-3 text-xs">
          <div class="p-3 bg-slate-50 rounded-xl border border-slate-200 flex items-center justify-between">
            <span class="text-slate-600 font-semibold">{{ t('contracts.mileageLimit') }}:</span>
            <strong class="text-slate-900 font-mono text-sm">{{ reportData.calculation.mileageLimit.toLocaleString() }} {{ t('contracts.km') }}</strong>
          </div>
          <div class="p-3 bg-slate-50 rounded-xl border border-slate-200 flex items-center justify-between">
            <span class="text-slate-600 font-semibold">{{ t('contracts.excessMileageFee') }}:</span>
            <strong class="text-slate-900 font-mono text-sm">{{ reportData.calculation.excessMileageFee }} {{ t('contracts.sar') }} / {{ t('contracts.km') }}</strong>
          </div>
        </div>
      </div>

      <!-- 5. Yearly Schedule Table (Types 2 & 3) -->
      <div v-if="reportData.calculation.yearlySchedule && reportData.calculation.yearlySchedule.length > 0" class="break-inside-avoid space-y-3">
        <h3 class="text-sm font-black text-slate-900 flex items-center justify-between border-s-4 border-amber-600 ps-2">
          <span v-if="reportData.contract.contractTypeId === 4">{{ t('contracts.reportYearlyScheduleLeaseToOwn') }}</span>
          <span v-else>{{ t('contracts.reportYearlyScheduleGeneral') }}</span>
          <span v-if="reportData.contract.contractTypeId === 4" class="text-xs text-slate-500 font-normal">
            {{ t('contracts.reportScheduleSubtitleLease', { ins: reportData.contract.insurancePercentage }) }}
          </span>
          <span v-else class="text-xs text-slate-500 font-normal">
            {{ t('contracts.reportScheduleSubtitleGeneral', { ins: reportData.contract.insurancePercentage, maint: reportData.contract.maintenancePercentage, rep: reportData.contract.replacementCarPercentage, admin: reportData.contract.adminFeesPercentage }) }}
          </span>
        </h3>

        <div class="overflow-x-auto rounded-2xl border-2 border-slate-200">
          <table class="w-full text-center text-xs">
            <thead class="bg-slate-900 text-white font-bold">
              <tr>
                <th class="p-3">{{ t('contracts.reportYearCol') }}</th>
                <th class="p-3">{{ t('contracts.reportNetCarValueCol') }}</th>
                <th class="p-3">{{ t('contracts.reportDepreciationCol') }}</th>
                <th class="p-3">{{ t('contracts.reportInsuranceCol') }}</th>
                <th v-if="reportData.contract.contractTypeId !== 4" class="p-3">{{ t('contracts.maintenance') }}</th>
                <th v-if="reportData.contract.contractTypeId !== 4" class="p-3">{{ t('contracts.replacementCar') }}</th>
                <th v-if="reportData.contract.contractTypeId !== 4" class="p-3">{{ t('contracts.adminFees') }}</th>
                <th class="p-3 bg-red-950/80">{{ t('contracts.reportTotalCostCol') }}</th>
                <th class="p-3 bg-blue-950/80">{{ t('contracts.reportMonthlyCostCol') }}</th>
              </tr>
            </thead>
            <tbody class="divide-y divide-slate-200 font-mono">
              <tr v-for="item in reportData.calculation.yearlySchedule" :key="item.year" class="hover:bg-slate-50">
                <td class="p-3 font-bold text-slate-900 bg-slate-100 font-sans">{{ t('contracts.reportYearPrefix', { year: item.year }) }}</td>
                <td class="p-3">
                  {{ formatMoney(item.netCarValue) }} {{ t('contracts.sar') }}
                  <span v-if="reportData.contract.contractTypeId === 4 && item.year === reportData.calculation.yearlySchedule.length" class="text-[10px] text-amber-700 block">{{ t('contracts.reportTransfersToCustomer') }}</span>
                </td>
                <td class="p-3 text-amber-700 font-bold">{{ formatMoney(item.depreciationCost) }} {{ t('contracts.sar') }}</td>
                <td class="p-3">{{ formatMoney(item.insuranceCost) }} {{ t('contracts.sar') }}</td>
                <td v-if="reportData.contract.contractTypeId !== 4" class="p-3">{{ formatMoney(item.maintenanceCost) }} {{ t('contracts.sar') }}</td>
                <td v-if="reportData.contract.contractTypeId !== 4" class="p-3">{{ formatMoney(item.replacementCarCost) }} {{ t('contracts.sar') }}</td>
                <td v-if="reportData.contract.contractTypeId !== 4" class="p-3">{{ formatMoney(item.adminFeesCost) }} {{ t('contracts.sar') }}</td>
                <td class="p-3 font-bold text-red-700 bg-red-50/50">{{ formatMoney(item.totalEstimatedCost) }} {{ t('contracts.sar') }}</td>
                <td class="p-3 font-black text-blue-800 bg-blue-50/50">{{ formatMoney(item.monthlyCost) }} {{ t('contracts.sar') }}</td>
              </tr>
            </tbody>
          </table>
        </div>

        <!-- Terms Summary in Report -->
        <div v-if="reportData.contract.contractTypeId === 4" class="grid grid-cols-1 sm:grid-cols-2 gap-2 text-xs pt-1">
          <div class="p-2.5 bg-slate-50 rounded-xl border border-slate-200">
            <span class="text-slate-500 block text-[11px]">{{ t('contracts.reportEstInsurance') }}</span>
            <strong class="text-slate-900 font-mono">{{ formatMoney(reportData.calculation.totalInsuranceCost || reportData.calculation.yearlySchedule[reportData.calculation.yearlySchedule.length - 1].insuranceCost) }} {{ t('contracts.sar') }}</strong>
          </div>
          <div class="p-2.5 bg-slate-50 rounded-xl border border-slate-200">
            <span class="text-slate-500 block text-[11px]">{{ t('contracts.reportEstDepreciationLease') }}</span>
            <strong class="text-amber-700 font-mono">{{ formatMoney(reportData.calculation.totalDepreciationCost || reportData.calculation.yearlySchedule[reportData.calculation.yearlySchedule.length - 1].depreciationCost) }} {{ t('contracts.sar') }}</strong>
          </div>
        </div>

        <div v-else class="grid grid-cols-2 sm:grid-cols-4 gap-2 text-xs pt-1">
          <div class="p-2.5 bg-slate-50 rounded-xl border border-slate-200">
            <span class="text-slate-500 block text-[11px]">{{ t('contracts.reportEstInsurance') }}</span>
            <strong class="text-slate-900 font-mono">{{ formatMoney(reportData.calculation.totalInsuranceCost || reportData.calculation.yearlySchedule[reportData.calculation.yearlySchedule.length - 1].insuranceCost) }} {{ t('contracts.sar') }}</strong>
          </div>
          <div class="p-2.5 bg-slate-50 rounded-xl border border-slate-200">
            <span class="text-slate-500 block text-[11px]">{{ t('contracts.reportEstMaintenance') }}</span>
            <strong class="text-slate-900 font-mono">{{ formatMoney(reportData.calculation.totalMaintenanceCost || reportData.calculation.yearlySchedule[reportData.calculation.yearlySchedule.length - 1].maintenanceCost) }} {{ t('contracts.sar') }}</strong>
          </div>
          <div class="p-2.5 bg-slate-50 rounded-xl border border-slate-200">
            <span class="text-slate-500 block text-[11px]">{{ t('contracts.reportEstReplacement') }}</span>
            <strong class="text-slate-900 font-mono">{{ formatMoney(reportData.calculation.totalReplacementCarCost || reportData.calculation.yearlySchedule[reportData.calculation.yearlySchedule.length - 1].replacementCarCost) }} {{ t('contracts.sar') }}</strong>
          </div>
          <div class="p-2.5 bg-slate-50 rounded-xl border border-slate-200">
            <span class="text-slate-500 block text-[11px]">{{ t('contracts.reportEstAdminFees') }}</span>
            <strong class="text-slate-900 font-mono">{{ formatMoney(reportData.calculation.totalAdminFeesCost || reportData.calculation.yearlySchedule[reportData.calculation.yearlySchedule.length - 1].adminFeesCost) }} {{ t('contracts.sar') }}</strong>
          </div>
        </div>
      </div>

      <!-- 6. Financial Summary & Profitability Breakdown -->
      <div class="break-inside-avoid space-y-3">
        <h3 class="text-sm font-black text-slate-900 flex items-center gap-2 border-s-4 border-emerald-600 ps-2">
          {{ t('contracts.reportFinancialTitle') }}
        </h3>

        <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
          <!-- Column 1: Rent & Deductions -->
          <div class="bg-slate-50 p-5 rounded-2xl border border-slate-200 space-y-3 text-xs">
            <div class="flex items-center justify-between pb-2 border-b border-slate-200">
              <span class="text-slate-600 font-semibold">{{ t('contracts.reportRentBeforeDown') }}</span>
              <strong class="text-slate-900 font-mono text-sm">
                {{ formatMoney(reportData.calculation.monthlyRentBeforeDownPayment) }} {{ t('contracts.sar') }}
              </strong>
            </div>

            <div class="flex items-center justify-between pb-2 border-b border-slate-200">
              <span class="text-slate-600 font-semibold">{{ t('contracts.downPayment') }}:</span>
              <strong class="text-amber-700 font-mono text-sm">
                {{ formatMoney(reportData.calculation.downPayment) }} {{ t('contracts.sar') }}
              </strong>
            </div>

            <div class="flex items-center justify-between pb-2 border-b border-slate-200">
              <span class="text-slate-600 font-semibold">{{ t('contracts.downPaymentDiscount') }}:</span>
              <strong class="text-amber-700 font-mono text-sm">
                {{ formatMoney(reportData.calculation.downPaymentMonthlyDiscount) }} {{ t('contracts.sar') }}/{{ t('contracts.months') }}
              </strong>
            </div>

            <div class="flex items-center justify-between pt-1">
              <span class="font-black text-slate-900">{{ t('contracts.adjustedRent') }}:</span>
              <strong class="text-emerald-700 font-mono text-base font-black">
                {{ formatMoney(reportData.calculation.adjustedMonthlyRent) }} {{ t('contracts.sar') }}
              </strong>
            </div>
          </div>

          <!-- Column 2: Total Revenues & Net Profits -->
          <div class="bg-emerald-50/40 p-5 rounded-2xl border border-emerald-200 space-y-3 text-xs">
            <div class="flex items-center justify-between pb-2 border-b border-emerald-200/60">
              <span class="text-slate-700 font-semibold">{{ t('contracts.totalRevenue') }}:</span>
              <strong class="text-slate-900 font-mono text-sm font-black">
                {{ formatMoney(reportData.calculation.totalEstimatedRevenue) }} {{ t('contracts.sar') }}
              </strong>
            </div>

            <div class="flex items-center justify-between pb-2 border-b border-emerald-200/60">
              <span class="text-slate-700 font-semibold">{{ t('contracts.totalCosts') }}:</span>
              <strong class="text-red-700 font-mono text-sm font-bold">
                {{ formatMoney(reportData.calculation.totalEstimatedCost) }} {{ t('contracts.sar') }}
              </strong>
            </div>

            <div class="flex items-center justify-between pb-2 border-b border-emerald-200/60">
              <span class="font-bold text-slate-900">{{ t('contracts.expectedProfit') }}:</span>
              <strong class="text-emerald-700 font-mono text-base font-black">
                {{ formatMoney(reportData.calculation.expectedProfit) }} {{ t('contracts.sar') }}
              </strong>
            </div>

            <div class="flex items-center justify-between pt-1">
              <span class="font-bold text-slate-900">{{ t('contracts.monthlyProfit') }}:</span>
              <strong class="text-blue-700 font-mono text-sm font-black">
                {{ formatMoney(reportData.calculation.monthlyCompanyProfit) }} {{ t('contracts.sar') }} / {{ t('contracts.months') }}
              </strong>
            </div>
          </div>
        </div>

        <!-- VAT Tax Breakdown Strip (for Rental Contracts) -->
        <div v-if="reportData.contract.contractTypeId !== 1" class="p-4 rounded-2xl bg-gradient-to-r from-blue-50/80 via-indigo-50/60 to-blue-50/80 border-2 border-blue-200/80 space-y-3 text-xs avoid-break">
          <div class="flex items-center justify-between pb-2 border-b border-blue-200/60">
            <div class="flex items-center gap-2">
              <span class="text-base">🧾</span>
              <strong class="text-slate-900 text-sm">{{ t('contracts.reportVatStatement') }}</strong>
            </div>
            <NTag :type="reportData.contract.isTaxApplied ? 'info' : 'default'" size="small" round :bordered="false" class="font-bold">
              {{ reportData.contract.isTaxApplied ? t('contracts.reportVatAppliedBadge') : t('contracts.reportVatExemptBadge') }}
            </NTag>
          </div>

          <div class="grid grid-cols-1 sm:grid-cols-4 gap-3 text-center">
            <div class="p-2.5 bg-white rounded-xl border border-slate-200">
              <span class="text-slate-500 block text-[11px] mb-0.5">{{ t('contracts.reportRentBeforeTax') }}</span>
              <strong class="text-slate-900 font-mono text-sm">
                {{ formatMoney(reportData.contract.monthlyRent || (reportData.contract.isTaxApplied ? reportData.calculation.monthlyRentBeforeDownPayment - reportData.calculation.monthlyTaxAmount : reportData.calculation.monthlyRentBeforeDownPayment)) }} {{ t('contracts.sar') }}
              </strong>
            </div>

            <div class="p-2.5 bg-white rounded-xl border border-blue-200">
              <span class="text-blue-700 block text-[11px] font-semibold mb-0.5">
                {{ t('contracts.reportVatAmountLabel', { percent: reportData.contract.isTaxApplied ? '15%' : '0%' }) }}
              </span>
              <strong class="text-blue-700 font-mono text-sm">
                {{ reportData.contract.isTaxApplied ? `+${formatMoney(reportData.contract.monthlyTaxAmount || reportData.calculation.monthlyTaxAmount)} ${t('contracts.sar')}` : `0.00 ${t('contracts.sar')}` }}
              </strong>
            </div>

            <div class="p-2.5 bg-gradient-to-br from-emerald-600 to-teal-700 text-white rounded-xl shadow-xs">
              <span class="text-emerald-100 block text-[11px] font-bold mb-0.5">{{ t('contracts.reportMonthlyDueFromCustomer') }}</span>
              <strong class="text-white font-mono text-base font-black">
                {{ formatMoney(reportData.contract.monthlyRentWithTax || reportData.calculation.monthlyRentWithTax || reportData.calculation.adjustedMonthlyRent) }} {{ t('contracts.sar') }}
              </strong>
            </div>

            <div class="p-2.5 bg-white rounded-xl border border-slate-200">
              <span class="text-slate-500 block text-[11px] mb-0.5">{{ t('contracts.reportTotalContractWithTax') }}</span>
              <strong class="text-slate-900 font-mono text-sm font-black">
                {{ formatMoney(reportData.contract.totalPriceWithTax || reportData.calculation.totalPriceWithTax || reportData.contract.totalPrice) }} {{ t('contracts.sar') }}
              </strong>
            </div>
          </div>
        </div>
      </div>

      <!-- 7. Notes & Signatures -->
      <div class="break-inside-avoid space-y-6 pt-4 border-t border-slate-200">
        <div v-if="reportData.contract.notes" class="p-4 rounded-xl bg-slate-50 border border-slate-200 text-xs">
          <strong class="block mb-1 text-slate-800">{{ t('contracts.reportNotesTerms') }}</strong>
          <p class="text-slate-600 leading-relaxed">{{ reportData.contract.notes }}</p>
        </div>

        <!-- Official Signatures -->
        <div class="grid grid-cols-2 gap-8 text-center pt-8 text-xs font-bold text-slate-800">
          <div class="space-y-12">
            <p>{{ t('contracts.reportParty1Sign') }}</p>
            <div class="border-b border-dashed border-slate-400 w-3/4 mx-auto" />
            <p class="text-[11px] text-slate-400">{{ t('contracts.reportOfficialStamp') }}</p>
          </div>

          <div class="space-y-12">
            <p>{{ t('contracts.reportParty2Sign') }}</p>
            <div class="border-b border-dashed border-slate-400 w-3/4 mx-auto" />
            <p class="text-[11px] text-slate-400">{{ t('contracts.reportSignatureAndName') }}</p>
          </div>
        </div>

        <div class="text-center text-[10px] text-slate-400 pt-6">
          {{ t('contracts.reportGeneratedNote', { app: appName, date: reportData.generatedAt }) }}
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
