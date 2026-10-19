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
import { NButton, NSpace, NTag } from 'naive-ui'
import { ResultStatusEnum, isApiSuccess } from '~/models/ApiResponse'
import type {
  CreateCustomerDto,
  CustomerDto,
  UpdateCustomerDto,
} from '~/models/Customer'
import { useCustomerService } from '~/services/customer.service'

const { t } = useI18n()
const dialog = useDialog()
const message = useMessage()
const customerService = useCustomerService()
const accountStore = useAccountStore()

// State
const customers = ref<CustomerDto[]>([])
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

const formData = reactive<CreateCustomerDto>({
  name: '',
  nationalId: '',
  mobile: '',
  address: '',
})

// Form Validation Rules
const rules: FormRules = {
  name: [
    { required: true, message: t('customers.validations.nameRequired'), trigger: ['blur', 'change'] },
  ],
  nationalId: [
    { required: true, message: t('customers.validations.nationalIdRequired'), trigger: ['blur', 'change'] },
  ],
  mobile: [
    { required: true, message: t('customers.validations.mobileRequired'), trigger: ['blur', 'change'] },
  ],
  address: [
    { required: true, message: t('customers.validations.addressRequired'), trigger: ['blur', 'change'] },
  ],
}

// Fetch Customers Table Data
async function loadCustomers() {
  loading.value = true
  try {
    const res = await customerService.getPaged(queryParams)
    if (isApiSuccess(res) && res.data) {
      customers.value = res.data.items
      totalCount.value = res.data.totalCount
    }
  } catch (err) {
    console.error('Error loading customers', err)
  } finally {
    loading.value = false
  }
}

const debouncedSearch = useDebounceFn(() => {
  queryParams.pageIndex = 1
  loadCustomers()
}, 300)

// Open Add Modal
function openAddModal() {
  isEditing.value = false
  editingId.value = null
  Object.assign(formData, {
    name: '',
    nationalId: '',
    mobile: '',
    address: '',
  })
  showModal.value = true
}

// Open Edit Modal
function openEditModal(customer: CustomerDto) {
  isEditing.value = true
  editingId.value = customer.id
  Object.assign(formData, {
    name: customer.name,
    nationalId: customer.nationalId,
    mobile: customer.mobile,
    address: customer.address,
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
        const payload: UpdateCustomerDto = {
          id: editingId.value,
          ...formData,
        }
        const res = await customerService.update(editingId.value, payload)
        if (isApiSuccess(res)) {
          message.success(t('customers.updateSuccess'))
          showModal.value = false
          loadCustomers()
        }
      } else {
        const res = await customerService.create(formData)
        if (isApiSuccess(res)) {
          message.success(t('customers.createSuccess'))
          showModal.value = false
          loadCustomers()
        }
      }
    } catch (err) {
      console.error('Error saving customer', err)
    } finally {
      submitting.value = false
    }
  })
}

// Delete Confirmation
function handleDelete(customer: CustomerDto) {
  dialog.warning({
    title: t('common.delete'),
    content: `${t('customers.deleteConfirm')} (${customer.name})`,
    positiveText: t('common.delete'),
    negativeText: t('common.cancel'),
    onPositiveClick: async () => {
      try {
        const res = await customerService.remove(customer.id)
        if (isApiSuccess(res)) {
          message.success(t('customers.deleteSuccess'))
          loadCustomers()
        }
      } catch (err) {
        console.error('Error deleting customer', err)
      }
    },
  })
}

