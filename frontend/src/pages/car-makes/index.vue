<script setup lang="ts">
import {
  ArrowClockwise24Regular as RefreshIcon,
  Add24Regular as AddIcon,
  Search24Regular as SearchIcon,
  Edit24Regular as EditIcon,
  Delete24Regular as DeleteIcon,
  VehicleCar24Regular as CarIcon,
} from '@vicons/fluent'
import type { DataTableColumns, FormInst, FormRules } from 'naive-ui'
import { NButton, NIcon, NPopconfirm, NSpace, NTag } from 'naive-ui'
import { isApiSuccess } from '~/models/ApiResponse'
import type { CarMakeDto } from '~/models/CarMake'
import { useCarMakeService } from '~/services/car-make.service'

const { t } = useI18n()
const message = useMessage()
const router = useRouter()
const accountStore = useAccountStore()
const carMakeService = useCarMakeService()

// State
const carMakes = ref<CarMakeDto[]>([])
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
})

const rules: FormRules = {
  name: [
    { required: true, message: t('carMakes.validations.nameRequired'), trigger: ['blur', 'input'] },
    { max: 100, message: 'الاسم يجب ألا يتجاوز 100 حرف', trigger: ['blur', 'input'] },
  ],
}

// Permissions
const canCreate = computed(() => accountStore.hasPermission('CarMakes.Create'))
const canUpdate = computed(() => accountStore.hasPermission('CarMakes.Update'))
const canDelete = computed(() => accountStore.hasPermission('CarMakes.Delete'))

// Fetch Car Makes
async function loadCarMakes() {
  loading.value = true
  try {
    const res = await carMakeService.getPaged({
      pageIndex: pageIndex.value,
      pageSize: pageSize.value,
      search: search.value.trim() || undefined,
    })

    if (isApiSuccess(res) && res.data) {
      carMakes.value = res.data.items || []
      totalCount.value = res.data.totalCount || 0
    }
  } catch (err: any) {
    message.error(err?.response?.data?.message || err?.message || 'فشل تحميل أنواع السيارات')
  } finally {
    loading.value = false
  }
}

// Handlers
function handleSearch() {
  pageIndex.value = 1
  loadCarMakes()
}

function handlePageChange(page: number) {
  pageIndex.value = page
  loadCarMakes()
}

function handlePageSizeChange(size: number) {
  pageSize.value = size
  pageIndex.value = 1
  loadCarMakes()
}

function openAddModal() {
  isEditing.value = false
  editingId.value = null
  formModel.value = { name: '' }
  showModal.value = true
}

function openEditModal(row: CarMakeDto) {
  isEditing.value = true
  editingId.value = row.id
  formModel.value = { name: row.name }
  showModal.value = true
}

async function handleSubmit() {
  formRef.value?.validate(async (errors) => {
    if (errors) return

    submitLoading.value = true
    try {
      if (isEditing.value && editingId.value) {
        const res = await carMakeService.update(editingId.value, { name: formModel.value.name.trim() })
        if (isApiSuccess(res)) {
          message.success(t('carMakes.updateSuccess'))
          showModal.value = false
          loadCarMakes()
        } else {
          message.error(res.message || 'فشل تحديث نوع السيارة')
        }
      } else {
        const res = await carMakeService.create({ name: formModel.value.name.trim() })
        if (isApiSuccess(res)) {
          message.success(t('carMakes.createSuccess'))
          showModal.value = false
          pageIndex.value = 1
          loadCarMakes()
        } else {
          message.error(res.message || 'فشل إضافة نوع السيارة')
        }
      }
    } catch (err: any) {
      message.error(err?.response?.data?.message || err?.message || 'حدث خطأ أثناء الحفظ')
    } finally {
      submitLoading.value = false
    }
  })
}

async function handleDelete(row: CarMakeDto) {
  try {
    const res = await carMakeService.remove(row.id)
    if (isApiSuccess(res)) {
      message.success(t('carMakes.deleteSuccess'))
      if (carMakes.value.length === 1 && pageIndex.value > 1) {
        pageIndex.value -= 1
      }
      loadCarMakes()
    } else {
      message.error(res.message || 'فشل حذف نوع السيارة')
    }
  } catch (err: any) {
    message.error(err?.response?.data?.message || err?.message || 'لا يمكن حذف هذا النوع لارتباطه ببيانات أخرى')
  }
}

function viewModels(makeId: number) {
  router.push({ path: '/car-models', query: { makeId } })
}

