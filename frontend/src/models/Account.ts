export interface LoginViewModel {
  email: string
  password: string
}

export interface RegisterViewModel {
  fullName: string
  email: string
  mobile?: string
  password: string
  confirmPassword: string
}

export interface ForgotPasswordViewModel {
  email: string
}

export interface ResetPasswordViewModel {
  email: string
  token: string
  newPassword: string
  confirmNewPassword: string
}

export interface AuthResponseDto {
  userId: string
  fullName: string
  email: string
  roles: string[]
  permissions: string[]
  accessToken: string
  refreshToken: string
  expiresAt: string
}

export interface AccountUser {
  userId: string
  fullName: string
  email: string
  roles: string[]
  permissions: string[]
  token: string
  refreshToken: string
  expiresAt: string
}
