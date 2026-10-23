import { useApi } from '~/composables/useApi'
import type { ApiResponse } from '~/models/ApiResponse'
import type { SystemSettingDto, UpdateSystemSettingRequest } from '~/models/Settings'

export const useSettingsService = () => {
  const api = useApi('settings')

  async function getAll(): Promise<ApiResponse<SystemSettingDto[]>> {
    return api.get<ApiResponse<SystemSettingDto[]>>('')
  }

  async function getByKey(key: string): Promise<ApiResponse<SystemSettingDto>> {
    return api.get<ApiResponse<SystemSettingDto>>(key)
  }

  async function update(key: string, dto: UpdateSystemSettingRequest): Promise<ApiResponse<SystemSettingDto>> {
    return api.put<ApiResponse<SystemSettingDto>>(key, dto)
  }

  return {
    getAll,
    getByKey,
    update,
  }
}
