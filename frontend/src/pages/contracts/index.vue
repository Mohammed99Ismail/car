<script setup lang="ts">
import {
  Add24Regular as AddIcon,
  ArrowClockwise24Regular as RefreshIcon,
  Delete24Regular as DeleteIcon,
  DocumentBulletList24Regular as ContractTypesIcon,
  DocumentTable24Regular as ExcelIcon,
  DocumentText24Regular as ContractsIcon,
  Edit24Regular as EditIcon,
  Eye24Regular as ViewIcon,
  Filter24Regular as FilterIcon,
  Money24Regular as MoneyIcon,
  Search24Regular as SearchIcon,
  Settings24Regular as SettingsIcon,
  VehicleCar24Regular as CarIcon,
} from '@vicons/fluent'
import { useDebounceFn } from '@vueuse/core'
import type { DataTableColumns } from 'naive-ui'
import {
  NAlert,
  NBadge,
  NButton,
  NDataTable,
  NIcon,
  NInput,
  NInputNumber,
  NModal,
  NPagination,
  NSelect,
  NSpace,
  NTag,
  NTooltip,
} from 'naive-ui'
import type { LookupItemDto } from '~/models/ApiResponse'
import { isApiSuccess } from '~/models/ApiResponse'
import type { CarDto } from '~/models/Car'
import type {
  ContractDto,
  ContractQueryParameters,
  ContractTermsConfigDto,
} from '~/models/Contract'
import { useCarService } from '~/services/car.service'
import { useContractTypeService } from '~/services/contract-type.service'
import { useContractService } from '~/services/contract.service'
import { useLookupService } from '~/services/lookup.service'
import ExportExcelModal from '~/components/common/ExportExcelModal.vue'
import { exportToExcel, type ExcelColumn } from '~/utils/excelExport'

const router = useRouter()
const dialog = useDialog()
const message = useMessage()
const accountStore = useAccountStore()

const contractService = useContractService()
const contractTypeService = useContractTypeService()
const carService = useCarService()
const lookupService = useLookupService()

// Permissions
const canCreate = computed(() => accountStore.hasPermission('Contracts.Create'))
const canDelete = computed(() => accountStore.hasPermission('Contracts.Delete'))

// Contracts List State
const contracts = ref<ContractDto[]>([])
const loading = ref(false)
const totalCount = ref(0)
const queryParams = reactive<ContractQueryParameters>({
  pageIndex: 1,
  pageSize: 10,
  search: '',
  contractTypeId: null,
  branchId: null,
  status: null,
})

// Lookups for filters and forms
const contractTypesList = ref<LookupItemDto[]>([])
const branchesList = ref<LookupItemDto[]>([])

// Global Terms Configuration Modal State
const showTermsModal = ref(false)
const savingTerms = ref(false)
const termsConfig = reactive<ContractTermsConfigDto>({
  insurancePercentage: 4,
  maintenancePercentage: 5,
  replacementCarPercentage: 1,
  adminFeesPercentage: 1,
  annualDepreciationPercentage: 25,
  lastModifiedDate: '',
})

// Excel Export State & Columns
const showExportModal = ref(false)
const exporting = ref(false)

