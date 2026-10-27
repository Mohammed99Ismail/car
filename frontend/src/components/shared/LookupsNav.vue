<script setup lang="ts">
import {
  Table24Regular as HubIcon,
  Tag24Regular as MakesIcon,
  VehicleCar24Regular as ModelsIcon,
  Color24Regular as ColorsIcon,
  DocumentBulletList24Regular as ContractTypesIcon,
  ChevronDown24Regular as DropdownIcon,
  Navigation24Regular as NavIcon,
} from '@vicons/fluent'
import type { DropdownOption } from 'naive-ui'

interface Props {
  activeKey?: 'hub' | 'makes' | 'models' | 'colors' | 'contract-types'
}

const props = defineProps<Props>()
const { t } = useI18n()
const router = useRouter()
const accountStore = useAccountStore()

const dropdownOptions = computed<DropdownOption[]>(() => {
  const options: DropdownOption[] = [
    {
      label: () => t('lookups.hub'),
      key: '/lookups',
      icon: () => h(HubIcon, { class: 'w-4 h-4' }),
    },
  ]

  const items: DropdownOption[] = []
  if (accountStore.hasPermission('CarMakes.View')) {
    items.push({
      label: () => t('carMakes.title'),
      key: '/car-makes',
      icon: () => h(MakesIcon, { class: 'w-4 h-4 text-emerald-500' }),
    })
  }
  if (accountStore.hasPermission('CarModels.View')) {
    items.push({
      label: () => t('carModels.title'),
      key: '/car-models',
      icon: () => h(ModelsIcon, { class: 'w-4 h-4 text-blue-500' }),
    })
  }
  if (accountStore.hasPermission('CarColors.View')) {
    items.push({
      label: () => t('carColors.title'),
      key: '/car-colors',
      icon: () => h(ColorsIcon, { class: 'w-4 h-4 text-purple-500' }),
    })
  }
  if (accountStore.hasPermission('ContractTypes.View')) {
    items.push({
      label: () => t('menu.contractTypes'),
      key: '/contract-types',
      icon: () => h(ContractTypesIcon, { class: 'w-4 h-4 text-amber-500' }),
    })
  }

  if (items.length > 0) {
    options.push({ type: 'divider', key: 'd1' }, ...items)
  }

  return options
})

function handleSelect(key: string) {
  router.push(key)
}
</script>

<template>
  <div class="mb-6 bg-gradient-to-r from-slate-50 via-white to-slate-50 dark:from-slate-800/80 dark:via-slate-800 dark:to-slate-800/80 p-3 rounded-2xl border border-slate-200/80 dark:border-slate-700/80 shadow-sm flex flex-wrap items-center justify-between gap-3">
    <!-- Lookups Tabs -->
    <div class="flex flex-wrap items-center gap-1.5 sm:gap-2">
      <router-link to="/lookups" custom v-slot="{ navigate }">
        <n-button
          size="medium"
          :type="activeKey === 'hub' ? 'primary' : 'default'"
          :secondary="activeKey !== 'hub'"
          class="rounded-xl font-medium transition-all duration-200 hover:scale-[1.02]"
          @click="navigate"
        >
          <template #icon>
            <HubIcon class="w-4 h-4" />
          </template>
          {{ t('lookups.hub') }}
        </n-button>
      </router-link>

      <router-link
        v-if="accountStore.hasPermission('CarMakes.View')"
        to="/car-makes"
        custom
        v-slot="{ navigate }"
      >
        <n-button
          size="medium"
          :type="activeKey === 'makes' ? 'primary' : 'default'"
          :secondary="activeKey !== 'makes'"
          class="rounded-xl font-medium transition-all duration-200 hover:scale-[1.02]"
          @click="navigate"
        >
          <template #icon>
            <MakesIcon class="w-4 h-4 text-emerald-500" />
          </template>
          {{ t('carMakes.title') }}
        </n-button>
      </router-link>

      <router-link
        v-if="accountStore.hasPermission('CarModels.View')"
        to="/car-models"
        custom
        v-slot="{ navigate }"
      >
        <n-button
          size="medium"
          :type="activeKey === 'models' ? 'primary' : 'default'"
          :secondary="activeKey !== 'models'"
          class="rounded-xl font-medium transition-all duration-200 hover:scale-[1.02]"
          @click="navigate"
        >
          <template #icon>
            <ModelsIcon class="w-4 h-4 text-blue-500" />
          </template>
          {{ t('carModels.title') }}
        </n-button>
      </router-link>

      <router-link
        v-if="accountStore.hasPermission('CarColors.View')"
        to="/car-colors"
        custom
        v-slot="{ navigate }"
      >
        <n-button
          size="medium"
          :type="activeKey === 'colors' ? 'primary' : 'default'"
          :secondary="activeKey !== 'colors'"
          class="rounded-xl font-medium transition-all duration-200 hover:scale-[1.02]"
          @click="navigate"
        >
          <template #icon>
            <ColorsIcon class="w-4 h-4 text-purple-500" />
          </template>
          {{ t('carColors.title') }}
        </n-button>
      </router-link>

      <router-link
        v-if="accountStore.hasPermission('ContractTypes.View')"
        to="/contract-types"
        custom
        v-slot="{ navigate }"
      >
        <n-button
          size="medium"
          :type="activeKey === 'contract-types' ? 'primary' : 'default'"
          :secondary="activeKey !== 'contract-types'"
          class="rounded-xl font-medium transition-all duration-200 hover:scale-[1.02]"
          @click="navigate"
        >
          <template #icon>
            <ContractTypesIcon class="w-4 h-4 text-amber-500" />
          </template>
          {{ t('menu.contractTypes') }}
        </n-button>
      </router-link>
    </div>

    <!-- Quick Dropdown List Button -->
    <div class="flex items-center gap-2">
      <n-dropdown
        trigger="click"
        :options="dropdownOptions"
        @select="handleSelect"
      >
        <n-button
          size="medium"
          secondary
          type="info"
          class="rounded-xl font-medium"
        >
          <template #icon>
            <NavIcon class="w-4 h-4" />
          </template>
          <span>{{ t('lookups.title') }}</span>
          <DropdownIcon class="w-3.5 h-3.5 ms-1.5 opacity-75" />
        </n-button>
      </n-dropdown>
    </div>
  </div>
</template>
