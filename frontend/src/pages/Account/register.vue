<script setup lang="ts">
import type { FormInst, FormRules } from 'naive-ui/es/form/src/interface'
import { storeToRefs } from 'pinia'
import type { RegisterViewModel } from '~/models/Account'

const { t } = useI18n()
const accountStore = useAccountStore()
const { isLoading } = storeToRefs(accountStore)
const registerInfo = ref<RegisterViewModel>({
  fullName: '',
  email: '',
  mobile: '',
  password: '',
  confirmPassword: '',
})
const router = useRouter()
const formRef = ref<FormInst | null>(null)

async function register() {
  try {
    if (formRef.value) {
      await formRef.value.validate()
    }
    const registerResult = await accountStore.register(registerInfo.value)
    if (registerResult) {
      useNotifyStore().success(t('register.successMessage'))
      router.push('/')
    }
  } catch (err) {
    console.error('Registration validation or API error:', err)
  }
}

const rules: FormRules = {
  fullName: [
    {
      required: true,
      trigger: ['blur', 'change'],
      message: t('register.validations.fullNameRequired'),
    },
  ],
  email: [
    {
      required: true,
      type: 'email',
      trigger: ['blur', 'change'],
      message: t('register.validations.emailRequired'),
    },
  ],
  password: [
    {
      required: true,
      min: 6,
      trigger: ['blur', 'change'],
      message: t('register.validations.passwordRequired'),
    },
  ],
  confirmPassword: [
    {
      required: true,
      validator: (_rule, value) => {
        if (!value) return new Error(t('register.validations.passwordRequired'))
        if (value !== registerInfo.value.password) {
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
  title: register
  layout: auth
  allowAnonymous: true
</route>

<template>
  <div class="bg flex justify-center items-center min-h-screen py-10">
    <div class="register-box w-full px-3 md:px-0">
      <div class="md:shadow-xl bg-white dark:bg-slate-800 rounded-xl p-8 border border-slate-100 dark:border-slate-700">
        <div class="text-center mb-6">
          <h2 class="text-2xl font-bold text-slate-800 dark:text-white">
            {{ t('register.title') }}
          </h2>
        </div>

        <n-form ref="formRef" :model="registerInfo" :rules="rules" @submit.prevent="register">
          <n-form-item class="mb-2" path="fullName" :label="t('register.fullName')">
            <n-input
              v-model:value="registerInfo.fullName"
              autofocus
              :placeholder="t('register.fullName')"
            />
          </n-form-item>

          <n-form-item class="mb-2" path="email" :label="t('register.email')">
            <n-input
              v-model:value="registerInfo.email"
              :placeholder="t('register.email')"
            />
          </n-form-item>

          <n-form-item class="mb-2" path="mobile" :label="t('register.mobile')">
            <n-input
              v-model:value="registerInfo.mobile"
              :placeholder="t('register.mobile')"
            />
          </n-form-item>

          <n-form-item class="mb-2" path="password" :label="t('register.password')">
            <n-input
              v-model:value="registerInfo.password"
              type="password"
              show-password-on="mousedown"
              :placeholder="t('register.password')"
            />
          </n-form-item>

          <n-form-item class="mb-4" path="confirmPassword" :label="t('register.confirmPassword')">
            <n-input
              v-model:value="registerInfo.confirmPassword"
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
            @click="register"
          >
            {{ t('register.button') }}
          </n-button>
        </n-form>

        <div class="text-center pt-6 text-sm text-slate-500 border-t border-slate-100 dark:border-slate-700 mt-6">
          <span>{{ t('register.haveAccount') }}</span>
          <RouterLink to="/account/login" class="no-underline mx-1 text-primary font-semibold hover:underline">
            {{ t('register.login') }}
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
.register-box {
  max-width: 440px;
  position: relative;
  z-index: 2;
}
</style>
