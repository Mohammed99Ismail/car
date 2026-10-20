<script setup lang="ts">
import {
  Camera24Regular as CameraIcon,
  Delete24Regular as DeleteIcon,
  CheckmarkCircle24Regular as VerifiedIcon,
  CalendarLtr24Regular as CalendarIcon,
  Mail24Regular as MailIcon,
  Shield24Regular as ShieldIcon,
} from '@vicons/fluent'
import { storeToRefs } from 'pinia'
import { useMessage } from 'naive-ui'

const { t, locale } = useI18n()
const message = useMessage()
const store = useProfileStore()
const { userProfile, isUploadingAvatar, isDeletingAvatar } = storeToRefs(store)

const fileInputRef = ref<HTMLInputElement | null>(null)

function triggerUpload() {
  fileInputRef.value?.click()
}

async function onFileSelected(event: Event) {
  const target = event.target as HTMLInputElement
  const file = target.files?.[0]
  if (!file) return

  const result = await store.uploadAvatar(file)
  if (result.success) {
    message.success(result.message || t('profile.uploadAvatar'))
  } else {
    message.error(result.message || 'Error uploading avatar')
  }
  // reset input
  target.value = ''
}

async function handleDeleteAvatar() {
  const result = await store.deleteAvatar()
  if (result.success) {
    message.success(result.message || t('common.deletedMessage'))
  } else {
    message.error(result.message || 'Error deleting avatar')
  }
}

function formatDate(dateStr?: string) {
  if (!dateStr) return '-'
  try {
    const d = new Date(dateStr)
    return d.toLocaleDateString(locale.value === 'ar' ? 'ar-EG' : 'en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric',
    })
  } catch {
    return dateStr
  }
}
</script>

<template>
  <div class="profile-header-card relative overflow-hidden rounded-2xl p-6 md:p-8 mb-6 border border-slate-200/80 dark:border-slate-700/60 shadow-sm bg-white/80 dark:bg-slate-900/80 backdrop-blur-md">
    <!-- Decorative gradient glow behind avatar -->
    <div class="absolute -top-16 -start-16 w-48 h-48 bg-primary/10 rounded-full blur-3xl pointer-events-none"></div>
    <div class="absolute -bottom-16 -end-16 w-48 h-48 bg-blue-500/10 rounded-full blur-3xl pointer-events-none"></div>

    <div class="relative flex flex-col md:flex-row items-center md:items-start gap-6">
      <!-- Avatar Section with Actions -->
      <div class="flex flex-col items-center gap-2.5">
        <div class="relative group">
          <div
            class="w-28 h-28 rounded-full ring-4 ring-primary/20 dark:ring-primary/30 overflow-hidden shadow-lg flex items-center justify-center text-white text-3xl font-bold select-none"
            style="background: linear-gradient(135deg, #0ea5e9 0%, #2563eb 100%);"
          >
            <img
              v-if="userProfile?.avatarUrl"
              :src="userProfile.avatarUrl"
              :alt="userProfile?.fullName || 'User'"
              class="w-full h-full object-cover"
            />
            <span v-else class="text-white drop-shadow-sm">
              {{ userProfile?.fullName?.charAt(0) || 'U' }}
            </span>
          </div>

          <!-- Camera overlay trigger -->
          <button
            type="button"
            class="absolute inset-0 rounded-full bg-black/40 text-white flex flex-col items-center justify-center opacity-0 group-hover:opacity-100 transition-opacity duration-200 cursor-pointer backdrop-blur-[2px]"
            :disabled="isUploadingAvatar || isDeletingAvatar"
            @click="triggerUpload"
            :title="t('profile.changeAvatar')"
          >
            <n-spin v-if="isUploadingAvatar" size="small" stroke="#fff" />
            <template v-else>
              <n-icon size="24">
                <CameraIcon />
              </n-icon>
              <span class="text-[11px] font-semibold mt-1">{{ t('profile.changeAvatar') }}</span>
            </template>
          </button>

          <input
            ref="fileInputRef"
            type="file"
            accept="image/png, image/jpeg, image/jpg, image/webp"
            class="hidden"
            @change="onFileSelected"
          />
        </div>

        <!-- Avatar Action Buttons -->
        <div class="flex items-center gap-1.5">
          <n-button
            size="tiny"
            secondary
            type="primary"
            round
            :loading="isUploadingAvatar"
            :disabled="isDeletingAvatar"
            @click="triggerUpload"
          >
            <template #icon>
              <n-icon><CameraIcon /></n-icon>
            </template>
            {{ userProfile?.avatarUrl ? t('profile.changeAvatar') : t('profile.uploadAvatar') }}
          </n-button>

          <n-popconfirm
            v-if="userProfile?.avatarUrl"
            :positive-text="t('common.confirm')"
            :negative-text="t('common.cancel')"
            @positive-click="handleDeleteAvatar"
          >
            <template #trigger>
              <n-button
                size="tiny"
                secondary
                type="error"
                round
                :loading="isDeletingAvatar"
                :disabled="isUploadingAvatar"
              >
                <template #icon>
                  <n-icon><DeleteIcon /></n-icon>
                </template>
                {{ t('profile.deleteAvatar') }}
              </n-button>
            </template>
            {{ t('profile.confirmDeleteAvatar') }}
          </n-popconfirm>
        </div>
      </div>

      <!-- User Info & Meta Badges -->
      <div class="flex-1 text-center md:text-start">
        <div class="flex flex-wrap items-center justify-center md:justify-start gap-3 mb-2">
          <h2 class="text-2xl font-bold text-slate-800 dark:text-white tracking-tight">
            {{ userProfile?.fullName || 'المستخدم' }}
          </h2>
          <n-tag v-if="userProfile?.isActive" type="success" size="small" round :bordered="false" class="font-medium">
            <template #icon>
              <n-icon><VerifiedIcon /></n-icon>
            </template>
            {{ t('profile.active') }}
          </n-tag>
          <n-tag v-else type="warning" size="small" round :bordered="false">
            {{ t('profile.inactive') }}
          </n-tag>
        </div>

        <!-- Email & Join Date -->
        <div class="flex flex-wrap items-center justify-center md:justify-start gap-4 text-sm text-slate-500 dark:text-slate-400 mb-4">
          <div class="flex items-center gap-1.5">
            <n-icon size="16" class="text-primary"><MailIcon /></n-icon>
            <span>{{ userProfile?.email || '-' }}</span>
          </div>
          <div class="flex items-center gap-1.5">
            <n-icon size="16" class="text-slate-400"><CalendarIcon /></n-icon>
            <span>{{ t('profile.joinedDate') }}: {{ formatDate(userProfile?.createdDate) }}</span>
          </div>
        </div>

        <!-- Role Badges -->
        <div class="flex flex-wrap items-center justify-center md:justify-start gap-2">
          <span class="text-xs font-semibold text-slate-400 dark:text-slate-500 flex items-center gap-1">
            <n-icon><ShieldIcon /></n-icon>
            {{ t('profile.roles') }}:
          </span>
          <template v-if="userProfile?.roles && userProfile.roles.length > 0">
            <n-tag
              v-for="role in userProfile.roles"
              :key="role"
              type="info"
              size="small"
              round
              :bordered="false"
              class="font-medium"
            >
              {{ role }}
            </n-tag>
          </template>
          <span v-else class="text-xs text-slate-400">-</span>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.profile-header-card {
  transition: all 0.3s ease;
}
</style>
