<script setup lang="ts">
import {
  ArrowClockwise24Regular as RefreshIcon,
  Add24Regular as AddIcon,
  Search24Regular as SearchIcon,
  Edit24Regular as EditIcon,
  Delete24Regular as DeleteIcon,
  Filter24Regular as FilterIcon,
  VehicleCar24Regular as ModelIcon,
} from '@vicons/fluent'
import type { DataTableColumns, FormInst, FormRules, SelectOption } from 'naive-ui'
import { NButton, NPopconfirm, NSpace, NTag } from 'naive-ui'
import type { LookupItemDto } from '~/models/ApiResponse'
import { isApiSuccess } from '~/models/ApiResponse'
import type { CarModelDto } from '~/models/CarModel'
import { useCarModelService } from '~/services/car-model.service'
import { useLookupService } from '~/services/lookup.service'

const { t } = useI18n()
const message = useMessage()
const route = useRoute()
const accountStore = useAccountStore()
const carModelService = useCarModelService()
const lookupService = useLookupService()

// State
const carModels = ref<CarModelDto[]>([])
const makesList = ref<LookupItemDto[]>([])
const loading = ref(false)
const search = ref('')
const selectedMakeId = ref<number | null>(null)
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
  makeId: null as number | null,
  name: '',
})

const rules: FormRules = {
  makeId: [
    { required: true, type: 'number', message: t('carModels.validations.makeRequired'), trigger: ['blur', 'change'] },
  ],
  name: [
    { required: true, message: t('carModels.validations.nameRequired'), trigger: ['blur', 'input'] },
    { max: 100, message: 'الاسم يجب ألا يتجاوز 100 حرف', trigger: ['blur', 'input'] },
  ],
}

// Permissions
const canCreate = computed(() => accountStore.hasPermission('CarModels.Create'))
const canUpdate = computed(() => accountStore.hasPermission('CarModels.Update'))
const canDelete = computed(() => accountStore.hasPermission('CarModels.Delete'))

// Makes options for select
const makeSelectOptions = computed<SelectOption[]>(() =>
  makesList.value.map((m) => ({
    label: m.name,
    value: m.id,
  })),
)

// Load makes lookup for filters & modal
async function loadMakes() {
  try {
    const res = await lookupService.getMakes()
    if (isApiSuccess(res) && res.data) {
      makesList.value = res.data
    }
  } catch (err) {
    console.error('Failed to load makes lookup', err)
  }
}

// Fetch Car Models
async function loadCarModels() {
  loading.value = true
  try {
    const res = await carModelService.getPaged({
      pageIndex: pageIndex.value,
      pageSize: pageSize.value,
      search: search.value.trim() || undefined,
      makeId: selectedMakeId.value || undefined,
    })

    if (isApiSuccess(res) && res.data) {
      carModels.value = res.data.items || []
      totalCount.value = res.data.totalCount || 0
    }
  } catch (err: any) {
    message.error(err?.response?.data?.message || err?.message || 'فشل تحميل موديلات السيارات')
  } finally {
    loading.value = false
  }
}

// Handlers
function handleSearch() {
  pageIndex.value = 1
  loadCarModels()
}

function handleMakeFilterChange(val: number | null) {
  selectedMakeId.value = val
  pageIndex.value = 1
  loadCarModels()
}

function handlePageChange(page: number) {
  pageIndex.value = page
  loadCarModels()
}

function handlePageSizeChange(size: number) {
  pageSize.value = size
  pageIndex.value = 1
  loadCarModels()
}

function openAddModal() {
  isEditing.value = false
  editingId.value = null
  formModel.value = {
    makeId: selectedMakeId.value || (makesList.value[0]?.id ?? null),
    name: '',
  }
  showModal.value = true
}

function openEditModal(row: CarModelDto) {
  isEditing.value = true
  editingId.value = row.id
  formModel.value = {
    makeId: row.makeId,
    name: row.name,
  }
  showModal.value = true
}

async function handleSubmit() {
  formRef.value?.validate(async (errors) => {
    if (errors) return

    submitLoading.value = true
    try {
      if (isEditing.value && editingId.value) {
        const res = await carModelService.update(editingId.value, {
          makeId: formModel.value.makeId!,
          name: formModel.value.name.trim(),
        })
        if (isApiSuccess(res)) {
          message.success(t('carModels.updateSuccess'))
          showModal.value = false
          loadCarModels()
        } else {
          message.error(res.message || 'فشل تحديث موديل السيارة')
        }
      } else {
        const res = await carModelService.create({
          makeId: formModel.value.makeId!,
          name: formModel.value.name.trim(),
        })
        if (isApiSuccess(res)) {
          message.success(t('carModels.createSuccess'))
          showModal.value = false
          pageIndex.value = 1
          loadCarModels()
        } else {
          message.error(res.message || 'فشل إضافة موديل السيارة')
        }
      }
    } catch (err: any) {
      message.error(err?.response?.data?.message || err?.message || 'حدث خطأ أثناء الحفظ')
    } finally {
      submitLoading.value = false
    }
  })
}

