<script setup lang="ts">
import {
  Add24Regular as AddIcon,
  Delete24Regular as DeleteIcon,
  Edit24Regular as EditIcon,
  ArrowClockwise24Regular as RefreshIcon,
  Search24Regular as SearchIcon,
} from '@vicons/fluent'
import { useDebounceFn } from '@vueuse/core'
import type { DataTableColumns, FormInst, FormRules } from 'naive-ui'
import { NButton, NSpace } from 'naive-ui'
import { ResultStatusEnum, isApiSuccess } from '~/models/ApiResponse'
import type {
  BranchDto,
  CreateBranchDto,
  UpdateBranchDto,
} from '~/models/Branch'
import { useBranchService } from '~/services/branch.service'

const { t } = useI18n()
const dialog = useDialog()
const message = useMessage()
const branchService = useBranchService()
const accountStore = useAccountStore()

// State
const branches = ref<BranchDto[]>([])
const loading = ref(false)
const totalCount = ref(0)
const queryParams = reactive({
  pageIndex: 1,
  pageSize: 10,
  search: '',
})

// Modal & Form State
const showModal = ref(false)
const isEditing = ref(false)
const submitting = ref(false)
const formRef = ref<FormInst | null>(null)
const editingId = ref<string | null>(null)

const formData = reactive<CreateBranchDto>({
  name: '',
  address: '',
  mobile: '',
})

// Validation Rules
const rules: FormRules = {
  name: [
    { required: true, message: t('branches.validations.nameRequired'), trigger: ['blur', 'change'] },
  ],
  address: [
    { required: true, message: t('branches.validations.addressRequired'), trigger: ['blur', 'change'] },
  ],
  mobile: [
    { required: true, message: t('branches.validations.mobileRequired'), trigger: ['blur', 'change'] },
  ],
}

// Fetch Branches
async function loadBranches() {
  loading.value = true
  try {
    const res = await branchService.getPaged(queryParams)
    if (isApiSuccess(res) && res.data) {
      branches.value = res.data.items
      totalCount.value = res.data.totalCount
    }
  } catch (err) {
    console.error('Error loading branches', err)
  } finally {
    loading.value = false
  }
}

const debouncedSearch = useDebounceFn(() => {
  queryParams.pageIndex = 1
  loadBranches()
}, 300)

// Open Add Modal
function openAddModal() {
  isEditing.value = false
  editingId.value = null
  Object.assign(formData, {
    name: '',
    address: '',
    mobile: '',
  })
  showModal.value = true
}

// Open Edit Modal
function openEditModal(branch: BranchDto) {
  isEditing.value = true
  editingId.value = branch.id
  Object.assign(formData, {
    name: branch.name,
    address: branch.address,
    mobile: branch.mobile,
  })
  showModal.value = true
}

// Save (Create / Update)
async function handleSave() {
  formRef.value?.validate(async (errors) => {
    if (errors) return

    submitting.value = true
    try {
      if (isEditing.value && editingId.value) {
        const payload: UpdateBranchDto = {
          id: editingId.value,
          ...formData,
        }
        const res = await branchService.update(editingId.value, payload)
        if (isApiSuccess(res)) {
          message.success(t('branches.updateSuccess'))
          showModal.value = false
          loadBranches()
        }
      } else {
        const res = await branchService.create(formData)
        if (isApiSuccess(res)) {
          message.success(t('branches.createSuccess'))
          showModal.value = false
          loadBranches()
        }
      }
    } catch (err) {
      console.error('Error saving branch', err)
    } finally {
      submitting.value = false
    }
  })
}

// Delete Confirmation
function handleDelete(branch: BranchDto) {
  dialog.warning({
    title: t('common.delete'),
    content: `${t('branches.deleteConfirm')} (${branch.name})`,
    positiveText: t('common.delete'),
    negativeText: t('common.cancel'),
    onPositiveClick: async () => {
      try {
        const res = await branchService.remove(branch.id)
        if (isApiSuccess(res)) {
          message.success(t('branches.deleteSuccess'))
          loadBranches()
        }
      } catch (err) {
        console.error('Error deleting branch', err)
      }
    },
  })
}

