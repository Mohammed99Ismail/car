<script setup lang="ts">
import {
  PersonAdd24Regular as AddUserIcon,
  ArrowClockwise24Regular as RefreshIcon,
  Search24Regular as SearchIcon,
  Shield24Regular as ShieldIcon,
} from '@vicons/fluent'
import { useDebounceFn } from '@vueuse/core'
import type { DataTableColumns, FormInst, FormRules } from 'naive-ui'
import { NButton, NSpace, NSwitch, NTag, NTooltip } from 'naive-ui'
import type { BilingualLookupDto } from '~/models/ApiResponse'
import { isApiSuccess, localizedName } from '~/models/ApiResponse'
import type { UserDto } from '~/models/User'
import { useLookupService } from '~/services/lookup.service'
import { useUserService } from '~/services/user.service'

const { t, locale } = useI18n()
const message = useMessage()
const dialog = useDialog()
const userService = useUserService()
const lookupService = useLookupService()
const accountStore = useAccountStore()

// List state
const users = ref<UserDto[]>([])
const loading = ref(false)
const togglingUserId = ref<number | null>(null)
const totalCount = ref(0)
const queryParams = reactive({ pageIndex: 1, pageSize: 10, search: '' })

// Roles lookup
const allRoles = ref<BilingualLookupDto[]>([])
const roleOptions = computed(() =>
  allRoles.value.map(r => ({ label: localizedName(r, locale.value), value: r.id })),
)

const canCreate = computed(() => accountStore.hasPermission('Users.Create'))
const canUpdate = computed(() => accountStore.hasPermission('Users.Update'))

// Roles modal
const showRolesModal = ref(false)
const rolesModalLoading = ref(false)
const updatingRoles = ref(false)
const selectedUser = ref<UserDto | null>(null)
const selectedRoleIds = ref<number[]>([])

// Create user modal
const showCreateModal = ref(false)
const creating = ref(false)
const createFormRef = ref<FormInst | null>(null)
const createForm = reactive({
  fullName: '',
  email: '',
  password: '',
  confirmPassword: '',
  roleIds: [] as number[],
})

const createRules: FormRules = {
  fullName: [{ required: true, message: () => t('users.validation.fullNameRequired'), trigger: ['blur', 'input'] }],
  email: [
    { required: true, message: () => t('users.validation.emailRequired'), trigger: ['blur', 'input'] },
    { type: 'email', message: () => t('users.validation.emailInvalid'), trigger: ['blur'] },
  ],
  password: [
    { required: true, message: () => t('users.validation.passwordRequired'), trigger: ['blur', 'input'] },
    { min: 8, message: () => t('users.validation.passwordMin'), trigger: ['blur'] },
  ],
  confirmPassword: [
    { required: true, message: () => t('users.validation.passwordRequired'), trigger: ['blur', 'input'] },
    {
      validator: (_r, value: string) => value === createForm.password,
      message: () => t('users.validation.passwordMismatch'),
      trigger: ['blur', 'input'],
    },
  ],
}

async function loadRoles() {
  try {
    const res = await lookupService.getRoles()
    if (isApiSuccess(res) && res.data) allRoles.value = res.data
  }
  catch (err) {
    console.error('Error loading roles lookup', err)
  }
}

async function loadUsers() {
  loading.value = true
  try {
    const res = await userService.getPaged(queryParams)
    if (isApiSuccess(res) && res.data) {
      users.value = res.data.items
      totalCount.value = res.data.totalCount
    }
  }
  catch (err) {
    console.error('Error loading users', err)
  }
  finally {
    loading.value = false
  }
}

const debouncedSearch = useDebounceFn(() => {
  queryParams.pageIndex = 1
  loadUsers()
}, 300)

