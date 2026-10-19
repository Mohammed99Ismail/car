<script setup lang="ts">
import {
  Add24Regular as AddIcon,
  Delete24Regular as DeleteIcon,
  Edit24Regular as EditIcon,
  Filter24Regular as FilterIcon,
  Image24Regular as ImageIcon,
  ArrowClockwise24Regular as RefreshIcon,
  Search24Regular as SearchIcon,
} from '@vicons/fluent'
import { useDebounceFn } from '@vueuse/core'
import type { DataTableColumns, FormInst, FormRules } from 'naive-ui'
import { NButton, NImage, NSpace, NTag } from 'naive-ui'
import type { ApiResponse, LookupItemDto, PagedList } from '~/models/ApiResponse'
import { ResultStatusEnum, isApiSuccess } from '~/models/ApiResponse'
import type {
  CarDto,
  CarQueryParameters,
  CreateCarDto,
  UpdateCarDto,
} from '~/models/Car'
import { useCarService } from '~/services/car.service'
import { useLookupService } from '~/services/lookup.service'

const { t } = useI18n()
const dialog = useDialog()
const message = useMessage()
const carService = useCarService()
const lookupService = useLookupService()
const accountStore = useAccountStore()

// State
const cars = ref<CarDto[]>([])
const loading = ref(false)
const totalCount = ref(0)
const queryParams = reactive<CarQueryParameters>({
  pageIndex: 1,
  pageSize: 10,
  search: '',
  makeId: undefined,
  modelId: undefined,
  branchId: undefined,
  year: undefined,
})

// Lookups for filters and form
const makesList = ref<LookupItemDto[]>([])
const modelsList = ref<LookupItemDto[]>([])
const formModelsList = ref<LookupItemDto[]>([])
const branchesList = ref<LookupItemDto[]>([])
const yearsList = ref<number[]>([])

// Modal & Form State
const showModal = ref(false)
const isEditing = ref(false)
const submitting = ref(false)
const formRef = ref<FormInst | null>(null)
const editingId = ref<string | null>(null)

const formData = reactive<{
  makeId: string | null
  modelId: string | null
  year: number | null
  color: string
  plateNumber: string
  branchId: string | null
  purchaseDate: number | null // Naive UI date picker uses timestamp
  purchasePrice: number | null
  imagePath: string | null
  chassisNumber: string
  engineNumber: string
  notes: string | null
}>({
  makeId: null,
  modelId: null,
  year: new Date().getFullYear(),
  color: '',
  plateNumber: '',
  branchId: null,
  purchaseDate: Date.now(),
  purchasePrice: null,
  imagePath: null,
  chassisNumber: '',
  engineNumber: '',
  notes: null,
})

const uploadingImage = ref(false)

// Form Validation Rules
const rules: FormRules = {
  makeId: [{ required: true, message: 'الماركة مطلوبة', trigger: ['blur', 'change'] }],
  modelId: [{ required: true, message: 'الموديل مطلوب', trigger: ['blur', 'change'] }],
  year: [{ required: true, type: 'number', message: 'سنة الصنع مطلوبة', trigger: ['blur', 'change'] }],
  color: [{ required: true, message: 'اللون مطلوب', trigger: ['blur', 'change'] }],
  plateNumber: [{ required: true, message: 'رقم اللوحة مطلوب', trigger: ['blur', 'change'] }],
  branchId: [{ required: true, message: 'الفرع مطلوب', trigger: ['blur', 'change'] }],
  purchaseDate: [{ required: true, type: 'number', message: 'تاريخ الشراء مطلوب', trigger: ['blur', 'change'] }],
  purchasePrice: [{ required: true, type: 'number', message: 'سعر الشراء مطلوب', trigger: ['blur', 'change'] }],
  chassisNumber: [{ required: true, message: 'رقم الشاصي مطلوب', trigger: ['blur', 'change'] }],
  engineNumber: [{ required: true, message: 'رقم المحرك مطلوب', trigger: ['blur', 'change'] }],
}

// Fetch Initial Lookups
async function loadFilterLookups() {
  try {
    const [makesRes, branchesRes, yearsRes] = await Promise.all([
      lookupService.getMakes(),
      lookupService.getBranches(),
      lookupService.getYears(),
    ])
    if (makesRes.data) makesList.value = makesRes.data
    if (branchesRes.data) branchesList.value = branchesRes.data
    if (yearsRes.data) yearsList.value = yearsRes.data
  } catch (err) {
    console.error('Error loading lookups', err)
  }
}

