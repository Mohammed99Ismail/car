<script setup lang="ts">
import type { FormInst, FormRules } from 'naive-ui/es/form/src/interface'
import { storeToRefs } from 'pinia'
import type { LoginViewModel } from '~/models/Account'

const { t } = useI18n()
const accountStore = useAccountStore()
const { isLoading } = storeToRefs(accountStore)
const loginInfo = ref<LoginViewModel>({
  email: 'admin@carshowroom.com',
  password: 'Admin@123456',
})
const loginFailed = ref(false)
const router = useRouter()
const formRef = ref<FormInst | null>(null)

async function login() {
  try {
    if (formRef.value) {
      await formRef.value.validate()
    }
    const loginSucceed = await accountStore.login(loginInfo.value)
    if (loginSucceed) {
      useNotifyStore().success(t('login.successMessage'))
      router.push('/')
    } else {
      setLoginFailed()
    }
  } catch (err) {
    console.error('Validation or login error:', err)
    setLoginFailed()
  }
}

function setLoginFailed() {
  loginFailed.value = true
  setTimeout(() => {
    loginFailed.value = false
  }, 2000)
}

const rules: FormRules = {
  email: [
    {
      required: true,
      trigger: ['blur', 'change'],
      message: t('login.validations.userNameRequired'),
    },
  ],
  password: [
    {
      required: true,
      trigger: ['blur', 'change'],
      message: t('login.validations.passwordRequired'),
    },
  ],
}
</script>

<route lang="yaml">
meta:
  title: login
  layout: auth
  allowAnonymous: true
</route>

<template>
  <div class="bg flex justify-center items-center min-h-screen py-10">
    <div class="login-box w-full px-3 md:px-0">
      <div class="md:shadow-xl bg-white dark:bg-slate-800 rounded-xl p-8 border border-slate-100 dark:border-slate-700" :class="{ failed: loginFailed }">
        <div class="text-center mb-6">
          <div class="inline-flex items-center justify-center w-14 h-14 rounded-2xl bg-primary/10 text-primary mb-3">
            <svg class="w-8 h-8" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 19H5a2 2 0 01-2-2V7a2 2 0 012-2h14a2 2 0 012 2v10a2 2 0 01-2 2zM16 11V7a4 4 0 00-8 0v4M12 14v3" />
            </svg>
          </div>
          <h2 class="text-2xl font-bold text-slate-800 dark:text-white">
            {{ t('login.title') }}
          </h2>
          <p class="text-sm text-slate-500 dark:text-slate-400 mt-1">
            {{ t('login.message') }}
          </p>
        </div>

        <n-form ref="formRef" :model="loginInfo" :rules="rules" @submit.prevent="login">
          <n-form-item class="mb-3" path="email" :label="t('login.username')">
            <n-input
              id="login-email"
              v-model:value="loginInfo.email"
              autofocus
              :placeholder="t('login.username')"
            />
          </n-form-item>

          <n-form-item class="mb-3" path="password" :label="t('login.password')">
            <n-input
              id="login-password"
              v-model:value="loginInfo.password"
              type="password"
              show-password-on="mousedown"
              :placeholder="t('login.password')"
            />
          </n-form-item>

          <div class="flex items-center justify-between mb-5 text-sm">
            <RouterLink
              to="/Account/ForgotPassword"
              class="no-underline text-primary hover:underline cursor-pointer"
            >
              {{ t('login.forgetPassword') }}
            </RouterLink>
          </div>

          <n-button
            attr-type="submit"
            size="large"
            block
            type="primary"
            :loading="isLoading"
            @click="login"
          >
            {{ t('login.loginButton') }}
          </n-button>
        </n-form>

        <div class="text-center pt-6 text-sm text-slate-500 border-t border-slate-100 dark:border-slate-700 mt-6">
          <span>{{ t('login.haveNotAccount') }}</span>
          <RouterLink to="/Account/Register" class="no-underline mx-1 text-primary font-semibold hover:underline">
            {{ t('login.createAccount') }}
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

  .failed {
    animation: shake 0.82s cubic-bezier(0.36, 0.07, 0.19, 0.97) both;
    transform: translate3d(0, 0, 0);
  }
}

@keyframes shake {
  10%, 90% { transform: translate3d(-1px, 0, 0); }
  20%, 80% { transform: translate3d(2px, 0, 0); }
  30%, 50%, 70% { transform: translate3d(-4px, 0, 0); }
  40%, 60% { transform: translate3d(4px, 0, 0); }
}
</style>
