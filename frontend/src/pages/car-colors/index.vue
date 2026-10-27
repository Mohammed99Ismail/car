<script setup lang="ts">
import {
  ArrowClockwise24Regular as RefreshIcon,
  Add24Regular as AddIcon,
  Search24Regular as SearchIcon,
  Edit24Regular as EditIcon,
  Delete24Regular as DeleteIcon,
  VehicleCar24Regular as CarIcon,
  Color24Regular as ColorIcon,
} from '@vicons/fluent'
import type { DataTableColumns, FormInst, FormRules } from 'naive-ui'
import { NButton, NPopconfirm, NSpace, NTag, NColorPicker } from 'naive-ui'
import { isApiSuccess } from '~/models/ApiResponse'
import type { CarColorDto } from '~/models/CarColor'
import { useCarColorService } from '~/services/car-color.service'

const { t } = useI18n()
const message = useMessage()
const router = useRouter()
const accountStore = useAccountStore()
const carColorService = useCarColorService()

// State
const carColors = ref<CarColorDto[]>([])
const loading = ref(false)
const search = ref('')
const totalCount = ref(0)
const pageIndex = ref(1)
const pageSize = ref(10)

// Modal State
const showModal = ref(false)
const isEditing = ref(false)
const editingId = ref<number | null>(null)
const submitLoading = ref(false)
const formRef = ref<FormInst | null>(null)
const formModel = ref({
  name: '',
  hexCode: '#FFFFFF',
})

const rules: FormRules = {
  name: [
    { required: true, message: t('carColors.validations.nameRequired'), trigger: ['blur', 'input'] },
    { max: 50, message: 'اسم اللون يجب ألا يتجاوز 50 حرفاً', trigger: ['blur', 'input'] },
  ],
}

// Permissions
const canCreate = computed(() => accountStore.hasPermission('CarColors.Create'))
const canUpdate = computed(() => accountStore.hasPermission('CarColors.Update'))
const canDelete = computed(() => accountStore.hasPermission('CarColors.Delete'))

// Fetch Car Colors
async function loadCarColors() {
  loading.value = true
  try {
    const res = await carColorService.getPaged({
      pageIndex: pageIndex.value,
      pageSize: pageSize.value,
      search: search.value.trim() || undefined,
    })

    if (isApiSuccess(res) && res.data) {
      carColors.value = res.data.items || []
      totalCount.value = res.data.totalCount || 0
    }
  } catch (err: any) {
    message.error(err?.response?.data?.message || err?.message || 'فشل تحميل ألوان السيارات')
  } finally {
    loading.value = false
  }
}

// Handlers
function handleSearch() {
  pageIndex.value = 1
  loadCarColors()
}

function handlePageChange(page: number) {
  pageIndex.value = page
  loadCarColors()
}

function handlePageSizeChange(size: number) {
  pageSize.value = size
  pageIndex.value = 1
  loadCarColors()
}

function openAddModal() {
  isEditing.value = false
  editingId.value = null
  formModel.value = { name: '', hexCode: '#111827' }
  showModal.value = true
}

function openEditModal(row: CarColorDto) {
  isEditing.value = true
  editingId.value = row.id
  formModel.value = {
    name: row.name,
    hexCode: row.hexCode || '#111827',
  }
  showModal.value = true
}

async function handleSubmit() {
  formRef.value?.validate(async (errors) => {
    if (errors) return

    submitLoading.value = true
    try {
      const payload = {
        name: formModel.value.name.trim(),
        hexCode: formModel.value.hexCode?.trim() || null,
      }

      if (isEditing.value && editingId.value) {
        const res = await carColorService.update(editingId.value, payload)
        if (isApiSuccess(res)) {
          message.success(t('carColors.updateSuccess'))
          showModal.value = false
          loadCarColors()
        } else {
          message.error(res.message || 'فشل تحديث بيانات اللون')
        }
      } else {
        const res = await carColorService.create(payload)
        if (isApiSuccess(res)) {
          message.success(t('carColors.createSuccess'))
          showModal.value = false
          pageIndex.value = 1
          loadCarColors()
        } else {
          message.error(res.message || 'فشل إضافة اللون')
        }
      }
    } catch (err: any) {
      message.error(err?.response?.data?.message || err?.message || 'حدث خطأ أثناء الحفظ')
    } finally {
      submitLoading.value = false
    }
  })
}

async function handleDelete(row: CarColorDto) {
  try {
    const res = await carColorService.remove(row.id)
    if (isApiSuccess(res)) {
      message.success(t('carColors.deleteSuccess'))
      if (carColors.value.length === 1 && pageIndex.value > 1) {
        pageIndex.value -= 1
      }
      loadCarColors()
    } else {
      message.error(res.message || 'فشل حذف اللون')
    }
  } catch (err: any) {
    message.error(err?.response?.data?.message || err?.message || 'لا يمكن حذف هذا اللون لارتباطه بسيارات مسجلة')
  }
}

