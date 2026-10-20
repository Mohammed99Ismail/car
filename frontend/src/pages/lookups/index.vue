<script setup lang="ts">
import {
  ArrowClockwise24Regular as RefreshIcon,
  Tag24Regular as MakesIcon,
  VehicleCar24Regular as ModelsIcon,
  DocumentBulletList24Regular as ContractTypesIcon,
  ArrowRight24Regular as ArrowRightIcon,
  Add24Regular as AddIcon,
} from '@vicons/fluent'
import { isApiSuccess } from '~/models/ApiResponse'
import { useCarMakeService } from '~/services/car-make.service'
import { useCarModelService } from '~/services/car-model.service'
import { useContractTypeService } from '~/services/contract-type.service'

const { t } = useI18n()
const router = useRouter()
const accountStore = useAccountStore()

const carMakeService = useCarMakeService()
const carModelService = useCarModelService()
const contractTypeService = useContractTypeService()

const loading = ref(false)
const stats = ref({
  makesCount: 0,
  modelsCount: 0,
  contractTypesCount: 0,
})

async function loadStats() {
  loading.value = true
  try {
    const [makesRes, modelsRes, contractRes] = await Promise.all([
      carMakeService.getPaged({ pageIndex: 1, pageSize: 1 }),
      carModelService.getPaged({ pageIndex: 1, pageSize: 1 }),
      contractTypeService.getAll(),
    ])

    if (isApiSuccess(makesRes) && makesRes.data) {
      stats.value.makesCount = makesRes.data.totalCount || 0
    }
    if (isApiSuccess(modelsRes) && modelsRes.data) {
      stats.value.modelsCount = modelsRes.data.totalCount || 0
    }
    if (isApiSuccess(contractRes) && contractRes.data) {
      stats.value.contractTypesCount = contractRes.data.length || 0
    }
  } catch (err) {
    console.error('Failed to load lookup stats', err)
  } finally {
    loading.value = false
  }
}

onMounted(() => {
  loadStats()
})
</script>

<route lang="yaml">
meta:
  title: lookups
  layout: default
</route>

