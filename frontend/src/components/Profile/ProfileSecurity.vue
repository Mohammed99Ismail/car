<script lang="ts" setup>
import {
  LockClosed24Regular as LockIcon,
  Key24Regular as KeyIcon,
  ShieldCheckmark24Regular as SecurityIcon,
} from '@vicons/fluent'
import { storeToRefs } from 'pinia'
import { useMessage } from 'naive-ui'

const { t } = useI18n()
const message = useMessage()
const store = useProfileStore()
const { isChangingPassword } = storeToRefs(store)

const formModel = reactive({
  currentPassword: '',
  newPassword: '',
  confirmPassword: '',
})

async function handleChangePassword() {
  if (!formModel.currentPassword) {
    message.warning(t('profile.oldPassword') + ' ' + (t('users.validation.passwordRequired') || 'is required'))
    return
  }

  if (!formModel.newPassword) {
    message.warning(t('profile.newPassword') + ' ' + (t('users.validation.passwordRequired') || 'is required'))
    return
  }

  if (formModel.newPassword.length < 6) {
    message.warning(t('profile.passwordHint'))
    return
  }

  if (formModel.newPassword !== formModel.confirmPassword) {
    message.warning(t('profile.passwordMismatch'))
    return
  }

  const result = await store.changePassword({
    currentPassword: formModel.currentPassword,
    newPassword: formModel.newPassword,
  })

  if (result.success) {
    message.success(result.message || t('profile.changePasswordBtn'))
    formModel.currentPassword = ''
    formModel.newPassword = ''
    formModel.confirmPassword = ''
  } else {
    message.error(result.message || 'Error updating password')
  }
}
</script>

<template>
  <div class="py-4 max-w-2xl mx-auto">
    <div class="bg-white dark:bg-slate-900 border border-slate-200/80 dark:border-slate-800 rounded-2xl p-6 md:p-8 shadow-sm">
      <div class="flex items-center gap-3 mb-6">
        <div class="w-10 h-10 rounded-xl bg-primary/10 text-primary flex items-center justify-center">
          <n-icon size="22"><SecurityIcon /></n-icon>
        </div>
        <div>
          <h3 class="text-lg font-bold text-slate-800 dark:text-white">
            {{ t('profile.security') }}
          </h3>
          <p class="text-xs text-slate-500 dark:text-slate-400">
            {{ t('profile.passwordHint') }}
          </p>
        </div>
      </div>

      <n-form size="large" @submit.prevent="handleChangePassword">
        <n-grid :span="24" :y-gap="18">
          <!-- Current Password -->
          <n-form-item-gi :span="24" path="currentPassword" :label="t('profile.oldPassword')">
            <n-input
              v-model:value="formModel.currentPassword"
              type="password"
              show-password-on="click"
              :placeholder="t('profile.oldPassword')"
            >
              <template #prefix>
                <n-icon class="text-slate-400"><KeyIcon /></n-icon>
              </template>
            </n-input>
          </n-form-item-gi>

          <!-- New Password -->
          <n-form-item-gi :span="24" path="newPassword" :label="t('profile.newPassword')">
            <n-input
              v-model:value="formModel.newPassword"
              type="password"
              show-password-on="click"
              :placeholder="t('profile.newPassword')"
            >
              <template #prefix>
                <n-icon class="text-slate-400"><LockIcon /></n-icon>
              </template>
            </n-input>
          </n-form-item-gi>

          <!-- Confirm New Password -->
          <n-form-item-gi :span="24" path="confirmPassword" :label="t('profile.confirmPassword')">
            <n-input
              v-model:value="formModel.confirmPassword"
              type="password"
              show-password-on="click"
              :placeholder="t('profile.confirmPassword')"
            >
              <template #prefix>
                <n-icon class="text-slate-400"><LockIcon /></n-icon>
              </template>
            </n-input>
          </n-form-item-gi>

          <!-- Submit Button -->
          <n-gi :span="24">
            <div class="flex justify-end pt-4 border-t border-slate-100 dark:border-slate-800">
              <n-button
                attr-type="submit"
                size="medium"
                type="primary"
                :loading="isChangingPassword"
              >
                <template #icon>
                  <n-icon><SecurityIcon /></n-icon>
                </template>
                {{ t('profile.changePasswordBtn') }}
              </n-button>
            </div>
          </n-gi>
        </n-grid>
      </n-form>
    </div>
  </div>
</template>
