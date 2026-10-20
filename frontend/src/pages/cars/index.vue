<script setup lang="ts">
import {
  Add24Regular as AddIcon,
  Delete24Regular as DeleteIcon,
  Edit24Regular as EditIcon,
  Image24Regular as ImageIcon,
  ArrowClockwise24Regular as RefreshIcon,
  Search24Regular as SearchIcon,
} from '@vicons/fluent'
import { useDebounceFn } from '@vueuse/core'
import type { DataTableColumns, FormInst, FormRules } from 'naive-ui'
import { NButton, NImage, NSpace, NTag } from 'naive-ui'
import type { LookupItemDto } from '~/models/ApiResponse'
import { isApiSuccess } from '~/models/ApiResponse'
import type { CarDto, CarQueryParameters, SaveCarDto } from '~/models/Car'
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
  makeId: null,
  modelId: null,
  branchId: null,
  manufactureYear: null,
})

// Lookups for filters and form
const makesList = ref<LookupItemDto[]>([])
const filterModelsList = ref<LookupItemDto[]>([])
const formModelsList = ref<LookupItemDto[]>([])
const branchesList = ref<LookupItemDto[]>([])
const yearsList = ref<number[]>([])

// Permissions
const canCreate = computed(() => accountStore.hasPermission('Cars.Create'))
const canUpdate = computed(() => accountStore.hasPermission('Cars.Update'))
const canDelete = computed(() => accountStore.hasPermission('Cars.Delete'))

// Modal & Form State
const showModal = ref(false)
const isEditing = ref(false)
const submitting = ref(false)
const formRef = ref<FormInst | null>(null)
const editingId = ref<number | null>(null)