// Table Columns
const columns: DataTableColumns<CustomerDto> = [
  {
    title: t('customers.name'),
    key: 'name',
    render(row) {
      return h('div', { class: 'font-semibold text-slate-800 dark:text-white' }, row.name)
    },
  },
  {
    title: t('customers.nationalId'),
    key: 'nationalId',
    render(row) {
      return h(
        NTag,
        { type: 'warning', bordered: false, class: 'font-mono' },
        { default: () => row.nationalId },
      )
    },
  },
  {
    title: t('customers.mobile'),
    key: 'mobile',
    render(row) {
      return h('div', { dir: 'ltr', class: 'font-mono text-left' }, row.mobile)
    },
  },
  {
    title: t('customers.address'),
    key: 'address',
  },
  {
    title: t('common.actions'),
    key: 'actions',
    width: 140,
    render(row) {
      const actions = []
      if (accountStore.hasPermission('Customers.Update')) {
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
      if (accountStore.hasPermission('Customers.Delete')) {
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
  loadCustomers()
}

function onPageSizeChange(pageSize: number) {
  queryParams.pageSize = pageSize
  queryParams.pageIndex = 1
  loadCustomers()
}

onMounted(() => {
  loadCustomers()
})
</script>

<route lang="yaml">
meta:
  title: customers
  layout: default
</route>

<template>
  <div class="p-6">
    <!-- Header -->
    <div class="flex flex-col md:flex-row md:items-center justify-between pb-6 gap-4">
      <div>
        <h1 class="text-2xl font-bold text-slate-800 dark:text-white flex items-center gap-2">
          <span>👥</span> {{ t('customers.title') }}
        </h1>
        <p class="text-sm text-slate-500 mt-1">
          إدارة بيانات العملاء، أرقام الهوية الوطنية وبيانات التواصل
        </p>
      </div>

      <div class="flex items-center gap-2">
        <n-button secondary @click="loadCustomers">
          <template #icon><RefreshIcon class="w-4 h-4" /></template>
          {{ t('common.refresh') }}
        </n-button>
        <n-button v-can="'Customers.Create'" type="primary" @click="openAddModal">
          <template #icon><AddIcon class="w-5 h-5" /></template>
          {{ t('customers.create') }}
        </n-button>
      </div>
    </div>

    <!-- Search Bar -->
    <div class="bg-white dark:bg-slate-800 rounded-xl p-4 shadow-sm border border-slate-100 dark:border-slate-700 mb-6">
      <div class="max-w-md">
        <n-input
          v-model:value="queryParams.search"
          clearable
          :placeholder="t('customers.searchPlaceholder')"
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
        :data="customers"
        :loading="loading"
        :scroll-x="750"
        :row-key="(row: CustomerDto) => row.id"
        :pagination="{
          page: queryParams.pageIndex,
          pageSize: queryParams.pageSize,
          itemCount: totalCount,
          showSizePicker: true,
          pageSizes: [10, 20, 50],
          prefix: (info) => `إجمالي: ${info.itemCount} عميل`,
          onUpdatePage: onPageChange,
          onUpdatePageSize: onPageSizeChange,
        }"
        :bordered="false"
        class="min-h-[300px]"
      />
    </div>

    <!-- Add/Edit Modal (4 Fields) -->
    <n-modal
      v-model:show="showModal"
      preset="card"
      :title="isEditing ? t('customers.edit') : t('customers.create')"
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
        <n-form-item path="name" :label="t('customers.name')">
          <n-input v-model:value="formData.name" placeholder="الاسم الكامل للعميل" autofocus />
        </n-form-item>

        <!-- 2. National ID -->
        <n-form-item path="nationalId" :label="t('customers.nationalId')">
          <n-input v-model:value="formData.nationalId" placeholder="رقم الهوية الوطنية أو الإقامة" />
        </n-form-item>

        <!-- 3. Mobile -->
        <n-form-item path="mobile" :label="t('customers.mobile')">
          <n-input v-model:value="formData.mobile" placeholder="رقم الجوال (مثال: +966500000000)" />
        </n-form-item>

        <!-- 4. Address -->
        <n-form-item path="address" :label="t('customers.address')">
          <n-input v-model:value="formData.address" type="textarea" placeholder="العنوان التفصيلي" :rows="2" />
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
