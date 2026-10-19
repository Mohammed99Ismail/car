<script setup lang="ts">
import {
  Add24Regular as AddIcon,
  Checkmark24Regular as CheckIcon,
  Delete24Regular as DeleteIcon,
  Dismiss24Regular as DismissIcon,
  Edit24Regular as EditIcon,
  ArrowClockwise24Regular as RefreshIcon,
  Shield24Regular as ShieldIcon,
} from '@vicons/fluent'
import type { DataTableColumns, FormInst, FormRules } from 'naive-ui'
import { NButton, NSpace, NTag } from 'naive-ui'
import { isApiSuccess } from '~/models/ApiResponse'
import type { ModulePermissionsDto, RoleDto, SaveRoleDto } from '~/models/Role'
import { useRoleService } from '~/services/role.service'

const { t, locale } = useI18n()
const dialog = useDialog()
const message = useMessage()
const roleService = useRoleService()
const accountStore = useAccountStore()

// State
const roles = ref<RoleDto[]>([])
const permissionModules = ref<ModulePermissionsDto[]>([])
const loading = ref(false)

// Permissions for action buttons
const canCreate = computed(() => accountStore.hasPermission('Roles.Create'))
const canUpdate = computed(() => accountStore.hasPermission('Roles.Update'))
const canDelete = computed(() => accountStore.hasPermission('Roles.Delete'))

// Modal State
const showModal = ref(false)
const modalLoading = ref(false)
const isEditing = ref(false)
const submitting = ref(false)
const formRef = ref<FormInst | null>(null)
const editingId = ref<number | null>(null)

const formData = reactive<{
  nameAr: string
  nameEn: string
  permissionIds: number[]
}>({
  nameAr: '',
  nameEn: '',
  permissionIds: [],
})

const rules: FormRules = {
  nameAr: [{ required: true, message: 'الاسم بالعربية مطلوب', trigger: ['blur', 'input'] }],
  nameEn: [{ required: true, message: 'Name in English is required', trigger: ['blur', 'input'] }],
}

// All flat permissions list for select/deselect all
const allPermissionIds = computed<number[]>(() => {
  const ids: number[] = []
  permissionModules.value.forEach((m) => {
    m.permissions.forEach((p) => {
      ids.push(p.id)
    })
  })
  return ids
})

async function loadData() {
  loading.value = true
  try {
    const [rolesRes, permsRes] = await Promise.all([
      roleService.getPaged({ pageIndex: 1, pageSize: 100 }),
      roleService.getPermissions(),
    ])
    if (isApiSuccess(rolesRes) && rolesRes.data) {
      roles.value = rolesRes.data.items
    }
    if (isApiSuccess(permsRes) && permsRes.data) {
      permissionModules.value = permsRes.data
    }
  }
  catch (err) {
    console.error('Error loading roles data', err)
  }
  finally {
    loading.value = false
  }
}

function openAddModal() {
  isEditing.value = false
  editingId.value = null
  formData.nameAr = ''
  formData.nameEn = ''
  formData.permissionIds = []
  showModal.value = true
}

async function openEditModal(role: RoleDto) {
  isEditing.value = true
  editingId.value = role.id
  formData.nameAr = role.nameAr
  formData.nameEn = role.nameEn
  formData.permissionIds = []
  showModal.value = true
  modalLoading.value = true

  try {
    const res = await roleService.getById(role.id)
    if (isApiSuccess(res) && res.data) {
      formData.permissionIds = [...res.data.permissionIds]
    }
  }
  catch (err) {
    console.error('Error fetching role details', err)
  }
  finally {
    modalLoading.value = false
  }
}

function selectAllPermissions() {
  formData.permissionIds = [...allPermissionIds.value]
}

function deselectAllPermissions() {
  formData.permissionIds = []
}

function toggleModulePermissions(module: ModulePermissionsDto) {
  const modIds = module.permissions.map(p => p.id)
  const allSelected = modIds.every(id => formData.permissionIds.includes(id))

  if (allSelected) {
    formData.permissionIds = formData.permissionIds.filter(id => !modIds.includes(id))
  }
  else {
    const next = new Set([...formData.permissionIds, ...modIds])
    formData.permissionIds = Array.from(next)
  }
}

