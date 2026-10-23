<script setup lang="ts">
import {
  ArrowDownload24Regular as DownloadIcon,
  CheckmarkCircle24Regular as CheckIcon,
  DocumentTable24Regular as ExcelIcon,
  Filter24Regular as FilterIcon,
} from '@vicons/fluent'
import {
  NButton,
  NCard,
  NIcon,
  NInputNumber,
  NModal,
  NRadio,
  NRadioGroup,
  NSpace,
  NTag,
} from 'naive-ui'

interface Props {
  show: boolean
  title?: string
  totalCount: number
  currentPageCount: number
  loading?: boolean
  defaultScope?: 'all' | 'page' | 'custom'
  defaultFormat?: 'xlsx' | 'xlsm'
}

const props = withDefaults(defineProps<Props>(), {
  title: 'تصدير البيانات إلى ملف إكسيل (Excel)',
  loading: false,
  defaultScope: 'all',
  defaultFormat: 'xlsx',
})

const emit = defineEmits<{
  (e: 'update:show', value: boolean): void
  (e: 'export', options: { scope: 'all' | 'page' | 'custom'; count: number; format: 'xlsx' | 'xlsm' }): void
}>()

// Internal State
const selectedScope = ref<'all' | 'page' | 'custom'>(props.defaultScope)
const selectedFormat = ref<'xlsx' | 'xlsm'>(props.defaultFormat)
const customCount = ref<number>(Math.min(props.totalCount || 50, 100))

// Sync customCount when totalCount changes
watch(
  () => props.totalCount,
  (newTotal) => {
    if (newTotal > 0 && customCount.value > newTotal) {
      customCount.value = newTotal
    } else if (customCount.value <= 0 && newTotal > 0) {
      customCount.value = Math.min(newTotal, 50)
    }
  },
  { immediate: true },
)

// Computed count of items that will actually be exported
const exportCount = computed(() => {
  if (props.totalCount === 0) return 0
  if (selectedScope.value === 'all') {
    return props.totalCount
  }
  if (selectedScope.value === 'page') {
    return Math.min(props.currentPageCount, props.totalCount)
  }
  return Math.max(1, Math.min(customCount.value || 1, props.totalCount))
})

function closeModal() {
  emit('update:show', false)
}

function handleExport() {
  emit('export', {
    scope: selectedScope.value,
    count: exportCount.value,
    format: selectedFormat.value,
  })
}

function setQuickCount(qty: number) {
  selectedScope.value = 'custom'
  customCount.value = Math.min(qty, props.totalCount)
}
</script>