// Fetch Cars Table Data
async function loadCars() {
  loading.value = true
  try {
    const res = await carService.getPaged(queryParams)
    if (isApiSuccess(res) && res.data) {
      cars.value = res.data.items
      totalCount.value = res.data.totalCount
    }
  } catch (err) {
    console.error('Error loading cars', err)
  } finally {
    loading.value = false
  }
}

const debouncedSearch = useDebounceFn(() => {
  queryParams.pageIndex = 1
  loadCars()
}, 300)

// Dependent filter for models
async function onFilterMakeChange(makeId: string | null) {
  queryParams.modelId = undefined
  modelsList.value = []
  if (makeId) {
    const res = await lookupService.getModels(makeId)
    if (res.data) modelsList.value = res.data
  }
  queryParams.pageIndex = 1
  loadCars()
}

// Dependent form for models
async function onFormMakeChange(makeId: string | null) {
  formData.modelId = null
  formModelsList.value = []
  if (makeId) {
    const res = await lookupService.getModels(makeId)
    if (res.data) formModelsList.value = res.data
  }
}

// Image upload handler
async function handleImageFileChange(e: Event) {
  const target = e.target as HTMLInputElement
  if (!target.files || target.files.length === 0) return

  const file = target.files[0]
  uploadingImage.value = true
  try {
    const res = await carService.uploadImage(file)
    if (res.status === ResultStatusEnum.Success && res.data) {
      formData.imagePath = res.data.webPath
      message.success('تم رفع الصورة بنجاح')
    }
  } catch (err: any) {
    message.error(err.response?.data?.message || 'فشل رفع الصورة')
  } finally {
    uploadingImage.value = false
  }
}

// Open Add Modal
function openAddModal() {
  isEditing.value = false
  editingId.value = null
  formModelsList.value = []
  Object.assign(formData, {
    makeId: null,
    modelId: null,
    year: new Date().getFullYear(),
    color: '',
    plateNumber: '',
    branchId: null,
    purchaseDate: Date.now(),
    purchasePrice: null,
    imagePath: null,
    chassisNumber: '',
    engineNumber: '',
    notes: null,
  })
  showModal.value = true
}

// Open Edit Modal
async function openEditModal(car: CarDto) {
  isEditing.value = true
  editingId.value = car.id
  formModelsList.value = []

  // Pre-load models for this make
  if (car.makeId) {
    const res = await lookupService.getModels(car.makeId)
    if (res.data) formModelsList.value = res.data
  }

  Object.assign(formData, {
    makeId: car.makeId,
    modelId: car.modelId,
    year: car.year,
    color: car.color,
    plateNumber: car.plateNumber,
    branchId: car.branchId,
    purchaseDate: car.purchaseDate ? new Date(car.purchaseDate).getTime() : Date.now(),
    purchasePrice: car.purchasePrice,
    imagePath: car.imagePath,
    chassisNumber: car.chassisNumber,
    engineNumber: car.engineNumber,
    notes: car.notes,
  })

  showModal.value = true
}

// Save (Create / Update)
async function handleSave() {
  formRef.value?.validate(async (errors) => {
    if (errors) return

    submitting.value = true
    try {
      const payload: CreateCarDto = {
        makeId: formData.makeId!,
        modelId: formData.modelId!,
        year: Number(formData.year),
        color: formData.color,
        plateNumber: formData.plateNumber,
        branchId: formData.branchId!,
        purchaseDate: new Date(formData.purchaseDate!).toISOString(),
        purchasePrice: Number(formData.purchasePrice),
        imagePath: formData.imagePath,
        chassisNumber: formData.chassisNumber,
        engineNumber: formData.engineNumber,
        notes: formData.notes,
      }

      if (isEditing.value && editingId.value) {
        const res = await carService.update(editingId.value, {
          id: editingId.value,
          ...payload,
        })
        if (isApiSuccess(res)) {
          message.success(t('cars.updateSuccess'))
          showModal.value = false
          loadCars()
        }
      } else {
        const res = await carService.create(payload)
        if (isApiSuccess(res)) {
          message.success(t('cars.createSuccess'))
          showModal.value = false
          loadCars()
        }
      }
    } catch (err: any) {
      console.error('Error saving car', err)
    } finally {
      submitting.value = false
    }
  })
}