// Table Columns
const columns: DataTableColumns<BranchDto> = [
  {
    title: t('branches.name'),
    key: 'name',
    render(row) {
      return h('div', { class: 'font-semibold text-slate-800 dark:text-white' }, row.name)
    },
  },
  {
    title: t('branches.address'),
    key: 'address',
  },
  {
    title: t('branches.mobile'),
    key: 'mobile',
    render(row) {
      return h('div', { dir: 'ltr', class: 'font-mono text-left' }, row.mobile)
    },
  },
  {
    title: t('common.actions'),
    key: 'actions',
    width: 140,
    render(row) {
      const actions = []
      if (accountStore.hasPermission('Branches.Update')) {
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
      if (accountStore.hasPermission('Branches.Delete')) {
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
  loadBranches()
}

function onPageSizeChange(pageSize: number) {
  queryParams.pageSize = pageSize
  queryParams.pageIndex = 1
  loadBranches()
}

onMounted(() => {
  loadBranches()
})
</script>

<route lang="yaml">
meta:
  title: branches
  layout: default
</route>

<template>
  <div class="p-6">
    <!-- Header -->
    <div class="flex flex-col md:flex-row md:items-center justify-between pb-6 gap-4">
      <div>
        <h1 class="text-2xl font-bold text-slate-800 dark:text-white flex items-center gap-2">
          <span>🏢</span> {{ t('branches.title') }}
        </h1>
        <p class="text-sm text-slate-500 mt-1">
          إدارة فروع المعرض، العناوين، وبيانات التواصل
        </p>
      </div>

      <div class="flex items-center gap-2">
        <n-button secondary @click="loadBranches">
          <template #icon><RefreshIcon class="w-4 h-4" /></template>
          {{ t('common.refresh') }}
        </n-button>
        <n-button v-can="'Branches.Create'" type="primary" @click="openAddModal">
          <template #icon><AddIcon class="w-5 h-5" /></template>
          {{ t('branches.create') }}
        </n-button>
      </div>
    </div>

    <!-- Search Bar -->
    <div class="bg-white dark:bg-slate-800 rounded-xl p-4 shadow-sm border border-slate-100 dark:border-slate-700 mb-6">
      <div class="max-w-md">
        <n-input
          v-model:value="queryParams.search"
          clearable
          :placeholder="t('branches.searchPlaceholder')"
          @input="debouncedSearch"
        >
          <template #prefix><SearchIcon class="w-4 h-4 text-slate-400" /></template>
        </n-input>
      </div>
    </div>

    <!-- Data Table -->
    <div class="bg-white dark:bg-slate-800 rounded-xl shadow-sm border border-slate-100 dark:border-slate-700 overflow-hidden">
      <n-data-table
        remote
        :columns="columns"
        :data="branches"
        :loading="loading"
        :scroll-x="650"
        :row-key="(row: BranchDto) => row.id"
        :pagination="{
          page: queryParams.pageIndex,
          pageSize: queryParams.pageSize,
          itemCount: totalCount,
          showSizePicker: true,
          pageSizes: [10, 20, 50],
          prefix: (info) => `إجمالي: ${info.itemCount} فرع`,
          onUpdatePage: onPageChange,
          onUpdatePageSize: onPageSizeChange,
        }"
        :bordered="false"
        class="min-h-[300px]"
      />
    </div>

    <!-- Add/Edit Modal (3 Fields) -->
    <n-modal
      v-model:show="showModal"
      preset="card"
      :title="isEditing ? t('branches.edit') : t('branches.create')"
      class="max-w-lg"
      :segmented="{ content: true, action: true }"
    >
      <n-form
        ref="formRef"
        :model="formData"
        :rules="rules"
        label-placement="top"
        require-mark-placement="right-hanging"
      >
        <!-- 1. Name -->
        <n-form-item path="name" :label="t('branches.name')">
          <n-input v-model:value="formData.name" placeholder="اسم الفرع" autofocus />
        </n-form-item>

        <!-- 2. Address -->
        <n-form-item path="address" :label="t('branches.address')">
          <n-input v-model:value="formData.address" type="textarea" placeholder="عنوان وموقع الفرع" :rows="2" />
        </n-form-item>

        <!-- 3. Mobile -->
        <n-form-item path="mobile" :label="t('branches.mobile')">
          <n-input v-model:value="formData.mobile" placeholder="رقم هاتف / جوال الفرع" />
        </n-form-item>
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
