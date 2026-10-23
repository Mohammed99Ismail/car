<script setup lang="ts">
import {
  ArrowLeft24Regular as BackIcon,
  Checkmark24Regular as SaveIcon,
  VehicleCar24Regular as CarIcon,
  DocumentText24Regular as ContractIcon,
  People24Regular as CustomerIcon,
  Person24Regular as EmployeeIcon,
  Building24Regular as BranchIcon,
  Calendar24Regular as DateIcon,
  Money24Regular as MoneyIcon,
  Speedometer24Regular as MileageIcon,
  Shield24Regular as InsuranceIcon,
  Wrench24Regular as MaintenanceIcon,
  ArrowRepeatAll24Regular as ReplacementIcon,
  Briefcase24Regular as AdminFeesIcon,
  Warning24Regular as WarningIcon,
} from '@vicons/fluent'
import { useDebounceFn } from '@vueuse/core'
import type { FormInst, FormRules } from 'naive-ui'
import {
  NAlert,
  NBadge,
  NButton,
  NCard,
  NCol,
  NDatePicker,
  NDivider,
  NForm,
  NFormItem,
  NGi,
  NGrid,
  NIcon,
  NInput,
  NInputNumber,
  NRadio,
  NRadioButton,
  NRadioGroup,
  NRow,
  NSelect,
  NSpace,
  NSpin,
  NStatistic,
  NSwitch,
  NTable,
  NTag,
  NTooltip,
} from 'naive-ui'
import type { LookupItemDto } from '~/models/ApiResponse'
import { isApiSuccess } from '~/models/ApiResponse'
import type { CarDto } from '~/models/Car'
import type {
  ContractCalculationResultDto,
  ContractTermsConfigDto,
  CreateContractRequest,
} from '~/models/Contract'
import { useCarService } from '~/services/car.service'
import { useContractService } from '~/services/contract.service'
import { useCustomerService } from '~/services/customer.service'
import { useLookupService } from '~/services/lookup.service'
import { useUserService } from '~/services/user.service'

const router = useRouter()
const message = useMessage()

const contractService = useContractService()
const customerService = useCustomerService()
const userService = useUserService()
const carService = useCarService()
const lookupService = useLookupService()

function parseLocalDate(dateStr: string): number {
  const [y, m, d] = dateStr.split('-').map(Number)
  return new Date(y, m - 1, d, 0, 0, 0, 0).getTime()
}

function toDateOnlyIso(ts: number | Date | null | undefined): string {
  if (!ts) return ''
  const d = new Date(ts)
  const y = d.getFullYear()
  const m = String(d.getMonth() + 1).padStart(2, '0')
  const day = String(d.getDate()).padStart(2, '0')
  return `${y}-${m}-${day}T00:00:00`
}

// Form Data State
const form = reactive<{
  contractTypeId: number
  carId: number | null
  branchId: number | null
  customerId: number | null
  userId: number | null
  startDate: number
  endDate: number
  monthlyRent: number | null
  downPayment: number | null
  mileageLimit: number | null
  excessMileageFee: number | null
  insurancePercentage: number | null
  maintenancePercentage: number | null
  replacementCarPercentage: number | null
  adminFeesPercentage: number | null
  annualDepreciationPercentage: number | null
  applyTax: boolean
  taxPercentage: number
  notes: string
}>({
  contractTypeId: 3, // Default to Long-term
  carId: null,
  branchId: null,
  customerId: null,
  userId: null,
  startDate: parseLocalDate('2026-10-01'),
  endDate: parseLocalDate('2028-10-01'),
  monthlyRent: 1000,
  downPayment: 2000,
  mileageLimit: 10000,
  excessMileageFee: 0.50,
  insurancePercentage: 4,
  maintenancePercentage: 5,
  replacementCarPercentage: 1,
  adminFeesPercentage: 1,
  annualDepreciationPercentage: 25,
  applyTax: true,
  taxPercentage: 15,
  notes: '',
})

// Lookups
const branches = ref<LookupItemDto[]>([])
const customers = ref<LookupItemDto[]>([])
const users = ref<LookupItemDto[]>([])
const cars = ref<CarDto[]>([])
const termsConfig = ref<ContractTermsConfigDto | null>(null)

// Calculation Result
const calculation = ref<ContractCalculationResultDto | null>(null)
const calculating = ref(false)
const submitting = ref(false)

// Selected Car
const selectedCar = computed(() => {
  if (!form.carId) return null
  return cars.value.find(c => c.id === form.carId) || null
})

const scheduleTab = ref<'cards' | 'table'>('cards')

function formatMoney(val: number | null | undefined): string {
  if (val === null || val === undefined) return '0.00'
  return Number(val).toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 })
}

const netCarValueAtEnd = computed(() => {
  if (!calculation.value) return 0
  if (calculation.value.netCarValueAtEnd !== undefined && calculation.value.netCarValueAtEnd !== null && calculation.value.netCarValueAtEnd !== 0) {
    return calculation.value.netCarValueAtEnd
  }
  const sched = calculation.value.yearlySchedule
  if (sched && sched.length > 0) {
    return sched[sched.length - 1].netCarValue
  }
  return calculation.value.carValueUsed
})

const totalDepreciationCost = computed(() => {
  if (!calculation.value) return 0
  if (calculation.value.totalDepreciationCost !== undefined && calculation.value.totalDepreciationCost !== null && calculation.value.totalDepreciationCost !== 0) {
    return calculation.value.totalDepreciationCost
  }
  const sched = calculation.value.yearlySchedule
  if (sched && sched.length > 0) {
    return sched[sched.length - 1].depreciationCost
  }
  return 0
})

const totalInsuranceCost = computed(() => {
  if (!calculation.value) return 0
  if (calculation.value.totalInsuranceCost !== undefined && calculation.value.totalInsuranceCost !== null && calculation.value.totalInsuranceCost !== 0) {
    return calculation.value.totalInsuranceCost
  }
  const sched = calculation.value.yearlySchedule
  if (sched && sched.length > 0) {
    return sched[sched.length - 1].insuranceCost
  }
  return 0
})

const totalMaintenanceCost = computed(() => {
  if (!calculation.value) return 0
  if (calculation.value.totalMaintenanceCost !== undefined && calculation.value.totalMaintenanceCost !== null && calculation.value.totalMaintenanceCost !== 0) {
    return calculation.value.totalMaintenanceCost
  }
  const sched = calculation.value.yearlySchedule
  if (sched && sched.length > 0) {
    return sched[sched.length - 1].maintenanceCost
  }
  return 0
})

const totalReplacementCarCost = computed(() => {
  if (!calculation.value) return 0
  if (calculation.value.totalReplacementCarCost !== undefined && calculation.value.totalReplacementCarCost !== null && calculation.value.totalReplacementCarCost !== 0) {
    return calculation.value.totalReplacementCarCost
  }
  const sched = calculation.value.yearlySchedule
  if (sched && sched.length > 0) {
    return sched[sched.length - 1].replacementCarCost
  }
  return 0
})

const totalAdminFeesCost = computed(() => {
  if (!calculation.value) return 0
  if (calculation.value.totalAdminFeesCost !== undefined && calculation.value.totalAdminFeesCost !== null && calculation.value.totalAdminFeesCost !== 0) {
    return calculation.value.totalAdminFeesCost
  }
  const sched = calculation.value.yearlySchedule
  if (sched && sched.length > 0) {
    return sched[sched.length - 1].adminFeesCost
  }
  return 0
})

const monthlyCarCost = computed(() => {
  if (!calculation.value) return 0
  if (calculation.value.monthlyCarCost !== undefined && calculation.value.monthlyCarCost !== null && calculation.value.monthlyCarCost !== 0) {
    return calculation.value.monthlyCarCost
  }
  const sched = calculation.value.yearlySchedule
  if (sched && sched.length > 0) {
    return sched[sched.length - 1].monthlyCost
  }
  return 0
})

// Date disabling rule for end date
function isEndDateDisabled(ts: number): boolean {
  if (!form.startDate) return false
  const start = new Date(form.startDate)
  start.setHours(0, 0, 0, 0)

  // End date cannot be before or on the start date
  if (ts <= start.getTime()) return true

  // For Short-term / On-demand (type 2), cannot exceed 1 year
  if (form.contractTypeId === 2) {
    const maxEnd = new Date(start)
    maxEnd.setFullYear(start.getFullYear() + 1)
    return ts > maxEnd.getTime()
  }

  return false
}