<template>
  <n-modal
    :show="show"
    preset="card"
    :title="title"
    class="max-w-lg w-full shadow-2xl rounded-2xl"
    :segmented="{ content: true, action: true }"
    :mask-closable="!loading"
    :closable="!loading"
    @update:show="closeModal"
  >
    <div class="space-y-5 py-1">
      <!-- Info Header Banner -->
      <div class="flex items-center gap-3 p-3.5 rounded-xl bg-emerald-50/80 dark:bg-emerald-950/30 border border-emerald-200 dark:border-emerald-800/50">
        <div class="w-10 h-10 rounded-lg bg-emerald-500/10 dark:bg-emerald-500/20 text-emerald-600 dark:text-emerald-400 flex items-center justify-center shrink-0">
          <n-icon size="24"><ExcelIcon /></n-icon>
        </div>
        <div class="flex-1 min-w-0">
          <h4 class="text-sm font-semibold text-emerald-900 dark:text-emerald-200">
            تصدير تقرير إكسيل معتمد (.{{ selectedFormat }})
          </h4>
          <p class="text-xs text-emerald-700/80 dark:text-emerald-300/80 mt-0.5">
            يدعم اللغة العربية (RTL) مع التنسيق التلقائي للأعمدة والمبالغ
          </p>
        </div>
        <n-tag type="success" size="small" round class="font-mono font-medium">
          {{ totalCount }} إجمالي متاح
        </n-tag>
      </div>

      <!-- Format Selection -->
      <div class="space-y-2">
        <label class="block text-sm font-semibold text-slate-700 dark:text-slate-200">
          صيغة ملف الإكسيل:
        </label>
        <div class="grid grid-cols-2 gap-3">
          <!-- XLSX Option -->
          <div
            class="flex items-center gap-3 p-3 rounded-xl border transition-all cursor-pointer"
            :class="[
              selectedFormat === 'xlsx'
                ? 'border-emerald-500 bg-emerald-50/50 dark:bg-emerald-950/20 ring-1 ring-emerald-500'
                : 'border-slate-200 dark:border-slate-700 hover:bg-slate-50 dark:hover:bg-slate-800/50',
            ]"
            @click="selectedFormat = 'xlsx'"
          >
            <n-radio :checked="selectedFormat === 'xlsx'" value="xlsx" @change="selectedFormat = 'xlsx'" />
            <div>
              <div class="text-sm font-bold text-slate-800 dark:text-slate-100 font-mono">
                .xlsx (قياسي)
              </div>
              <div class="text-xs text-slate-500 dark:text-slate-400">
                مصنف Excel قياسي متوافق مع كافة البرامج والهواتف
              </div>
            </div>
          </div>

          <!-- XLSM Option -->
          <div
            class="flex items-center gap-3 p-3 rounded-xl border transition-all cursor-pointer"
            :class="[
              selectedFormat === 'xlsm'
                ? 'border-emerald-500 bg-emerald-50/50 dark:bg-emerald-950/20 ring-1 ring-emerald-500'
                : 'border-slate-200 dark:border-slate-700 hover:bg-slate-50 dark:hover:bg-slate-800/50',
            ]"
            @click="selectedFormat = 'xlsm'"
          >
            <n-radio :checked="selectedFormat === 'xlsm'" value="xlsm" @change="selectedFormat = 'xlsm'" />
            <div>
              <div class="text-sm font-bold text-emerald-700 dark:text-emerald-300 font-mono">
                .xlsm (ماكرو)
              </div>
              <div class="text-xs text-slate-500 dark:text-slate-400">
                مصنف Excel مدعوم بماكرو وقوالب الـ VBA
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Scope Selection -->
      <div class="space-y-3">
        <label class="block text-sm font-semibold text-slate-700 dark:text-slate-200">
          حدد نطاق وسجلات التصدير:
        </label>

        <n-radio-group v-model:value="selectedScope" name="exportScope" class="w-full flex flex-col gap-2.5">
          <!-- Option 1: All Filtered Results -->
          <div
            class="flex items-center justify-between p-3 rounded-xl border transition-all cursor-pointer"
            :class="[
              selectedScope === 'all'
                ? 'border-emerald-500 bg-emerald-50/40 dark:bg-emerald-950/20 ring-1 ring-emerald-500'
                : 'border-slate-200 dark:border-slate-700 hover:bg-slate-50 dark:hover:bg-slate-800/50',
            ]"
            @click="selectedScope = 'all'"
          >
            <div class="flex items-center gap-3">
              <n-radio value="all" />
              <div>
                <div class="text-sm font-medium text-slate-800 dark:text-slate-100 flex items-center gap-2">
                  <span>جميع النتائج المطابقة للفلتر</span>
                  <span class="text-xs text-emerald-600 dark:text-emerald-400 font-semibold bg-emerald-100 dark:bg-emerald-900/40 px-2 py-0.5 rounded-full">
                    شامل الفلتر
                  </span>
                </div>
                <div class="text-xs text-slate-500 dark:text-slate-400 mt-0.5">
                  تصدير جميع السجلات التي تطابق معايير البحث والفلترة دون التقيد بحد الصفحة
                </div>
              </div>
            </div>
            <div class="text-right shrink-0">
              <span class="text-base font-bold font-mono text-emerald-600 dark:text-emerald-400">
                {{ totalCount }}
              </span>
              <span class="text-xs text-slate-400 block">سجل</span>
            </div>
          </div>

          <!-- Option 2: Current Page Only -->
          <div
            class="flex items-center justify-between p-3 rounded-xl border transition-all cursor-pointer"
            :class="[
              selectedScope === 'page'
                ? 'border-emerald-500 bg-emerald-50/40 dark:bg-emerald-950/20 ring-1 ring-emerald-500'
                : 'border-slate-200 dark:border-slate-700 hover:bg-slate-50 dark:hover:bg-slate-800/50',
            ]"
            @click="selectedScope = 'page'"
          >
            <div class="flex items-center gap-3">
              <n-radio value="page" />
              <div>
                <div class="text-sm font-medium text-slate-800 dark:text-slate-100">
                  الصفحة الحالية فقط
                </div>
                <div class="text-xs text-slate-500 dark:text-slate-400 mt-0.5">
                  تصدير العناصر المعروضة حالياً في جدول الصفحة الحالية فقط
                </div>
              </div>
            </div>
            <div class="text-right shrink-0">
              <span class="text-base font-bold font-mono text-slate-700 dark:text-slate-300">
                {{ currentPageCount }}
              </span>
              <span class="text-xs text-slate-400 block">سجل</span>
            </div>
          </div>

          <!-- Option 3: Custom Count -->
          <div
            class="p-3 rounded-xl border transition-all cursor-pointer space-y-3"
            :class="[
              selectedScope === 'custom'
                ? 'border-emerald-500 bg-emerald-50/40 dark:bg-emerald-950/20 ring-1 ring-emerald-500'
                : 'border-slate-200 dark:border-slate-700 hover:bg-slate-50 dark:hover:bg-slate-800/50',
            ]"
            @click="selectedScope = 'custom'"
          >
            <div class="flex items-center justify-between">
              <div class="flex items-center gap-3">
                <n-radio value="custom" />
                <div>
                  <div class="text-sm font-medium text-slate-800 dark:text-slate-100">
                    تحديد عدد مخصص من السجلات
                  </div>
                  <div class="text-xs text-slate-500 dark:text-slate-400 mt-0.5">
                    التحكم في العدد المراد تصديره مستقلاً عن حد الـ Pagination (مثلاً 50، 100، إلخ)
                  </div>
                </div>
              </div>
              <div v-if="selectedScope === 'custom'" class="text-right shrink-0">
                <span class="text-base font-bold font-mono text-emerald-600 dark:text-emerald-400">
                  {{ exportCount }}
                </span>
                <span class="text-xs text-slate-400 block">سجل</span>
              </div>
            </div>

            <!-- Custom Count Input & Quick Chips -->
            <div v-if="selectedScope === 'custom'" class="pt-2 border-t border-slate-100 dark:border-slate-800 flex flex-col sm:flex-row sm:items-center justify-between gap-3" @click.stop>
              <div class="flex items-center gap-2">
                <span class="text-xs text-slate-600 dark:text-slate-400 font-medium">العدد المطلوب:</span>
                <n-input-number
                  v-model:value="customCount"
                  :min="1"
                  :max="totalCount > 0 ? totalCount : 10000"
                  size="small"
                  class="w-32"
                />
              </div>

              <!-- Quick Selection Buttons -->
              <div class="flex items-center gap-1.5 flex-wrap">
                <button
                  v-if="totalCount >= 25"
                  type="button"
                  class="text-xs px-2 py-1 rounded bg-slate-100 dark:bg-slate-700 hover:bg-emerald-100 dark:hover:bg-emerald-900/50 text-slate-700 dark:text-slate-300 font-mono transition-colors"
                  @click="setQuickCount(25)"
                >
                  25
                </button>
                <button
                  v-if="totalCount >= 50"
                  type="button"
                  class="text-xs px-2 py-1 rounded bg-slate-100 dark:bg-slate-700 hover:bg-emerald-100 dark:hover:bg-emerald-900/50 text-slate-700 dark:text-slate-300 font-mono transition-colors"
                  @click="setQuickCount(50)"
                >
                  50
                </button>
                <button
                  v-if="totalCount >= 100"
                  type="button"
                  class="text-xs px-2 py-1 rounded bg-slate-100 dark:bg-slate-700 hover:bg-emerald-100 dark:hover:bg-emerald-900/50 text-slate-700 dark:text-slate-300 font-mono transition-colors"
                  @click="setQuickCount(100)"
                >
                  100
                </button>
                <button
                  v-if="totalCount >= 200"
                  type="button"
                  class="text-xs px-2 py-1 rounded bg-slate-100 dark:bg-slate-700 hover:bg-emerald-100 dark:hover:bg-emerald-900/50 text-slate-700 dark:text-slate-300 font-mono transition-colors"
                  @click="setQuickCount(200)"
                >
                  200
                </button>
                <button
                  type="button"
                  class="text-xs px-2 py-1 rounded bg-slate-100 dark:bg-slate-700 hover:bg-emerald-100 dark:hover:bg-emerald-900/50 text-slate-700 dark:text-slate-300 transition-colors"
                  @click="setQuickCount(totalCount)"
                >
                  الكل
                </button>
              </div>
            </div>
          </div>
        </n-radio-group>
      </div>

      <!-- Live Summary Bar -->
      <div class="bg-slate-50 dark:bg-slate-800/60 p-3 rounded-xl flex items-center justify-between text-xs text-slate-600 dark:text-slate-300">
        <span class="flex items-center gap-1.5">
          <n-icon size="16" class="text-emerald-500"><CheckIcon /></n-icon>
          <span>سيتم تصدير:</span>
        </span>
        <span class="font-bold text-slate-900 dark:text-white font-mono text-sm">
          {{ exportCount }} سجل بصيغة .{{ selectedFormat }}
        </span>
      </div>
    </div>

    <!-- Actions -->
    <template #action>
      <div class="flex items-center justify-end gap-3">
        <n-button :disabled="loading" @click="closeModal">
          إلغاء
        </n-button>
        <n-button
          type="primary"
          :loading="loading"
          :disabled="exportCount === 0"
          class="!bg-emerald-600 hover:!bg-emerald-700 !border-emerald-600"
          @click="handleExport"
        >
          <template #icon>
            <n-icon><DownloadIcon /></n-icon>
          </template>
          تصدير إلى ملف .{{ selectedFormat }} ({{ exportCount }})
        </n-button>
      </div>
    </template>
  </n-modal>
</template>