const contractExcelColumns: ExcelColumn<ContractDto>[] = [
  { header: 'م', key: '#', width: 6 },
  { header: 'رقم العقد', key: 'contractNumber', width: 16 },
  { header: 'نوع العقد', key: 'contractTypeNameAr', width: 18 },
  {
    header: 'اسم المستفيد',
    key: 'customerName',
    width: 26,
    formatter: (_val, row) => row.customerName || row.userFullName || '—',
  },
  {
    header: 'نوع المستفيد',
    width: 14,
    formatter: (_val, row) => (row.customerId ? 'عميل' : row.userId ? 'موظف' : '—'),
  },
  {
    header: 'رقم الهوية / الإقامة',
    key: 'customerNationalId',
    width: 22,
    formatter: (val) => val || '—',
  },
  {
    header: 'رقم الجوال',
    key: 'customerMobile',
    width: 18,
    formatter: (val) => val || '—',
  },
  { header: 'الفرع', key: 'branchName', width: 18 },
  {
    header: 'السيارة',
    width: 24,
    formatter: (_val, row) => `${row.carMakeName || ''} ${row.carModelName || ''}`.trim() || '—',
  },
  { header: 'سنة الصنع', key: 'carManufactureYear', width: 12 },
  { header: 'رقم اللوحة', key: 'carPlateNumber', width: 16 },
  { header: 'لون السيارة', key: 'carColor', width: 14 },
  {
    header: 'رقم الشاسيه (الهيكل)',
    key: 'carChassisNumber',
    width: 24,
    formatter: (val) => val || '—',
  },
  {
    header: 'تاريخ بداية العقد',
    key: 'startDate',
    width: 18,
    formatter: (val) => (val ? new Date(val).toLocaleDateString('ar-EG') : '—'),
  },
  {
    header: 'تاريخ نهاية العقد',
    key: 'endDate',
    width: 18,
    formatter: (val) => (val ? new Date(val).toLocaleDateString('ar-EG') : '—'),
  },
  { header: 'المدة (أيام)', key: 'periodDays', width: 14 },
  { header: 'المدة (شهور)', key: 'periodMonths', width: 14 },
  {
    header: 'قسط الإيجار الشهري قبل الدفعة (ر.س)',
    key: 'monthlyRent',
    width: 26,
    formatter: (val) => (val != null ? Number(val).toLocaleString('en-US') : '—'),
  },
  {
    header: 'الدفعة المقدمة (ر.س)',
    key: 'downPayment',
    width: 20,
    formatter: (val) => (val != null ? Number(val).toLocaleString('en-US') : 0),
  },
  {
    header: 'الإيجار الشهري المقترح بعد الدفعة (ر.س)',
    key: 'adjustedMonthlyRent',
    width: 26,
    formatter: (val) => (val != null ? Number(val).toLocaleString('en-US') : '—'),
  },
  {
    header: 'تطبيق ضريبة القيمة المضافة',
    key: 'isTaxApplied',
    width: 22,
    formatter: (val) => (val ? 'نعم (15%)' : 'لا (معفى)'),
  },
  {
    header: 'نسبة الضريبة (%)',
    key: 'taxPercentage',
    width: 16,
    formatter: (val, row) => (row.isTaxApplied ? `${val || 15}%` : '0%'),
  },
  {
    header: 'قيمة الضريبة الشهرية (ر.س)',
    key: 'monthlyTaxAmount',
    width: 22,
    formatter: (val) => (val != null ? Number(val).toLocaleString('en-US') : 0),
  },
  {
    header: 'الإيجار الشهري شامل الضريبة (ر.س)',
    key: 'monthlyRentWithTax',
    width: 28,
    formatter: (val, row) => {
      const v = val != null ? val : (row.adjustedMonthlyRent || row.monthlyRent || 0)
      return Number(v).toLocaleString('en-US')
    },
  },
  {
    header: 'إجمالي قيمة العقد شامل الضريبة (ر.س)',
    key: 'totalPriceWithTax',
    width: 30,
    formatter: (val, row) => {
      const v = val || row.totalPrice || 0
      return Number(v).toLocaleString('en-US')
    },
  },
  {
    header: 'إجمالي قيمة العقد / الإيرادات التقديرية (ر.س)',
    key: 'totalEstimatedRevenue',
    width: 28,
    formatter: (val, row) => {
      const v = val || row.totalPrice || 0
      return Number(v).toLocaleString('en-US')
    },
  },
  {
    header: 'المبلغ المدفوع (ر.س)',
    key: 'paidPrice',
    width: 20,
    formatter: (val) => (val != null ? Number(val).toLocaleString('en-US') : 0),
  },
  {
    header: 'المبلغ المتبقي (ر.س)',
    key: 'remainingPrice',
    width: 20,
    formatter: (val) => (val != null ? Number(val).toLocaleString('en-US') : 0),
  },
  {
    header: 'القيمة الدفترية للسيارة وقت العقد (ر.س)',
    key: 'carBookValueSnapshot',
    width: 26,
    formatter: (val) => (val != null ? Number(val).toLocaleString('en-US') : 0),
  },
  {
    header: 'إجمالي التكلفة التقديرية (ر.س)',
    key: 'totalEstimatedCost',
    width: 26,
    formatter: (val) => (val != null ? Number(val).toLocaleString('en-US') : 0),
  },
  {
    header: 'إجمالي الأرباح المتوقعة (ر.س)',
    key: 'expectedProfit',
    width: 26,
    formatter: (val) => (val != null ? Number(val).toLocaleString('en-US') : 0),
  },
  {
    header: 'فائدة الشركة الشهرية (ر.س)',
    key: 'monthlyCompanyProfit',
    width: 24,
    formatter: (val) => (val != null ? Number(val).toLocaleString('en-US') : 0),
  },
  {
    header: 'نسبة التأمين الشامل (%)',
    key: 'insurancePercentage',
    width: 20,
    formatter: (val) => (val != null ? `${val}%` : '—'),
  },
  {
    header: 'نسبة تكلفة الصيانة (%)',
    key: 'maintenancePercentage',
    width: 20,
    formatter: (val) => (val != null ? `${val}%` : '—'),
  },
  {
    header: 'نسبة سيارة بديلة (%)',
    key: 'replacementCarPercentage',
    width: 20,
    formatter: (val) => (val != null ? `${val}%` : '—'),
  },
  {
    header: 'نسبة المصاريف الإدارية (%)',
    key: 'adminFeesPercentage',
    width: 22,
    formatter: (val) => (val != null ? `${val}%` : '—'),
  },
  {
    header: 'نسبة الإهلاك السنوي (%)',
    key: 'annualDepreciationPercentage',
    width: 22,
    formatter: (val) => (val != null ? `${val}%` : '—'),
  },
  {
    header: 'حالة العقد',
    key: 'status',
    width: 16,
    formatter: (val) => (val === 'Active' ? 'ساري' : val === 'Completed' ? 'مكتمل' : val === 'Cancelled' ? 'ملغي' : val || '—'),
  },
  {
    header: 'ملاحظات وشروط العقد',
    key: 'notes',
    width: 32,
    formatter: (val) => val || '—',
  },
  {
    header: 'تاريخ إنشاء العقد',
    key: 'createdDate',
    width: 20,
    formatter: (val) => (val ? new Date(val).toLocaleDateString('ar-EG') : '—'),
  },
]