// Delete Confirmation
function handleDelete(car: CarDto) {
  dialog.warning({
    title: t('common.delete'),
    content: `${t('cars.deleteConfirm')} (${car.makeName} ${car.modelName} - ${car.plateNumber})`,
    positiveText: t('common.delete'),
    negativeText: t('common.cancel'),
    onPositiveClick: async () => {
      try {
        const res = await carService.remove(car.id)
        if (isApiSuccess(res)) {
          message.success(t('cars.deleteSuccess'))
          loadCars()
        }
      } catch (err) {
        console.error('Error deleting car', err)
      }
    },
  })
}

// Table Columns
const columns: DataTableColumns<CarDto> = [
  {
    title: t('cars.imagePath'),
    key: 'imagePath',
    width: 80,
    render(row) {
      if (row.imagePath) {
        return h(NImage, {
          width: 50,
          height: 38,
          src: row.imagePath,
          previewSrc: row.imagePath,
          class: 'rounded object-cover shadow-sm',
        })
      }
      return h(
        'div',
        { class: 'w-12 h-9 bg-slate-100 dark:bg-slate-700 rounded flex items-center justify-center text-slate-400' },
        [h(ImageIcon, { class: 'w-5 h-5' })],
      )
    },
  },
  {
    title: t('cars.plateNumber'),
    key: 'plateNumber',
    width: 120,
    render(row) {
      return h(
        NTag,
        { type: 'info', bordered: false, class: 'font-mono font-bold' },
        { default: () => row.plateNumber },
      )
    },
  },
  {
    title: t('cars.make'),
    key: 'makeName',
    sorter: 'default',
  },
  {
    title: t('cars.model'),
    key: 'modelName',
  },
  {
    title: t('cars.year'),
    key: 'year',
    width: 90,
  },
  {
    title: t('cars.color'),
    key: 'color',
    width: 100,
  },
  {
    title: t('cars.branch'),
    key: 'branchName',
  },
  {
    title: t('cars.purchasePrice'),
    key: 'purchasePrice',
    render(row) {
      return `${row.purchasePrice.toLocaleString()} ${t('common.currency')}`
    },
  },
  {
    title: t('cars.chassisNumber'),
    key: 'chassisNumber',
    ellipsis: { tooltip: true },
  },
  {
    title: t('common.actions'),
    key: 'actions',
    width: 140,
    render(row) {
      const actions = []
      if (accountStore.hasPermission('Cars.Update')) {
        actions.push(
          h(
            NButton,
            {
              size: 'small',
              type: 'primary',
              secondary: true,
              onClick: () => openEditModal(row),
            },
            {
              icon: () => h(EditIcon),
              default: () => t('common.edit'),
            },
          ),
        )
      }
      if (accountStore.hasPermission('Cars.Delete')) {
        actions.push(
          h(
            NButton,
            {
              size: 'small',
              type: 'error',
              secondary: true,
              onClick: () => handleDelete(row),
            },
            {
              icon: () => h(DeleteIcon),
            },
          ),
        )
      }
      return h(NSpace, { size: 'small' }, { default: () => actions })
    },
  },
]

// Pagination handler
function onPageChange(page: number) {
  queryParams.pageIndex = page
  loadCars()
}

function onPageSizeChange(pageSize: number) {
  queryParams.pageSize = pageSize
  queryParams.pageIndex = 1
  loadCars()
}

onMounted(() => {
  loadFilterLookups()
  loadCars()
})
</script>

<route lang="yaml">
meta:
  title: cars
  layout: default
</route>