async function handleToggleActive(user: UserDto, value: boolean) {
  const currentUserId = Number(accountStore.user?.userId || (accountStore.user as any)?.id)
  if (user.id === currentUserId) {
    message.warning(t('users.cannotModifySelf'))
    return
  }

  if (!value) {
    dialog.warning({
      title: t('users.deactivate'),
      content: t('users.confirmDeactivate'),
      positiveText: t('common.confirm'),
      negativeText: t('common.cancel'),
      onPositiveClick: async () => {
        await executeStatusChange(user, false)
      },
    })
    return
  }

  await executeStatusChange(user, true)
}

async function executeStatusChange(user: UserDto, value: boolean) {
  togglingUserId.value = user.id
  const previous = user.isActive
  user.isActive = value
  try {
    const res = await userService.setStatus(user.id, value)
    if (isApiSuccess(res)) {
      message.success(res.message || t('users.statusChangedSuccess'))
    } else {
      user.isActive = previous
      message.error(res.message || t('common.500Message'))
    }
  } catch (err: any) {
    user.isActive = previous
    const msg = err?.response?.data?.message || err?.message || t('common.500Message')
    message.error(msg)
    console.error('Error changing status', err)
  } finally {
    togglingUserId.value = null
  }
}

async function openRolesModal(user: UserDto) {
  selectedUser.value = user
  selectedRoleIds.value = []
  showRolesModal.value = true
  rolesModalLoading.value = true
  try {
    const res = await userService.getById(user.id)
    if (isApiSuccess(res) && res.data) selectedRoleIds.value = [...res.data.roleIds]
  }
  catch (err) {
    console.error('Error loading user details', err)
  }
  finally {
    rolesModalLoading.value = false
  }
}

async function saveUserRoles() {
  if (!selectedUser.value) return
  updatingRoles.value = true
  try {
    const res = await userService.updateRoles(selectedUser.value.id, { roleIds: selectedRoleIds.value })
    if (isApiSuccess(res)) {
      message.success(t('users.rolesUpdatedSuccess'))
      showRolesModal.value = false
      loadUsers()
    }
  }
  catch (err) {
    console.error('Error updating user roles', err)
  }
  finally {
    updatingRoles.value = false
  }
}

function openCreateModal() {
  Object.assign(createForm, { fullName: '', email: '', password: '', confirmPassword: '', roleIds: [] })
  showCreateModal.value = true
}

function handleCreate() {
  createFormRef.value?.validate(async (errors) => {
    if (errors) return
    creating.value = true
    try {
      const res = await userService.create({
        fullName: createForm.fullName.trim(),
        email: createForm.email.trim(),
        password: createForm.password,
        roleIds: createForm.roleIds,
      })
      if (isApiSuccess(res)) {
        message.success(t('users.createSuccess'))
        showCreateModal.value = false
        queryParams.pageIndex = 1
        loadUsers()
      }
    }
    catch (err) {
      console.error('Error creating user', err)
    }
    finally {
      creating.value = false
    }
  })
}