async function handleSave() {
  formRef.value?.validate(async (errors) => {
    if (errors) return

    submitting.value = true
    try {
      const payload: SaveRoleDto = {
        nameAr: formData.nameAr.trim(),
        nameEn: formData.nameEn.trim(),
        permissionIds: formData.permissionIds,
      }

      if (isEditing.value && editingId.value) {
        const res = await roleService.update(editingId.value, payload)
        if (isApiSuccess(res)) {
          message.success(t('roles.updateSuccess'))
          showModal.value = false
          loadData()
        }
      }
      else {
        const res = await roleService.create(payload)
        if (isApiSuccess(res)) {
          message.success(t('roles.createSuccess'))
          showModal.value = false
          loadData()
        }
      }
    }
    catch (err) {
      console.error('Error saving role', err)
    }
    finally {
      submitting.value = false
    }
  })
}

function handleDelete(role: RoleDto) {
  if (role.isSystem) {
    message.warning(t('roles.cannotDeleteSystemRole'))
    return
  }

  const roleName = locale.value?.startsWith('en') ? role.nameEn : role.nameAr
  dialog.warning({
    title: t('common.delete'),
    content: `${t('roles.deleteConfirm')} (${roleName})`,
    positiveText: t('common.delete'),
    negativeText: t('common.cancel'),
    onPositiveClick: async () => {
      try {
        const res = await roleService.remove(role.id)
        if (isApiSuccess(res)) {
          message.success(t('roles.deleteSuccess'))
          loadData()
        }
      }
      catch (err) {
        console.error('Error deleting role', err)
      }
    },
  })
}