const formData = reactive<{
  makeId: number | null
  modelId: number | null
  manufactureYear: number | null
  color: string
  plateNumber: string
  branchId: number | null
  purchaseDate: number | null
  purchasePrice: number | null
  imagePath: string | null
  chassisNumber: string
  engineNumber: string
  notes: string | null
}>({
  makeId: null,
  modelId: null,
  manufactureYear: new Date().getFullYear(),
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
  makeId: [{ required: true, type: 'number', message: 'الشركة المصنعة مطلوبة', trigger: ['blur', 'change'] }],
  modelId: [{ required: true, type: 'number', message: 'الموديل مطلوب', trigger: ['blur', 'change'] }],
  manufactureYear: [{ required: true, type: 'number', message: 'سنة الصنع مطلوبة', trigger: ['blur', 'change'] }],
  color: [{ required: true, message: 'اللون مطلوب', trigger: ['blur', 'input'] }],
  plateNumber: [{ required: true, message: 'رقم اللوحة مطلوب', trigger: ['blur', 'input'] }],
  branchId: [{ required: true, type: 'number', message: 'الفرع مطلوب', trigger: ['blur', 'change'] }],
  purchaseDate: [{ required: true, type: 'number', message: 'تاريخ الشراء مطلوب', trigger: ['blur', 'change'] }],
  purchasePrice: [{ required: true, type: 'number', message: 'سعر الشراء مطلوب', trigger: ['blur', 'input'] }],
  chassisNumber: [{ required: true, message: 'رقم الشاسيه / الهيكل مطلوب', trigger: ['blur', 'input'] }],
  engineNumber: [{ required: true, message: 'رقم المحرك مطلوب', trigger: ['blur', 'input'] }],
}

// Fetch Initial Lookups
async function loadFilterLookups() {
  try {
    const [makesRes, branchesRes, yearsRes] = await Promise.all([
      lookupService.getMakes(),
      lookupService.getBranches(),
      lookupService.getYears(),
    ])
    if (isApiSuccess(makesRes) && makesRes.data) makesList.value = makesRes.data
    if (isApiSuccess(branchesRes) && branchesRes.data) branchesList.value = branchesRes.data
    if (isApiSuccess(yearsRes) && yearsRes.data) yearsList.value = yearsRes.data
  }
  catch (err) {
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
  }
  catch (err) {
    console.error('Error loading cars', err)
  }
  finally {
    loading.value = false
  }
}

const debouncedSearch = useDebounceFn(() => {
  queryParams.pageIndex = 1
  loadCars()
}, 300)

// Dependent filter for models
async function onFilterMakeChange(makeId: number | null) {
  queryParams.modelId = null
  filterModelsList.value = []
  if (makeId) {
    const res = await lookupService.getModels(makeId)
    if (isApiSuccess(res) && res.data) filterModelsList.value = res.data
  }
  queryParams.pageIndex = 1
  loadCars()
}

// Dependent form for models
async function onFormMakeChange(makeId: number | null) {
  formData.modelId = null
  formModelsList.value = []
  if (makeId) {
    const res = await lookupService.getModels(makeId)
    if (isApiSuccess(res) && res.data) formModelsList.value = res.data
  }
}

// Image upload handler
async function handleImageFileChange(e: Event) {
  const target = e.target as HTMLInputElement
  if (!target.files || target.files.length === 0) return

  const file = target.files[0]
  uploadingImage.value = true
  try {
    const res = await carService.uploadImage(file, editingId.value)
    if (isApiSuccess(res) && res.data) {
      formData.imagePath = res.data
      message.success('تم رفع الصورة بنجاح')
    }
  }
  catch (err: any) {
    message.error(err.response?.data?.message || 'فشل رفع الصورة')
  }
  finally {
    uploadingImage.value = false
    target.value = ''
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
    manufactureYear: new Date().getFullYear(),
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
    if (isApiSuccess(res) && res.data) formModelsList.value = res.data
  }

  Object.assign(formData, {
    makeId: car.makeId,
    modelId: car.modelId,
    manufactureYear: car.manufactureYear,
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
      const payload: SaveCarDto = {
        makeId: formData.makeId!,
        modelId: formData.modelId!,
        manufactureYear: Number(formData.manufactureYear),
        color: formData.color.trim(),
        plateNumber: formData.plateNumber.trim(),
        branchId: formData.branchId!,
        purchaseDate: new Date(formData.purchaseDate!).toISOString(),
        purchasePrice: Number(formData.purchasePrice),
        chassisNumber: formData.chassisNumber.trim(),
        engineNumber: formData.engineNumber.trim(),
        notes: formData.notes?.trim() || null,
        imagePath: formData.imagePath,
      }

      if (isEditing.value && editingId.value) {
        const res = await carService.update(editingId.value, payload)
        if (isApiSuccess(res)) {
          message.success(t('cars.updateSuccess'))
          showModal.value = false
          loadCars()
        }
      }
      else {
        const res = await carService.create(payload)
        if (isApiSuccess(res)) {
          message.success(t('cars.createSuccess'))
          showModal.value = false
          loadCars()
        }
      }
    }
    catch (err: any) {
      console.error('Error saving car', err)
    }
    finally {
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
      }
      catch (err) {
        console.error('Error deleting car', err)
      }
    },
  })
}

