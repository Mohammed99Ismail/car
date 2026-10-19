<script setup lang="ts">
import {
  Edit24Regular as EditIcon,
  Person24Regular as PersonIcon,
  ArrowClockwise24Regular as RefreshIcon,
  Search24Regular as SearchIcon,
  Shield24Regular as ShieldIcon,
} from '@vicons/fluent'
import { useDebounceFn } from '@vueuse/core'
import type { DataTableColumns } from 'naive-ui'
import { NButton, NSelect, NSpace, NSwitch, NTag } from 'naive-ui'
import type { LookupItemDto } from '~/models/ApiResponse'
import { ResultStatusEnum, isApiSuccess } from '~/models/ApiResponse'
import type { UserDto } from '~/models/User'
import { useLookupService } from '~/services/lookup.service'
import { useUserService } from '~/services/user.service'

const { t } = useI18n()
const message = useMessage()
const userService = useUserService()
const lookupService = useLookupService()
const accountStore = useAccountStore()

// State
const users = ref<UserDto[]>([])
const loading = ref(false)
const totalCount = ref(0)
const queryParams = reactive({
  pageIndex: 1,
  pageSize: 10,
  search: '',
})

// Roles Lookup
const allRoles = ref<LookupItemDto[]>([])

// Manage Roles Modal State
const showRolesModal = ref(false)
const updatingRoles = ref(false)
const selectedUser = ref<UserDto | null>(null)
const selectedRoles = ref<string[]>([])

async function loadRoles() {
  try {
    const res = await lookupService.getRoles()
    if (res.data) allRoles.value = res.data
  } catch (err) {
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
  } catch (err) {
    console.error('Error loading users', err)
  } finally {
    loading.value = false
  }
}

const debouncedSearch = useDebounceFn(() => {
  queryParams.pageIndex = 1
  loadUsers()
}, 300)

async function handleToggleActive(user: UserDto, value: boolean) {
  try {
    const res = await userService.toggleActive(user.id)
    if (isApiSuccess(res)) {
      user.isActive = value
      message.success(t('users.statusChangedSuccess'))
    } else {
      user.isActive = !value // revert
    }
  } catch (err) {
    user.isActive = !value
    console.error('Error toggling active', err)
  }
}

function openRolesModal(user: UserDto) {
  selectedUser.value = user
  selectedRoles.value = [...(user.roles || [])]
  showRolesModal.value = true
}

async function saveUserRoles() {
  if (!selectedUser.value) return

  updatingRoles.value = true
  try {
    const res = await userService.updateRoles(selectedUser.value.id, {
      roles: selectedRoles.value,
    })
    if (isApiSuccess(res)) {
      message.success(t('users.rolesUpdatedSuccess'))
      selectedUser.value.roles = [...selectedRoles.value]
      showRolesModal.value = false
    }
  } catch (err) {
    console.error('Error updating user roles', err)
  } finally {
    updatingRoles.value = false
  }
}

