<script setup lang="ts">
import {
  Add24Regular as AddIcon,
  Delete24Regular as DeleteIcon,
  Edit24Regular as EditIcon,
  ArrowClockwise24Regular as RefreshIcon,
  ShieldCheckmark24Regular as ShieldIcon,
  Checkmark24Regular as CheckIcon,
  Dismiss24Regular as DismissIcon,
} from '@vicons/fluent'
import type { DataTableColumns, FormInst, FormRules } from 'naive-ui'
import { NButton, NSpace, NTag, NCheckbox, NCheckboxGroup } from 'naive-ui'
import { ResultStatusEnum, isApiSuccess } from '~/models/ApiResponse'
import type {
  CreateRoleDto,
  PermissionDto,
  RoleDto,
  UpdateRoleDto,
} from '~/models/Role'
import { useRoleService } from '~/services/role.service'

const { t } = useI18n()
const dialog = useDialog()
const message = useMessage()
const roleService = useRoleService()
const accountStore = useAccountStore()

// State
const roles = ref<RoleDto[]>([])
const allPermissions = ref<PermissionDto[]>([])
const loading = ref(false)

// Modal State
const showModal = ref(false)
const isEditing = ref(false)
const submitting = ref(false)
const formRef = ref<FormInst | null>(null)
const editingId = ref<string | null>(null)

const formData = reactive<{
  name: string
  description: string
  permissions: string[]
}>({
  name: '',
  description: '',
  permissions: [],
})

const rules: FormRules = {
  name: [{ required: true, message: 'اسم الدور مطلوب', trigger: ['blur', 'change'] }],
}

// Group permissions by module
const permissionsByModule = computed(() => {
  const groups: Record<string, PermissionDto[]> = {}
  allPermissions.value.forEach((p) => {
    if (!groups[p.module]) {
      groups[p.module] = []
    }
    groups[p.module].push(p)
  })
  return groups
})

async function loadData() {
  loading.value = true
  try {
    const [rolesRes, permsRes] = await Promise.all([
      roleService.getAll(),
      roleService.getPermissions(),
    ])
    if (isApiSuccess(rolesRes) && rolesRes.data) {
      roles.value = rolesRes.data
    }
    if (isApiSuccess(permsRes) && permsRes.data) {
      allPermissions.value = permsRes.data
    }
  } catch (err) {
    console.error('Error loading roles data', err)
  } finally {
    loading.value = false
  }
}

function openAddModal() {
  isEditing.value = false
  editingId.value = null
  formData.name = ''
  formData.description = ''
  formData.permissions = []
  showModal.value = true
}

function openEditModal(role: RoleDto) {
  isEditing.value = true
  editingId.value = role.id
  formData.name = role.name
  formData.description = role.description || ''
  formData.permissions = [...(role.permissions || [])]
  showModal.value = true
}

function selectAllPermissions() {
  formData.permissions = allPermissions.value.map((p) => p.name)
}

function deselectAllPermissions() {
  formData.permissions = []
}

function toggleModulePermissions(moduleName: string) {
  const modulePerms = (permissionsByModule.value[moduleName] || []).map((p) => p.name)
  const allSelected = modulePerms.every((p) => formData.permissions.includes(p))

  if (allSelected) {
    formData.permissions = formData.permissions.filter((p) => !modulePerms.includes(p))
  } else {
    const newSet = new Set([...formData.permissions, ...modulePerms])
    formData.permissions = Array.from(newSet)
  }
}

async function handleSave() {
  formRef.value?.validate(async (errors) => {
    if (errors) return

    submitting.value = true
    try {
      if (isEditing.value && editingId.value) {
        const payload: UpdateRoleDto = {
          id: editingId.value,
          name: formData.name,
          description: formData.description,
          permissions: formData.permissions,
        }
        const res = await roleService.update(editingId.value, payload)
        if (isApiSuccess(res)) {
          message.success(t('roles.updateSuccess'))
          showModal.value = false
          loadData()
        }
      } else {
        const payload: CreateRoleDto = {
          name: formData.name,
          description: formData.description,
          permissions: formData.permissions,
        }
        const res = await roleService.create(payload)
        if (isApiSuccess(res)) {
          message.success(t('roles.createSuccess'))
          showModal.value = false
          loadData()
        }
      }
    } catch (err) {
      console.error('Error saving role', err)
    } finally {
      submitting.value = false
    }
  })
}

function handleDelete(role: RoleDto) {
  if (role.isSystemRole) {
    message.warning(t('roles.cannotDeleteSystemRole'))
    return
  }

  dialog.warning({
    title: t('common.delete'),
    content: `${t('roles.deleteConfirm')} (${role.name})`,
    positiveText: t('common.delete'),
    negativeText: t('common.cancel'),
    onPositiveClick: async () => {
      try {
        const res = await roleService.remove(role.id)
        if (isApiSuccess(res)) {
          message.success(t('roles.deleteSuccess'))
          loadData()
        }
      } catch (err) {
        console.error('Error deleting role', err)
      }
    },
  })
}

