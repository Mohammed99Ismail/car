<script setup lang="ts">
import {
  Dismiss24Filled as CloseIcon,
  People24Regular as CustomersIcon,
  People24Filled as CustomersIconActive,
  Board24Regular as DashboardIcon,
  VehicleCar24Regular as CarsIcon,
  VehicleCar24Filled as CarsIconActive,
  Building24Regular as BranchesIcon,
  Building24Filled as BranchesIconActive,
  DocumentBulletList24Regular as ContractTypesIcon,
  DocumentBulletList24Filled as ContractTypesIconActive,
  ShieldKeyhole24Regular as RolesIcon,
  ShieldKeyhole24Filled as RolesIconActive,
  Person24Regular as UsersIcon,
  Person24Filled as UsersIconActive,
} from '@vicons/fluent'

import { storeToRefs } from 'pinia'
import type { SidebarMenuOption } from './SidebarMenu.vue'

const layoutStore = useLayoutStore()
const accountStore = useAccountStore()
const { collapsed, forceCollapsed, mobileMode, mobileMenuClosed } =
  storeToRefs(layoutStore)
const { t } = useI18n()

const effectiveCollapsed = computed(() => {
  if (mobileMode.value) return mobileMenuClosed.value

  return collapsed.value || forceCollapsed.value
})

const menuOptions = computed<SidebarMenuOption[]>(() => {
  const options: SidebarMenuOption[] = [
    {
      label: t('menu.dashboard'),
      route: '/',
      key: 'index',
      icon: DashboardIcon,
    },
  ]

  if (accountStore.hasPermission('Cars.View')) {
    options.push({
      label: t('menu.cars'),
      route: '/cars',
      key: 'cars',
      icon: CarsIcon,
      activeIcon: CarsIconActive,
    })
  }

  if (accountStore.hasPermission('Customers.View')) {
    options.push({
      label: t('menu.customers'),
      route: '/customers',
      key: 'customers',
      icon: CustomersIcon,
      activeIcon: CustomersIconActive,
    })
  }

  if (accountStore.hasPermission('Branches.View')) {
    options.push({
      label: t('menu.branches'),
      route: '/branches',
      key: 'branches',
      icon: BranchesIcon,
      activeIcon: BranchesIconActive,
    })
  }

  if (accountStore.hasPermission('ContractTypes.View')) {
    options.push({
      label: t('menu.contractTypes'),
      route: '/contract-types',
      key: 'contract-types',
      icon: ContractTypesIcon,
      activeIcon: ContractTypesIconActive,
    })
  }

  if (accountStore.hasPermission('Roles.View')) {
    options.push({
      label: t('menu.roles'),
      route: '/roles',
      key: 'roles',
      icon: RolesIcon,
      activeIcon: RolesIconActive,
    })
  }

  if (accountStore.hasPermission('Users.View')) {
    options.push({
      label: t('menu.users'),
      route: '/users',
      key: 'users',
      icon: UsersIcon,
      activeIcon: UsersIconActive,
    })
  }

  return options
})

const router = useRouter()
//mobile view
router.beforeEach(() => {
  layoutStore.closeSidebar()
})
</script>

<template>
  <n-layout-sider :native-scrollbar="false" collapse-mode="width" :collapsed-width="mobileMode ? 0 : 64"
    :collapsed="effectiveCollapsed"
    :class="{ 'collapsed': effectiveCollapsed, 'mobile-mode': mobileMode, 'support-mode': layoutStore.supportEnabled }">
    <div class="logo-container mb-4">
      <div flex w-full justify-between items-center>
        <div flex w-full justify-start items-center>
          <div class="logo-bg"><img src="@/assets/images/logo.png" alt="logo" class="logo"></div>
          <h1 class="main-title">
            {{ t('title') }}
          </h1>
        </div>

        <n-button v-if="mobileMode" mx-2 size="small" tertiary circle @click="layoutStore.closeSidebar">
          <template #icon>
            <NIcon size="1.2rem">
              <CloseIcon />
            </NIcon>
          </template>
        </n-button>
      </div>
    </div>
    <SidebarMenu :collapsed-width="mobileMode ? 0 : 64" :collapsed-icon-size="mobileMode ? 30 : 20"
      :options="menuOptions" />
  </n-layout-sider>
</template>

<style lang="scss">
.n-scrollbar {
  z-index: 1;
}

.logo-container {
  display: flex;
  align-items: center;
  padding: 1.5rem 0.8rem 0.5rem 1.1rem;
  transition: all 100ms;
  line-height: 1;

  .main-title {
    font-family: Quicksand, Shabnam;
    font-size: 1.3rem;
    font-weight: 500;
    user-select: none;
  }

  .logo-bg {
    width: 38px;
    height: 38px;
    display: flex;
    margin: 0 .34rem;
    justify-content: center;
    align-items: center;

    .logo {
      width: 34px;
      object-fit: cover;
    }
  }

  .text-logo {
    max-width: 175px;
  }
}

.mobile-mode {
  max-width: 100% !important;
  width: 100% !important;
}

.mobile-mode.collapsed {
  max-width: 0 !important;
}

.collapsed {
  .logo-container {
    padding: 1.5rem 0.5rem 0.5rem .5rem;
  }

  .main-title {
    display: none;
  }

  .n-menu-item-group>.n-menu-item-group-title {
    display: none;
  }

  .p-button-label {
    display: none;
  }
}

.n-menu .n-menu-item-content:not(.n-menu-item-content--disabled):hover::before {
  background-color: rgba(189, 189, 189, 0.15);
}

.n-menu-tooltip span {
  color: #e4e4e4 !important;
}

.n-layout-sider {
  background-color: transparent;
}

.p-button {
  .p-button-label {
    text-align: left;
  }
}

.rtl {
  .logo {
    margin-left: 0.8rem;
    margin-right: .5rem;
  }

  .n-menu-item-group-title {
    margin-left: auto;
    margin-right: 32px;
  }
}

.support-mode {
  .n-scrollbar>.n-scrollbar-container {
    max-height: calc(100% - 120px);
  }
}

.n-menu-item {
  user-select: none;
}

.main-menu {
  .active {
    .p-button {

      .p-button-label,
      .p-button-icon {
        color: var(--primary-color);
      }
    }

    ul>li>a {
      display: block;
    }
  }

  .separator {
    border-bottom: solid 1px #f4f4f5;
    margin-bottom: .5rem;
  }
}

.p-sidebar-header {
  justify-content: center;
  font-weight: bold;
  padding-top: 1.7rem !important;
}

.p-sidebar-header-content {
  width: 100%;
}

.n-menu-item-group .n-submenu .n-menu-item-content.n-menu-item-content--collapsed {
  padding-left: 22px !important;
}

.n-menu .n-menu-item-group .n-menu-item-group-title {
  height: 20px;
}
</style>
