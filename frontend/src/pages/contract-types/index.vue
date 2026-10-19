<script setup lang="ts">
import {
  ArrowClockwise24Regular as RefreshIcon,
  Info24Regular as InfoIcon,
} from '@vicons/fluent'
import type { DataTableColumns } from 'naive-ui'
import { NTag } from 'naive-ui'
import { ResultStatusEnum, isApiSuccess } from '~/models/ApiResponse'
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
    title: t('contractTypes.code'),
    key: 'code',
    width: 140,
    render(row) {
      return h(
        NTag,
        { type: 'info', bordered: false, class: 'font-mono font-bold' },
        { default: () => row.code },
      )
    },
  },
  {
    title: t('contractTypes.nameAr'),
    key: 'nameAr',
    render(row) {
      return h('div', { class: 'font-semibold text-slate-800 dark:text-white' }, row.nameAr)
    },
  },
  {
    title: t('contractTypes.nameEn'),
    key: 'nameEn',
    render(row) {
      return h('div', { class: 'font-mono text-slate-600 dark:text-slate-300' }, row.nameEn)
    },
  },
  {
    title: t('contractTypes.descriptionAr'),
    key: 'descriptionAr',
    render(row) {
      return row.descriptionAr || '-'
    },
  },
  {
    title: t('contractTypes.descriptionEn'),
    key: 'descriptionEn',
    render(row) {
      return row.descriptionEn || '-'
    },
  },
  {
    title: t('contractTypes.status'),
    key: 'isActive',
    width: 100,
    render(row) {
      return h(
        NTag,
        { type: row.isActive ? 'success' : 'default', bordered: false },
        { default: () => (row.isActive ? t('contractTypes.active') : t('contractTypes.inactive')) },
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
  <div class="p-6">
    <!-- Header -->
    <div class="flex flex-col md:flex-row md:items-center justify-between pb-6 gap-4">
      <div>
        <h1 class="text-2xl font-bold text-slate-800 dark:text-white flex items-center gap-2">
          <span>📋</span> {{ t('contractTypes.title') }}
        </h1>
        <p class="text-sm text-slate-500 mt-1">
          أنواع العقود المعتمدة في النظام (إيجار يومي، أسبوعي، شهري، وبيع معرض)
        </p>
      </div>

      <div class="flex items-center gap-2">
        <n-button secondary @click="loadContractTypes">
          <template #icon><RefreshIcon class="w-4 h-4" /></template>
          {{ t('common.refresh') }}
        </n-button>
      </div>
    </div>

    <!-- Read-Only Notice Alert -->
    <n-alert type="info" class="mb-6 rounded-xl" :show-icon="true">
      <template #icon>
        <InfoIcon class="w-5 h-5 text-blue-500" />
      </template>
      <div class="text-sm font-medium">
        {{ t('contractTypes.readOnlyNotice') }}
      </div>
    </n-alert>

    <!-- Data Table -->
    <div class="bg-white dark:bg-slate-800 rounded-xl shadow-sm border border-slate-100 dark:border-slate-700 overflow-hidden">
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