// Quick duration helper (in months)
function setDurationMonths(months: number) {
  const start = new Date(form.startDate)
  const y = start.getFullYear()
  const m = start.getMonth()
  const d = start.getDate()
  const end = new Date(y, m + months, d, 0, 0, 0, 0)
  form.endDate = end.getTime()
  triggerCalculation()
}

// Watchers & Live Trigger
watch(
  () => form.contractTypeId,
  newType => {
    if (newType === 1) {
      // Service contract: no down payment, no monthly rent, user instead of customer
      form.downPayment = 0
      form.monthlyRent = 0
    } else if (newType === 2) {
      // Short-term: no down payment, duration <= 12 months
      form.downPayment = 0
      if (!form.monthlyRent || form.monthlyRent === 0) form.monthlyRent = 1000
      // adjust end date to 12 months max if greater or invalid
      const start = new Date(form.startDate)
      const maxEnd = new Date(start)
      maxEnd.setFullYear(start.getFullYear() + 1)
      if (form.endDate > maxEnd.getTime() || form.endDate <= form.startDate) {
        form.endDate = maxEnd.getTime()
      }
    } else if (newType === 3) {
      // Long-term: duration >= 12 months
      if (!form.monthlyRent || form.monthlyRent === 0) form.monthlyRent = 1000
      const start = new Date(form.startDate)
      const minEnd = new Date(start)
      minEnd.setFullYear(start.getFullYear() + 1)
      if (form.endDate < minEnd.getTime()) {
        const twoYears = new Date(start)
        twoYears.setFullYear(start.getFullYear() + 2)
        form.endDate = twoYears.getTime()
      }
    } else if (newType === 4) {
      // Lease to own
      if (!form.monthlyRent || form.monthlyRent === 0) form.monthlyRent = 1000
    }
    triggerCalculation()
  },
)

// Watch endDate to clamp if Short-Term exceeds 1 year
watch(
  () => form.endDate,
  newEnd => {
    if (form.contractTypeId === 2 && form.startDate && newEnd) {
      const start = new Date(form.startDate)
      const maxEnd = new Date(start)
      maxEnd.setFullYear(start.getFullYear() + 1)
      if (newEnd > maxEnd.getTime()) {
        form.endDate = maxEnd.getTime()
        message.warning('عقد الإيجار قصير الأجل أو حسب الطلب لا يمكن أن يتجاوز سنة واحدة (12 شهراً).')
      }
    }
  },
)

const triggerCalculation = useDebounceFn(async () => {
  if (!form.startDate || !form.endDate) return

  calculating.value = true
  try {
    const res = await contractService.calculate({
      contractTypeId: form.contractTypeId,
      carId: form.carId,
      carPurchasePrice: selectedCar.value?.purchasePrice ?? null,
      carPurchaseDate: selectedCar.value?.purchaseDate ?? null,
      carBookValue: selectedCar.value?.bookValue ?? null,
      startDate: toDateOnlyIso(form.startDate),
      endDate: toDateOnlyIso(form.endDate),
      monthlyRent: form.monthlyRent,
      downPayment: form.downPayment,
      mileageLimit: form.mileageLimit,
      excessMileageFee: form.excessMileageFee,
      insurancePercentage: form.insurancePercentage,
      maintenancePercentage: form.maintenancePercentage,
      replacementCarPercentage: form.replacementCarPercentage,
      adminFeesPercentage: form.adminFeesPercentage,
      annualDepreciationPercentage: form.annualDepreciationPercentage,
      applyTax: form.applyTax,
      taxPercentage: form.taxPercentage,
    })

    if (isApiSuccess(res) && res.data) {
      calculation.value = res.data
    }
  } catch (err) {
    console.error('Calculation error', err)
  } finally {
    calculating.value = false
  }
}, 300)

// Load Dependencies
async function loadInit() {
  try {
    const [brRes, cuRes, uRes, carsRes, tcRes] = await Promise.all([
      lookupService.getBranches(),
      customerService.getPaged({ pageIndex: 1, pageSize: 100 }),
      userService.getPaged({ pageIndex: 1, pageSize: 100 }),
      carService.getPaged({ pageIndex: 1, pageSize: 100 }),
      contractService.getTermsConfig(),
    ])

    if (isApiSuccess(brRes) && brRes.data) {
      branches.value = brRes.data
      if (branches.value.length > 0 && !form.branchId) {
        form.branchId = branches.value[0].id
      }
    }
    if (isApiSuccess(cuRes) && cuRes.data) {
      customers.value = cuRes.data.items.map(c => ({
        id: c.id,
        name: c.nationalId ? `${c.name} (${c.nationalId})` : (c.mobile ? `${c.name} (${c.mobile})` : c.name),
      }))
      if (customers.value.length > 0 && !form.customerId) {
        form.customerId = customers.value[0].id
      }
    }
    if (isApiSuccess(uRes) && uRes.data) {
      users.value = uRes.data.items.map(u => ({
        id: u.id,
        name: u.fullName ? `${u.fullName} (${u.email})` : u.email,
      }))
      if (users.value.length > 0 && !form.userId) {
        form.userId = users.value[0].id
      }
    }
    if (isApiSuccess(carsRes) && carsRes.data) {
      cars.value = carsRes.data.items
      // If cars exist, pick the first or example car
      if (cars.value.length > 0 && !form.carId) {
        form.carId = cars.value[0].id
      }
    }
    if (isApiSuccess(tcRes) && tcRes.data) {
      termsConfig.value = tcRes.data
      form.insurancePercentage = tcRes.data.insurancePercentage
      form.maintenancePercentage = tcRes.data.maintenancePercentage
      form.replacementCarPercentage = tcRes.data.replacementCarPercentage
      form.adminFeesPercentage = tcRes.data.adminFeesPercentage
      form.annualDepreciationPercentage = tcRes.data.annualDepreciationPercentage
    }

    triggerCalculation()
  } catch (err) {
    message.error('فشل تحميل البيانات الأساسية')
  }
}

// Submit Create
async function handleCreate() {
  if (!form.carId) {
    message.warning('يرجى اختيار السيارة أولاً')
    return
  }
  if (!form.branchId) {
    message.warning('يرجى اختيار الفرع')
    return
  }
  if (form.contractTypeId === 1 && !form.userId) {
    message.warning('عقد الخدمة يتطلب اختيار الموظف المستفيد')
    return
  }
  if (form.contractTypeId > 1 && !form.customerId) {
    message.warning('عقد الإيجار يتطلب اختيار العميل المستفيد')
    return
  }

  // Duration validations
  const diffDays = (form.endDate - form.startDate) / (1000 * 3600 * 24)
  if (form.contractTypeId === 2 && diffDays > 366) {
    message.error('عقد الإيجار قصير الأجل يجب ألا يتجاوز سنة واحدة (12 شهراً)')
    return
  }
  if (form.contractTypeId === 3 && diffDays < 360) {
    message.error('عقد الإيجار طويل الأجل يجب أن يكون سنة واحدة على الأقل (12 شهراً)')
    return
  }

  submitting.value = true
  try {
    const payload: CreateContractRequest = {
      contractTypeId: form.contractTypeId,
      carId: form.carId,
      branchId: form.branchId,
      customerId: form.contractTypeId === 1 ? null : form.customerId,
      userId: form.contractTypeId === 1 ? form.userId : null,
      startDate: toDateOnlyIso(form.startDate)!,
      endDate: toDateOnlyIso(form.endDate)!,
      monthlyRent: form.monthlyRent,
      downPayment: form.downPayment,
      mileageLimit: form.mileageLimit,
      excessMileageFee: form.excessMileageFee,
      insurancePercentage: form.insurancePercentage,
      maintenancePercentage: form.maintenancePercentage,
      replacementCarPercentage: form.replacementCarPercentage,
      adminFeesPercentage: form.adminFeesPercentage,
      annualDepreciationPercentage: form.annualDepreciationPercentage,
      applyTax: form.applyTax,
      taxPercentage: form.taxPercentage,
      notes: form.notes,
    }

    const res = await contractService.create(payload)
    if (isApiSuccess(res) && res.data) {
      message.success(`تم إنشاء العقد رقم "${res.data.contractNumber}" بنجاح!`)
      router.push(`/contracts/${res.data.id}/report`)
    } else {
      message.error(res.message || 'فشل إنشاء العقد')
    }
  } catch (err: any) {
    message.error('حدث خطأ أثناء حفظ العقد')
  } finally {
    submitting.value = false
  }
}

onMounted(() => {
  loadInit()
})
</script>

