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
  Table24Regular as LookupsIcon,
  Table24Filled as LookupsIconActive,
  Tag24Regular as MakesIcon,
  Tag24Filled as MakesIconActive,
  Shapes24Regular as ModelsIcon,
  Shapes24Filled as ModelsIconActive,
  Color24Regular as ColorsIcon,
  Color24Filled as ColorsIconActive,
  DocumentBulletList24Regular as ContractTypesIcon,
  DocumentBulletList24Filled as ContractTypesIconActive,
  DocumentText24Regular as ContractsIcon,
  DocumentText24Filled as ContractsIconActive,
  ShieldKeyhole24Regular as RolesIcon,
  ShieldKeyhole24Filled as RolesIconActive,
  Person24Regular as UsersIcon,
  Person24Filled as UsersIconActive,
  Settings24Regular as SettingsIcon,
  Settings24Filled as SettingsIconActive,
} from '@vicons/fluent'

import { storeToRefs } from 'pinia'
import type { SidebarMenuOption } from './SidebarMenu.vue'
import { appConfig } from '~/config/app.config'

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

  if (accountStore.hasPermission('Contracts.View')) {
    options.push({
      label: t('menu.contracts'),
      route: '/contracts',
      key: 'contracts',
      icon: ContractsIcon,
      activeIcon: ContractsIconActive,
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

  // Lookups Submenu (Dropdown list button for all lookups)
  const lookupChildren: SidebarMenuOption[] = [
    {
      label: t('menu.lookupsHub'),
      route: '/lookups',
      key: 'lookups',
      icon: LookupsIcon,
      activeIcon: LookupsIconActive,
    },
  ]

  if (accountStore.hasPermission('CarMakes.View')) {
    lookupChildren.push({
      label: t('menu.carMakes'),
      route: '/car-makes',
      key: 'car-makes',
      icon: MakesIcon,
      activeIcon: MakesIconActive,
    })
  }

  if (accountStore.hasPermission('CarModels.View')) {
    lookupChildren.push({
      label: t('menu.carModels'),
      route: '/car-models',
      key: 'car-models',
      icon: ModelsIcon,
      activeIcon: ModelsIconActive,
    })
  }

  if (accountStore.hasPermission('CarColors.View')) {
    lookupChildren.push({
      label: t('menu.carColors'),
      route: '/car-colors',
      key: 'car-colors',
      icon: ColorsIcon,
      activeIcon: ColorsIconActive,
    })
  }

  if (accountStore.hasPermission('ContractTypes.View')) {
    lookupChildren.push({
      label: t('menu.contractTypes'),
      route: '/contract-types',
      key: 'contract-types',
      icon: ContractTypesIcon,
      activeIcon: ContractTypesIconActive,
    })
  }

  if (lookupChildren.length > 0) {
    options.push({
      label: t('menu.lookups'),
      key: 'lookups-group',
      icon: LookupsIcon,
      activeIcon: LookupsIconActive,
      children: lookupChildren,
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

  if (accountStore.hasPermission('Settings.View')) {
    options.push({
      label: t('menu.settings'),
      route: '/settings',
      key: 'settings',
      icon: SettingsIcon,
      activeIcon: SettingsIconActive,
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
    :class="{ 'collapsed': effectiveCollapsed, 'mobile-mode': mobileMode }">
    <div class="logo-container mb-4">
      <div class="flex w-full justify-between items-center overflow-hidden">
        <div class="flex w-full justify-start items-center gap-2 overflow-hidden">
          <div class="logo-bg shrink-0">
            <img :src="appConfig.logoPath" :alt="appConfig.logoAlt" class="logo">
          </div>
          <h1 class="main-title" :title="appConfig.appName">
            {{ appConfig.appName }}
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
  padding: 1.25rem 0.8rem 0.5rem 0.8rem;
  transition: all 100ms;
  line-height: 1.2;

  .main-title {
    font-family: 'Cairo', Quicksand, sans-serif;
    font-size: 1.05rem;
    font-weight: 700;
    line-height: 1.35;
    user-select: none;
    max-width: 175px;
    display: -webkit-box;
    -webkit-line-clamp: 2;
    -webkit-box-orient: vertical;
    overflow: hidden;
    text-overflow: ellipsis;
    word-break: break-word;
  }

  .logo-bg {
    width: 38px;
    height: 38px;
    min-width: 38px;
    min-height: 38px;
    max-width: 38px;
    max-height: 38px;
    display: flex;
    justify-content: center;
    align-items: center;
    overflow: hidden;
    flex-shrink: 0;
    border-radius: 10px;
    background: rgba(241, 245, 249, 0.7);
    border: 1px solid rgba(226, 232, 240, 0.8);
    padding: 2px;

    .logo {
      width: 100%;
      height: 100%;
      max-width: 100%;
      max-height: 100%;
      object-fit: contain;
      display: block;
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