function viewCarsWithColor(colorId: number) {
  router.push({ path: '/cars', query: { colorId } })
}

// Table Columns
const columns: DataTableColumns<CarColorDto> = [
  {
    title: '#',
    key: 'id',
    width: 70,
    render(row) {
      return h(NTag, { size: 'small', bordered: false, class: 'font-mono' }, { default: () => `#${row.id}` })
    },
  },
  {
    title: t('carColors.name'),
    key: 'name',
    render(row) {
      return h('div', { class: 'flex items-center gap-3' }, [
        h('span', {
          class: 'w-6 h-6 rounded-full border-2 border-slate-200 dark:border-slate-700 shadow-sm shrink-0',
          style: { backgroundColor: row.hexCode || '#cccccc' },
        }),
        h('div', { class: 'flex flex-col' }, [
          h('span', { class: 'font-bold text-slate-800 dark:text-white text-base' }, row.name),
          row.hexCode
            ? h('span', { class: 'text-xs text-slate-400 font-mono tracking-wider uppercase' }, row.hexCode)
            : null,
        ]),
      ])
    },
  },
  {
    title: t('carColors.preview'),
    key: 'hexCode',
    width: 140,
    render(row) {
      return h('div', { class: 'flex items-center gap-2' }, [
        h('div', {
          class: 'w-10 h-6 rounded-md border border-slate-300 dark:border-slate-600 shadow-2xs',
          style: { backgroundColor: row.hexCode || '#ffffff' },
        }),
        h(NTag, { size: 'small', bordered: false, class: 'font-mono text-xs uppercase' }, {
          default: () => row.hexCode || 'غير محدد',
        }),
      ])
    },
  },
  {
    title: t('carColors.carsCount'),
    key: 'carsCount',
    width: 170,
    render(row) {
      return h(
        NButton,
        {
          size: 'small',
          secondary: true,
          type: row.carsCount > 0 ? 'info' : 'default',
          class: 'rounded-lg text-xs font-semibold',
          onClick: () => viewCarsWithColor(row.id),
        },
        {
          default: () => `${row.carsCount} ${t('menu.cars')}`,
          icon: () => h(CarIcon, { class: 'w-3.5 h-3.5' }),
        },
      )
    },
  },
  {
    title: t('common.actions'),
    key: 'actions',
    width: 150,
    render(row) {
      const actions: any[] = []

      if (canUpdate.value) {
        actions.push(
          h(
            NButton,
            {
              size: 'small',
              secondary: true,
              type: 'warning',
              class: 'rounded-lg',
              onClick: () => openEditModal(row),
            },
            {
              default: () => t('common.edit'),
              icon: () => h(EditIcon, { class: 'w-3.5 h-3.5' }),
            },
          ),
        )
      }

      if (canDelete.value) {
        actions.push(
          h(
            NPopconfirm,
            {
              onPositiveClick: () => handleDelete(row),
              positiveText: t('common.confirm'),
              negativeText: t('common.cancel'),
            },
            {
              trigger: () =>
                h(
                  NButton,
                  {
                    size: 'small',
                    secondary: true,
                    type: 'error',
                    class: 'rounded-lg',
                  },
                  {
                    default: () => t('common.delete'),
                    icon: () => h(DeleteIcon, { class: 'w-3.5 h-3.5' }),
                  },
                ),
              default: () => `${t('carColors.deleteConfirm')} (${row.name})`,
            },
          ),
        )
      }

      return h(NSpace, { size: 'small' }, { default: () => actions })
    },
  },
]

onMounted(() => {
  loadCarColors()
})
</script>

<route lang="yaml">
meta:
  title: carColors
  layout: default
</route>