async function handleExport({ scope, count, format = 'xlsx' }: { scope: 'all' | 'page' | 'custom'; count: number; format?: 'xlsx' | 'xlsm' }) {
  exporting.value = true
  try {
    let exportData: ContractDto[] = []

    if (scope === 'page') {
      exportData = [...contracts.value]
    } else {
      // Fetch with current active filters (contractType, branch, status, search) and chosen quantity
      const res = await contractService.getPaged({
        ...queryParams,
        pageIndex: 1,
        pageSize: count,
      })
      if (isApiSuccess(res) && res.data) {
        exportData = res.data.items
      }
    }

    if (!exportData || exportData.length === 0) {
      message.warning('لا توجد بيانات عقود مطابقة للتصدير')
      return
    }

    const today = new Date().toISOString().slice(0, 10)
    exportToExcel({
      filename: `قائمة_العقود_${today}`,
      sheetName: 'العقود',
      format,
      columns: contractExcelColumns,
      data: exportData,
    })

    message.success(`تم تصدير ${exportData.length} عقد إلى ملف .${format} بنجاح`)
    showExportModal.value = false
  } catch (err) {
    console.error('Error exporting contracts to Excel:', err)
    message.error('حدث خطأ أثناء تصدير ملف الإكسيل للعقود')
  } finally {
    exporting.value = false
  }
}

// Metrics / Statistics
const stats = computed(() => {
  const total = totalCount.value
  const active = contracts.value.filter((c: ContractDto) => c.status === 'Active').length
  const totalRev = contracts.value.reduce((acc: number, c: ContractDto) => acc + (c.totalEstimatedRevenue || c.totalPrice || 0), 0)
  const totalProf = contracts.value.reduce((acc: number, c: ContractDto) => acc + (c.expectedProfit || 0), 0)
  return { total, active, totalRev, totalProf }
})

// Load Data
async function loadContracts() {
  loading.value = true
  try {
    const res = await contractService.getPaged(queryParams)
    if (isApiSuccess(res) && res.data) {
      contracts.value = res.data.items
      totalCount.value = res.data.totalCount
    }
  } catch (err) {
    message.error('فشل تحميل قائمة العقود')
  } finally {
    loading.value = false
  }
}

async function loadLookups() {
  try {
    const [ctRes, brRes, termsRes] = await Promise.all([
      contractTypeService.getAll(),
      lookupService.getBranches(),
      contractService.getTermsConfig(),
    ])

    if (isApiSuccess(ctRes) && ctRes.data) {
      contractTypesList.value = ctRes.data.map((ct: any) => ({ id: ct.id, name: ct.nameAr }))
    }
    if (isApiSuccess(brRes) && brRes.data) branchesList.value = brRes.data

    if (isApiSuccess(termsRes) && termsRes.data) {
      Object.assign(termsConfig, termsRes.data)
    }
  } catch (err) {
    console.error('Error loading lookups', err)
  }
}

const onSearchInput = useDebounceFn(() => {
  queryParams.pageIndex = 1
  loadContracts()
}, 400)