const columns = computed<DataTableColumns<RoleDto>>(() => [
  {
    title: t('roles.name'),
    key: 'nameAr',
    render(row) {
      const primaryName = locale.value?.startsWith('en') ? row.nameEn : row.nameAr
      const secondaryName = locale.value?.startsWith('en') ? row.nameAr : row.nameEn
      return h('div', { class: 'flex items-center gap-2' }, [
        h(ShieldIcon, { class: 'w-5 h-5 text-primary shrink-0' }),
        h('div', [
          h('div', { class: 'font-semibold text-slate-800 dark:text-white' }, primaryName),
          h('div', { class: 'text-xs text-slate-400 font-mono' }, secondaryName),
        ]),
        row.isSystem
          ? h(NTag, { type: 'warning', size: 'small', bordered: false }, { default: () => 'نظام' })
          : null,
      ])
    },
  },
  {
    title: t('roles.permissionsCount'),
    key: 'permissionsCount',
    width: 140,
    render(row) {
      return h(
        NTag,
        { type: 'info', bordered: false, class: 'font-mono' },
        { default: () => `${row.permissionsCount} صلاحية` },
      )
    },
  },
  {
    title: 'عدد المستخدمين',
    key: 'usersCount',
    width: 130,
    render(row) {
      return h(
        NTag,
        { type: 'default', bordered: false },
        { default: () => `${row.usersCount} مستخدم` },
      )
    },
  },
  {
    title: t('common.actions'),
    key: 'actions',
    width: 140,
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
      if (canDelete.value && !row.isSystem) {
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
])

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
  <div class="p-4 md:p-6">
    <!-- Header -->
    <div class="flex flex-col md:flex-row md:items-center justify-between pb-6 gap-4">
      <div>
        <h1 class="text-2xl font-bold flex items-center gap-2">
          <span>🛡️</span> {{ t('roles.title') }}
        </h1>
        <p class="text-sm text-slate-500 mt-1">
          {{ t('roles.subtitle') || 'إدارة الأدوار وتحديد الصلاحيات الممنوحة لكل دور بمرونة كاملة' }}
        </p>
      </div>

      <div class="flex items-center gap-2">
        <n-button id="roles-refresh-btn" secondary @click="loadData">
          <template #icon>
            <RefreshIcon class="w-4 h-4" />
          </template>
          {{ t('common.refresh') }}
        </n-button>
        <n-button v-if="canCreate" id="roles-create-btn" type="primary" @click="openAddModal">
          <template #icon>
            <AddIcon class="w-5 h-5" />
          </template>
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
        :scroll-x="650"
        :bordered="false"
        class="min-h-[250px]"
      />
    </div>

    <!-- Add/Edit Role Modal -->
    <n-modal
      v-model:show="showModal"
      preset="card"
      :title="isEditing ? t('roles.edit') : t('roles.create')"
      style="width: min(780px, 95vw)"
      :segmented="{ content: true, action: true }"
    >
      <n-spin :show="modalLoading">
        <n-form
          ref="formRef"
          :model="formData"
          :rules="rules"
          label-placement="top"
          require-mark-placement="right-hanging"
        >
          <div class="grid grid-cols-1 sm:grid-cols-2 gap-4 mb-4">
            <n-form-item path="nameAr" label="الاسم (بالعربية)">
              <n-input id="role-name-ar" v-model:value="formData.nameAr" placeholder="مثال: مدير المبيعات" autofocus />
            </n-form-item>

            <n-form-item path="nameEn" label="Name (English)">
              <n-input id="role-name-en" v-model:value="formData.nameEn" placeholder="e.g. Sales Manager" dir="ltr" />
            </n-form-item>
          </div>

          <!-- Permissions Section -->
          <div class="mt-4 pt-4 border-t border-slate-100 dark:border-slate-700">
            <div class="flex items-center justify-between mb-4 flex-wrap gap-2">
              <div>
                <h3 class="font-bold flex items-center gap-2">
                  <span>🔑</span> {{ t('roles.permissions') }}
                  <n-tag size="small" type="primary" round>
                    {{ formData.permissionIds.length }} مختارة
                  </n-tag>
                </h3>
                <p class="text-xs text-slate-400 mt-1">
                  حدد الصلاحيات المطلوبة لهذا الدور بدقة
                </p>
              </div>

              <div class="flex items-center gap-2">
                <n-button size="small" secondary @click="selectAllPermissions">
                  <template #icon>
                    <CheckIcon class="w-4 h-4" />
                  </template>
                  {{ t('roles.selectAll') }}
                </n-button>
                <n-button size="small" secondary @click="deselectAllPermissions">
                  <template #icon>
                    <DismissIcon class="w-4 h-4" />
                  </template>
                  {{ t('roles.deselectAll') }}
                </n-button>
              </div>
            </div>

            <!-- Modules Checkbox Groups -->
            <div class="space-y-4 max-h-[380px] overflow-y-auto px-1">
              <div
                v-for="mod in permissionModules"
                :key="mod.module"
                class="bg-slate-50 dark:bg-slate-900/50 p-4 rounded-xl border border-slate-200 dark:border-slate-800"
              >
                <div class="flex items-center justify-between mb-3">
                  <span class="font-bold text-slate-700 dark:text-slate-200 text-sm">
                    📁 {{ mod.module }}
                  </span>
                  <n-button size="tiny" text type="primary" @click="toggleModulePermissions(mod)">
                    تحديد / إلغاء الوحدة
                  </n-button>
                </div>

                <div class="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 gap-2">
                  <label
                    v-for="p in mod.permissions"
                    :key="p.id"
                    class="flex items-start gap-2 p-2 rounded-lg bg-white dark:bg-slate-800 border border-slate-100 dark:border-slate-700 cursor-pointer hover:border-primary/50 transition"
                  >
                    <input
                      v-model="formData.permissionIds"
                      type="checkbox"
                      :value="p.id"
                      class="mt-1 accent-primary rounded cursor-pointer"
                    >
                    <div class="min-w-0">
                      <div class="font-semibold text-xs text-slate-800 dark:text-white truncate">
                        {{ locale?.startsWith('en') ? p.nameEn : p.nameAr }}
                      </div>
                      <div class="text-[11px] text-slate-400 font-mono truncate" dir="ltr">
                        {{ p.key }}
                      </div>
                    </div>
                  </label>
                </div>
              </div>
            </div>
          </div>
        </n-form>
      </n-spin>

      <template #action>
        <div class="flex justify-end gap-2">
          <n-button @click="showModal = false">
            {{ t('common.cancel') }}
          </n-button>
          <n-button id="role-submit-btn" type="primary" :loading="submitting" :disabled="modalLoading" @click="handleSave">
            {{ isEditing ? t('common.update') : t('common.save') }}
          </n-button>
        </div>
      </template>
    </n-modal>
  </div>
</template>
