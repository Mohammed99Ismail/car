<script setup lang="ts">
import {
  ArrowClockwise24Regular as RefreshIcon,
  Settings24Regular as SettingsIcon,
  LockClosed24Regular as LockIcon,
  Search24Regular as SearchIcon,
  Copy24Regular as CopyIcon,
  Checkmark24Regular as CheckIcon,
} from '@vicons/fluent'
import type { DataTableColumns } from 'naive-ui'
import {
  NAlert,
  NButton,
  NCard,
  NDataTable,
  NIcon,
  NInput,
  NSpace,
  NTag,
  NTooltip,
} from 'naive-ui'
import { isApiSuccess } from '~/models/ApiResponse'
import type { SystemSettingDto } from '~/models/Settings'
import { useSettingsService } from '~/services/settings.service'

const message = useMessage()
const router = useRouter()
const accountStore = useAccountStore()
const settingsService = useSettingsService()

const settingsList = ref<SystemSettingDto[]>([])
const loading = ref(false)
const searchQuery = ref('')
const selectedCategory = ref('ALL')
const copiedKey = ref<string | null>(null)

async function loadSettings() {
  loading.value = true
  try {
    const res = await settingsService.getAll()
    if (isApiSuccess(res) && res.data) {
      settingsList.value = res.data
    } else {
      message.error(res.message || 'فشل تحميل إعدادات النظام')
    }
  } catch (err) {
    message.error('حدث خطأ أثناء تحميل الإعدادات')
  } finally {
    loading.value = false
  }
}

// Extract distinct categories
const categories = computed(() => {
  const cats = new Set<string>()
  settingsList.value.forEach((s) => {
    if (s.category) cats.add(s.category)
  })
  return Array.from(cats)
})

// Filtered list based on search and category
const filteredSettings = computed(() => {
  return settingsList.value.filter((item) => {
    const matchesCategory =
      selectedCategory.value === 'ALL' || item.category === selectedCategory.value
    if (!matchesCategory) return false

    if (!searchQuery.value.trim()) return true
    const q = searchQuery.value.toLowerCase().trim()
    return (
      item.key.toLowerCase().includes(q) ||
      item.nameAr.toLowerCase().includes(q) ||
      item.nameEn.toLowerCase().includes(q) ||
      (item.descriptionAr && item.descriptionAr.toLowerCase().includes(q))
    )
  })
})

function copyToClipboard(text: string) {
  navigator.clipboard.writeText(text)
  copiedKey.value = text
  message.success(`تم نسخ الرمز: ${text}`)
  setTimeout(() => {
    if (copiedKey.value === text) copiedKey.value = null
  }, 2000)
}

const columns: DataTableColumns<SystemSettingDto> = [
  {
    title: 'رمز الإعداد (Key)',
    key: 'key',
    width: 200,
    render(row) {
      const isCopied = copiedKey.value === row.key
      return h(
        'div',
        { class: 'flex items-center gap-2' },
        [
          h(
            NTag,
            { type: 'primary', bordered: false, class: 'font-mono font-bold text-xs px-2.5 py-1 rounded-lg' },
            { default: () => row.key },
          ),
          h(
            NButton,
            {
              size: 'tiny',
              quaternary: true,
              circle: true,
              class: 'text-slate-400 hover:text-blue-600',
              onClick: () => copyToClipboard(row.key),
            },
            {
              icon: () => h(NIcon, { component: isCopied ? CheckIcon : CopyIcon }),
            },
          ),
        ],
      )
    },
  },
  {
    title: 'اسم الإعداد',
    key: 'nameAr',
    minWidth: 220,
    render(row) {
      return h('div', { class: 'space-y-0.5' }, [
        h('div', { class: 'font-black text-slate-800 dark:text-white text-sm' }, row.nameAr),
        h('div', { class: 'font-mono text-[11px] text-slate-400 dark:text-slate-500' }, row.nameEn),
      ])
    },
  },
  {
    title: 'التصنيف',
    key: 'category',
    width: 150,
    render(row) {
      return h(
        NTag,
        {
          bordered: false,
          class: 'bg-slate-100 text-slate-700 dark:bg-slate-700 dark:text-slate-200 rounded-lg text-xs font-semibold',
        },
        { default: () => row.category },
      )
    },
  },
  {
    title: 'القيمة المعتمدة',
    key: 'value',
    width: 150,
    render(row) {
      const displayValue = row.dataType === 'Percentage' ? `${row.value}%` : row.value
      return h(
        'div',
        { class: 'inline-flex items-center gap-1.5 px-3 py-1 rounded-xl bg-blue-50 dark:bg-blue-950/60 border border-blue-200 dark:border-blue-800 text-blue-700 dark:text-blue-300 font-black font-mono text-base shadow-xs' },
        [
          h('span', displayValue),
        ],
      )
    },
  },
  {
    title: 'صلاحية التعديل',
    key: 'isReadOnly',
    width: 150,
    render(row) {
      return row.isReadOnly
        ? h(
            NTag,
            {
              type: 'warning',
              bordered: false,
              class: 'rounded-lg text-xs font-bold gap-1 px-2.5 py-0.5',
            },
            {
              icon: () => h(NIcon, { component: LockIcon, class: 'w-3.5 h-3.5' }),
              default: () => 'للقراءة فقط (ReadOnly)',
            },
          )
        : h(
            NTag,
            { type: 'success', bordered: false, class: 'rounded-lg text-xs font-bold' },
            { default: () => 'قابل للتعديل' },
          )
    },
  },
  {
    title: 'الوصف ونطاق التطبيق',
    key: 'descriptionAr',
    minWidth: 280,
    render(row) {
      return h(
        'div',
        { class: 'text-xs text-slate-600 dark:text-slate-300 leading-relaxed' },
        row.descriptionAr || '—',
      )
    },
  },
  {
    title: 'تاريخ التحديث',
    key: 'lastModifiedDate',
    width: 130,
    render(row) {
      const d = row.lastModifiedDate || row.createdDate
      return h(
        'div',
        { class: 'text-[11px] font-mono text-slate-500' },
        new Date(d).toLocaleDateString('ar-SA'),
      )
    },
  },
]