function onFilterChange() {
  queryParams.pageIndex = 1
  loadContracts()
}

// Terms Config Actions
async function saveTermsConfig() {
  savingTerms.value = true
  try {
    const res = await contractService.updateTermsConfig({
      insurancePercentage: termsConfig.insurancePercentage,
      maintenancePercentage: termsConfig.maintenancePercentage,
      replacementCarPercentage: termsConfig.replacementCarPercentage,
      adminFeesPercentage: termsConfig.adminFeesPercentage,
      annualDepreciationPercentage: termsConfig.annualDepreciationPercentage,
    })
    if (isApiSuccess(res)) {
      message.success('تم تحديث نسب البنود الأربعة للإهلاك بنجاح!')
      showTermsModal.value = false
    } else {
      message.error(res.message || 'فشل تحديث الإعدادات')
    }
  } catch {
    message.error('حدث خطأ أثناء حفظ الإعدادات')
  } finally {
    savingTerms.value = false
  }
}

// Delete Contract
function handleDelete(row: ContractDto) {
  dialog.warning({
    title: 'تأكيد حذف العقد',
    content: `هل أنت متأكد من حذف العقد رقم "${row.contractNumber}"؟ لا يمكن التراجع عن هذا الإجراء.`,
    positiveText: 'نعم، احذف',
    negativeText: 'إلغاء',
    onPositiveClick: async () => {
      try {
        const res = await contractService.remove(row.id)
        if (isApiSuccess(res)) {
          message.success('تم حذف العقد بنجاح')
          loadContracts()
        } else {
          message.error(res.message || 'فشل الحذف')
        }
      } catch {
        message.error('حدث خطأ أثناء الحذف')
      }
    },
  })
}

// Contract Type Color Helper
function getTypeBadge(typeId: number, nameAr: string) {
  switch (typeId) {
    case 1:
      return { type: 'info', color: '#8b5cf6', label: nameAr || 'عقد خدمة', icon: '🛠️' }
    case 2:
      return { type: 'warning', color: '#f59e0b', label: nameAr || 'إيجار قصير الأجل', icon: '⏱️' }
    case 3:
      return { type: 'primary', color: '#3b82f6', label: nameAr || 'إيجار طويل الأجل', icon: '📅' }
    case 4:
      return { type: 'success', color: '#10b981', label: nameAr || 'إيجار منتهي بالتمليك', icon: '🔑' }
    default:
      return { type: 'default', color: '#64748b', label: nameAr, icon: '📋' }
  }
}

