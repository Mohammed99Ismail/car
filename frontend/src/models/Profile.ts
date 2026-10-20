export interface UserAttachment {
  id: number
  fileName: string
  filePath: string
  contentType: string
  fileSizeBytes: number
  documentType?: string
  createdDate: string
}

export interface UserProfile {
  id: number
  fullName: string
  email: string
  phoneNumber?: string
  avatarUrl?: string
  isActive: boolean
  createdDate: string
  roles: string[]
  permissions: string[]
  attachments: UserAttachment[]
}

export interface UpdateProfileModel {
  fullName: string
  phoneNumber?: string
}

export interface ChangePasswordModel {
  currentPassword: string
  newPassword: string
  confirmPassword?: string
}

// Backward-compatibility aliases
export type Profile = UserProfile
export interface ProfileSettings {
  emailNotification: boolean
  pushNotification: boolean
  monthlyReport: boolean
}
