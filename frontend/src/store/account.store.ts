import { acceptHMRUpdate, defineStore } from 'pinia'
import tokenService from '~/common/api/token.service'
import type {
  AccountUser,
  ForgotPasswordViewModel,
  LoginViewModel,
  RegisterViewModel,
  ResetPasswordViewModel,
} from '~/models/Account'
import { ResultStatusEnum } from '~/models/ApiResponse'
import { useAccountService } from '~/services/account.service'

export const useAccountStore = defineStore(
  'account',
  () => {
    const accountService = useAccountService()
    const user = ref<AccountUser | null>(null)
    const isLoading = ref(false)
    const loginFailed = ref(false)

    // Load from tokenService or localStorage on init
    const initialUser = tokenService.getUser()
    if (initialUser && initialUser.token) {
      user.value = initialUser
    }

    const permissions = computed<string[]>(() => user.value?.permissions ?? [])
    const roles = computed<string[]>(() => user.value?.roles ?? [])

    function hasPermission(permission: string): boolean {
      if (!user.value) return false
      if (
        user.value.roles?.includes('SuperAdmin') ||
        user.value.permissions?.includes('super-admin')
      ) {
        return true
      }

      const p = permission.toLowerCase().trim()
      return (
        user.value.permissions?.some((userPerm) => {
          const up = userPerm.toLowerCase().trim()
          return (
            up === p ||
            up === `view-${p.replace('.view', '')}` ||
            up === `create-${p.replace('.create', '')}` ||
            up === `update-${p.replace('.update', '')}` ||
            up === `delete-${p.replace('.delete', '')}` ||
            up === p.replace('.', '-')
          )
        }) ?? false
      )
    }

    function hasRole(role: string): boolean {
      if (!user.value) return false
      return user.value.roles?.includes(role) ?? false
    }

    async function login(loginInfo: LoginViewModel): Promise<boolean> {
      isLoading.value = true
      loginFailed.value = false
      try {
        const response: any = await accountService.login(loginInfo)
        const isOk =
          response.isSuccess === true ||
          response.resultStatus === 200 ||
          response.status === ResultStatusEnum.Success ||
          response.status === 0

        if (isOk && response.data) {
          const data = response.data
          const u = data.user || data
          user.value = {
            userId: String(u.id || data.userId || ''),
            fullName: u.fullName || data.fullName || 'المستخدم',
            email: u.email || data.email || '',
            roles: u.roles || data.roles || [],
            permissions: u.permissions || data.permissions || [],
            token: data.accessToken,
            refreshToken: data.refreshToken,
            expiresAt: data.expiresAt,
          }
          tokenService.setUser(user.value)
          return true
        }

        loginFailed.value = true
        return false
      } catch (err) {
        loginFailed.value = true
        return false
      } finally {
        isLoading.value = false
      }
    }

    async function register(registerInfo: RegisterViewModel): Promise<boolean> {
      isLoading.value = true
      try {
        const response: any = await accountService.register(registerInfo)
        const isOk =
          response.isSuccess === true ||
          response.resultStatus === 200 ||
          response.status === ResultStatusEnum.Success ||
          response.status === 0

        if (isOk && response.data) {
          const data = response.data
          const u = data.user || data
          user.value = {
            userId: String(u.id || data.userId || ''),
            fullName: u.fullName || data.fullName || 'المستخدم',
            email: u.email || data.email || '',
            roles: u.roles || data.roles || [],
            permissions: u.permissions || data.permissions || [],
            token: data.accessToken,
            refreshToken: data.refreshToken,
            expiresAt: data.expiresAt,
          }
          tokenService.setUser(user.value)
          return true
        }
        return false
      } catch {
        return false
      } finally {
        isLoading.value = false
      }
    }

    async function forgotPassword(model: ForgotPasswordViewModel): Promise<boolean> {
      isLoading.value = true
      try {
        const response = await accountService.forgotPassword(model)
        return response.status === ResultStatusEnum.Success
      } catch {
        return false
      } finally {
        isLoading.value = false
      }
    }

    async function resetPassword(model: ResetPasswordViewModel): Promise<boolean> {
      isLoading.value = true
      try {
        const response = await accountService.resetPassword(model)
        return response.status === ResultStatusEnum.Success
      } catch {
        return false
      } finally {
        isLoading.value = false
      }
    }

    async function logout() {
      const refreshToken = user.value?.refreshToken
      if (refreshToken) {
        try {
          await accountService.logout(refreshToken)
        } catch {
          // ignore logout network errors
        }
      }
      user.value = null
      tokenService.removeUser()
    }

    function isAuthenticated(): boolean {
      return Boolean(user.value?.token)
    }

    return {
      user,
      permissions,
      roles,
      isLoading,
      loginFailed,
      hasPermission,
      hasRole,
      login,
      register,
      forgotPassword,
      resetPassword,
      logout,
      isAuthenticated,
    }
  },
  {
    persist: {
      omit: ['isLoading', 'loginFailed'],
    },
  },
)

if (import.meta.hot)
  import.meta.hot.accept(acceptHMRUpdate(useAccountStore, import.meta.hot))