// Data Table Columns
const columns: DataTableColumns<ContractDto> = [
  {
    title: 'رقم العقد',
    key: 'contractNumber',
    width: 150,
    render(row) {
      return h('div', { class: 'flex items-center gap-2' }, [
        h(NTag, { type: 'primary', bordered: false, class: 'font-mono font-bold text-sm tracking-wide' }, { default: () => row.contractNumber }),
      ])
    },
  },
  {
    title: 'نوع العقد',
    key: 'contractTypeId',
    width: 190,
    render(row) {
      const badge = getTypeBadge(row.contractTypeId, row.contractTypeNameAr)
      return h(
        NTag,
        {
          type: badge.type as any,
          bordered: true,
          round: true,
          class: 'text-xs font-semibold py-1 px-2.5',
        },
        {
          icon: () => h('span', { class: 'mr-1' }, badge.icon),
          default: () => badge.label,
        },
      )
    },
  },
  {
    title: 'السيارة',
    key: 'carPlateNumber',
    width: 220,
    render(row) {
      return h('div', { class: 'flex flex-col gap-1' }, [
        h('div', { class: 'font-bold text-slate-800 dark:text-slate-100 text-sm' }, `${row.carMakeName} ${row.carModelName} (${row.carManufactureYear})`),
        h('div', { class: 'flex items-center gap-1.5' }, [
          h('span', { class: 'px-2 py-0.5 bg-amber-50 text-amber-800 border border-amber-300 rounded font-mono text-xs font-bold' }, row.carPlateNumber),
          row.carColor ? h('span', { class: 'text-xs text-slate-500' }, `• ${row.carColor}`) : null,
        ]),
      ])
    },
  },
  {
    title: 'المستفيد',
    key: 'customerId',
    width: 190,
    render(row) {
      if (row.contractTypeId === 1) {
        return h('div', { class: 'flex flex-col text-xs' }, [
          h('span', { class: 'font-bold text-purple-700 dark:text-purple-300 flex items-center gap-1' }, [
            h('span', '👤'),
            row.userFullName || 'موظف النظام',
          ]),
          h('span', { class: 'text-slate-400' }, row.userEmail || 'عقد خدمة للمعرض'),
        ])
      }
      return h('div', { class: 'flex flex-col text-xs' }, [
        h('span', { class: 'font-bold text-slate-800 dark:text-slate-200' }, row.customerName || '—'),
        h('span', { class: 'text-slate-500 font-mono' }, row.customerMobile || ''),
      ])
    },
  },
  {
    title: 'المدة والتواريخ',
    key: 'periodMonths',
    width: 160,
    render(row) {
      const start = new Date(row.startDate).toLocaleDateString('ar-SA')
      const end = new Date(row.endDate).toLocaleDateString('ar-SA')
      return h('div', { class: 'flex flex-col text-xs gap-0.5' }, [
        h('div', { class: 'flex items-center gap-1 font-semibold text-blue-600 dark:text-blue-400' }, [
          h('span', '⏱️'),
          `${row.periodMonths} شهر (${row.periodDays} يوم)`,
        ]),
        h('span', { class: 'text-slate-400 text-[11px]' }, `${start} ← ${end}`),
      ])
    },
  },
  {
    title: 'الإيجار والماليات',
    key: 'totalPrice',
    width: 200,
    render(row) {
      const isTax = row.isTaxApplied && row.contractTypeId !== 1
      const rentBeforeTax = row.adjustedMonthlyRent || row.monthlyRent
      const rentWithTax = row.monthlyRentWithTax || rentBeforeTax

      return h('div', { class: 'flex flex-col text-xs gap-1' }, [
        rentWithTax
          ? h('div', { class: 'flex items-center gap-1.5' }, [
              h('span', { class: 'font-black text-slate-900 dark:text-white' }, `${rentWithTax.toLocaleString()} ر.س`),
              h(
                NTag,
                {
                  size: 'tiny',
                  type: isTax ? 'info' : 'default',
                  bordered: false,
                  round: true,
                  class: 'text-[10px] font-bold px-1.5',
                },
                { default: () => (isTax ? 'شامل 15% ضريبة' : 'معفى') },
              ),
            ])
          : null,
        isTax && rentBeforeTax && rentBeforeTax !== rentWithTax
          ? h('div', { class: 'text-slate-400 text-[11px]' }, `قبل الضريبة: ${rentBeforeTax.toLocaleString()} ر.س`)
          : null,
        row.downPayment && row.downPayment > 0
          ? h('div', { class: 'text-amber-600 dark:text-amber-400 text-[11px]' }, `دفعة: ${row.downPayment.toLocaleString()} ر.س`)
          : null,
        h(
          'div',
          { class: 'font-bold text-emerald-600 dark:text-emerald-400 text-[11px]' },
          `الإجمالي: ${(row.totalPriceWithTax || row.totalPrice).toLocaleString()} ر.س`,
        ),
      ])
    },
  },
  {
    title: 'الأرباح المتوقعة',
    key: 'expectedProfit',
    width: 160,
    render(row) {
      if (row.expectedProfit <= 0) return h('span', { class: 'text-slate-400' }, '—')
      return h('div', { class: 'flex flex-col text-xs' }, [
        h('span', { class: 'font-bold text-emerald-700 dark:text-emerald-300' }, `${row.expectedProfit.toLocaleString()} ر.س`),
        h('span', { class: 'text-[11px] text-slate-500' }, `فائدة: ${row.monthlyCompanyProfit.toLocaleString()} ر.س/شهر`),
      ])
    },
  },
  {
    title: 'الحالة',
    key: 'status',
    width: 100,
    render(row) {
      const type = row.status === 'Active' ? 'success' : row.status === 'Completed' ? 'info' : 'error'
      const label = row.status === 'Active' ? 'ساري' : row.status === 'Completed' ? 'مكتمل' : 'ملغي'
      return h(NTag, { type: type as any, bordered: false, round: true, size: 'small' }, { default: () => label })
    },
  },
  {
    title: 'الإجراءات',
    key: 'actions',
    width: 140,
    render(row) {
      return h(NSpace, { size: 'small' }, {
        default: () => [
          // View Report
          h(
            NTooltip,
            {},
            {
              trigger: () =>
                h(
                  NButton,
                  {
                    size: 'small',
                    type: 'primary',
                    quaternary: true,
                    onClick: () => router.push(`/contracts/${row.id}/report`),
                  },
                  { icon: () => h(NIcon, null, { default: () => h(ViewIcon) }) },
                ),
              default: () => 'عرض التقرير المالي الشامل',
            },
          ),
          // Delete
          canDelete.value
            ? h(
                NTooltip,
                {},
                {
                  trigger: () =>
                    h(
                      NButton,
                      {
                        size: 'small',
                        type: 'error',
                        quaternary: true,
                        onClick: () => handleDelete(row),
                      },
                      { icon: () => h(NIcon, null, { default: () => h(DeleteIcon) }) },
                    ),
                  default: () => 'حذف العقد',
                },
              )
            : null,
        ],
      })
    },
  },
]

