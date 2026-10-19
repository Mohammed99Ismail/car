import { useApi } from '~/composables/useApi'
import type { ApiResponse, LookupItemDto } from '~/models/ApiResponse'

export const useLookupService = () => {
  const api = useApi('lookups')

  async function getMakes(search?: string): Promise<ApiResponse<LookupItemDto[]>> {
    return api.get<ApiResponse<LookupItemDto[]>>('makes', { search })
  }

  async function getModels(
    makeId: string,
    search?: string,
  ): Promise<ApiResponse<LookupItemDto[]>> {
    return api.get<ApiResponse<LookupItemDto[]>>(`models/${makeId}`, { search })
  }

  async function getYears(): Promise<ApiResponse<number[]>> {
    return api.get<ApiResponse<number[]>>('years')
  }

  async function getBranches(search?: string): Promise<ApiResponse<LookupItemDto[]>> {
    return api.get<ApiResponse<LookupItemDto[]>>('branches', { search })
  }

  async function getRoles(): Promise<ApiResponse<LookupItemDto[]>> {
    return api.get<ApiResponse<LookupItemDto[]>>('roles')
  }

  return {
    getMakes,
    getModels,
    getYears,
    getBranches,
    getRoles,
  }
}
