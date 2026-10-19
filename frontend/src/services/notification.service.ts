import type { ListResult } from '~/models/ListResult'
import type { Notification } from '~/models/Notification'

export const useNotificationsService = () => {
  async function getNotificationsList(): Promise<ListResult<Notification>> {
    // Return empty list safely without making network requests
    return {
      items: [],
      totalCount: 0,
    }
  }

  return {
    getNotificationsList,
  }
}
