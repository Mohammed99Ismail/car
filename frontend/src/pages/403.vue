<route lang="yaml">
meta:
  layout: default
  title: accessDeniedTitle
</route>

<script lang="ts" setup>
import { ShieldDismiss24Regular as ShieldIcon, ArrowLeft24Regular as BackIcon } from '@vicons/fluent'
import { NButton, NIcon } from 'naive-ui'

const { t } = useI18n()
const router = useRouter()
</script>

<template>
  <div class="min-h-[75vh] flex flex-col items-center justify-center p-6 text-center">
    <div class="w-24 h-24 rounded-3xl bg-rose-500/10 dark:bg-rose-500/20 text-rose-500 flex items-center justify-center mb-6 ring-8 ring-rose-500/5 shadow-inner">
      <NIcon size="52">
        <ShieldIcon />
      </NIcon>
    </div>

    <div class="inline-flex items-center gap-2 px-3.5 py-1 rounded-full bg-rose-100 dark:bg-rose-950/60 text-rose-700 dark:text-rose-300 text-xs font-bold mb-3 tracking-wider uppercase">
      <span>403</span>
      <span>•</span>
      <span>{{ t('common.accessDeniedBadge') }}</span>
    </div>

    <h1 class="text-2xl sm:text-3xl font-extrabold text-slate-800 dark:text-slate-100 mb-2">
      {{ t('common.accessDeniedTitle') }}
    </h1>

    <p class="text-slate-500 dark:text-slate-400 max-w-md text-sm sm:text-base mb-8 leading-relaxed">
      {{ t('common.accessDeniedDesc') }}
    </p>

    <div class="flex items-center justify-center gap-3">
      <NButton
        type="primary"
        round
        size="large"
        class="font-semibold !px-8 shadow-lg shadow-indigo-500/20"
        @click="router.push('/')"
      >
        <template #icon>
          <NIcon><BackIcon /></NIcon>
        </template>
        {{ t('common.backToHome') }}
      </NButton>
    </div>
  </div>
</template>