// Table Columns with full responsive widths
const columns: DataTableColumns<CarDto> = [
  {
    title: t('cars.imagePath'),
    key: 'imagePath',
    width: 75,
    render(row) {
      if (row.imagePath) {
        return h(NImage, {
          width: 48,
          height: 36,
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
    minWidth: 120,
  },
  {
    title: t('cars.model'),
    key: 'modelName',
    minWidth: 120,
  },
  {
    title: t('cars.year'),
    key: 'manufactureYear',
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
    minWidth: 120,
  },
  {
    title: t('cars.purchasePrice'),
    key: 'purchasePrice',
    minWidth: 130,
    render(row) {
      return `${row.purchasePrice.toLocaleString('en-US')} ${t('common.currency')}`
    },
  },
  {
    title: t('cars.chassisNumber'),
    key: 'chassisNumber',
    minWidth: 150,
    ellipsis: { tooltip: true },
  },
  {
    title: t('common.actions'),
    key: 'actions',
    width: 130,
    render(row) {
      const actions = []
      if (canUpdate.value) {
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
      if (canDelete.value) {
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
  <div class="p-4 md:p-6">
    <!-- Header -->
    <div class="flex flex-col md:flex-row md:items-center justify-between pb-6 gap-4">
      <div>
        <h1 class="text-2xl font-bold flex items-center gap-2">
          <span>🚗</span> {{ t('cars.title') }}
        </h1>
        <p class="text-sm text-slate-500 mt-1">
          إدارة سجلات السيارات، الفروع، وبيانات الشراء والمواصفات
        </p>
      </div>

      <div class="flex items-center gap-2">
        <n-button id="cars-refresh-btn" secondary @click="loadCars">
          <template #icon>
            <RefreshIcon class="w-4 h-4" />
          </template>
          {{ t('common.refresh') }}
        </n-button>
        <n-button v-if="canCreate" id="cars-create-btn" type="primary" @click="openAddModal">
          <template #icon>
            <AddIcon class="w-5 h-5" />
          </template>
          {{ t('cars.create') }}
        </n-button>
      </div>
    </div>

    <!-- Filters Bar -->
    <div class="bg-white dark:bg-slate-800 rounded-xl p-4 shadow-sm border border-slate-100 dark:border-slate-700 mb-6">
      <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-5 gap-3">
        <!-- Search Input -->
        <n-input
          id="cars-search-input"
          v-model:value="queryParams.search"
          clearable
          :placeholder="t('cars.searchPlaceholder')"
          @update:value="debouncedSearch"
        >
          <template #prefix>
            <SearchIcon class="w-4 h-4 text-slate-400" />
          </template>
        </n-input>

        <!-- Make Filter -->
        <LookupSelect
          id="cars-filter-make"
          v-model:value="queryParams.makeId"
          lookup-type="makes"
          :placeholder="t('cars.filterByMake')"
          @update:value="onFilterMakeChange"
        />

        <!-- Model Filter (Dependent) -->
        <LookupSelect
          id="cars-filter-model"
          v-model:value="queryParams.modelId"
          lookup-type="models"
          :make-id="queryParams.makeId"
          :disabled="!queryParams.makeId"
          :placeholder="t('cars.model')"
          @update:value="() => { queryParams.pageIndex = 1; loadCars(); }"
        />

        <!-- Branch Filter -->
        <LookupSelect
          id="cars-filter-branch"
          v-model:value="queryParams.branchId"
          lookup-type="branches"
          :placeholder="t('cars.filterByBranch')"
          @update:value="() => { queryParams.pageIndex = 1; loadCars(); }"
        />

        <!-- Year Filter -->
        <LookupSelect
          id="cars-filter-year"
          v-model:value="queryParams.manufactureYear"
          lookup-type="years"
          :placeholder="t('cars.filterByYear')"
          @update:value="() => { queryParams.pageIndex = 1; loadCars(); }"
        />
      </div>
    </div>

    <!-- Cars Data Table (Fully Responsive with scroll-x) -->
    <div class="bg-white dark:bg-slate-800 rounded-xl shadow-sm border border-slate-100 dark:border-slate-700 overflow-hidden">
      <n-data-table
        remote
        :columns="columns"
        :data="cars"
        :loading="loading"
        :row-key="(row: CarDto) => row.id"
        :scroll-x="1150"
        :pagination="{
          page: queryParams.pageIndex,
          pageSize: queryParams.pageSize,
          itemCount: totalCount,
          showSizePicker: true,
          pageSizes: [10, 20, 50],
          prefix: ({ itemCount }) => `إجمالي: ${itemCount} سيارة`,
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
      style="width: min(840px, 95vw)"
      :segmented="{ content: true, action: true }"
    >
      <n-form
        ref="formRef"
        :model="formData"
        :rules="rules"
        label-placement="top"
        require-mark-placement="right-hanging"
      >
        <div class="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 gap-4">
          <!-- 1. Make -->
          <n-form-item path="makeId" :label="t('cars.make')">
            <LookupSelect
              id="car-form-make"
              v-model:value="formData.makeId"
              lookup-type="makes"
              :placeholder="t('common.select')"
              @update:value="onFormMakeChange"
            />
          </n-form-item>

          <!-- 2. Model (Dependent) -->
          <n-form-item path="modelId" :label="t('cars.model')">
            <LookupSelect
              id="car-form-model"
              v-model:value="formData.modelId"
              lookup-type="models"
              :make-id="formData.makeId"
              :disabled="!formData.makeId"
              :placeholder="formData.makeId ? t('common.select') : 'اختر الماركة أولاً'"
            />
          </n-form-item>

          <!-- 3. Year -->
          <n-form-item path="manufactureYear" :label="t('cars.year')">
            <LookupSelect
              id="car-form-year"
              v-model:value="formData.manufactureYear"
              lookup-type="years"
              :placeholder="t('cars.year')"
            />
          </n-form-item>

          <!-- 4. Color -->
          <n-form-item path="color" :label="t('cars.color')">
            <n-input id="car-form-color" v-model:value="formData.color" placeholder="مثال: أبيض لؤلؤي، أسود ملكي" />
          </n-form-item>

          <!-- 5. Plate Number -->
          <n-form-item path="plateNumber" :label="t('cars.plateNumber')">
            <n-input id="car-form-plate" v-model:value="formData.plateNumber" placeholder="مثال: أ ب ج 1234" />
          </n-form-item>

          <!-- 6. Branch -->
          <n-form-item path="branchId" :label="t('cars.branch')">
            <LookupSelect
              id="car-form-branch"
              v-model:value="formData.branchId"
              lookup-type="branches"
              :placeholder="t('common.select')"
            />
          </n-form-item>

          <!-- 7. Purchase Date -->
          <n-form-item path="purchaseDate" :label="t('cars.purchaseDate')">
            <n-date-picker
              id="car-form-purchasedate"
              v-model:value="formData.purchaseDate"
              type="date"
              class="w-full"
            />
          </n-form-item>

          <!-- 8. Purchase Price -->
          <n-form-item path="purchasePrice" :label="t('cars.purchasePrice')">
            <n-input-number
              id="car-form-price"
              v-model:value="formData.purchasePrice"
              :min="0"
              class="w-full"
              placeholder="0.00"
            >
              <template #suffix>
                {{ t('common.currency') }}
              </template>
            </n-input-number>
          </n-form-item>

          <!-- 9. Chassis Number -->
          <n-form-item path="chassisNumber" :label="t('cars.chassisNumber')">
            <n-input id="car-form-chassis" v-model:value="formData.chassisNumber" placeholder="رقم الهيكل VIN" />
          </n-form-item>

          <!-- 10. Engine Number -->
          <n-form-item path="engineNumber" :label="t('cars.engineNumber')">
            <n-input id="car-form-engine" v-model:value="formData.engineNumber" placeholder="رقم المحرك" />
          </n-form-item>

          <!-- 11. Image Upload -->
          <n-form-item :label="t('cars.imagePath')" class="sm:col-span-2">
            <div class="flex items-center gap-4 w-full">
              <div v-if="formData.imagePath" class="relative group">
                <img
                  :src="formData.imagePath"
                  alt="Car preview"
                  class="w-24 h-16 object-cover rounded-lg border border-slate-200 dark:border-slate-700"
                >
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
                >
                <label
                  for="car-image-upload"
                  class="cursor-pointer inline-flex items-center gap-2 px-4 py-2 bg-slate-100 hover:bg-slate-200 dark:bg-slate-700 dark:hover:bg-slate-600 rounded-lg text-sm font-medium transition"
                >
                  <span>📷</span>
                  <span v-if="uploadingImage">جاري الرفع...</span>
                  <span v-else>{{ formData.imagePath ? 'تغيير الصورة' : t('cars.uploadImage') }}</span>
                </label>
                <p class="text-xs text-slate-400 mt-1">
                  يدعم JPG, PNG, WEBP حتى 5MB
                </p>
              </div>
            </div>
          </n-form-item>

          <!-- 12. Notes -->
          <n-form-item path="notes" :label="t('cars.notes')" class="sm:col-span-2 md:col-span-3">
            <n-input
              id="car-form-notes"
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
          <n-button id="car-form-save-btn" type="primary" :loading="submitting" @click="handleSave">
            {{ isEditing ? t('common.update') : t('common.save') }}
          </n-button>
        </div>
      </template>
    </n-modal>
  </div>
</template>
