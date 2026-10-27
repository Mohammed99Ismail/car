<script setup lang="ts">
import {
  PanelLeftContract24Regular as CollapseIcon,
  PanelLeftExpand20Regular as ExpandIcon,
  Navigation20Regular as MenuIcon,
} from '@vicons/fluent'

import { storeToRefs } from 'pinia'
import { appConfig } from '~/config/app.config'

const layoutStore = useLayoutStore()
const { collapsed, isRtl, mobileMode } = storeToRefs(layoutStore)
</script>

<template>
  <n-page-header class="px-2 py-3 navbar relative z-100">
    <template #title>
      <div class="flex items-center">
        <div flex w-full justify-start items-center>
          <div v-if="mobileMode" class="w-8 h-8 max-w-[32px] max-h-[32px] flex items-center justify-center shrink-0 overflow-hidden rounded-lg bg-slate-100/80 dark:bg-slate-800/80 p-0.5 border border-slate-200/50 dark:border-slate-700/50 me-2">
            <img :src="appConfig.logoPath" :alt="appConfig.logoAlt" class="w-full h-full max-w-full max-h-full object-contain shrink-0 block">
          </div>

          <n-button mx-2 size="small" quaternary circle :class="{ 'rotate-180': isRtl }"
            @click="layoutStore.toggleSidebar">
            <template #icon>
              <NIcon size="1.2rem">
                <MenuIcon v-if="mobileMode" />
                <ExpandIcon v-else-if="collapsed" />
                <CollapseIcon v-else />
              </NIcon>
            </template>
          </n-button>
        </div>
        <BreadCrumb />
      </div>
    </template>
    <template #extra>
      <div class="flex items-center">
        <ThemeCustomize />
        <ThemeSwitch class="mx-1" />
        <LanguageSelect class="mx-1" />
        <FullScreenButton class="mx-1" />
        <Notifications class="mx-1" />
        <UserProfile class="mx-1" />
      </div>
    </template>
  </n-page-header>
</template>

<style lang="scss">
.navbar {
  border-bottom: solid 1px var(--border-color);
  padding-bottom: 0.4rem;
}

.rtl {
  .n-page-header .n-page-header__title {
    margin-right: 0;
    margin-left: 1.5rem;
  }
}
</style>
