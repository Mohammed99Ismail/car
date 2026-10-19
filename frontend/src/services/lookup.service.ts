import { useApi } from '~/composables/useApi'
import type { ApiResponse, BilingualLookupDto, LookupItemDto } from '~/models/ApiResponse'

export const useLookupService = () => {
  const api = useApi('lookups')

  async function getMakes(search?: string): Promise<ApiResponse<LookupItemDto[]>> {
    return api.get<ApiResponse<LookupItemDto[]>>('makes', search ? { search } : undefined)
  }

  async function getModels(makeId: number, search?: string): Promise<ApiResponse<LookupItemDto[]>> {
    return api.get<ApiResponse<LookupItemDto[]>>(`makes/${makeId}/models`, search ? { search } : undefined)
  }

  async function getYears(): Promise<ApiResponse<number[]>> {
    return api.get<ApiResponse<number[]>>('years')
  }

  async function getBranches(search?: string): Promise<ApiResponse<LookupItemDto[]>> {
    return api.get<ApiResponse<LookupItemDto[]>>('branches', search ? { search } : undefined)
  }

  async function getRoles(): Promise<ApiResponse<BilingualLookupDto[]>> {
    return api.get<ApiResponse<BilingualLookupDto[]>>('roles')
  }

  return { getMakes, getModels, getYears, getBranches, getRoles }
}