// Table Columns
const columns: DataTableColumns<CarMakeDto> = [
  {
    title: '#',
    key: 'id',
    width: 70,
    render(row) {
      return h(NTag, { size: 'small', bordered: false, class: 'font-mono' }, { default: () => `#${row.id}` })
    },
  },
  {
    title: t('carMakes.name'),
    key: 'name',
    render(row) {
      return h('div', { class: 'flex items-center gap-2 font-bold text-slate-800 dark:text-white text-base' }, [
        h('span', { class: 'text-xl' }, '🚗'),
        h('span', row.name),
      ])
    },
  },
  {
    title: t('carMakes.modelsCount'),
    key: 'modelsCount',
    width: 170,
    render(row) {
      return h(
        NButton,
        {
          size: 'small',
          secondary: true,
          type: row.modelsCount > 0 ? 'info' : 'default',
          class: 'rounded-lg text-xs font-semibold',
          onClick: () => viewModels(row.id),
        },
        {
          default: () => `${row.modelsCount} ${t('carModels.title')}`,
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
              default: () => `${t('carMakes.deleteConfirm')} (${row.name})`,
            },
          ),
        )
      }

      return h(NSpace, { size: 'small' }, { default: () => actions })
    },
  },
]

onMounted(() => {
  loadCarMakes()
})
</script>

<route lang="yaml">
meta:
  title: carMakes
  layout: default
</route>

<template>
  <div class="p-4 sm:p-6 max-w-7xl mx-auto">
    <!-- Shared Lookups Navigation -->
    <LookupsNav active-key="makes" />

    <!-- Page Header -->
    <div class="flex flex-col sm:flex-row sm:items-center justify-between pb-6 gap-4 border-b border-slate-100 dark:border-slate-800">
      <div>
        <h1 class="text-2xl sm:text-3xl font-bold text-slate-800 dark:text-white flex items-center gap-3">
          <span class="p-2.5 rounded-2xl bg-emerald-50 dark:bg-emerald-950/60 text-emerald-600 dark:text-emerald-400 border border-emerald-100 dark:border-emerald-800/60 shadow-sm">
            🏷️
          </span>
          {{ t('carMakes.title') }}
        </h1>
        <p class="text-sm text-slate-500 dark:text-slate-400 mt-1.5 ms-1">
          {{ t('carMakes.subtitle') }}
        </p>
      </div>

      <div class="flex items-center gap-2.5">
        <n-button secondary class="rounded-xl" @click="loadCarMakes">
          <template #icon><RefreshIcon class="w-4 h-4" /></template>
          {{ t('common.refresh') }}
        </n-button>

        <n-button
          v-if="canCreate"
          type="primary"
          class="rounded-xl shadow-sm hover:shadow-md transition-shadow"
          @click="openAddModal"
        >
          <template #icon><AddIcon class="w-4 h-4" /></template>
          {{ t('carMakes.create') }}
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
          :placeholder="t('carMakes.searchPlaceholder')"
          @update:value="handleSearch"
          @clear="handleSearch"
        >
          <template #prefix>
            <SearchIcon class="w-4 h-4 text-slate-400" />
          </template>
        </n-input>
      </div>

      <div class="flex items-center gap-3 text-sm text-slate-500 dark:text-slate-400">
        <n-tag type="success" size="medium" :bordered="false" class="rounded-lg font-bold">
          {{ totalCount }} {{ t('carMakes.title') }}
        </n-tag>
      </div>
    </div>

    <!-- Data Table Card -->
    <div class="bg-white dark:bg-slate-800/90 rounded-2xl shadow-sm border border-slate-200/80 dark:border-slate-700/80 overflow-hidden backdrop-blur-sm">
      <n-data-table
        :columns="columns"
        :data="carMakes"
        :loading="loading"
        :scroll-x="850"
        :row-key="(row: CarMakeDto) => row.id"
        :bordered="false"
        class="min-h-[280px]"
      />

      <!-- Pagination -->
      <div class="p-4 border-t border-slate-100 dark:border-slate-700/80 flex flex-wrap items-center justify-between gap-3">
        <div class="text-xs text-slate-400">
          {{ t('common.total') }}: {{ totalCount }}
        </div>
        <n-pagination
          :page="pageIndex"
          :page-size="pageSize"
          :item-count="totalCount"
          :page-sizes="[10, 20, 50]"
          show-size-picker
          @update:page="handlePageChange"
          @update:page-size="handlePageSizeChange"
        />
      </div>
    </div>

    <!-- Create / Edit Modal -->
    <n-modal
      v-model:show="showModal"
      preset="card"
      class="max-w-md rounded-2xl shadow-2xl"
      :title="isEditing ? t('carMakes.edit') : t('carMakes.create')"
    >
      <n-form
        ref="formRef"
        :model="formModel"
        :rules="rules"
        label-placement="top"
        require-mark-placement="right-hanging"
      >
        <n-form-item path="name" :label="t('carMakes.name')">
          <n-input
            v-model:value="formModel.name"
            placeholder="مثال: Toyota, Hyundai, Honda..."
            class="rounded-xl"
            @keyup.enter="handleSubmit"
          />
        </n-form-item>

        <div class="flex justify-end gap-2.5 mt-4">
          <n-button secondary class="rounded-xl" @click="showModal = false">
            {{ t('common.cancel') }}
          </n-button>
          <n-button
            type="primary"
            :loading="submitLoading"
            class="rounded-xl px-5"
            @click="handleSubmit"
          >
            {{ isEditing ? t('common.update') : t('common.save') }}
          </n-button>
        </div>
      </n-form>
    </n-modal>
  </div>
</template>
