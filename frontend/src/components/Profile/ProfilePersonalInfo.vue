<script lang="ts" setup>
import {
  Person24Regular as PersonIcon,
  Mail24Regular as MailIcon,
  Call24Regular as PhoneIcon,
  Save24Regular as SaveIcon,
  ArrowReset24Regular as ResetIcon,
  Shield24Regular as ShieldIcon,
} from '@vicons/fluent'
import { storeToRefs } from 'pinia'
import { useMessage } from 'naive-ui'

const { t, locale } = useI18n()
const message = useMessage()
const store = useProfileStore()
const { userProfile, isSaving } = storeToRefs(store)

const formModel = reactive({
  fullName: '',
  phoneNumber: '',
})

// Initialize form from store
watch(
  () => userProfile.value,
  (val) => {
    if (val) {
      formModel.fullName = val.fullName || ''
      formModel.phoneNumber = val.phoneNumber || ''
    }
  },
  { immediate: true },
)

function resetForm() {
  if (userProfile.value) {
    formModel.fullName = userProfile.value.fullName || ''
    formModel.phoneNumber = userProfile.value.phoneNumber || ''
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

async function handleSave() {
  if (!formModel.fullName.trim()) {
    message.warning(t('profile.fullName') + ' ' + (t('users.validation.fullNameRequired') || 'required'))
    return
  }

  const result = await store.updateProfile({
    fullName: formModel.fullName.trim(),
    phoneNumber: formModel.phoneNumber.trim() || undefined,
  })

  if (result.success) {
    message.success(result.message || t('profile.saveChanges'))
  } else {
    message.error(result.message || 'Error updating profile')
  }
}
</script>

<template>
  <div class="py-4 max-w-4xl mx-auto">
    <n-form size="large" @submit.prevent="handleSave">
      <div class="bg-white dark:bg-slate-900 border border-slate-200/80 dark:border-slate-800 rounded-2xl p-6 md:p-8 shadow-sm">
        <h3 class="text-lg font-bold text-slate-800 dark:text-white mb-6 flex items-center gap-2">
          <n-icon size="20" class="text-primary"><PersonIcon /></n-icon>
          {{ t('profile.personalInfo') }}
        </h3>

        <n-grid :span="24" :x-gap="24" :y-gap="18">
          <!-- Full Name -->
          <n-form-item-gi :span="12" path="fullName" :label="t('profile.fullName')">
            <n-input
              v-model:value="formModel.fullName"
              :placeholder="t('profile.fullName')"
              clearable
            >
              <template #prefix>
                <n-icon class="text-slate-400"><PersonIcon /></n-icon>
              </template>
            </n-input>
          </n-form-item-gi>

          <!-- Email (Read-Only) -->
          <n-form-item-gi :span="12" path="email" :label="t('profile.email')">
            <n-input
              :value="userProfile?.email || ''"
              disabled
              :placeholder="t('profile.email')"
            >
              <template #prefix>
                <n-icon class="text-slate-400"><MailIcon /></n-icon>
              </template>
            </n-input>
          </n-form-item-gi>

          <!-- Phone Number -->
          <n-form-item-gi :span="12" path="phoneNumber" :label="t('profile.phone')">
            <n-input
              v-model:value="formModel.phoneNumber"
              :placeholder="t('profile.phone')"
              clearable
            >
              <template #prefix>
                <n-icon class="text-slate-400"><PhoneIcon /></n-icon>
              </template>
            </n-input>
          </n-form-item-gi>

          <!-- Account Status & Join Date -->
          <n-form-item-gi :span="12" :label="t('profile.status')">
            <div class="h-[40px] w-full flex items-center justify-between px-3.5 rounded-lg border border-slate-200/80 dark:border-slate-700/60 bg-slate-50/70 dark:bg-slate-800/40 text-sm">
              <div class="flex items-center gap-2">
                <span
                  class="inline-block w-2.5 h-2.5 rounded-full"
                  :class="userProfile?.isActive ? 'bg-emerald-500' : 'bg-amber-500'"
                ></span>
                <span class="font-medium text-slate-700 dark:text-slate-200">
                  {{ userProfile?.isActive ? t('profile.active') : t('profile.inactive') }}
                </span>
              </div>
              <span class="text-xs text-slate-400">
                {{ t('profile.joinedDate') }}: {{ formatDate(userProfile?.createdDate) }}
              </span>
            </div>
          </n-form-item-gi>

          <!-- Roles and Permissions Info Box -->
          <n-gi :span="24">
            <div class="rounded-xl p-4 bg-slate-50 dark:bg-slate-800/50 border border-slate-200/60 dark:border-slate-800">
              <div class="flex items-center gap-2 mb-2 font-semibold text-sm text-slate-700 dark:text-slate-300">
                <n-icon size="18" class="text-primary"><ShieldIcon /></n-icon>
                <span>{{ t('profile.roles') }}</span>
              </div>
              <div class="flex flex-wrap gap-2">
                <template v-if="userProfile?.roles && userProfile.roles.length > 0">
                  <n-tag
                    v-for="r in userProfile.roles"
                    :key="r"
                    type="info"
                    size="small"
                    round
                  >
                    {{ r }}
                  </n-tag>
                </template>
                <span v-else class="text-xs text-slate-400">-</span>
              </div>
            </div>
          </n-gi>

          <!-- Actions -->
          <n-gi :span="24">
            <div class="flex items-center justify-end gap-3 pt-4 border-t border-slate-100 dark:border-slate-800">
              <n-button
                type="default"
                size="medium"
                :disabled="isSaving"
                @click="resetForm"
              >
                <template #icon>
                  <n-icon><ResetIcon /></n-icon>
                </template>
                {{ t('profile.reset') }}
              </n-button>

              <n-button
                attr-type="submit"
                size="medium"
                type="primary"
                :loading="isSaving"
              >
                <template #icon>
                  <n-icon><SaveIcon /></n-icon>
                </template>
                {{ t('profile.saveChanges') }}
              </n-button>
            </div>
          </n-gi>
        </n-grid>
      </div>
    </n-form>
  </div>
</template>