<template>
  <div class="p-4 sm:p-6 max-w-7xl mx-auto">
    <!-- Lookups Navigation Bar -->
    <LookupsNav active-key="hub" />

    <!-- Header -->
    <div class="flex flex-col sm:flex-row sm:items-center justify-between pb-6 gap-4 border-b border-slate-100 dark:border-slate-800">
      <div>
        <h1 class="text-2xl sm:text-3xl font-bold text-slate-800 dark:text-white flex items-center gap-3">
          <span class="p-2.5 rounded-2xl bg-indigo-50 dark:bg-indigo-950/60 text-indigo-600 dark:text-indigo-400 border border-indigo-100 dark:border-indigo-800/60 shadow-sm">
            🗂️
          </span>
          {{ t('lookups.title') }}
        </h1>
        <p class="text-sm text-slate-500 dark:text-slate-400 mt-1.5 ms-1">
          {{ t('lookups.subtitle') }}
        </p>
      </div>

      <div class="flex items-center gap-2">
        <n-button secondary class="rounded-xl" @click="loadStats">
          <template #icon><RefreshIcon class="w-4 h-4" /></template>
          {{ t('common.refresh') }}
        </n-button>
      </div>
    </div>

    <!-- Lookups Cards Grid -->
    <div class="grid grid-cols-1 md:grid-cols-3 gap-6 my-8">
      <!-- 1. Car Makes Card -->
      <div class="bg-white dark:bg-slate-800 rounded-3xl p-6 border border-slate-200/80 dark:border-slate-700/80 shadow-sm hover:shadow-xl transition-all duration-300 flex flex-col justify-between group">
        <div>
          <div class="flex items-center justify-between mb-4">
            <div class="w-14 h-14 rounded-2xl bg-emerald-50 dark:bg-emerald-950/50 flex items-center justify-center text-2xl border border-emerald-100 dark:border-emerald-800 text-emerald-600 dark:text-emerald-400">
              🏷️
            </div>
            <n-tag type="success" size="large" :bordered="false" class="rounded-xl font-bold font-mono">
              {{ stats.makesCount }}
            </n-tag>
          </div>

          <h2 class="text-xl font-bold text-slate-800 dark:text-white mb-2 group-hover:text-emerald-600 dark:group-hover:text-emerald-400 transition-colors">
            {{ t('carMakes.title') }}
          </h2>
          <p class="text-sm text-slate-500 dark:text-slate-400 leading-relaxed mb-6">
            {{ t('carMakes.subtitle') }}
          </p>
        </div>

        <div class="flex items-center gap-2 pt-4 border-t border-slate-100 dark:border-slate-700/60">
          <n-button
            type="primary"
            class="flex-1 rounded-xl shadow-xs"
            @click="router.push('/car-makes')"
          >
            {{ t('lookups.manageMakes') }}
            <template #icon>
              <ArrowRightIcon class="w-4 h-4 rtl:rotate-180" />
            </template>
          </n-button>
        </div>
      </div>

      <!-- 2. Car Models Card -->
      <div class="bg-white dark:bg-slate-800 rounded-3xl p-6 border border-slate-200/80 dark:border-slate-700/80 shadow-sm hover:shadow-xl transition-all duration-300 flex flex-col justify-between group">
        <div>
          <div class="flex items-center justify-between mb-4">
            <div class="w-14 h-14 rounded-2xl bg-blue-50 dark:bg-blue-950/50 flex items-center justify-center text-2xl border border-blue-100 dark:border-blue-800 text-blue-600 dark:text-blue-400">
              🚘
            </div>
            <n-tag type="info" size="large" :bordered="false" class="rounded-xl font-bold font-mono">
              {{ stats.modelsCount }}
            </n-tag>
          </div>

          <h2 class="text-xl font-bold text-slate-800 dark:text-white mb-2 group-hover:text-blue-600 dark:group-hover:text-blue-400 transition-colors">
            {{ t('carModels.title') }}
          </h2>
          <p class="text-sm text-slate-500 dark:text-slate-400 leading-relaxed mb-6">
            {{ t('carModels.subtitle') }}
          </p>
        </div>

        <div class="flex items-center gap-2 pt-4 border-t border-slate-100 dark:border-slate-700/60">
          <n-button
            type="info"
            class="flex-1 rounded-xl shadow-xs"
            @click="router.push('/car-models')"
          >
            {{ t('lookups.manageModels') }}
            <template #icon>
              <ArrowRightIcon class="w-4 h-4 rtl:rotate-180" />
            </template>
          </n-button>
        </div>
      </div>

      <!-- 3. Contract Types Card -->
      <div class="bg-white dark:bg-slate-800 rounded-3xl p-6 border border-slate-200/80 dark:border-slate-700/80 shadow-sm hover:shadow-xl transition-all duration-300 flex flex-col justify-between group">
        <div>
          <div class="flex items-center justify-between mb-4">
            <div class="w-14 h-14 rounded-2xl bg-amber-50 dark:bg-amber-950/50 flex items-center justify-center text-2xl border border-amber-100 dark:border-amber-800 text-amber-600 dark:text-amber-400">
              📋
            </div>
            <n-tag type="warning" size="large" :bordered="false" class="rounded-xl font-bold font-mono">
              {{ stats.contractTypesCount }}
            </n-tag>
          </div>

          <h2 class="text-xl font-bold text-slate-800 dark:text-white mb-2 group-hover:text-amber-600 dark:group-hover:text-amber-400 transition-colors">
            {{ t('menu.contractTypes') }}
          </h2>
          <p class="text-sm text-slate-500 dark:text-slate-400 leading-relaxed mb-6">
            أنواع العقود المعتمدة في النظام (إيجار قصير وطويل الأجل وإيجار منتهي بالتمليك) للقراءة فقط.
          </p>
        </div>

        <div class="flex items-center gap-2 pt-4 border-t border-slate-100 dark:border-slate-700/60">
          <n-button
            type="warning"
            class="flex-1 rounded-xl shadow-xs"
            @click="router.push('/contract-types')"
          >
            {{ t('lookups.manageContractTypes') }}
            <template #icon>
              <ArrowRightIcon class="w-4 h-4 rtl:rotate-180" />
            </template>
          </n-button>
        </div>
      </div>
    </div>
  </div>
</template>