<template>
  <div class="p-6">
    <!-- Header -->
    <div class="flex flex-col md:flex-row md:items-center justify-between pb-6 gap-4">
      <div>
        <h1 class="text-2xl font-bold text-slate-800 dark:text-white flex items-center gap-2">
          <span>🚗</span> {{ t('cars.title') }}
        </h1>
        <p class="text-sm text-slate-500 mt-1">
          إدارة سجلات السيارات، الفروع، وبيانات الشراء والمواصفات
        </p>
      </div>

      <div class="flex items-center gap-2">
        <n-button secondary @click="loadCars">
          <template #icon><RefreshIcon class="w-4 h-4" /></template>
          {{ t('common.refresh') }}
        </n-button>
        <n-button v-can="'Cars.Create'" type="primary" @click="openAddModal">
          <template #icon><AddIcon class="w-5 h-5" /></template>
          {{ t('cars.create') }}
        </n-button>
      </div>
    </div>

    <!-- Filters Bar -->
    <div class="bg-white dark:bg-slate-800 rounded-xl p-4 shadow-sm border border-slate-100 dark:border-slate-700 mb-6">
      <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-5 gap-3">
        <!-- Search Input -->
        <n-input
          v-model:value="queryParams.search"
          clearable
          :placeholder="t('cars.searchPlaceholder')"
          @input="debouncedSearch"
        >
          <template #prefix><SearchIcon class="w-4 h-4 text-slate-400" /></template>
        </n-input>

        <!-- Make Filter -->
        <n-select
          v-model:value="queryParams.makeId"
          clearable
          filterable
          :placeholder="t('cars.filterByMake')"
          :options="makesList.map(m => ({ label: m.name, value: m.id }))"
          @update:value="onFilterMakeChange"
        />

        <!-- Model Filter (Dependent) -->
        <n-select
          v-model:value="queryParams.modelId"
          clearable
          filterable
          :disabled="!queryParams.makeId"
          :placeholder="t('cars.model')"
          :options="modelsList.map(m => ({ label: m.name, value: m.id }))"
          @update:value="() => { queryParams.pageIndex = 1; loadCars(); }"
        />

        <!-- Branch Filter -->
        <n-select
          v-model:value="queryParams.branchId"
          clearable
          filterable
          :placeholder="t('cars.filterByBranch')"
          :options="branchesList.map(b => ({ label: b.name, value: b.id }))"
          @update:value="() => { queryParams.pageIndex = 1; loadCars(); }"
        />

        <!-- Year Filter -->
        <n-select
          v-model:value="queryParams.year"
          clearable
          :placeholder="t('cars.filterByYear')"
          :options="yearsList.map(y => ({ label: String(y), value: y }))"
          @update:value="() => { queryParams.pageIndex = 1; loadCars(); }"
        />
      </div>
    </div>

    <!-- Cars Data Table -->
    <div class="bg-white dark:bg-slate-800 rounded-xl shadow-sm border border-slate-100 dark:border-slate-700 overflow-hidden">
      <n-data-table
        remote
        :columns="columns"
        :data="cars"
        :loading="loading"
        :pagination="{
          page: queryParams.pageIndex,
          pageSize: queryParams.pageSize,
          itemCount: totalCount,
          showSizePicker: true,
          pageSizes: [10, 20, 50],
          prefix: (info) => `إجمالي: ${info.itemCount} سيارة`,
          onUpdatePage: onPageChange,
          onUpdatePageSize: onPageSizeChange,
        }"
        :bordered="false"
        class="min-h-[300px]"
      />
    </div>

    <!-- Add/Edit Modal (12 Fields) -->
    <n-modal
      v-model:show="showModal"
      preset="card"
      :title="isEditing ? t('cars.edit') : t('cars.create')"
      class="max-w-3xl"
      :segmented="{ content: true, action: true }"
    >
      <n-form
        ref="formRef"
        :model="formData"
        :rules="rules"
        label-placement="top"
        require-mark-placement="right-hanging"
      >
        <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
          <!-- 1. Make -->
          <n-form-item path="makeId" :label="t('cars.make')">
            <n-select
              v-model:value="formData.makeId"
              filterable
              :placeholder="t('common.select')"
              :options="makesList.map(m => ({ label: m.name, value: m.id }))"
              @update:value="onFormMakeChange"
            />
          </n-form-item>

          <!-- 2. Model (Dependent) -->
          <n-form-item path="modelId" :label="t('cars.model')">
            <n-select
              v-model:value="formData.modelId"
              filterable
              :disabled="!formData.makeId"
              :placeholder="formData.makeId ? t('common.select') : 'اختر الماركة أولاً'"
              :options="formModelsList.map(m => ({ label: m.name, value: m.id }))"
            />
          </n-form-item>

          <!-- 3. Year -->
          <n-form-item path="year" :label="t('cars.year')">
            <n-select
              v-model:value="formData.year"
              :placeholder="t('cars.year')"
              :options="yearsList.map(y => ({ label: String(y), value: y }))"
            />
          </n-form-item>

          <!-- 4. Color -->
          <n-form-item path="color" :label="t('cars.color')">
            <n-input v-model:value="formData.color" placeholder="مثال: أبيض لؤلؤي، أسود ملكي" />
          </n-form-item>

          <!-- 5. Plate Number -->
          <n-form-item path="plateNumber" :label="t('cars.plateNumber')">
            <n-input v-model:value="formData.plateNumber" placeholder="مثال: أ ب ج 1234" />
          </n-form-item>

          <!-- 6. Branch -->
          <n-form-item path="branchId" :label="t('cars.branch')">
            <n-select
              v-model:value="formData.branchId"
              filterable
              :placeholder="t('common.select')"
              :options="branchesList.map(b => ({ label: b.name, value: b.id }))"
            />
          </n-form-item>

          <!-- 7. Purchase Date -->
          <n-form-item path="purchaseDate" :label="t('cars.purchaseDate')">
            <n-date-picker
              v-model:value="formData.purchaseDate"
              type="date"
              class="w-full"
            />
          </n-form-item>

          <!-- 8. Purchase Price -->
          <n-form-item path="purchasePrice" :label="t('cars.purchasePrice')">
            <n-input-number
              v-model:value="formData.purchasePrice"
              :min="0"
              class="w-full"
              placeholder="0.00"
            >
              <template #suffix>{{ t('common.currency') }}</template>
            </n-input-number>
          </n-form-item>

          <!-- 9. Chassis Number -->
          <n-form-item path="chassisNumber" :label="t('cars.chassisNumber')">
            <n-input v-model:value="formData.chassisNumber" placeholder="رقم الهيكل VIN" />
          </n-form-item>

          <!-- 10. Engine Number -->
          <n-form-item path="engineNumber" :label="t('cars.engineNumber')">
            <n-input v-model:value="formData.engineNumber" placeholder="رقم المحرك" />
          </n-form-item>

          <!-- 11. Image Upload -->
          <n-form-item :label="t('cars.imagePath')" class="md:col-span-2">
            <div class="flex items-center gap-4 w-full">
              <div v-if="formData.imagePath" class="relative group">
                <img
                  :src="formData.imagePath"
                  alt="Car preview"
                  class="w-24 h-16 object-cover rounded-lg border border-slate-200 dark:border-slate-700"
                />
                <button
                  type="button"
                  class="absolute -top-2 -right-2 bg-red-500 text-white rounded-full w-5 h-5 flex items-center justify-center text-xs"
                  @click="formData.imagePath = null"
                >
                  ✕
                </button>
              </div>

              <div class="flex-1">
                <input
                  id="car-image-upload"
                  type="file"
                  accept="image/jpeg,image/png,image/webp"
                  class="hidden"
                  @change="handleImageFileChange"
                />
                <label
                  for="car-image-upload"
                  class="cursor-pointer inline-flex items-center gap-2 px-4 py-2 bg-slate-100 hover:bg-slate-200 dark:bg-slate-700 dark:hover:bg-slate-600 rounded-lg text-sm font-medium transition"
                >
                  <span>📷</span>
                  <span v-if="uploadingImage">جاري الرفع...</span>
                  <span v-else>{{ formData.imagePath ? 'تغيير الصورة' : t('cars.uploadImage') }}</span>
                </label>
                <p class="text-xs text-slate-400 mt-1">يدعم JPG, PNG, WEBP حتى 5MB</p>
              </div>
            </div>
          </n-form-item>

          <!-- 12. Notes -->
          <n-form-item path="notes" :label="t('cars.notes')" class="md:col-span-3">
            <n-input
              v-model:value="formData.notes"
              type="textarea"
              placeholder="أي ملاحظات فنية أو إضافات..."
              :rows="3"
            />
          </n-form-item>
        </div>
      </n-form>

      <template #action>
        <div class="flex justify-end gap-2">
          <n-button @click="showModal = false">
            {{ t('common.cancel') }}
          </n-button>
          <n-button type="primary" :loading="submitting" @click="handleSave">
            {{ isEditing ? t('common.update') : t('common.save') }}
          </n-button>
        </div>
      </template>
    </n-modal>
  </div>
</template>
