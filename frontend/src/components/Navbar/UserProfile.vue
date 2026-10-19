<script setup lang="ts">
import {
  DoorArrowRight20Regular as LogoutIcon,
  Person20Regular as ProfileIcon,
} from '@vicons/fluent'
import { NIcon } from 'naive-ui'

const { t } = useI18n()
const router = useRouter()
const accountStore = useAccountStore()

const items = computed(() => [
  {
    icon: () => h(NIcon, null, { default: () => h(ProfileIcon) }),
    label: t('userMenu.profile'),
    key: 'profile',
  },
  {
    icon: () => h(NIcon, null, { default: () => h(LogoutIcon) }),
    label: t('userMenu.logout'),
    key: 'logout',
  },
])

async function handleSelect(key: string) {
  if (key === 'logout') {
    await accountStore.logout()
    router.push('/account/login')
  } else if (key === 'profile') {
    router.push('/account/profile')
  }
}
</script>

<template>
  <div class="flex items-center" v-bind="$attrs">
    <n-dropdown :options="items" @select="handleSelect">
      <div class="flex items-center gap-2 cursor-pointer p-1 rounded-full hover:bg-slate-100 dark:hover:bg-slate-800 transition">
        <div class="w-8 h-8 rounded-full bg-gradient-to-tr from-primary to-blue-500 text-white font-bold flex items-center justify-center text-sm shadow-sm">
          {{ accountStore.user?.fullName?.charAt(0) || 'U' }}
        </div>
        <span class="text-xs font-semibold text-slate-700 dark:text-slate-200 hidden md:inline">
          {{ accountStore.user?.fullName || 'المستخدم' }}
        </span>
      </div>
    </n-dropdown>
  </div>
</template>

<style lang="scss">
</style>
