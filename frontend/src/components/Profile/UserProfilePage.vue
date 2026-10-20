<script setup lang="ts">
import {
  Person20Regular as PersonIcon,
  ShieldKeyhole20Regular as SecurityIcon,
  Attach20Regular as AttachIcon,
} from '@vicons/fluent'
import { storeToRefs } from 'pinia'

const store = useProfileStore()
const { isLoading } = storeToRefs(store)
const { t } = useI18n()

onMounted(async () => {
  await store.loadUserProfile()
})
</script>

<template>
  <div class="user-profile-page max-w-5xl mx-auto py-2">
    <!-- Profile Page Title Header -->
    <div class="mb-6">
      <h1 class="text-2xl font-black text-slate-800 dark:text-white tracking-tight">
        {{ t('profile.title') }}
      </h1>
      <p class="text-sm text-slate-500 dark:text-slate-400 mt-1">
        {{ t('profile.subtitle') }}
      </p>
    </div>

    <!-- Loading State -->
    <div v-if="isLoading && !store.userProfile" class="flex justify-center items-center py-24">
      <n-spin size="large" />
    </div>

    <div v-else>
      <!-- Profile Header Summary Card -->
      <ProfileHeader />

      <!-- Tabs Navigation -->
      <n-card class="rounded-2xl border-slate-200/80 dark:border-slate-800 shadow-sm" :bordered="true">
        <n-tabs type="line" animated justify-content="start" size="large">
          <!-- 1. Personal Info -->
          <n-tab-pane name="PersonalInfo">
            <template #tab>
              <div class="flex items-center gap-2">
                <n-icon size="18"><PersonIcon /></n-icon>
                <span>{{ t('profile.personalInfo') }}</span>
              </div>
            </template>
            <ProfilePersonalInfo />
          </n-tab-pane>

          <!-- 2. Security & Password -->
          <n-tab-pane name="Security">
            <template #tab>
              <div class="flex items-center gap-2">
                <n-icon size="18"><SecurityIcon /></n-icon>
                <span>{{ t('profile.security') }}</span>
              </div>
            </template>
            <ProfileSecurity />
          </n-tab-pane>

          <!-- 3. Documents & Attachments -->
          <n-tab-pane name="Attachments">
            <template #tab>
              <div class="flex items-center gap-2">
                <n-icon size="18"><AttachIcon /></n-icon>
                <span>{{ t('profile.attachments') }}</span>
                <span
                  v-if="store.userProfile?.attachments?.length"
                  class="text-[11px] px-1.5 py-0.2 rounded-full bg-primary/10 text-primary font-bold"
                >
                  {{ store.userProfile.attachments.length }}
                </span>
              </div>
            </template>
            <ProfileAttachments />
          </n-tab-pane>
        </n-tabs>
      </n-card>
    </div>
  </div>
</template>

<style scoped>
.user-profile-page :deep(.n-tabs-bar) {
  height: 3px;
  border-radius: 3px;
}
</style>