const columns: DataTableColumns<UserDto> = [
  {
    title: t('users.fullName'),
    key: 'fullName',
    render(row) {
      return h('div', { class: 'flex items-center gap-2' }, [
        h('div', { class: 'w-8 h-8 rounded-full bg-primary/10 text-primary flex items-center justify-center font-bold text-sm' }, row.fullName.charAt(0)),
        h('div', [
          h('div', { class: 'font-semibold text-slate-800 dark:text-white' }, row.fullName),
          h('div', { class: 'text-xs text-slate-400' }, row.email),
        ]),
      ])
    },
  },
  {
    title: t('users.mobile'),
    key: 'mobile',
    render(row) {
      return row.mobile ? h('div', { dir: 'ltr', class: 'font-mono text-left' }, row.mobile) : '-'
    },
  },
  {
    title: t('users.roles'),
    key: 'roles',
    render(row) {
      return h(
        NSpace,
        { size: 'small' },
        {
          default: () =>
            (row.roles || []).map((role) =>
              h(
                NTag,
                {
                  type: role === 'SuperAdmin' ? 'warning' : 'info',
                  size: 'small',
                  bordered: false,
                },
                { default: () => role },
              ),
            ),
        },
      )
    },
  },
  {
    title: t('users.status'),
    key: 'isActive',
    width: 120,
    render(row) {
      return h(NSwitch, {
        value: row.isActive,
        disabled: !accountStore.hasPermission('Users.ToggleActive'),
        'onUpdate:value': (val: boolean) => handleToggleActive(row, val),
      })
    },
  },
  {
    title: t('common.actions'),
    key: 'actions',
    width: 140,
    render(row) {
      const actions = []
      if (accountStore.hasPermission('Users.ManageRoles')) {
        actions.push(
          h(
            NButton,
            {
              size: 'small',
              type: 'primary',
              secondary: true,
              onClick: () => openRolesModal(row),
            },
            {
              icon: () => h(ShieldIcon),
              default: () => t('users.editRoles'),
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
  <div class="p-6">
    <!-- Header -->
    <div class="flex flex-col md:flex-row md:items-center justify-between pb-6 gap-4">
      <div>
        <h1 class="text-2xl font-bold text-slate-800 dark:text-white flex items-center gap-2">
          <span>👥</span> {{ t('users.title') }}
        </h1>
        <p class="text-sm text-slate-500 mt-1">
          إدارة حسابات المستخدمين، تفعيل/تعطيل الحسابات، وتعيين الأدوار بمرونة
        </p>
      </div>

      <div class="flex items-center gap-2">
        <n-button secondary @click="loadUsers">
          <template #icon><RefreshIcon class="w-4 h-4" /></template>
          {{ t('common.refresh') }}
        </n-button>
      </div>
    </div>

    <!-- Search Bar -->
    <div class="bg-white dark:bg-slate-800 rounded-xl p-4 shadow-sm border border-slate-100 dark:border-slate-700 mb-6">
      <div class="max-w-md">
        <n-input
          v-model:value="queryParams.search"
          clearable
          placeholder="ابحث بالاسم الكامل أو البريد الإلكتروني..."
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
        :data="users"
        :loading="loading"
        :pagination="{
          page: queryParams.pageIndex,
          pageSize: queryParams.pageSize,
          itemCount: totalCount,
          showSizePicker: true,
          pageSizes: [10, 20, 50],
          prefix: (info) => `إجمالي: ${info.itemCount} مستخدم`,
          onUpdatePage: onPageChange,
          onUpdatePageSize: onPageSizeChange,
        }"
        :bordered="false"
        class="min-h-[300px]"
      />
    </div>

    <!-- Assign Roles Modal -->
    <n-modal
      v-model:show="showRolesModal"
      preset="card"
      :title="t('users.editRoles')"
      class="max-w-md"
      :segmented="{ content: true, action: true }"
    >
      <div v-if="selectedUser" class="space-y-4">
        <div class="p-3 bg-slate-50 dark:bg-slate-900 rounded-lg">
          <div class="font-bold text-slate-800 dark:text-white">
            {{ selectedUser.fullName }}
          </div>
          <div class="text-xs text-slate-400">
            {{ selectedUser.email }}
          </div>
        </div>

        <div>
          <label class="block text-sm font-semibold text-slate-700 dark:text-slate-200 mb-2">
            الأدوار الممنوحة لهذا المستخدم:
          </label>
          <n-select
            v-model:value="selectedRoles"
            multiple
            filterable
            placeholder="اختر دوراً أو أكثر..."
            :options="allRoles.map(r => ({ label: r.name, value: r.name }))"
          />
        </div>
      </div>

      <template #action>
        <div class="flex justify-end gap-2">
          <n-button @click="showRolesModal = false">
            {{ t('common.cancel') }}
          </n-button>
          <n-button type="primary" :loading="updatingRoles" @click="saveUserRoles">
            {{ t('common.save') }}
          </n-button>
        </div>
      </template>
    </n-modal>
  </div>
</template>