onMounted(() => {
  loadLookups()
  loadContracts()
})
</script>

<template>
  <div class="p-4 md:p-6 space-y-6">
    <!-- Header Section -->
    <div class="flex flex-col md:flex-row md:items-center justify-between gap-4 bg-white dark:bg-slate-800 p-6 rounded-2xl shadow-sm border border-slate-100 dark:border-slate-700/60">
      <div class="flex items-center gap-3">
        <div class="w-12 h-12 rounded-xl bg-gradient-to-tr from-blue-600 to-indigo-600 flex items-center justify-center text-white shadow-lg shadow-blue-500/20">
          <NIcon size="24"><ContractsIcon /></NIcon>
        </div>
        <div>
          <h1 class="text-xl md:text-2xl font-black text-slate-800 dark:text-white tracking-tight flex items-center gap-2">
            إدارة العقود والحسابات
            <span class="text-xs font-semibold px-2.5 py-0.5 rounded-full bg-blue-100 dark:bg-blue-900/50 text-blue-700 dark:text-blue-300">
              4 أنواع معتمدة
            </span>
          </h1>
          <p class="text-xs md:text-sm text-slate-500 dark:text-slate-400 mt-1">
            إدارة عقود التأجير والخدمة، حساب الإهلاك اللحظي، وإصدار التقارير المالية الدقيقة
          </p>
        </div>
      </div>

      <div class="flex flex-wrap items-center gap-2.5">
        <NButton
          secondary
          type="info"
          class="font-semibold shadow-sm rounded-xl"
          @click="showTermsModal = true"
        >
          <template #icon>
            <NIcon><SettingsIcon /></NIcon>
          </template>
          إعدادات نسب البنود الأربعة
        </NButton>

        <NButton
          secondary
          class="font-semibold shadow-sm rounded-xl !text-emerald-700 dark:!text-emerald-400 !border-emerald-300 dark:!border-emerald-700 hover:!bg-emerald-50 dark:hover:!bg-emerald-950/40"
          @click="showExportModal = true"
        >
          <template #icon>
            <NIcon><ExcelIcon class="text-emerald-600 dark:text-emerald-400" /></NIcon>
          </template>
          تصدير إكسيل
        </NButton>

        <NButton
          v-if="canCreate"
          type="primary"
          class="font-bold shadow-md shadow-blue-500/20 rounded-xl"
          @click="router.push('/contracts/create')"
        >
          <template #icon>
            <NIcon><AddIcon /></NIcon>
          </template>
          إنشاء عقد جديد
        </NButton>

        <NButton
          secondary
          class="rounded-xl"
          :loading="loading"
          @click="loadContracts"
        >
          <template #icon>
            <NIcon><RefreshIcon /></NIcon>
          </template>
        </NButton>
      </div>
    </div>

    <!-- Statistics Cards -->
    <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
      <div class="bg-gradient-to-br from-blue-500/10 to-indigo-500/5 p-5 rounded-2xl border border-blue-200/50 dark:border-blue-900/30 flex items-center justify-between">
        <div>
          <p class="text-xs font-semibold text-slate-500 dark:text-slate-400 uppercase tracking-wider">إجمالي العقود</p>
          <p class="text-2xl font-black text-slate-800 dark:text-white mt-1">{{ totalCount }}</p>
        </div>
        <div class="w-11 h-11 rounded-xl bg-blue-600 text-white flex items-center justify-center shadow-md">
          <NIcon size="20"><ContractsIcon /></NIcon>
        </div>
      </div>

      <div class="bg-gradient-to-br from-emerald-500/10 to-teal-500/5 p-5 rounded-2xl border border-emerald-200/50 dark:border-emerald-900/30 flex items-center justify-between">
        <div>
          <p class="text-xs font-semibold text-slate-500 dark:text-slate-400 uppercase tracking-wider">العقود السارية</p>
          <p class="text-2xl font-black text-emerald-600 dark:text-emerald-400 mt-1">{{ stats.active }}</p>
        </div>
        <div class="w-11 h-11 rounded-xl bg-emerald-600 text-white flex items-center justify-center shadow-md">
          <span class="text-lg">⚡</span>
        </div>
      </div>

      <div class="bg-gradient-to-br from-purple-500/10 to-violet-500/5 p-5 rounded-2xl border border-purple-200/50 dark:border-purple-900/30 flex items-center justify-between">
        <div>
          <p class="text-xs font-semibold text-slate-500 dark:text-slate-400 uppercase tracking-wider">إجمالي الإيرادات المقدرة</p>
          <p class="text-2xl font-black text-purple-700 dark:text-purple-300 mt-1">{{ stats.totalRev.toLocaleString() }} <span class="text-xs font-normal">ر.س</span></p>
        </div>
        <div class="w-11 h-11 rounded-xl bg-purple-600 text-white flex items-center justify-center shadow-md">
          <NIcon size="20"><MoneyIcon /></NIcon>
        </div>
      </div>

      <div class="bg-gradient-to-br from-amber-500/10 to-yellow-500/5 p-5 rounded-2xl border border-amber-200/50 dark:border-amber-900/30 flex items-center justify-between">
        <div>
          <p class="text-xs font-semibold text-slate-500 dark:text-slate-400 uppercase tracking-wider">إجمالي الأرباح المتوقعة</p>
          <p class="text-2xl font-black text-amber-600 dark:text-amber-400 mt-1">{{ stats.totalProf.toLocaleString() }} <span class="text-xs font-normal">ر.س</span></p>
        </div>
        <div class="w-11 h-11 rounded-xl bg-amber-600 text-white flex items-center justify-center shadow-md">
          <span class="text-lg">📈</span>
        </div>
      </div>
    </div>

    <!-- Filters Section -->
    <div class="bg-white dark:bg-slate-800 p-4 rounded-2xl shadow-sm border border-slate-100 dark:border-slate-700/60">
      <div class="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-4 gap-3">
        <div>
          <NInput
            v-model:value="queryParams.search"
            placeholder="بحث برقم العقد، اللوحة، العميل..."
            clearable
            class="rounded-xl"
            @input="onSearchInput"
          >
            <template #prefix>
              <NIcon class="text-slate-400"><SearchIcon /></NIcon>
            </template>
          </NInput>
        </div>

        <div>
          <NSelect
            v-model:value="queryParams.contractTypeId"
            :options="[
              { label: 'جميع أنواع العقود', value: null },
              ...contractTypesList.map((c: LookupItemDto) => ({ label: c.name, value: c.id })),
            ]"
            placeholder="نوع العقد"
            clearable
            class="rounded-xl"
            @update:value="onFilterChange"
          />
        </div>

        <div>
          <NSelect
            v-model:value="queryParams.branchId"
            :options="[
              { label: 'جميع الفروع', value: null },
              ...branchesList.map((b: LookupItemDto) => ({ label: b.name, value: b.id })),
            ]"
            placeholder="الفرع"
            clearable
            class="rounded-xl"
            @update:value="onFilterChange"
          />
        </div>

        <div>
          <NSelect
            v-model:value="queryParams.status"
            :options="[
              { label: 'جميع الحالات', value: null },
              { label: 'ساري', value: 'Active' },
              { label: 'مكتمل', value: 'Completed' },
              { label: 'ملغي', value: 'Cancelled' },
            ]"
            placeholder="الحالة"
            clearable
            class="rounded-xl"
            @update:value="onFilterChange"
          />
        </div>
      </div>
    </div>

    <!-- Contracts Table -->
    <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-sm border border-slate-100 dark:border-slate-700/60 overflow-hidden">
      <NDataTable
        :columns="columns"
        :data="contracts"
        :loading="loading"
        :pagination="false"
        :bordered="false"
        :scroll-x="1600"
      />

      <div class="p-4 border-t border-slate-100 dark:border-slate-700/60 flex items-center justify-between">
        <span class="text-xs text-slate-500">
          عرض {{ contracts.length }} من إجمالي {{ totalCount }} عقد
        </span>
        <NPagination
          v-model:page="queryParams.pageIndex"
          v-model:page-size="queryParams.pageSize"
          :item-count="totalCount"
          :page-sizes="[10, 20, 50]"
          show-size-picker
          @update:page="loadContracts"
          @update:page-size="loadContracts"
        />
      </div>
    </div>

    <!-- Modal for 4 Deduction Terms Configuration -->
    <NModal
      v-model:show="showTermsModal"
      preset="card"
      title="الإعدادات العامة لنسب البنود الأربعة للإهلاك"
      class="max-w-xl rounded-2xl shadow-2xl"
    >
      <div class="space-y-5">
        <NAlert type="info" :bordered="false" class="rounded-xl">
          هذه النسب المئوية السنوية تطبق تلقائياً كقيم افتراضية عند إنشاء عقود الإيجار التي تخضع لبنود الخصم (قصير الأجل وطويل الأجل)، ويمكنك تعديلها يدوياً لكل عقد على حدة.
        </NAlert>

        <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
          <div class="bg-slate-50 dark:bg-slate-900/50 p-4 rounded-xl border border-slate-200/60 dark:border-slate-700/60">
            <label class="text-xs font-bold text-slate-700 dark:text-slate-300 block mb-1.5">
              1. التأمين الشامل (%)
            </label>
            <NInputNumber
              v-model:value="termsConfig.insurancePercentage"
              :min="0"
              :max="100"
              :step="0.5"
              class="w-full"
            >
              <template #suffix>%</template>
            </NInputNumber>
            <p class="text-[11px] text-slate-400 mt-1">الافتراضي: 4% سنوياً من قيمة السيارة</p>
          </div>

          <div class="bg-slate-50 dark:bg-slate-900/50 p-4 rounded-xl border border-slate-200/60 dark:border-slate-700/60">
            <label class="text-xs font-bold text-slate-700 dark:text-slate-300 block mb-1.5">
              2. الصيانة (%)
            </label>
            <NInputNumber
              v-model:value="termsConfig.maintenancePercentage"
              :min="0"
              :max="100"
              :step="0.5"
              class="w-full"
            >
              <template #suffix>%</template>
            </NInputNumber>
            <p class="text-[11px] text-slate-400 mt-1">الافتراضي: 5% سنوياً من قيمة السيارة</p>
          </div>

          <div class="bg-slate-50 dark:bg-slate-900/50 p-4 rounded-xl border border-slate-200/60 dark:border-slate-700/60">
            <label class="text-xs font-bold text-slate-700 dark:text-slate-300 block mb-1.5">
              3. سيارة بديلة (%)
            </label>
            <NInputNumber
              v-model:value="termsConfig.replacementCarPercentage"
              :min="0"
              :max="100"
              :step="0.5"
              class="w-full"
            >
              <template #suffix>%</template>
            </NInputNumber>
            <p class="text-[11px] text-slate-400 mt-1">الافتراضي: 1% سنوياً من قيمة السيارة</p>
          </div>

          <div class="bg-slate-50 dark:bg-slate-900/50 p-4 rounded-xl border border-slate-200/60 dark:border-slate-700/60">
            <label class="text-xs font-bold text-slate-700 dark:text-slate-300 block mb-1.5">
              4. مصاريف إدارية (%)
            </label>
            <NInputNumber
              v-model:value="termsConfig.adminFeesPercentage"
              :min="0"
              :max="100"
              :step="0.5"
              class="w-full"
            >
              <template #suffix>%</template>
            </NInputNumber>
            <p class="text-[11px] text-slate-400 mt-1">الافتراضي: 1% سنوياً من قيمة السيارة</p>
          </div>
        </div>

        <div class="bg-blue-50/50 dark:bg-blue-950/20 p-4 rounded-xl border border-blue-200/50 dark:border-blue-900/40">
          <label class="text-xs font-bold text-blue-900 dark:text-blue-300 block mb-1.5">
            معدل إهلاك السيارة السنوي (%)
          </label>
          <NInputNumber
            v-model:value="termsConfig.annualDepreciationPercentage"
            :min="1"
            :max="100"
            :step="1"
            class="w-full"
          >
            <template #suffix>%</template>
          </NInputNumber>
          <p class="text-[11px] text-slate-500 mt-1">
            يتم إهلاك السيارة خلال أربع سنوات بمعدل 25% سنوياً حتى تصبح القيمة الدفترية 1 ريال فقط.
          </p>
        </div>

        <div class="flex items-center justify-end gap-3 pt-2">
          <NButton @click="showTermsModal = false">إلغاء</NButton>
          <NButton
            type="primary"
            class="font-bold px-6 rounded-xl"
            :loading="savingTerms"
            @click="saveTermsConfig"
          >
            حفظ الإعدادات
          </NButton>
        </div>
      </div>
    </NModal>

    <!-- Excel Export Modal -->
    <ExportExcelModal
      v-model:show="showExportModal"
      title="تصدير قائمة العقود إلى إكسيل (Excel)"
      :total-count="totalCount"
      :current-page-count="contracts.length"
      :loading="exporting"
      @export="handleExport"
    />
  </div>
</template>
