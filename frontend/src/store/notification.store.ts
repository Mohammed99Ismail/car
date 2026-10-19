import { acceptHMRUpdate, defineStore } from 'pinia'
import type { Notification } from '~/models/Notification'
import { useNotificationsService } from '~/services/notification.service'

export const useNotificationStore = defineStore('Notification', () => {
  const notificationService = useNotificationsService()
  const notifications = ref<Notification[]>([])
  const isLoading = ref(false)

  async function getNotifications() {
    isLoading.value = true
    try {
      const response = await notificationService.getNotificationsList()
      notifications.value = response?.items ?? []
    }
    catch {
      notifications.value = []
    }
    finally {
      isLoading.value = false
    }
  }

  function clearAll() {
    notifications.value = []
  }

  return {
    notifications,
    isLoading,
    getNotifications,
    clearAll,
  }
})

if (import.meta.hot) {
  import.meta.hot.accept(acceptHMRUpdate(useNotificationStore, import.meta.hot))
}