const columns = computed<DataTableColumns<UserDto>>(() => [
  {
    title: t('users.fullName'),
    key: 'fullName',
    minWidth: 220,
    render(row) {
      return h('div', { class: 'flex items-center gap-2' }, [
        h('div', { class: 'w-8 h-8 shrink-0 rounded-full bg-primary/10 text-primary flex items-center justify-center font-bold text-sm' }, row.fullName.charAt(0)),
        h('div', { class: 'min-w-0' }, [
          h('div', { class: 'font-semibold truncate' }, row.fullName),
          h('div', { class: 'text-xs text-slate-400 truncate', dir: 'ltr' }, row.email),
        ]),
      ])
    },
  },
  {
    title: t('users.roles'),
    key: 'roles',
    minWidth: 180,
    render(row) {
      if (!row.roles?.length) return h('span', { class: 'text-slate-400' }, '-')
      return h(NSpace, { size: 'small' }, {
        default: () => row.roles.map(role =>
          h(NTag, { type: 'info', size: 'small', bordered: false }, { default: () => role }),
        ),
      })
    },
  },
  {
    title: t('users.createdDate'),
    key: 'createdDate',
    width: 130,
    render(row) {
      return row.createdDate ? new Date(row.createdDate).toLocaleDateString('en-GB') : '-'
    },
  },
  {
    title: t('users.status'),
    key: 'isActive',
    width: 170,
    render(row) {
      const currentUserId = Number(accountStore.user?.userId || (accountStore.user as any)?.id)
      const isSelf = row.id === currentUserId
      const isToggling = togglingUserId.value === row.id

      const statusTag = h(
        NTag,
        {
          type: row.isActive ? 'success' : 'default',
          size: 'small',
          round: true,
          bordered: false,
          class: 'font-semibold',
        },
        {
          default: () => (row.isActive ? t('users.active') : t('users.inactive')),
        },
      )

      if (isSelf) {
        return h('div', { class: 'flex items-center gap-2' }, [
          statusTag,
          h(
            NTooltip,
            { trigger: 'hover' },
            {
              trigger: () =>
                h(NSwitch, {
                  value: row.isActive,
                  disabled: true,
                  size: 'small',
                }),
              default: () => t('users.cannotModifySelf'),
            },
          ),
        ])
      }

      const switchControl = h(NSwitch, {
        value: row.isActive,
        loading: isToggling,
        disabled: !canUpdate.value || isToggling,
        size: 'small',
        'onUpdate:value': (newVal: boolean) => handleToggleActive(row, newVal),
      })

      return h('div', { class: 'flex items-center gap-2' }, [
        statusTag,
        switchControl,
      ])
    },
  },
  {
    title: t('common.actions'),
    key: 'actions',
    width: 150,
    render(row) {
      if (!canUpdate.value) return null
      return h(NButton, { size: 'small', type: 'primary', secondary: true, onClick: () => openRolesModal(row) }, {
        icon: () => h(ShieldIcon),
        default: () => t('users.editRoles'),
      })
    },
  },
])

function onPageChange(page: number) {
  queryParams.pageIndex = page
  loadUsers()
}

function onPageSizeChange(pageSize: number) {
  queryParams.pageSize = pageSize
  queryParams.pageIndex = 1
  loadUsers()
}

onMounted(() => {
  loadRoles()
  loadUsers()
})
</script>

<route lang="yaml">
meta:
  title: users
  layout: default
</route>