<template>
  <div class="p-4 sm:p-6 max-w-7xl mx-auto">
    <!-- Shared Lookups Navigation -->
    <LookupsNav active-key="colors" />

    <!-- Page Header -->
    <div class="flex flex-col sm:flex-row sm:items-center justify-between pb-6 gap-4 border-b border-slate-100 dark:border-slate-800">
      <div>
        <h1 class="text-2xl sm:text-3xl font-bold text-slate-800 dark:text-white flex items-center gap-3">
          <span class="p-2.5 rounded-2xl bg-purple-50 dark:bg-purple-950/60 text-purple-600 dark:text-purple-400 border border-purple-100 dark:border-purple-800/60 shadow-sm">
            🎨
          </span>
          {{ t('carColors.title') }}
        </h1>
        <p class="text-sm text-slate-500 dark:text-slate-400 mt-1.5 ms-1">
          {{ t('carColors.subtitle') }}
        </p>
      </div>

      <div class="flex items-center gap-2.5">
        <n-button secondary class="rounded-xl" @click="loadCarColors">
          <template #icon><RefreshIcon class="w-4 h-4" /></template>
          {{ t('common.refresh') }}
        </n-button>

        <n-button
          v-if="canCreate"
          type="primary"
          class="rounded-xl shadow-sm hover:shadow-md transition-shadow !bg-purple-600 hover:!bg-purple-700"
          @click="openAddModal"
        >
          <template #icon><AddIcon class="w-4 h-4" /></template>
          {{ t('carColors.create') }}
        </n-button>
      </div>
    </div>

    <!-- Filters & Stats Bar -->
    <div class="my-6 flex flex-col sm:flex-row sm:items-center justify-between gap-4">
      <div class="w-full sm:w-80">
        <n-input
          v-model:value="search"
          clearable
          class="rounded-xl shadow-xs"
          :placeholder="t('carColors.searchPlaceholder')"
          @update:value="handleSearch"
          @clear="handleSearch"
        >
          <template #prefix>
            <SearchIcon class="w-4 h-4 text-slate-400" />
          </template>
        </n-input>
      </div>

      <div class="flex items-center gap-2 text-sm text-slate-500 font-medium">
        <span>إجمالي الألوان:</span>
        <n-tag type="info" size="small" class="font-bold font-mono">
          {{ totalCount }}
        </n-tag>
      </div>
    </div>

    <!-- Table -->
    <div class="bg-white dark:bg-slate-800 rounded-2xl border border-slate-200/80 dark:border-slate-700/80 shadow-sm overflow-hidden">
      <n-data-table
        remote
        :columns="columns"
        :data="carColors"
        :loading="loading"
        :row-key="(row: CarColorDto) => row.id"
        :pagination="{
          page: pageIndex,
          pageSize,
          itemCount: totalCount,
          showSizePicker: true,
          pageSizes: [10, 20, 50],
          prefix: ({ itemCount }) => `إجمالي: ${itemCount} لون`,
          onUpdatePage: handlePageChange,
          onUpdatePageSize: handlePageSizeChange,
        }"
        :bordered="false"
        class="min-h-[300px]"
      />
    </div>

    <!-- Add/Edit Modal -->
    <n-modal
      v-model:show="showModal"
      preset="card"
      :title="isEditing ? t('carColors.edit') : t('carColors.create')"
      class="max-w-md w-full rounded-2xl shadow-xl"
    >
      <n-form
        ref="formRef"
        :model="formModel"
        :rules="rules"
        label-placement="top"
        require-mark-placement="right-hanging"
      >
        <!-- Color Name -->
        <n-form-item path="name" :label="t('carColors.name')">
          <n-input
            v-model:value="formModel.name"
            placeholder="مثال: أبيض لؤلؤي، أسود ملكي، رمادي فلكي"
            class="rounded-xl"
          />
        </n-form-item>

        <!-- Color Hex & Picker -->
        <n-form-item path="hexCode" :label="t('carColors.hexCode')">
          <div class="space-y-3 w-full">
            <div class="flex items-center gap-3">
              <n-color-picker
                v-model:value="formModel.hexCode"
                :show-alpha="false"
                class="w-16"
              />
              <n-input
                v-model:value="formModel.hexCode"
                placeholder="#FFFFFF"
                class="rounded-xl font-mono uppercase"
              />
            </div>

            <!-- Live Preview Pill -->
            <div class="flex items-center gap-3 p-3 bg-slate-50 dark:bg-slate-900/60 rounded-xl border border-slate-200/60 dark:border-slate-700/60">
              <span
                class="w-8 h-8 rounded-full border-2 border-white dark:border-slate-700 shadow-md shrink-0 transition-transform duration-300 hover:scale-110"
                :style="{ backgroundColor: formModel.hexCode || '#ffffff' }"
              />
              <div class="flex flex-col">
                <span class="text-xs text-slate-400">معاينة اللون والاسم:</span>
                <span class="text-sm font-bold text-slate-800 dark:text-white">
                  {{ formModel.name || 'اسم اللون سيظهر هنا' }}
                </span>
              </div>
            </div>
          </div>
        </n-form-item>

        <!-- Actions -->
        <div class="flex items-center justify-end gap-3 mt-6 pt-4 border-t border-slate-100 dark:border-slate-700/60">
          <n-button secondary class="rounded-xl" @click="showModal = false">
            {{ t('common.cancel') }}
          </n-button>
          <n-button
            type="primary"
            :loading="submitLoading"
            class="rounded-xl !bg-purple-600 hover:!bg-purple-700"
            @click="handleSubmit"
          >
            {{ t('common.save') }}
          </n-button>
        </div>
      </n-form>
    </n-modal>
  </div>
</template>