const columns: DataTableColumns<RoleDto> = [
  {
    title: t('roles.name'),
    key: 'name',
    render(row) {
      return h('div', { class: 'flex items-center gap-2' }, [
        h(ShieldIcon, { class: 'w-5 h-5 text-primary' }),
        h('span', { class: 'font-semibold text-slate-800 dark:text-white' }, row.name),
        row.isSystemRole
          ? h(NTag, { type: 'warning', size: 'small', bordered: false }, { default: () => 'نظام' })
          : null,
      ])
    },
  },
  {
    title: t('roles.description'),
    key: 'description',
    render(row) {
      return row.description || '-'
    },
  },
  {
    title: t('roles.permissionsCount'),
    key: 'permissions',
    render(row) {
      return h(
        NTag,
        { type: 'info', bordered: false, class: 'font-mono' },
        { default: () => `${row.permissions?.length || 0} صلاحية` },
      )
    },
  },
  {
    title: t('common.actions'),
    key: 'actions',
    width: 140,
    render(row) {
      const actions = []
      if (accountStore.hasPermission('Roles.Update')) {
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
      if (accountStore.hasPermission('Roles.Delete') && !row.isSystemRole) {
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

onMounted(() => {
  loadData()
})
</script>

<route lang="yaml">
meta:
  title: roles
  layout: default
</route>

<template>
  <div class="p-6">
    <!-- Header -->
    <div class="flex flex-col md:flex-row md:items-center justify-between pb-6 gap-4">
      <div>
        <h1 class="text-2xl font-bold text-slate-800 dark:text-white flex items-center gap-2">
          <span>🛡️</span> {{ t('roles.title') }}
        </h1>
        <p class="text-sm text-slate-500 mt-1">
          إدارة الأدوار وتحديد الصلاحيات الممنوحة لكل دور بمرونة كاملة
        </p>
      </div>

      <div class="flex items-center gap-2">
        <n-button secondary @click="loadData">
          <template #icon><RefreshIcon class="w-4 h-4" /></template>
          {{ t('common.refresh') }}
        </n-button>
        <n-button v-can="'Roles.Create'" type="primary" @click="openAddModal">
          <template #icon><AddIcon class="w-5 h-5" /></template>
          {{ t('roles.create') }}
        </n-button>
      </div>
    </div>

    <!-- Data Table -->
    <div class="bg-white dark:bg-slate-800 rounded-xl shadow-sm border border-slate-100 dark:border-slate-700 overflow-hidden">
      <n-data-table
        :columns="columns"
        :data="roles"
        :loading="loading"
        :bordered="false"
        class="min-h-[250px]"
      />
    </div>

    <!-- Add/Edit Role Modal -->
    <n-modal
      v-model:show="showModal"
      preset="card"
      :title="isEditing ? t('roles.edit') : t('roles.create')"
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
        <div class="grid grid-cols-1 md:grid-cols-2 gap-4 mb-4">
          <n-form-item path="name" :label="t('roles.name')">
            <n-input v-model:value="formData.name" placeholder="مثال: مدير المبيعات" autofocus />
          </n-form-item>

          <n-form-item path="description" :label="t('roles.description')">
            <n-input v-model:value="formData.description" placeholder="وصف مسؤوليات الدور" />
          </n-form-item>
        </div>

        <!-- Permissions Section -->
        <div class="mt-4 pt-4 border-t border-slate-100 dark:border-slate-700">
          <div class="flex items-center justify-between mb-4">
            <div>
              <h3 class="font-bold text-slate-800 dark:text-white flex items-center gap-2">
                <span>🔑</span> {{ t('roles.permissions') }}
                <n-tag size="small" type="primary" round>
                  {{ formData.permissions.length }} مختارة
                </n-tag>
              </h3>
              <p class="text-xs text-slate-400 mt-1">
                حدد الصلاحيات المطلوبة لهذا الدور بدقة
              </p>
            </div>

            <div class="flex items-center gap-2">
              <n-button size="small" secondary @click="selectAllPermissions">
                <template #icon><CheckIcon class="w-4 h-4" /></template>
                {{ t('roles.selectAll') }}
              </n-button>
              <n-button size="small" secondary @click="deselectAllPermissions">
                <template #icon><DismissIcon class="w-4 h-4" /></template>
                {{ t('roles.deselectAll') }}
              </n-button>
            </div>
          </div>

          <!-- Modules Checkbox Groups -->
          <div class="space-y-4 max-h-[360px] overflow-y-auto px-1">
            <div
              v-for="(perms, moduleName) in permissionsByModule"
              :key="moduleName"
              class="bg-slate-50 dark:bg-slate-900/50 p-4 rounded-xl border border-slate-200 dark:border-slate-800"
            >
              <div class="flex items-center justify-between mb-3">
                <span class="font-bold text-slate-700 dark:text-slate-200 text-sm">
                  📁 {{ moduleName }}
                </span>
                <n-button size="tiny" text type="primary" @click="toggleModulePermissions(String(moduleName))">
                  تبديل تحديد الوحدة
                </n-button>
              </div>

              <div class="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 gap-2">
                <label
                  v-for="p in perms"
                  :key="p.id"
                  class="flex items-start gap-2 p-2 rounded-lg bg-white dark:bg-slate-800 border border-slate-100 dark:border-slate-700 cursor-pointer hover:border-primary/50 transition"
                >
                  <input
                    type="checkbox"
                    :value="p.name"
                    v-model="formData.permissions"
                    class="mt-1 accent-primary rounded"
                  />
                  <div>
                    <div class="font-semibold text-xs text-slate-800 dark:text-white">
                      {{ p.code }}
                    </div>
                    <div class="text-[11px] text-slate-400">
                      {{ p.description || p.name }}
                    </div>
                  </div>
                </label>
              </div>
            </div>
          </div>
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