async function handleDelete(row: CarModelDto) {
  try {
    const res = await carModelService.remove(row.id)
    if (isApiSuccess(res)) {
      message.success(t('carModels.deleteSuccess'))
      if (carModels.value.length === 1 && pageIndex.value > 1) {
        pageIndex.value -= 1
      }
      loadCarModels()
    } else {
      message.error(res.message || 'فشل حذف موديل السيارة')
    }
  } catch (err: any) {
    message.error(err?.response?.data?.message || err?.message || 'لا يمكن حذف هذا الموديل لارتباطه ببيانات أخرى')
  }
}

// Table Columns
const columns: DataTableColumns<CarModelDto> = [
  {
    title: '#',
    key: 'id',
    width: 70,
    render(row) {
      return h(NTag, { size: 'small', bordered: false, class: 'font-mono' }, { default: () => `#${row.id}` })
    },
  },
  {
    title: t('carModels.make'),
    key: 'makeName',
    width: 200,
    render(row) {
      return h(
        NTag,
        {
          type: 'info',
          bordered: false,
          class: 'font-semibold rounded-lg px-2.5 py-1',
        },
        {
          default: () => row.makeName,
          icon: () => h('span', { class: 'me-1' }, '🚗'),
        },
      )
    },
  },
  {
    title: t('carModels.name'),
    key: 'name',
    render(row) {
      return h('div', { class: 'font-bold text-slate-800 dark:text-white text-base flex items-center gap-2' }, [
        h('span', { class: 'text-blue-500' }, '🚘'),
        h('span', row.name),
      ])
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
              default: () => `${t('carModels.deleteConfirm')} (${row.name})`,
            },
          ),
        )
      }

      return h(NSpace, { size: 'small' }, { default: () => actions })
    },
  },
]

onMounted(async () => {
  await loadMakes()
  // Check if makeId query param exists
  if (route.query.makeId) {
    selectedMakeId.value = Number(route.query.makeId)
  }
  loadCarModels()
})
</script>

<route lang="yaml">
meta:
  title: carModels
  layout: default
</route>

<template>
  <div class="p-4 sm:p-6 max-w-7xl mx-auto">
    <!-- Shared Lookups Navigation -->
    <LookupsNav active-key="models" />

    <!-- Page Header -->
    <div class="flex flex-col sm:flex-row sm:items-center justify-between pb-6 gap-4 border-b border-slate-100 dark:border-slate-800">
      <div>
        <h1 class="text-2xl sm:text-3xl font-bold text-slate-800 dark:text-white flex items-center gap-3">
          <span class="p-2.5 rounded-2xl bg-blue-50 dark:bg-blue-950/60 text-blue-600 dark:text-blue-400 border border-blue-100 dark:border-blue-800/60 shadow-sm">
            🚘
          </span>
          {{ t('carModels.title') }}
        </h1>
        <p class="text-sm text-slate-500 dark:text-slate-400 mt-1.5 ms-1">
          {{ t('carModels.subtitle') }}
        </p>
      </div>

      <div class="flex items-center gap-2.5">
        <n-button secondary class="rounded-xl" @click="loadCarModels">
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
          {{ t('carModels.create') }}
        </n-button>
      </div>
    </div>

    <!-- Filters & Stats Bar -->
    <div class="my-6 flex flex-col md:flex-row md:items-center justify-between gap-4">
      <div class="flex flex-wrap items-center gap-3">
        <!-- Make Selector Filter -->
        <div class="w-full sm:w-60">
          <LookupSelect
            v-model:value="selectedMakeId"
            lookup-type="makes"
            :placeholder="t('carModels.filterByMake')"
            @update:value="handleMakeFilterChange"
          />
        </div>

        <!-- Search Input -->
        <div class="w-full sm:w-72">
          <n-input
            v-model:value="search"
            clearable
            class="rounded-xl"
            :placeholder="t('carModels.searchPlaceholder')"
            @update:value="handleSearch"
            @clear="handleSearch"
          >
            <template #prefix>
              <SearchIcon class="w-4 h-4 text-slate-400" />
            </template>
          </n-input>
        </div>
      </div>

      <div class="flex items-center gap-3 text-sm text-slate-500 dark:text-slate-400">
        <n-tag type="info" size="medium" :bordered="false" class="rounded-lg font-bold">
          {{ totalCount }} {{ t('carModels.title') }}
        </n-tag>
      </div>
    </div>

    <!-- Data Table Card -->
    <div class="bg-white dark:bg-slate-800/90 rounded-2xl shadow-sm border border-slate-200/80 dark:border-slate-700/80 overflow-hidden backdrop-blur-sm">
      <n-data-table
        :columns="columns"
        :data="carModels"
        :loading="loading"
        :scroll-x="850"
        :row-key="(row: CarModelDto) => row.id"
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
      :title="isEditing ? t('carModels.edit') : t('carModels.create')"
    >
      <n-form
        ref="formRef"
        :model="formModel"
        :rules="rules"
        label-placement="top"
        require-mark-placement="right-hanging"
      >
        <n-form-item path="makeId" :label="t('carModels.make')">
          <LookupSelect
            v-model:value="formModel.makeId"
            lookup-type="makes"
            :placeholder="t('carModels.selectMake')"
          />
        </n-form-item>

        <n-form-item path="name" :label="t('carModels.name')">
          <n-input
            v-model:value="formModel.name"
            placeholder="مثال: Camry, Accord, Elantra, C-Class..."
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
