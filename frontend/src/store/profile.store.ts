import { acceptHMRUpdate, defineStore } from 'pinia'
import { isApiSuccess } from '~/models/ApiResponse'
import type {
  ChangePasswordModel,
  ProfileSettings,
  UpdateProfileModel,
  UserAttachment,
  UserProfile,
} from '~/models/Profile'
import { useProfileService } from '~/services/profile.service'
import { useAccountStore } from '~/store/account.store'

export const useProfileStore = defineStore('Profile', () => {
  const profileService = useProfileService()
  const accountStore = useAccountStore()

  const userProfile = ref<UserProfile | null>(null)
  const userSettings = ref<ProfileSettings>({
    emailNotification: true,
    pushNotification: true,
    monthlyReport: true,
  })
  const isLoading = ref(false)
  const isSaving = ref(false)
  const isChangingPassword = ref(false)
  const isUploadingAvatar = ref(false)
  const isDeletingAvatar = ref(false)
  const isUploadingAttachment = ref(false)

  async function loadUserProfile(): Promise<boolean> {
    isLoading.value = true
    try {
      const res = await profileService.getProfile()
      if (isApiSuccess(res) && res.data) {
        userProfile.value = res.data
        if (accountStore.user) {
          accountStore.user.fullName = res.data.fullName
          accountStore.user.email = res.data.email
        }
        return true
      }
      return false
    } catch (err) {
      console.error('Error loading user profile', err)
      return false
    } finally {
      isLoading.value = false
    }
  }

  async function updateProfile(model: UpdateProfileModel): Promise<{ success: boolean; message?: string }> {
    isSaving.value = true
    try {
      const res = await profileService.updateProfile(model)
      if (isApiSuccess(res) && res.data) {
        userProfile.value = res.data
        if (accountStore.user) {
          accountStore.user.fullName = res.data.fullName
        }
        return { success: true, message: res.message || 'تم التحديث بنجاح' }
      }
      return { success: false, message: res.message || 'فشل التحديث' }
    } catch (err: any) {
      const msg = err?.response?.data?.message || err?.message || 'فشل التحديث'
      return { success: false, message: msg }
    } finally {
      isSaving.value = false
    }
  }

  async function changePassword(model: ChangePasswordModel): Promise<{ success: boolean; message?: string }> {
    isChangingPassword.value = true
    try {
      const res = await profileService.changePassword(model)
      if (isApiSuccess(res)) {
        return { success: true, message: res.message || 'تم تغيير كلمة المرور بنجاح' }
      }
      return { success: false, message: res.message || 'فشل تغيير كلمة المرور' }
    } catch (err: any) {
      const msg = err?.response?.data?.message || err?.message || 'فشل تغيير كلمة المرور'
      return { success: false, message: msg }
    } finally {
      isChangingPassword.value = false
    }
  }

  async function uploadAvatar(file: File): Promise<{ success: boolean; url?: string; message?: string }> {
    isUploadingAvatar.value = true
    try {
      const res = await profileService.uploadAvatar(file)
      if (isApiSuccess(res) && res.data) {
        if (userProfile.value) {
          userProfile.value.avatarUrl = res.data
        }
        return { success: true, url: res.data, message: res.message || 'تم رفع الصورة بنجاح' }
      }
      return { success: false, message: res.message || 'فشل رفع الصورة' }
    } catch (err: any) {
      const msg = err?.response?.data?.message || err?.message || 'فشل رفع الصورة'
      return { success: false, message: msg }
    } finally {
      isUploadingAvatar.value = false
    }
  }

  async function deleteAvatar(): Promise<{ success: boolean; message?: string }> {
    isDeletingAvatar.value = true
    try {
      const res = await profileService.deleteAvatar()
      if (isApiSuccess(res)) {
        if (userProfile.value) {
          userProfile.value.avatarUrl = undefined
        }
        return { success: true, message: res.message || 'تم حذف الصورة بنجاح' }
      }
      return { success: false, message: res.message || 'فشل حذف الصورة' }
    } catch (err: any) {
      const msg = err?.response?.data?.message || err?.message || 'فشل حذف الصورة'
      return { success: false, message: msg }
    } finally {
      isDeletingAvatar.value = false
    }
  }

  async function uploadAttachment(
    file: File,
    documentType?: string,
  ): Promise<{ success: boolean; attachment?: UserAttachment; message?: string }> {
    isUploadingAttachment.value = true
    try {
      const res = await profileService.uploadAttachment(file, documentType)
      if (isApiSuccess(res) && res.data) {
        if (userProfile.value) {
          userProfile.value.attachments = [res.data, ...(userProfile.value.attachments || [])]
        }
        return { success: true, attachment: res.data, message: res.message || 'تم رفع الملف بنجاح' }
      }
      return { success: false, message: res.message || 'فشل رفع الملف' }
    } catch (err: any) {
      const msg = err?.response?.data?.message || err?.message || 'فشل رفع الملف'
      return { success: false, message: msg }
    } finally {
      isUploadingAttachment.value = false
    }
  }

  async function deleteAttachment(id: number): Promise<{ success: boolean; message?: string }> {
    try {
      const res = await profileService.deleteAttachment(id)
      if (isApiSuccess(res)) {
        if (userProfile.value) {
          userProfile.value.attachments = (userProfile.value.attachments || []).filter(a => a.id !== id)
        }
        return { success: true, message: res.message || 'تم حذف الملف' }
      }
      return { success: false, message: res.message || 'فشل حذف الملف' }
    } catch (err: any) {
      const msg = err?.response?.data?.message || err?.message || 'فشل حذف الملف'
      return { success: false, message: msg }
    }
  }

  // Backward compatibility
  async function loadSettings() {
    try {
      const settings = await profileService.getUserSettings()
      if (settings) {
        userSettings.value = settings
      }
    } catch {
      // ignore
    }
  }

  return {
    userProfile,
    userSettings,
    isLoading,
    isSaving,
    isChangingPassword,
    isUploadingAvatar,
    isDeletingAvatar,
    isUploadingAttachment,
    loadUserProfile,
    updateProfile,
    changePassword,
    uploadAvatar,
    deleteAvatar,
    uploadAttachment,
    deleteAttachment,
    loadSettings,
  }
})

if (import.meta.hot)
  import.meta.hot.accept(acceptHMRUpdate(useProfileStore, import.meta.hot))
