import { useApi } from '~/composables/useApi'
import type {
  AccountUser,
  AuthResponseDto,
  ForgotPasswordViewModel,
  LoginViewModel,
  RegisterViewModel,
  ResetPasswordViewModel,
} from '~/models/Account'
import type { ApiResponse } from '~/models/ApiResponse'

export const useAccountService = () => {
  const api = useApi('auth')

  async function login(loginInfo: LoginViewModel): Promise<ApiResponse<AuthResponseDto>> {
    return api.post<ApiResponse<AuthResponseDto>>('login', loginInfo)
  }

  async function register(
    registerModel: RegisterViewModel,
  ): Promise<ApiResponse<AuthResponseDto>> {
    return api.post<ApiResponse<AuthResponseDto>>('register', registerModel)
  }

  async function forgotPassword(
    model: ForgotPasswordViewModel,
  ): Promise<ApiResponse<boolean>> {
    return api.post<ApiResponse<boolean>>('forgot-password', model)
  }

  async function resetPassword(
    model: ResetPasswordViewModel,
  ): Promise<ApiResponse<boolean>> {
    return api.post<ApiResponse<boolean>>('reset-password', model)
  }

  async function logout(refreshToken: string): Promise<ApiResponse<boolean>> {
    return api.post<ApiResponse<boolean>>('logout', { refreshToken })
  }

  return {
    login,
    register,
    forgotPassword,
    resetPassword,
    logout,
  }
}