<template>
  <div class="p-4 md:p-6">
    <!-- Header -->
    <div class="flex flex-col md:flex-row md:items-center justify-between pb-6 gap-4">
      <div>
        <h1 class="text-2xl font-bold flex items-center gap-2">
          <span>👥</span> {{ t('users.title') }}
        </h1>
        <p class="text-sm text-slate-500 mt-1">
          {{ t('users.subtitle') }}
        </p>
      </div>

      <div class="flex items-center gap-2">
        <n-button id="users-refresh-btn" secondary @click="loadUsers">
          <template #icon>
            <RefreshIcon class="w-4 h-4" />
          </template>
          {{ t('common.refresh') }}
        </n-button>
        <n-button v-if="canCreate" id="users-create-btn" type="primary" @click="openCreateModal">
          <template #icon>
            <AddUserIcon class="w-5 h-5" />
          </template>
          {{ t('users.create') }}
        </n-button>
      </div>
    </div>

    <!-- Search -->
    <div class="bg-white dark:bg-slate-800 rounded-xl p-4 shadow-sm border border-slate-100 dark:border-slate-700 mb-6">
      <div class="max-w-md">
        <n-input
          id="users-search"
          v-model:value="queryParams.search"
          clearable
          :placeholder="t('users.searchPlaceholder')"
          @update:value="debouncedSearch"
        >
          <template #prefix>
            <SearchIcon class="w-4 h-4 text-slate-400" />
          </template>
        </n-input>
      </div>
    </div>

    <!-- Table -->
    <div class="bg-white dark:bg-slate-800 rounded-xl shadow-sm border border-slate-100 dark:border-slate-700 overflow-hidden">
      <n-data-table
        remote
        :columns="columns"
        :data="users"
        :loading="loading"
        :row-key="(row: UserDto) => row.id"
        :scroll-x="800"
        :pagination="{
          page: queryParams.pageIndex,
          pageSize: queryParams.pageSize,
          itemCount: totalCount,
          showSizePicker: true,
          pageSizes: [10, 20, 50],
          prefix: ({ itemCount }) => `${t('common.total')}: ${itemCount}`,
          onUpdatePage: onPageChange,
          onUpdatePageSize: onPageSizeChange,
        }"
        :bordered="false"
        class="min-h-[300px]"
      />
    </div>

    <!-- Create User Modal -->
    <n-modal
      v-model:show="showCreateModal"
      preset="card"
      :title="t('users.create')"
      style="width: min(560px, 95vw)"
      :segmented="{ content: true, action: true }"
    >
      <n-form
        ref="createFormRef"
        :model="createForm"
        :rules="createRules"
        label-placement="top"
        require-mark-placement="right-hanging"
      >
        <div class="grid grid-cols-1 sm:grid-cols-2 gap-x-4">
          <n-form-item path="fullName" :label="t('users.fullName')" class="sm:col-span-2">
            <n-input id="create-user-fullname" v-model:value="createForm.fullName" />
          </n-form-item>
          <n-form-item path="email" :label="t('users.email')" class="sm:col-span-2">
            <n-input id="create-user-email" v-model:value="createForm.email" dir="ltr" />
          </n-form-item>
          <n-form-item path="password" :label="t('users.password')">
            <n-input
              id="create-user-password"
              v-model:value="createForm.password"
              type="password"
              show-password-on="click"
              dir="ltr"
            />
          </n-form-item>
          <n-form-item path="confirmPassword" :label="t('users.confirmPassword')">
            <n-input
              id="create-user-confirm-password"
              v-model:value="createForm.confirmPassword"
              type="password"
              show-password-on="click"
              dir="ltr"
            />
          </n-form-item>
          <n-form-item path="roleIds" :label="t('users.roles')" class="sm:col-span-2">
            <n-select
              id="create-user-roles"
              v-model:value="createForm.roleIds"
              multiple
              filterable
              :options="roleOptions"
              :placeholder="t('users.selectRoles')"
            />
          </n-form-item>
        </div>
      </n-form>

      <template #action>
        <div class="flex justify-end gap-2">
          <n-button @click="showCreateModal = false">
            {{ t('common.cancel') }}
          </n-button>
          <n-button id="create-user-submit" type="primary" :loading="creating" @click="handleCreate">
            {{ t('common.save') }}
          </n-button>
        </div>
      </template>
    </n-modal>

    <!-- Assign Roles Modal -->
    <n-modal
      v-model:show="showRolesModal"
      preset="card"
      :title="t('users.editRoles')"
      style="width: min(480px, 95vw)"
      :segmented="{ content: true, action: true }"
    >
      <n-spin :show="rolesModalLoading">
        <div v-if="selectedUser" class="space-y-4">
          <div class="p-3 bg-slate-50 dark:bg-slate-900 rounded-lg">
            <div class="font-bold">
              {{ selectedUser.fullName }}
            </div>
            <div class="text-xs text-slate-400" dir="ltr">
              {{ selectedUser.email }}
            </div>
          </div>
          <div>
            <label class="block text-sm font-semibold mb-2">{{ t('users.assignedRoles') }}</label>
            <n-select
              id="user-roles-select"
              v-model:value="selectedRoleIds"
              multiple
              filterable
              :placeholder="t('users.selectRoles')"
              :options="roleOptions"
            />
          </div>
        </div>
      </n-spin>

      <template #action>
        <div class="flex justify-end gap-2">
          <n-button @click="showRolesModal = false">
            {{ t('common.cancel') }}
          </n-button>
          <n-button id="user-roles-save" type="primary" :loading="updatingRoles" :disabled="rolesModalLoading" @click="saveUserRoles">
            {{ t('common.save') }}
          </n-button>
        </div>
      </template>
    </n-modal>
  </div>
</template>
