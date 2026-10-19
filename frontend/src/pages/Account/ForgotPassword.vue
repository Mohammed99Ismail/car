<script setup lang="ts">
import type { FormInst, FormRules } from 'naive-ui/es/form/src/interface'
import { storeToRefs } from 'pinia'

const { t } = useI18n()
const accountStore = useAccountStore()
const { isLoading } = storeToRefs(accountStore)
const forgetInfo = ref<{ email: string }>({ email: '' })
const emailSent = ref(false)
const router = useRouter()
const formRef = ref<FormInst | null>(null)

async function handleForgotPassword() {
  formRef.value?.validate(async (errors: any) => {
    if (!errors) {
      const success = await accountStore.forgotPassword(forgetInfo.value)
      if (success) {
        emailSent.value = true
        useNotifyStore().success(t('forgot.sendEmailSuccess'))
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
}
</script>

<route lang="yaml">
meta:
  title: forgetPassword
  layout: auth
  allowAnonymous: true
</route>

<template>
  <div class="bg flex justify-center items-center min-h-screen py-10">
    <div class="login-box w-full px-3 md:px-0">
      <div class="md:shadow-xl bg-white dark:bg-slate-800 rounded-xl p-8 border border-slate-100 dark:border-slate-700">
        <div class="text-center mb-6">
          <h2 class="text-2xl font-bold text-slate-800 dark:text-white">
            {{ t('forgot.title') }}
          </h2>
          <p class="text-sm text-slate-500 dark:text-slate-400 mt-1">
            {{ t('forgot.subtitle') }}
          </p>
        </div>

        <n-alert v-if="emailSent" type="success" class="mb-4">
          {{ t('forgot.sendEmailSuccess') }}
        </n-alert>

        <n-form v-if="!emailSent" ref="formRef" :model="forgetInfo" :rules="rules" @submit.prevent="handleForgotPassword">
          <n-form-item class="mb-3" path="email" :label="t('forgot.email')">
            <n-input
              v-model:value="forgetInfo.email"
              autofocus
              :placeholder="t('forgot.email')"
            />
          </n-form-item>

          <n-button
            attr-type="submit"
            size="large"
            block
            type="primary"
            :loading="isLoading"
          >
            {{ t('forgot.button') }}
          </n-button>
        </n-form>

        <div class="flex items-center justify-between mt-6 pt-4 border-t border-slate-100 dark:border-slate-700 text-sm">
          <RouterLink to="/account/login" class="text-primary hover:underline font-semibold">
            {{ t('forgot.login') }}
          </RouterLink>
          <RouterLink to="/account/register" class="text-slate-500 hover:underline">
            {{ t('forgot.register') }}
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
