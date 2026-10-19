import { useApi } from '~/composables/useApi'
import type { ApiResponse, PagedList } from '~/models/ApiResponse'
import type { UpdateUserRolesDto, UserDto } from '~/models/User'

export interface UserQueryParameters {
  pageIndex?: number
  pageSize?: number
  search?: string
  sortBy?: string
  isDescending?: boolean
}

export const useUserService = () => {
  const api = useApi('users')

  async function getPaged(
    params: UserQueryParameters,
  ): Promise<ApiResponse<PagedList<UserDto>>> {
    return api.get<ApiResponse<PagedList<UserDto>>>('', params)
  }

  async function getById(id: string): Promise<ApiResponse<UserDto>> {
    return api.get<ApiResponse<UserDto>>(id)
  }

  async function toggleActive(id: string): Promise<ApiResponse<boolean>> {
    return api.put<ApiResponse<boolean>>(`${id}/toggle-active`)
  }

  async function updateRoles(
    id: string,
    dto: UpdateUserRolesDto,
  ): Promise<ApiResponse<boolean>> {
    return api.put<ApiResponse<boolean>>(`${id}/roles`, dto)
  }

  return {
    getPaged,
    getById,
    toggleActive,
    updateRoles,
  }
}
