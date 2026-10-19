<script setup lang="ts">
import type { FormInst, FormRules } from 'naive-ui/es/form/src/interface'
import { storeToRefs } from 'pinia'
import type { ResetPasswordViewModel } from '~/models/Account'

const { t } = useI18n()
const route = useRoute()
const router = useRouter()
const accountStore = useAccountStore()
const { isLoading } = storeToRefs(accountStore)

const resetInfo = ref<ResetPasswordViewModel>({
  email: (route.query.email as string) || '',
  token: (route.query.token as string) || '',
  newPassword: '',
  confirmNewPassword: '',
})

const formRef = ref<FormInst | null>(null)

async function handleResetPassword() {
  formRef.value?.validate(async (errors: any) => {
    if (!errors) {
      const success = await accountStore.resetPassword(resetInfo.value)
      if (success) {
        useNotifyStore().success(t('forgot.resetSuccess'))
        setTimeout(() => router.push('/account/login'), 800)
      }
    }
  })
}

const rules: FormRules = {
  email: [
    {
      required: true,
      type: 'email',
      trigger: ['blur', 'change'],
      message: t('forgot.emailValidation'),
    },
  ],
  token: [
    {
      required: true,
      trigger: ['blur', 'change'],
      message: 'رمز التحقق مطلوب',
    },
  ],
  newPassword: [
    {
      required: true,
      min: 6,
      trigger: ['blur', 'change'],
      message: t('register.validations.passwordRequired'),
    },
  ],
  confirmNewPassword: [
    {
      required: true,
      validator: (_rule, value) => {
        if (!value) return new Error(t('register.validations.passwordRequired'))
        if (value !== resetInfo.value.newPassword) {
          return new Error(t('register.validations.confirmPasswordMismatch'))
        }
        return true
      },
      trigger: ['blur', 'change'],
    },
  ],
}
</script>

<route lang="yaml">
meta:
  title: resetPassword
  layout: auth
  allowAnonymous: true
</route>

<template>
  <div class="bg flex justify-center items-center min-h-screen py-10">
    <div class="login-box w-full px-3 md:px-0">
      <div class="md:shadow-xl bg-white dark:bg-slate-800 rounded-xl p-8 border border-slate-100 dark:border-slate-700">
        <div class="text-center mb-6">
          <h2 class="text-2xl font-bold text-slate-800 dark:text-white">
            {{ t('forgot.resetPassword') }}
          </h2>
        </div>

        <n-form ref="formRef" :model="resetInfo" :rules="rules" @submit.prevent="handleResetPassword">
          <n-form-item class="mb-3" path="email" :label="t('forgot.email')">
            <n-input
              v-model:value="resetInfo.email"
              :placeholder="t('forgot.email')"
            />
          </n-form-item>

          <n-form-item class="mb-3" path="token" label="رمز التحقق (Token)">
            <n-input
              v-model:value="resetInfo.token"
              placeholder="رمز التحقق من الرابط"
            />
          </n-form-item>

          <n-form-item class="mb-3" path="newPassword" :label="t('forgot.newPassword')">
            <n-input
              v-model:value="resetInfo.newPassword"
              type="password"
              show-password-on="mousedown"
              :placeholder="t('forgot.newPassword')"
            />
          </n-form-item>

          <n-form-item class="mb-4" path="confirmNewPassword" :label="t('register.confirmPassword')">
            <n-input
              v-model:value="resetInfo.confirmNewPassword"
              type="password"
              show-password-on="mousedown"
              :placeholder="t('register.confirmPassword')"
            />
          </n-form-item>

          <n-button
            attr-type="submit"
            size="large"
            block
            type="primary"
            :loading="isLoading"
          >
            {{ t('forgot.resetButton') }}
          </n-button>
        </n-form>

        <div class="text-center mt-6 pt-4 border-t border-slate-100 dark:border-slate-700 text-sm">
          <RouterLink to="/account/login" class="text-primary hover:underline font-semibold">
            {{ t('forgot.login') }}
          </RouterLink>
        </div>
      </div>

      <div class="mt-4 flex justify-between items-center px-2">
        <LanguageSelect />
        <ThemeSwitch />
      </div>
    </div>
  </div>
</template>

<style lang="scss">
.login-box {
  max-width: 420px;
  position: relative;
  z-index: 2;
}
</style>