<template>
  <div class="p-4 md:p-6 space-y-6">
    <!-- Header -->
    <div class="flex flex-col md:flex-row md:items-center justify-between gap-4 bg-white dark:bg-slate-800 p-6 rounded-2xl shadow-sm border border-slate-100 dark:border-slate-700/60">
      <div class="flex items-center gap-3">
        <NButton
          secondary
          class="rounded-xl"
          @click="router.push('/contracts')"
        >
          <template #icon>
            <NIcon><BackIcon /></NIcon>
          </template>
        </NButton>
        <div>
          <h1 class="text-xl md:text-2xl font-black text-slate-800 dark:text-white tracking-tight flex items-center gap-2">
            إنشاء عقد سيارة جديد
          </h1>
          <p class="text-xs md:text-sm text-slate-500 dark:text-slate-400 mt-0.5">
            تحديد نوع العقد، السيارة، المستفيد، والمدد مع حسابات إهلاك لحظية دقيقة
          </p>
        </div>
      </div>

      <div class="flex items-center gap-3">
        <NButton
          secondary
          class="rounded-xl"
          @click="router.push('/contracts')"
        >
          إلغاء
        </NButton>

        <NButton
          type="primary"
          class="font-bold shadow-lg shadow-blue-500/20 rounded-xl px-6"
          :loading="submitting"
          @click="handleCreate"
        >
          <template #icon>
            <NIcon><SaveIcon /></NIcon>
          </template>
          حفظ وإصدار العقد
        </NButton>
      </div>
    </div>

    <!-- Main Two-Column Layout -->
    <div class="grid grid-cols-1 lg:grid-cols-12 gap-6 items-start">
      <!-- Left Column: Interactive Form (7 Cols) -->
      <div class="lg:col-span-7 space-y-6">
        <!-- 1. Contract Type Selection -->
        <div class="bg-white dark:bg-slate-800 p-5 rounded-2xl shadow-sm border border-slate-100 dark:border-slate-700/60 space-y-4">
          <h2 class="text-sm font-bold text-slate-800 dark:text-slate-100 flex items-center gap-2">
            <span class="w-6 h-6 rounded-lg bg-blue-100 dark:bg-blue-900/50 text-blue-600 dark:text-blue-300 flex items-center justify-center text-xs font-black">1</span>
            تحديد نوع العقد المطلوب
          </h2>

          <div class="grid grid-cols-1 sm:grid-cols-2 gap-3">
            <!-- Type 1: Service -->
            <div
              class="cursor-pointer p-4 rounded-xl border-2 transition-all flex flex-col justify-between"
              :class="form.contractTypeId === 1 ? 'border-purple-600 bg-purple-50/40 dark:bg-purple-950/20 shadow-sm' : 'border-slate-200 dark:border-slate-700/60 hover:border-slate-300'"
              @click="form.contractTypeId = 1"
            >
              <div>
                <div class="flex items-center justify-between mb-2">
                  <span class="text-base">🛠️</span>
                  <NTag size="small" type="info" :bordered="false" round>عقد خدمة</NTag>
                </div>
                <h3 class="font-bold text-sm text-slate-800 dark:text-slate-100">عقد خدمة (معرض)</h3>
                <p class="text-xs text-slate-500 dark:text-slate-400 mt-1">
                  السيارة مخصصة لموظف المعرض، لا توجد دفعة مقدمة ولا أقساط شهرية.
                </p>
              </div>
            </div>

            <!-- Type 2: Short-term -->
            <div
              class="cursor-pointer p-4 rounded-xl border-2 transition-all flex flex-col justify-between"
              :class="form.contractTypeId === 2 ? 'border-amber-500 bg-amber-50/40 dark:bg-amber-950/20 shadow-sm' : 'border-slate-200 dark:border-slate-700/60 hover:border-slate-300'"
              @click="form.contractTypeId = 2"
            >
              <div>
                <div class="flex items-center justify-between mb-2">
                  <span class="text-base">⏱️</span>
                  <NTag size="small" type="warning" :bordered="false" round>سنة أو أقل</NTag>
                </div>
                <h3 class="font-bold text-sm text-slate-800 dark:text-slate-100">إيجار قصير الأجل / حسب الطلب</h3>
                <p class="text-xs text-slate-500 dark:text-slate-400 mt-1">
                  مدة &le; 12 شهراً، بدون دفعة مقدمة، يحدد حد الكيلومتر وتطبق البنود الـ 4.
                </p>
              </div>
            </div>

            <!-- Type 3: Long-term -->
            <div
              class="cursor-pointer p-4 rounded-xl border-2 transition-all flex flex-col justify-between"
              :class="form.contractTypeId === 3 ? 'border-blue-600 bg-blue-50/40 dark:bg-blue-950/20 shadow-sm' : 'border-slate-200 dark:border-slate-700/60 hover:border-slate-300'"
              @click="form.contractTypeId = 3"
            >
              <div>
                <div class="flex items-center justify-between mb-2">
                  <span class="text-base">📅</span>
                  <NTag size="small" type="primary" :bordered="false" round>سنة فأكثر</NTag>
                </div>
                <h3 class="font-bold text-sm text-slate-800 dark:text-slate-100">إيجار طويل الأجل</h3>
                <p class="text-xs text-slate-500 dark:text-slate-400 mt-1">
                  مدة &ge; 12 شهراً، يدعم الدفعة المقدمة والإيجار، وتطبق البنود الـ 4.
                </p>
              </div>
            </div>

            <!-- Type 4: Lease-to-own -->
            <div
              class="cursor-pointer p-4 rounded-xl border-2 transition-all flex flex-col justify-between"
              :class="form.contractTypeId === 4 ? 'border-emerald-600 bg-emerald-50/40 dark:bg-emerald-950/20 shadow-sm' : 'border-slate-200 dark:border-slate-700/60 hover:border-slate-300'"
              @click="form.contractTypeId = 4"
            >
              <div>
                <div class="flex items-center justify-between mb-2">
                  <span class="text-base">🔑</span>
                  <NTag size="small" type="success" :bordered="false" round>منتهي بالتمليك</NTag>
                </div>
                <h3 class="font-bold text-sm text-slate-800 dark:text-slate-100">إيجار منتهي بالتمليك</h3>
                <p class="text-xs text-slate-500 dark:text-slate-400 mt-1">
                  يدعم الدفعة المقدمة والإيجار الشهري، ولا تخصم منه البنود الأربعة.
                </p>
              </div>
            </div>
          </div>
        </div>

        <!-- 2. Vehicle Selection & Book Value Card -->
        <div class="bg-white dark:bg-slate-800 p-5 rounded-2xl shadow-sm border border-slate-100 dark:border-slate-700/60 space-y-4">
          <h2 class="text-sm font-bold text-slate-800 dark:text-slate-100 flex items-center justify-between">
            <span class="flex items-center gap-2">
              <span class="w-6 h-6 rounded-lg bg-blue-100 dark:bg-blue-900/50 text-blue-600 dark:text-blue-300 flex items-center justify-center text-xs font-black">2</span>
              اختيار السيارة والقيمة الدفترية
            </span>
            <span class="text-xs text-slate-400">بحث باللوحة أو الموديل</span>
          </h2>

          <NSelect
            v-model:value="form.carId"
            :options="cars.map(c => ({
              label: `${c.plateNumber} — ${c.makeName} ${c.modelName} (${c.manufactureYear}) [دفترية: ${c.bookValue.toLocaleString()} ر.س]`,
              value: c.id
            }))"
            placeholder="ابحث برقم لوحة السيارة (حروف وأرقام) أو الموديل..."
            filterable
            class="rounded-xl"
            @update:value="triggerCalculation"
          />

          <!-- Selected Car Breakdown Card -->
          <div v-if="selectedCar" class="bg-gradient-to-br from-slate-50 to-blue-50/30 dark:from-slate-900/70 dark:to-blue-950/20 p-4 rounded-xl border border-blue-200/50 dark:border-blue-900/40 space-y-3">
            <div class="flex items-center justify-between">
              <div class="flex items-center gap-2">
                <span class="text-xl">🚗</span>
                <div>
                  <h4 class="font-black text-slate-800 dark:text-slate-100 text-sm">
                    {{ selectedCar.makeName }} {{ selectedCar.modelName }} ({{ selectedCar.manufactureYear }})
                  </h4>
                  <p class="text-xs text-slate-500">
                    اللون: {{ selectedCar.color }} • الشاسيه: {{ selectedCar.chassisNumber }}
                  </p>
                </div>
              </div>

              <!-- Plate Graphic -->
              <div class="px-3 py-1 bg-amber-50 border-2 border-amber-400 rounded-lg text-center shadow-sm">
                <span class="text-[10px] block text-slate-400 uppercase leading-none">KSA</span>
                <span class="font-mono font-black text-amber-900 text-sm tracking-wider">{{ selectedCar.plateNumber }}</span>
              </div>
            </div>

            <NDivider class="!my-2" />

            <!-- Book Value & Depreciation Stats -->
            <div class="grid grid-cols-2 sm:grid-cols-4 gap-2 text-center">
              <div class="bg-white/80 dark:bg-slate-800/80 p-2.5 rounded-lg border border-slate-200/50 dark:border-slate-700/50">
                <span class="text-[11px] text-slate-500 block">تاريخ الشراء</span>
                <span class="font-bold text-xs text-slate-700 dark:text-slate-200">
                  {{ new Date(selectedCar.purchaseDate).toLocaleDateString('ar-SA') }}
                </span>
              </div>

              <div class="bg-white/80 dark:bg-slate-800/80 p-2.5 rounded-lg border border-slate-200/50 dark:border-slate-700/50">
                <span class="text-[11px] text-slate-500 block">القيمة الشرائية</span>
                <span class="font-bold text-xs text-slate-800 dark:text-white">
                  {{ selectedCar.purchasePrice.toLocaleString() }} ر.س
                </span>
              </div>

              <div class="bg-white/80 dark:bg-slate-800/80 p-2.5 rounded-lg border border-slate-200/50 dark:border-slate-700/50">
                <span class="text-[11px] text-slate-500 block">مجمع الإهلاك</span>
                <span class="font-bold text-xs text-amber-600 dark:text-amber-400">
                  {{ selectedCar.accumulatedDepreciation.toLocaleString() }} ر.س
                </span>
              </div>

              <div class="bg-white/80 dark:bg-slate-800/80 p-2.5 rounded-lg border-2 border-blue-500/40 dark:border-blue-500/50">
                <span class="text-[11px] font-bold text-blue-700 dark:text-blue-300 block">القيمة الدفترية</span>
                <span class="font-black text-sm text-blue-600 dark:text-blue-400">
                  {{ selectedCar.bookValue.toLocaleString() }} ر.س
                </span>
              </div>
            </div>

            <p class="text-[11px] text-slate-500 dark:text-slate-400">
              * العمر الإنتاجي المقدر للسيارة: <strong class="text-slate-700 dark:text-slate-200">{{ selectedCar.productiveAge }} سنة</strong>
              <span v-if="selectedCar.productiveAge >= 4" class="text-amber-600 dark:text-amber-400 font-semibold mr-1">
                (تجاوزت 4 سنوات فأصبحت قيمتها الدفترية 1 ريال)
              </span>
            </p>
          </div>
        </div>

        <!-- 3. Beneficiary & Branch -->
        <div class="bg-white dark:bg-slate-800 p-5 rounded-2xl shadow-sm border border-slate-100 dark:border-slate-700/60 space-y-4">
          <h2 class="text-sm font-bold text-slate-800 dark:text-slate-100 flex items-center gap-2">
            <span class="w-6 h-6 rounded-lg bg-blue-100 dark:bg-blue-900/50 text-blue-600 dark:text-blue-300 flex items-center justify-center text-xs font-black">3</span>
            المستفيد والفرع المسؤول
          </h2>

          <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
            <!-- If Service: User selection -->
            <div v-if="form.contractTypeId === 1">
              <label class="text-xs font-bold text-slate-700 dark:text-slate-300 block mb-1.5">
                الموظف / مستخدم النظام المستفيد *
              </label>
              <NSelect
                v-model:value="form.userId"
                :options="users.map(u => ({ label: u.name, value: u.id }))"
                placeholder="اختر الموظف المستفيد من المعرض..."
                filterable
              />
            </div>

            <!-- If Rental / Lease-to-own: Customer selection -->
            <div v-else>
              <label class="text-xs font-bold text-slate-700 dark:text-slate-300 block mb-1.5">
                العميل المستفيد *
              </label>
              <NSelect
                v-model:value="form.customerId"
                :options="customers.map(c => ({ label: c.name, value: c.id }))"
                placeholder="اختر العميل المستفيد..."
                filterable
              />
            </div>

            <!-- Branch -->
            <div>
              <label class="text-xs font-bold text-slate-700 dark:text-slate-300 block mb-1.5">
                فرع المعرض *
              </label>
              <NSelect
                v-model:value="form.branchId"
                :options="branches.map(b => ({ label: b.name, value: b.id }))"
                placeholder="اختر الفرع..."
              />
            </div>
          </div>
        </div>

        <!-- 4. Contract Period & Dates -->
        <div class="bg-white dark:bg-slate-800 p-5 rounded-2xl shadow-sm border border-slate-100 dark:border-slate-700/60 space-y-4">
          <h2 class="text-sm font-bold text-slate-800 dark:text-slate-100 flex items-center justify-between">
            <span class="flex items-center gap-2">
              <span class="w-6 h-6 rounded-lg bg-blue-100 dark:bg-blue-900/50 text-blue-600 dark:text-blue-300 flex items-center justify-center text-xs font-black">4</span>
              فترة العقد والتواريخ
            </span>

            <!-- Quick Duration Buttons: Only show for Long-term (type 3) and Lease-to-own (type 4) -->
            <div v-if="form.contractTypeId !== 2 && form.contractTypeId !== 1" class="flex items-center gap-1.5">
              <NButton size="tiny" secondary round @click="setDurationMonths(12)">سنة</NButton>
              <NButton size="tiny" secondary round @click="setDurationMonths(18)">سنة ونصف</NButton>
              <NButton size="tiny" secondary round @click="setDurationMonths(24)">سنتان</NButton>
              <NButton size="tiny" secondary round @click="setDurationMonths(36)">3 سنوات</NButton>
            </div>
            <div v-else-if="form.contractTypeId === 2" class="text-xs font-semibold text-amber-600 dark:text-amber-400 bg-amber-50 dark:bg-amber-950/40 px-2.5 py-1 rounded-lg border border-amber-200/60 dark:border-amber-800/40">
              عقد قصير الأجل: الحد الأقصى سنة واحدة (12 شهراً)
            </div>
          </h2>

          <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
            <div>
              <label class="text-xs font-bold text-slate-700 dark:text-slate-300 block mb-1.5">تاريخ البداية</label>
              <NDatePicker
                v-model:value="form.startDate"
                type="date"
                class="w-full"
                @update:value="triggerCalculation"
              />
            </div>

            <div>
              <label class="text-xs font-bold text-slate-700 dark:text-slate-300 block mb-1.5">تاريخ النهاية</label>
              <NDatePicker
                v-model:value="form.endDate"
                type="date"
                class="w-full"
                :is-date-disabled="isEndDateDisabled"
                @update:value="triggerCalculation"
              />
            </div>
          </div>

          <!-- Duration indicators & warnings -->
          <div v-if="calculation" class="p-3 bg-slate-50 dark:bg-slate-900/40 rounded-xl text-xs space-y-2">
            <div class="flex flex-wrap items-center justify-between gap-2">
              <span class="font-semibold text-slate-600 dark:text-slate-300">
                المدة المحسوبة: <strong class="text-blue-600">{{ calculation.periodMonths }} شهر</strong> ({{ calculation.periodDays }} يوم)
              </span>
              <span class="font-semibold text-slate-600 dark:text-slate-300">
                سنوات العقد للإهلاك: <strong class="text-purple-600">{{ calculation.contractYears }} سنة</strong>
              </span>
            </div>

            <div v-if="form.contractTypeId === 2 && calculation.dailyRent" class="pt-2 border-t border-slate-200/60 dark:border-slate-800 flex flex-wrap items-center justify-between gap-2 text-slate-700 dark:text-slate-200">
              <span class="flex items-center gap-1">
                <span>الإيجار اليومي:</span>
                <strong class="text-emerald-600 font-mono font-bold">{{ formatMoney(calculation.dailyRent) }} ر.س/يوم</strong>
                <span class="text-[10px] text-slate-400">({{ calculation.monthlyRentBeforeDownPayment }} ÷ 30)</span>
              </span>
              <span class="flex items-center gap-1">
                <span>أيام الإيجار الفعلي:</span>
                <strong class="text-blue-600 font-mono font-bold">{{ calculation.rentDays }} يوم</strong>
                <span class="text-[10px] text-slate-400">(بدون بداية ونهاية اليوم)</span>
              </span>
              <span class="flex items-center gap-1">
                <span>الإيجار المستحق:</span>
                <strong class="text-emerald-700 dark:text-emerald-400 font-mono font-black text-sm">{{ formatMoney(calculation.totalEstimatedRevenue) }} ر.س</strong>
              </span>
            </div>
          </div>

          <div v-if="calculation?.warnings && calculation.warnings.length > 0" class="space-y-1.5">
            <NAlert
              v-for="(w, idx) in calculation.warnings"
              :key="idx"
              type="warning"
              class="rounded-xl"
            >
              {{ w }}
            </NAlert>
          </div>
        </div>

        <!-- 5. Financials & Mileage -->
        <div class="bg-white dark:bg-slate-800 p-5 rounded-2xl shadow-sm border border-slate-100 dark:border-slate-700/60 space-y-4">
          <h2 class="text-sm font-bold text-slate-800 dark:text-slate-100 flex items-center gap-2">
            <span class="w-6 h-6 rounded-lg bg-blue-100 dark:bg-blue-900/50 text-blue-600 dark:text-blue-300 flex items-center justify-center text-xs font-black">5</span>
            البيانات المالية وشروط المسافة
          </h2>

          <div class="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-4 gap-4">
            <!-- Monthly Rent -->
            <div>
              <label class="text-xs font-bold text-slate-700 dark:text-slate-300 block mb-1.5">
                قسط الإيجار الشهري
              </label>
              <NInputNumber
                v-model:value="form.monthlyRent"
                :min="0"
                :step="50"
                class="w-full"
                :disabled="form.contractTypeId === 1"
                @update:value="triggerCalculation"
              >
                <template #suffix>ر.س</template>
              </NInputNumber>
            </div>

            <!-- Down Payment -->
            <div>
              <label class="text-xs font-bold text-slate-700 dark:text-slate-300 block mb-1.5">
                الدفعة المقدمة
              </label>
              <NInputNumber
                v-model:value="form.downPayment"
                :min="0"
                :step="100"
                class="w-full"
                :disabled="form.contractTypeId === 1 || form.contractTypeId === 2"
                @update:value="triggerCalculation"
              >
                <template #suffix>ر.س</template>
              </NInputNumber>
              <span v-if="form.contractTypeId === 2" class="text-[10px] text-amber-600 block mt-0.5">
                لا توجد دفعة مقدمة في قصير الأجل
              </span>
            </div>

            <!-- Mileage Limit -->
            <div>
              <label class="text-xs font-bold text-slate-700 dark:text-slate-300 block mb-1.5">
                الحد الأقصى للمسافة
              </label>
              <NInputNumber
                v-model:value="form.mileageLimit"
                :min="0"
                :step="1000"
                class="w-full"
                :disabled="form.contractTypeId === 1"
                @update:value="triggerCalculation"
              >
                <template #suffix>كم</template>
              </NInputNumber>
            </div>

            <!-- Excess Mileage Fee -->
            <div>
              <label class="text-xs font-bold text-slate-700 dark:text-slate-300 block mb-1.5">
                رسوم الكيلومتر الزائد
              </label>
              <NInputNumber
                v-model:value="form.excessMileageFee"
                :min="0"
                :step="0.1"
                class="w-full"
                :disabled="form.contractTypeId === 1"
                @update:value="triggerCalculation"
              >
                <template #suffix>ر.س/كم</template>
              </NInputNumber>
            </div>
          </div>

          <!-- Tax (VAT 15%) Option & Breakdown Card -->
          <div v-if="form.contractTypeId !== 1" class="mt-4 pt-4 border-t border-slate-100 dark:border-slate-700/60">
            <div class="p-4 rounded-2xl bg-gradient-to-r from-blue-50/70 via-indigo-50/50 to-blue-50/70 dark:from-slate-800/90 dark:via-indigo-950/40 dark:to-slate-800/90 border border-blue-200/70 dark:border-indigo-800/60 shadow-xs space-y-3">
              <!-- Switch Row -->
              <div class="flex flex-col sm:flex-row sm:items-center justify-between gap-3">
                <div class="flex items-center gap-3">
                  <div class="w-10 h-10 rounded-xl bg-blue-600/10 dark:bg-blue-400/20 text-blue-600 dark:text-blue-300 flex items-center justify-center text-lg">
                    🧾
                  </div>
                  <div>
                    <div class="flex items-center gap-2">
                      <h4 class="font-bold text-sm text-slate-800 dark:text-slate-100">
                        تطبيق ضريبة القيمة المضافة (15%) على عقد الإيجار
                      </h4>
                      <NTag :type="form.applyTax ? 'info' : 'default'" size="small" round :bordered="false" class="font-bold text-xs">
                        {{ form.applyTax ? 'مطبقة 15%' : 'غير مطبقة (معفى)' }}
                      </NTag>
                    </div>
                    <p class="text-xs text-slate-500 dark:text-slate-400 mt-0.5">
                      تُحمل الضريبة المضافة (15%) على العميل المستفيد وتُضاف فوق قيمة قسط الإيجار.
                    </p>
                  </div>
                </div>

                <div class="flex items-center gap-2 pr-2 sm:pr-0">
                  <NSwitch
                    v-model:value="form.applyTax"
                    size="large"
                    @update:value="triggerCalculation"
                  >
                    <template #checked>مطبقة</template>
                    <template #unchecked>معفى</template>
                  </NSwitch>
                </div>
              </div>

              <!-- Tax Breakdown Live Strip -->
              <div v-if="form.applyTax && calculation" class="grid grid-cols-1 sm:grid-cols-3 gap-2.5 pt-2 border-t border-blue-200/50 dark:border-indigo-900/50 text-xs">
                <div class="bg-white/80 dark:bg-slate-900/70 p-2.5 rounded-xl border border-slate-200/60 dark:border-slate-700/60">
                  <span class="text-slate-500 block text-[11px]">الإيجار الشهري قبل الضريبة:</span>
                  <strong class="text-slate-800 dark:text-slate-200 font-mono text-sm">
                    {{ formatMoney(calculation.adjustedMonthlyRent) }} ر.س
                  </strong>
                </div>

                <div class="bg-white/80 dark:bg-slate-900/70 p-2.5 rounded-xl border border-blue-200 dark:border-blue-800/60">
                  <span class="text-blue-600 dark:text-blue-400 block text-[11px] font-semibold">ضريبة القيمة المضافة (15%):</span>
                  <strong class="text-blue-700 dark:text-blue-300 font-mono text-sm">
                    +{{ formatMoney(calculation.monthlyTaxAmount) }} ر.س
                  </strong>
                </div>

                <div class="bg-blue-600 text-white p-2.5 rounded-xl shadow-xs">
                  <span class="text-blue-100 block text-[11px] font-semibold">المستحق على العميل شهرياً (شامل الضريبة):</span>
                  <strong class="text-white font-mono text-base font-black">
                    {{ formatMoney(calculation.monthlyRentWithTax) }} ر.س
                  </strong>
                </div>
              </div>

              <div v-else-if="!form.applyTax" class="p-2.5 rounded-xl bg-slate-100/80 dark:bg-slate-900/40 text-xs text-slate-600 dark:text-slate-400">
                ℹ️ الضريبة غير مطبقة على هذا العقد — يدفع العميل القسط الأساسي فقط (<strong>{{ formatMoney(calculation?.adjustedMonthlyRent || form.monthlyRent) }} ر.س</strong>) دون إضافة أي ضريبة.
              </div>
            </div>
          </div>
        </div>

        <!-- 6. The 4 Deduction Terms (Types 2 & 3) -->
        <div
          v-if="form.contractTypeId === 2 || form.contractTypeId === 3"
          class="bg-white dark:bg-slate-800 p-5 rounded-2xl shadow-sm border border-slate-100 dark:border-slate-700/60 space-y-4"
        >
          <div class="flex items-center justify-between">
            <h2 class="text-sm font-bold text-slate-800 dark:text-slate-100 flex items-center gap-2">
              <span class="w-6 h-6 rounded-lg bg-blue-100 dark:bg-blue-900/50 text-blue-600 dark:text-blue-300 flex items-center justify-center text-xs font-black">6</span>
              نسب البنود الأربعة للإهلاك (تطبق سنوياً)
            </h2>
            <span class="text-xs text-blue-600 font-semibold">قيم قابلة للتعديل لكل عقد</span>
          </div>

          <div class="grid grid-cols-2 sm:grid-cols-4 gap-3">
            <div>
              <label class="text-xs text-slate-600 dark:text-slate-400 block mb-1 font-semibold">التأمين الشامل (%)</label>
              <NInputNumber
                v-model:value="form.insurancePercentage"
                :min="0"
                :max="100"
                :step="0.5"
                class="w-full"
                @update:value="triggerCalculation"
              >
                <template #suffix>%</template>
              </NInputNumber>
            </div>

            <div>
              <label class="text-xs text-slate-600 dark:text-slate-400 block mb-1 font-semibold">الصيانة (%)</label>
              <NInputNumber
                v-model:value="form.maintenancePercentage"
                :min="0"
                :max="100"
                :step="0.5"
                class="w-full"
                @update:value="triggerCalculation"
              >
                <template #suffix>%</template>
              </NInputNumber>
            </div>

            <div>
              <label class="text-xs text-slate-600 dark:text-slate-400 block mb-1 font-semibold">سيارة بديلة (%)</label>
              <NInputNumber
                v-model:value="form.replacementCarPercentage"
                :min="0"
                :max="100"
                :step="0.5"
                class="w-full"
                @update:value="triggerCalculation"
              >
                <template #suffix>%</template>
              </NInputNumber>
            </div>

            <div>
              <label class="text-xs text-slate-600 dark:text-slate-400 block mb-1 font-semibold">مصاريف إدارية (%)</label>
              <NInputNumber
                v-model:value="form.adminFeesPercentage"
                :min="0"
                :max="100"
                :step="0.5"
                class="w-full"
                @update:value="triggerCalculation"
              >
                <template #suffix>%</template>
              </NInputNumber>
            </div>
          </div>
        </div>

        <!-- 6. Lease-to-Own Deduction Terms (Type 4: Insurance Only + 100% Depreciation) -->
        <div
          v-if="form.contractTypeId === 4"
          class="bg-white dark:bg-slate-800 p-5 rounded-2xl shadow-sm border border-slate-100 dark:border-slate-700/60 space-y-4"
        >
          <div class="flex items-center justify-between">
            <h2 class="text-sm font-bold text-slate-800 dark:text-slate-100 flex items-center gap-2">
              <span class="w-6 h-6 rounded-lg bg-emerald-100 dark:bg-emerald-900/50 text-emerald-600 dark:text-emerald-300 flex items-center justify-center text-xs font-black">6</span>
              تكاليف عقد الإيجار المنتهي بالتمليك
            </h2>
            <span class="text-xs text-emerald-600 font-semibold">تأمين شامل + إهلاك كامل القيمة الدفترية فقط</span>
          </div>

          <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
            <div>
              <label class="text-xs text-slate-600 dark:text-slate-400 block mb-1 font-semibold">نسبة التأمين الشامل (%)</label>
              <NInputNumber
                v-model:value="form.insurancePercentage"
                :min="0"
                :max="100"
                :step="0.5"
                class="w-full"
                @update:value="triggerCalculation"
              >
                <template #suffix>%</template>
              </NInputNumber>
            </div>

            <div class="p-3 bg-emerald-50 dark:bg-emerald-950/30 rounded-xl border border-emerald-200/60 dark:border-emerald-800/40 text-xs text-emerald-900 dark:text-emerald-200 flex flex-col justify-center">
              <div class="font-bold flex items-center gap-1.5 mb-1">
                <span>🔑</span>
                <span>إهلاك السيارة في نهاية العقد:</span>
              </div>
              <p class="leading-relaxed">
                يساوي <strong>كامل القيمة الدفترية للسيارة (100%)</strong> وتصبح قيمتها المتبقية للشركة (0 ر.س) لانتقال ملكيتها للعميل، ولا توجد بنود صيانة أو سيارة بديلة أو مصاريف إدارية دورية.
              </p>
            </div>
          </div>
        </div>

        <!-- 7. Notes -->
        <div class="bg-white dark:bg-slate-800 p-5 rounded-2xl shadow-sm border border-slate-100 dark:border-slate-700/60 space-y-2">
          <label class="text-xs font-bold text-slate-700 dark:text-slate-300 block">شروط وملاحظات إضافية</label>
          <NInput
            v-model:value="form.notes"
            type="textarea"
            :rows="2"
            placeholder="أدخل أي شروط خاصة أو ملاحظات إضافية للعقد..."
            class="rounded-xl"
          />
        </div>
      </div>

      <!-- Right Column: Real-Time Live Calculation Engine (5 Cols Sticky) -->
      <div class="lg:col-span-5 sticky top-6 space-y-6">
        <div class="bg-gradient-to-br from-slate-900 via-indigo-950 to-slate-900 text-white p-5 sm:p-6 rounded-3xl shadow-2xl space-y-5 border border-indigo-800/60">
          <!-- Header -->
          <div class="flex items-center justify-between border-b border-indigo-800/60 pb-4">
            <div class="flex items-center gap-2.5">
              <span class="text-2xl animate-pulse">⚡</span>
              <div>
                <h3 class="font-black text-base text-white tracking-tight">محرك الحسابات اللحظية</h3>
                <p class="text-[11px] text-indigo-300">يُحسب فورياً مع كل إدخال وتعديل وفق الشروط المعتمدة</p>
              </div>
            </div>
            <NSpin v-if="calculating" size="small" />
          </div>

          <div v-if="calculation" class="space-y-4">
            <!-- 1. Major Revenue, Cost & Profit Cards -->
            <div class="grid grid-cols-3 gap-2">
              <div class="bg-white/10 p-2.5 rounded-2xl backdrop-blur-sm border border-white/5 flex flex-col justify-between">
                <span class="text-[10px] text-indigo-200 font-semibold block leading-tight">
                  {{ form.contractTypeId === 2 ? 'إجمالي الإيجار المستحق' : 'إجمالي الإيرادات التقديرية' }}
                </span>
                <div class="mt-1">
                  <span class="text-base font-black text-white tracking-tight">{{ formatMoney(calculation.totalEstimatedRevenue) }}</span>
                  <span class="text-[9px] text-indigo-300 mr-0.5">ر.س</span>
                </div>
              </div>

              <div class="bg-rose-950/40 p-2.5 rounded-2xl backdrop-blur-sm border border-rose-500/20 flex flex-col justify-between">
                <span class="text-[10px] text-rose-200 font-semibold block leading-tight">إجمالي التكاليف التقديرية</span>
                <div class="mt-1">
                  <span class="text-base font-black text-rose-300 tracking-tight">{{ formatMoney(calculation.totalEstimatedCost) }}</span>
                  <span class="text-[9px] text-rose-300 mr-0.5">ر.س</span>
                </div>
              </div>

              <div class="bg-emerald-950/40 p-2.5 rounded-2xl backdrop-blur-sm border border-emerald-500/20 flex flex-col justify-between">
                <span class="text-[10px] text-emerald-200 font-semibold block leading-tight">إجمالي الأرباح التقديرية</span>
                <div class="mt-1">
                  <span class="text-base font-black text-emerald-400 tracking-tight">{{ formatMoney(calculation.expectedProfit) }}</span>
                  <span class="text-[9px] text-emerald-300 mr-0.5">ر.س</span>
                </div>
              </div>
            </div>

            <!-- Short-term daily breakdown card -->
            <div v-if="form.contractTypeId === 2 && calculation.dailyRent" class="bg-amber-950/40 border border-amber-500/30 rounded-2xl p-3 text-xs space-y-1.5">
              <div class="flex items-center justify-between text-amber-300 font-bold text-[11px] pb-1 border-b border-amber-500/20">
                <span>⏱️ عقد قصير الأجل / حسب الطلب (حساب يومي)</span>
                <span>الحد الأقصى سنة واحدة</span>
              </div>
              <div class="flex items-center justify-between text-amber-100">
                <span>قيمة اليوم (الإيجار الشهري ÷ 30):</span>
                <strong class="font-mono text-amber-300">{{ formatMoney(calculation.dailyRent) }} ر.س/يوم</strong>
              </div>
              <div class="flex items-center justify-between text-amber-100">
                <span>أيام الإيجار الفعلي (بدون بداية ونهاية اليوم):</span>
                <strong class="font-mono text-amber-300">{{ calculation.rentDays }} يوم</strong>
              </div>
              <div class="flex items-center justify-between text-amber-100 font-semibold pt-1 border-t border-amber-500/20">
                <span>الإيجار المستحق التقديري:</span>
                <strong class="font-mono text-emerald-400 font-black text-sm">{{ formatMoney(calculation.totalEstimatedRevenue) }} ر.س</strong>
              </div>
            </div>

            <!-- 2. Car Value & Depreciation (قيمة السيارة وإهلاكها) -->
            <div class="bg-white/5 rounded-2xl p-3.5 space-y-2 border border-white/5">
              <div class="text-[11px] font-bold text-amber-300 flex items-center justify-between pb-1 border-b border-indigo-800/40">
                <span class="flex items-center gap-1.5">
                  <span>🚗</span>
                  <span>قيمة وإهلاك السيارة خلال مدة العقد</span>
                </span>
                <span class="text-[10px] text-indigo-300 font-normal">
                  {{ calculation.contractYears }} سنة ({{ calculation.periodMonths }} شهر)
                </span>
              </div>

              <div class="flex items-center justify-between text-xs pt-1">
                <span class="text-indigo-200">قيمة السيارة:</span>
                <span class="font-bold text-white font-mono">{{ formatMoney(calculation.carValueUsed) }} ر.س</span>
              </div>

              <div class="flex items-center justify-between text-xs">
                <span class="text-indigo-200">صافي قيمة السيارة خلال مدة العقد:</span>
                <span class="font-bold text-sky-300 font-mono">
                  {{ formatMoney(netCarValueAtEnd) }} ر.س
                  <span v-if="form.contractTypeId === 4" class="text-[10px] text-amber-300 mr-1 font-sans">(تؤول للمستأجر)</span>
                </span>
              </div>

              <div class="flex items-center justify-between text-xs">
                <span class="text-indigo-200">قيمة إهلاك السيارة نهاية مدة العقد:</span>
                <span class="font-bold text-amber-400 font-mono">
                  {{ formatMoney(totalDepreciationCost) }} ر.س
                  <span v-if="form.contractTypeId === 4" class="text-[10px] text-emerald-300 mr-1 font-sans">(= القيمة الدفترية)</span>
                </span>
              </div>
            </div>

            <!-- 3. The 4 Operational Terms (تقدير البنود الـ 4 التشغيلية لعقود الإيجار 2 و 3) -->
            <div v-if="form.contractTypeId === 2 || form.contractTypeId === 3" class="bg-white/5 rounded-2xl p-3.5 space-y-2 border border-white/5">
              <div class="text-[11px] font-bold text-indigo-300 flex items-center justify-between pb-1 border-b border-indigo-800/40">
                <span class="flex items-center gap-1.5">
                  <span>📋</span>
                  <span>تقدير بنود التشغيل الـ 4 خلال فترة العقد</span>
                </span>
                <span class="text-[10px] text-indigo-400">تراكمي لسنوات العقد</span>
              </div>

              <div class="flex items-center justify-between text-xs pt-1">
                <span class="text-indigo-200">تقدير تأمين شامل للسيارة ({{ form.insurancePercentage }}% سنوياً):</span>
                <span class="font-bold text-white font-mono">{{ formatMoney(totalInsuranceCost) }} ر.س</span>
              </div>

              <div class="flex items-center justify-between text-xs">
                <span class="text-indigo-200">تقدير تكلفة الصيانة خلال فترة العقد ({{ form.maintenancePercentage }}% سنوياً):</span>
                <span class="font-bold text-white font-mono">{{ formatMoney(totalMaintenanceCost) }} ر.س</span>
              </div>

              <div class="flex items-center justify-between text-xs">
                <span class="text-indigo-200">تقدير تكلفة سيارة بديلة خلال فترة العقد ({{ form.replacementCarPercentage }}% سنوياً):</span>
                <span class="font-bold text-white font-mono">{{ formatMoney(totalReplacementCarCost) }} ر.س</span>
              </div>

              <div class="flex items-center justify-between text-xs">
                <span class="text-indigo-200">تقدير تكلفة المصاريف الإدارية خلال فترة العقد ({{ form.adminFeesPercentage }}% سنوياً):</span>
                <span class="font-bold text-white font-mono">{{ formatMoney(totalAdminFeesCost) }} ر.س</span>
              </div>
            </div>

            <!-- 3 (Alternative). Lease-to-Own Operational Terms (Type 4: التأمين الشامل فقط) -->
            <div v-if="form.contractTypeId === 4" class="bg-white/5 rounded-2xl p-3.5 space-y-2 border border-white/5">
              <div class="text-[11px] font-bold text-emerald-300 flex items-center justify-between pb-1 border-b border-indigo-800/40">
                <span class="flex items-center gap-1.5">
                  <span>🛡️</span>
                  <span>تكاليف عقد الإيجار المنتهي بالتمليك</span>
                </span>
                <span class="text-[10px] text-emerald-400">تأمين شامل فقط</span>
              </div>

              <div class="flex items-center justify-between text-xs pt-1">
                <span class="text-indigo-200">تقدير تأمين شامل للسيارة ({{ form.insurancePercentage }}% سنوياً):</span>
                <span class="font-bold text-white font-mono">{{ formatMoney(totalInsuranceCost) }} ر.س</span>
              </div>

              <div class="flex items-center justify-between text-xs">
                <span class="text-indigo-200">تقدير تكلفة الصيانة والبديلة والإدارية:</span>
                <span class="font-bold text-slate-400 font-mono">0.00 ر.س (يتحملها المستأجر)</span>
              </div>
            </div>

            <!-- 4. Costs, Rent & Company Profit (التكاليف والإيجار وفائدة الشركة) -->
            <div class="bg-white/5 rounded-2xl p-3.5 space-y-2 text-xs text-indigo-100 border border-white/5">
              <div class="text-[11px] font-bold text-emerald-300 flex items-center gap-1.5 pb-1 border-b border-indigo-800/40">
                <span>💰</span>
                <span>الحسابات المالية وقسط الإيجار وفائدة الشركة</span>
              </div>

              <div class="flex items-center justify-between pt-1">
                <span class="text-indigo-200">إجـمالي التكلفة التقديرية:</span>
                <span class="font-bold text-rose-300 font-mono">{{ formatMoney(calculation.totalEstimatedCost) }} ر.س</span>
              </div>

              <div class="flex items-center justify-between">
                <span class="text-indigo-200">قيمة تكلفة للسيارة (شهرياً):</span>
                <span class="font-bold text-rose-200 font-mono">{{ formatMoney(monthlyCarCost) }} ر.س/شهر</span>
              </div>

              <div class="flex items-center justify-between">
                <span class="text-indigo-200">قيمة الفائدة للشركة من تشغيل السيارة:</span>
                <span class="font-black text-emerald-300 font-mono">{{ formatMoney(calculation.monthlyCompanyProfit) }} ر.س/شهر</span>
              </div>

              <div class="flex items-center justify-between border-t border-indigo-800/40 pt-1.5">
                <span class="text-indigo-200">قيمة قسط الإيجار التقديري المقترح قبل الدفعة المقدمة:</span>
                <span class="font-bold text-white font-mono">{{ formatMoney(calculation.monthlyRentBeforeDownPayment) }} ر.س</span>
              </div>

              <div v-if="calculation.downPayment > 0" class="flex items-center justify-between">
                <span class="text-indigo-200">قيمة الدفعة المقدمة:</span>
                <span class="font-bold text-amber-300 font-mono">{{ formatMoney(calculation.downPayment) }} ر.س</span>
              </div>

              <div v-if="calculation.downPaymentMonthlyDiscount > 0" class="flex items-center justify-between">
                <span class="text-indigo-200">خصم قيمة الدفعة المقدمة من قيمة قسط الإيجار الشهري المقترح:</span>
                <span class="font-bold text-amber-300 font-mono">{{ formatMoney(calculation.downPaymentMonthlyDiscount) }} ر.س/شهر</span>
              </div>

              <div class="flex items-center justify-between border-t border-indigo-800/60 pt-2 bg-slate-800/60 -mx-2 px-2 py-1.5 rounded-xl border border-white/5">
                <span class="font-semibold text-slate-300">الإيجار الشهري المقترح (قبل الضريبة):</span>
                <span class="font-bold text-white text-sm font-mono">{{ formatMoney(calculation.adjustedMonthlyRent) }} ر.س</span>
              </div>

              <!-- VAT Breakdown Card in Live Calculation Engine -->
              <div v-if="form.contractTypeId !== 1" class="space-y-2 pt-1">
                <div class="flex items-center justify-between text-xs">
                  <span class="text-blue-300 flex items-center gap-1">
                    <span>🧾</span>
                    <span>ضريبة القيمة المضافة {{ calculation.isTaxApplied ? `(${calculation.taxPercentage}%)` : '(معفى)' }}:</span>
                  </span>
                  <span class="font-bold font-mono" :class="calculation.isTaxApplied ? 'text-blue-300' : 'text-slate-400'">
                    {{ calculation.isTaxApplied ? `+${formatMoney(calculation.monthlyTaxAmount)} ر.س/شهر` : '0.00 ر.س (غير مطبقة)' }}
                  </span>
                </div>

                <!-- Customer Final Monthly Payment (Inclusive of VAT) -->
                <div class="flex items-center justify-between p-2.5 rounded-xl bg-gradient-to-r from-emerald-600/30 via-teal-600/30 to-emerald-600/30 border border-emerald-400/40 shadow-inner">
                  <div class="flex flex-col">
                    <span class="font-black text-emerald-300 text-xs">قسط الإيجار الشهري المستحق على العميل:</span>
                    <span class="text-[10px] text-emerald-200/80">{{ calculation.isTaxApplied ? 'شامل ضريبة القيمة المضافة 15%' : 'بدون ضريبة' }}</span>
                  </div>
                  <div class="text-left font-mono font-black text-emerald-300 text-base">
                    {{ formatMoney(calculation.monthlyRentWithTax) }} <span class="text-xs">ر.س</span>
                  </div>
                </div>

                <div v-if="calculation.isTaxApplied" class="flex items-center justify-between text-[11px] text-indigo-300 pt-0.5">
                  <span>إجمالي العقد شامل الضريبة ({{ calculation.periodMonths }} شهر):</span>
                  <span class="font-mono font-bold text-white">{{ formatMoney(calculation.totalPriceWithTax) }} ر.س</span>
                </div>
              </div>
            </div>

            <!-- 5. Detailed Breakdown by Year (حسابات السنوات 1، 2، 3، 4) -->
            <div v-if="calculation.yearlySchedule && calculation.yearlySchedule.length > 0" class="space-y-2.5">
              <div class="flex items-center justify-between text-xs font-bold text-indigo-200">
                <div class="flex items-center gap-1.5">
                  <span>📅</span>
                  <span>حسابات سنوات العقد (الأولى، الثانية، ...)</span>
                </div>
                <div class="flex items-center gap-1 bg-white/10 p-0.5 rounded-lg text-[10px]">
                  <button
                    type="button"
                    class="px-2 py-0.5 rounded transition-all"
                    :class="scheduleTab === 'cards' ? 'bg-indigo-600 text-white font-bold' : 'text-indigo-300 hover:text-white'"
                    @click="scheduleTab = 'cards'"
                  >
                    تفصيل بالسنوات
                  </button>
                  <button
                    type="button"
                    class="px-2 py-0.5 rounded transition-all"
                    :class="scheduleTab === 'table' ? 'bg-indigo-600 text-white font-bold' : 'text-indigo-300 hover:text-white'"
                    @click="scheduleTab = 'table'"
                  >
                    جدول مقارن
                  </button>
                </div>
              </div>

              <!-- View A: Year-by-Year Cards (Exact logic from contracts_examples.md) -->
              <div v-if="scheduleTab === 'cards'" class="space-y-2.5 max-h-[420px] overflow-y-auto pr-0.5">
                <div
                  v-for="item in calculation.yearlySchedule"
                  :key="item.year"
                  class="bg-white/5 rounded-2xl p-3 space-y-1.5 border border-indigo-700/40 text-xs"
                >
                  <div class="flex items-center justify-between border-b border-indigo-800/40 pb-1 font-bold">
                    <span class="text-amber-300 text-xs flex items-center gap-1">
                      <span>📌</span>
                      <span>{{ item.year === 1 ? 'خلال السنة الأولى:' : item.year === 2 ? 'خلال السنة الثانية:' : item.year === 3 ? 'خلال السنة الثالثة:' : `خلال السنة ${item.year}:` }}</span>
                    </span>
                    <span class="text-[11px] font-mono text-emerald-300">
                      التكلفة الشهرية: {{ formatMoney(item.monthlyCost) }} ر.س
                    </span>
                  </div>

                  <div class="grid grid-cols-2 gap-x-2 gap-y-1 text-[11px] pt-0.5">
                    <div class="flex justify-between">
                      <span class="text-indigo-200">صافي قيمة السيارة:</span>
                      <strong class="text-white font-mono">{{ formatMoney(item.netCarValue) }}</strong>
                    </div>

                    <div class="flex justify-between">
                      <span class="text-indigo-200">قيمة الإهلاك:</span>
                      <strong class="text-amber-300 font-mono">{{ formatMoney(item.depreciationCost) }}</strong>
                    </div>

                    <div class="flex justify-between">
                      <span class="text-indigo-200">التأمين الشامل:</span>
                      <strong class="text-white font-mono">{{ formatMoney(item.insuranceCost) }}</strong>
                    </div>

                    <div class="flex justify-between">
                      <span class="text-indigo-200">الصيانة:</span>
                      <strong class="text-white font-mono">{{ formatMoney(item.maintenanceCost) }}</strong>
                    </div>

                    <div class="flex justify-between">
                      <span class="text-indigo-200">سيارة بديلة:</span>
                      <strong class="text-white font-mono">{{ formatMoney(item.replacementCarCost) }}</strong>
                    </div>

                    <div class="flex justify-between">
                      <span class="text-indigo-200">مصاريف إدارية:</span>
                      <strong class="text-white font-mono">{{ formatMoney(item.adminFeesCost) }}</strong>
                    </div>
                  </div>

                  <div class="flex items-center justify-between pt-1 border-t border-indigo-800/30 text-xs">
                    <span class="font-bold text-rose-300">إجمالي التكلفة التقديرية:</span>
                    <strong class="font-black text-rose-400 font-mono">{{ formatMoney(item.totalEstimatedCost) }} ر.س</strong>
                  </div>
                </div>
              </div>

              <!-- View B: Comparative Table -->
              <div v-else class="overflow-x-auto rounded-xl border border-indigo-800/50">
                <table class="w-full text-center text-[10px] text-indigo-100">
                  <thead class="bg-indigo-900/60 font-semibold text-indigo-300">
                    <tr>
                      <th class="p-1.5 whitespace-nowrap">السنة</th>
                      <th class="p-1.5 whitespace-nowrap">صافي السيارة</th>
                      <th class="p-1.5 whitespace-nowrap">الإهلاك</th>
                      <th class="p-1.5 whitespace-nowrap">التأمين</th>
                      <th class="p-1.5 whitespace-nowrap">الصيانة</th>
                      <th class="p-1.5 whitespace-nowrap">بديلة</th>
                      <th class="p-1.5 whitespace-nowrap">إدارية</th>
                      <th class="p-1.5 whitespace-nowrap">إجمالي التكلفة</th>
                      <th class="p-1.5 whitespace-nowrap">التكلفة/شهر</th>
                    </tr>
                  </thead>
                  <tbody class="divide-y divide-indigo-800/40 font-mono">
                    <tr v-for="item in calculation.yearlySchedule" :key="item.year" class="hover:bg-white/5">
                      <td class="p-1.5 font-bold text-white font-sans">السنة {{ item.year }}</td>
                      <td class="p-1.5">{{ formatMoney(item.netCarValue) }}</td>
                      <td class="p-1.5 text-amber-300 font-bold">{{ formatMoney(item.depreciationCost) }}</td>
                      <td class="p-1.5">{{ formatMoney(item.insuranceCost) }}</td>
                      <td class="p-1.5">{{ formatMoney(item.maintenanceCost) }}</td>
                      <td class="p-1.5">{{ formatMoney(item.replacementCarCost) }}</td>
                      <td class="p-1.5">{{ formatMoney(item.adminFeesCost) }}</td>
                      <td class="p-1.5 font-bold text-rose-300">{{ formatMoney(item.totalEstimatedCost) }}</td>
                      <td class="p-1.5 font-bold text-emerald-300">{{ formatMoney(item.monthlyCost) }}</td>
                    </tr>
                  </tbody>
                </table>
              </div>
            </div>

            <!-- Action Button Inside Live Card -->
            <NButton
              type="primary"
              size="large"
              block
              class="font-black text-sm rounded-xl py-3 shadow-lg shadow-blue-500/30"
              :loading="submitting"
              @click="handleCreate"
            >
              <template #icon>
                <NIcon><SaveIcon /></NIcon>
              </template>
              تأكيد وإصدار العقد الآن
            </NButton>
          </div>

          <div v-else class="text-center py-10 text-indigo-300 text-xs">
            قم بتحديد بيانات السيارة وتواريخ العقد لحساب الأرقام والجدول اللحظي...
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
