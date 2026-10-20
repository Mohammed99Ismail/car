<script setup lang="ts">
import {
  ArrowClockwise24Regular as RefreshIcon,
  Info24Regular as InfoIcon,
} from '@vicons/fluent'
import type { DataTableColumns } from 'naive-ui'
import { NTag } from 'naive-ui'
import { isApiSuccess } from '~/models/ApiResponse'
import type { ContractTypeDto } from '~/models/ContractType'
import { useContractTypeService } from '~/services/contract-type.service'

const { t } = useI18n()
const contractTypeService = useContractTypeService()

const contractTypes = ref<ContractTypeDto[]>([])
const loading = ref(false)

async function loadContractTypes() {
  loading.value = true
  try {
    const res = await contractTypeService.getAll()
    if (isApiSuccess(res) && res.data) {
      contractTypes.value = res.data
    }
  } catch (err) {
    console.error('Error loading contract types', err)
  } finally {
    loading.value = false
  }
}

const columns: DataTableColumns<ContractTypeDto> = [
  {
    title: '#',
    key: 'id',
    width: 80,
    render(row) {
      return h(
        NTag,
        { type: 'info', bordered: false, class: 'font-mono font-bold' },
        { default: () => `#${row.id}` },
      )
    },
  },
  {
    title: 'نوع العقد (عربي)',
    key: 'nameAr',
    render(row) {
      return h('div', { class: 'font-bold text-slate-800 dark:text-white text-base flex items-center gap-2' }, [
        h('span', '📋'),
        h('span', row.nameAr),
      ])
    },
  },
  {
    title: 'Contract Type (English)',
    key: 'nameEn',
    render(row) {
      return h('div', { class: 'font-mono font-medium text-slate-600 dark:text-slate-300' }, row.nameEn)
    },
  },
  {
    title: 'النوع',
    key: 'type',
    width: 130,
    render() {
      return h(
        NTag,
        { type: 'warning', bordered: false, class: 'rounded-lg font-medium' },
        { default: () => 'مرجعي ثابت' },
      )
    },
  },
]

onMounted(() => {
  loadContractTypes()
})
</script>

<route lang="yaml">
meta:
  title: contractTypes
  layout: default
</route>

<template>
  <div class="p-4 sm:p-6 max-w-7xl mx-auto">
    <!-- Lookups Navigation Bar -->
    <LookupsNav active-key="contract-types" />

    <!-- Header -->
    <div class="flex flex-col sm:flex-row sm:items-center justify-between pb-6 gap-4 border-b border-slate-100 dark:border-slate-800">
      <div>
        <h1 class="text-2xl sm:text-3xl font-bold text-slate-800 dark:text-white flex items-center gap-3">
          <span class="p-2.5 rounded-2xl bg-amber-50 dark:bg-amber-950/60 text-amber-600 dark:text-amber-400 border border-amber-100 dark:border-amber-800/60 shadow-sm">
            📋
          </span>
          {{ t('menu.contractTypes') }}
        </h1>
        <p class="text-sm text-slate-500 dark:text-slate-400 mt-1.5 ms-1">
          أنواع العقود المعتمدة في النظام (إيجار يومي، أسبوعي، شهري، وبيع معرض) للقراءة فقط
        </p>
      </div>

      <div class="flex items-center gap-2">
        <n-button secondary class="rounded-xl" @click="loadContractTypes">
          <template #icon><RefreshIcon class="w-4 h-4" /></template>
          {{ t('common.refresh') }}
        </n-button>
      </div>
    </div>

    <!-- Read-Only Notice Alert -->
    <n-alert type="info" class="my-6 rounded-2xl shadow-xs" :show-icon="true">
      <template #icon>
        <InfoIcon class="w-5 h-5 text-blue-500" />
      </template>
      <div class="text-sm font-medium">
        هذا الجدول مرجعي للقراءة فقط بدون عمليات تعديل أو حذف لضمان سلامة العقود المسجلة بالمعرض.
      </div>
    </n-alert>

    <!-- Data Table Card -->
    <div class="bg-white dark:bg-slate-800/90 rounded-2xl shadow-sm border border-slate-200/80 dark:border-slate-700/80 overflow-hidden backdrop-blur-sm">
      <n-data-table
        :columns="columns"
        :data="contractTypes"
        :loading="loading"
        :scroll-x="600"
        :row-key="(row: ContractTypeDto) => row.id"
        :bordered="false"
        class="min-h-[220px]"
      />
    </div>
  </div>
</template>
