import axios from 'axios'
import tokenService from '~/common/api/token.service'
import { useApi } from '~/composables/useApi'
import type { ApiResponse } from '~/models/ApiResponse'
import type {
  ChangePasswordModel,
  ProfileSettings,
  UpdateProfileModel,
  UserAttachment,
  UserProfile,
} from '~/models/Profile'

export const useProfileService = () => {
  const api = useApi('profile')

  async function getProfile(): Promise<ApiResponse<UserProfile>> {
    return api.get<ApiResponse<UserProfile>>('')
  }

  async function updateProfile(model: UpdateProfileModel): Promise<ApiResponse<UserProfile>> {
    return api.put<ApiResponse<UserProfile>>('', model)
  }

  async function changePassword(model: ChangePasswordModel): Promise<ApiResponse<unknown>> {
    return api.post<ApiResponse<unknown>>('change-password', {
      currentPassword: model.currentPassword,
      newPassword: model.newPassword,
    })
  }

  async function uploadAvatar(file: File): Promise<ApiResponse<string>> {
    const formData = new FormData()
    formData.append('file', file)
    const token = tokenService.getLocalAccessToken()
    const response = await axios.post<ApiResponse<string>>('/api/profile/avatar', formData, {
      headers: {
        'Content-Type': 'multipart/form-data',
        ...(token ? { Authorization: `Bearer ${token}` } : {}),
      },
    })
    return response.data
  }

  async function deleteAvatar(): Promise<ApiResponse<unknown>> {
    return api.delete<ApiResponse<unknown>>('avatar')
  }

  async function uploadAttachment(file: File, documentType?: string): Promise<ApiResponse<UserAttachment>> {
    const formData = new FormData()
    formData.append('file', file)
    if (documentType) {
      formData.append('documentType', documentType)
    }
    const token = tokenService.getLocalAccessToken()
    const response = await axios.post<ApiResponse<UserAttachment>>('/api/profile/attachments', formData, {
      headers: {
        'Content-Type': 'multipart/form-data',
        ...(token ? { Authorization: `Bearer ${token}` } : {}),
      },
    })
    return response.data
  }

  async function deleteAttachment(attachmentId: number): Promise<ApiResponse<unknown>> {
    return api.delete<ApiResponse<unknown>>(`attachments/${attachmentId}`)
  }

  // Backward compatibility
  async function getUserProfile(): Promise<UserProfile> {
    const res = await getProfile()
    return res.data
  }

  async function getUserSettings(): Promise<ProfileSettings> {
    const res = await api.get<ApiResponse<ProfileSettings>>('user-settings')
    return res.data
  }

  return {
    getProfile,
    updateProfile,
    changePassword,
    uploadAvatar,
    deleteAvatar,
    uploadAttachment,
    deleteAttachment,
    getUserProfile,
    getUserSettings,
  }
}