onMounted(() => {
  if (!accountStore.hasPermission('Settings.View')) {
    message.error('عفواً، ليس لديك صلاحية للوصول إلى إعدادات النظام')
    router.replace('/')
    return
  }
  loadSettings()
})
</script>

<route lang="yaml">
meta:
  title: settings
  layout: default
</route>

<template>
  <div v-if="accountStore.hasPermission('Settings.View')" class="p-4 sm:p-6 md:p-8 max-w-7xl mx-auto space-y-6">
    <!-- Header Section -->
    <div class="flex flex-col sm:flex-row sm:items-center justify-between pb-6 gap-4 border-b border-slate-200/80 dark:border-slate-800">
      <div class="flex items-center gap-4">
        <div class="p-3 rounded-2xl bg-gradient-to-br from-blue-600 to-indigo-700 text-white shadow-md shadow-blue-500/20">
          <n-icon size="32" :component="SettingsIcon" />
        </div>
        <div>
          <div class="flex items-center gap-2">
            <h1 class="text-2xl sm:text-3xl font-black text-slate-900 dark:text-white">
              إعدادات النظام العامة
            </h1>
            <n-tag type="info" size="small" round :bordered="false" class="font-bold text-[11px]">
              System Settings
            </n-tag>
          </div>
          <p class="text-sm text-slate-500 dark:text-slate-400 mt-1">
            إدارة وضبط المتغيرات والمعايير التشغيلية والنسب المالية المعتمدة في النظام
          </p>
        </div>
      </div>

      <div class="flex items-center gap-2">
        <n-button secondary class="rounded-xl px-4" :loading="loading" @click="loadSettings">
          <template #icon>
            <n-icon :component="RefreshIcon" />
          </template>
          تحديث البيانات
        </n-button>
      </div>
    </div>

    <!-- Filter & Search Bar -->
    <div class="p-4 bg-white dark:bg-slate-800/90 rounded-2xl border border-slate-200/80 dark:border-slate-700/80 shadow-xs flex flex-col sm:flex-row items-center justify-between gap-3">
      <!-- Category Buttons -->
      <div class="flex items-center gap-2 overflow-x-auto w-full sm:w-auto pb-1 sm:pb-0">
        <n-button
          size="small"
          :type="selectedCategory === 'ALL' ? 'primary' : 'default'"
          :secondary="selectedCategory !== 'ALL'"
          class="rounded-xl font-bold"
          @click="selectedCategory = 'ALL'"
        >
          الكل ({{ settingsList.length }})
        </n-button>
        <n-button
          v-for="cat in categories"
          :key="cat"
          size="small"
          :type="selectedCategory === cat ? 'primary' : 'default'"
          :secondary="selectedCategory !== cat"
          class="rounded-xl font-bold"
          @click="selectedCategory = cat"
        >
          {{ cat }}
        </n-button>
      </div>

      <!-- Search Input -->
      <div class="w-full sm:w-72">
        <n-input
          v-model:value="searchQuery"
          placeholder="بحث بالرمز أو اسم الإعداد..."
          clearable
          class="rounded-xl"
        >
          <template #prefix>
            <n-icon :component="SearchIcon" class="text-slate-400" />
          </template>
        </n-input>
      </div>
    </div>

    <!-- Settings Data Table Card -->
    <div class="bg-white dark:bg-slate-800/90 rounded-2xl shadow-sm border border-slate-200/80 dark:border-slate-700/80 overflow-hidden backdrop-blur-sm">
      <div class="p-4 sm:p-5 border-b border-slate-100 dark:border-slate-700/70 flex items-center justify-between">
        <div class="flex items-center gap-2">
          <span class="w-2.5 h-2.5 rounded-full bg-blue-600 inline-block" />
          <h2 class="text-base font-black text-slate-800 dark:text-white">
            قائمة الإعدادات والمتغيرات المعتمدة
          </h2>
          <span class="text-xs text-slate-400 font-mono">({{ filteredSettings.length }})</span>
        </div>
      </div>

      <n-data-table
        :columns="columns"
        :data="filteredSettings"
        :loading="loading"
        :scroll-x="1100"
        :row-key="(row: SystemSettingDto) => row.id"
        :bordered="false"
        class="min-h-[220px]"
      />
    </div>

    <!-- Future Extensibility Architecture Banner -->
    <div class="p-5 rounded-2xl bg-gradient-to-r from-slate-50 to-blue-50/40 dark:from-slate-800/60 dark:to-slate-800 border border-blue-100 dark:border-slate-700/80 space-y-2">
      <div class="flex items-center gap-2 text-blue-700 dark:text-blue-400 font-black text-sm">
        <n-icon size="18" :component="SparkleIcon" />
        <span>بنية ديناميكية جاهزة لإضافة إعدادات جديدة للنظام في أي وقت</span>
      </div>
      <p class="text-xs text-slate-600 dark:text-slate-300 leading-relaxed">
        تم ربط هذا الجدول بمحرك إعدادات ديناميكي في قاعدة البيانات والباك إند، بحيث يمكن في أي وقت إضافة أي متغيرات أو قيم جديدة للنظام (مثل فترات السماح، شروط العقود، أرقام السجلات أو الرسوم الإضافية) لتظهر تلقائياً في هذا الجدول دون أي تعديل في بنية البرمجيات.
      </p>
    </div>
  </div>
</template>
