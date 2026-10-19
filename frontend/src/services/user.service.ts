import { useApi } from '~/composables/useApi'
import type { ApiResponse, BackendPagedList, PagedList, PagedQuery } from '~/models/ApiResponse'
import { toBackendQuery, toPagedResponse } from '~/models/ApiResponse'
import type { CreateUserDto, UpdateUserRolesDto, UserDetailDto, UserDto } from '~/models/User'

export type UserQueryParameters = PagedQuery

export const useUserService = () => {
  const api = useApi('users')

  async function getPaged(params: UserQueryParameters): Promise<ApiResponse<PagedList<UserDto>>> {
    const res = await api.get<ApiResponse<BackendPagedList<UserDto>>>('', toBackendQuery(params))
    return toPagedResponse(res)
  }

  async function getById(id: number): Promise<ApiResponse<UserDetailDto>> {
    return api.get<ApiResponse<UserDetailDto>>(String(id))
  }

  async function create(dto: CreateUserDto): Promise<ApiResponse<UserDetailDto>> {
    return api.post<ApiResponse<UserDetailDto>>('', dto)
  }

  async function setStatus(id: number, isActive: boolean): Promise<ApiResponse<unknown>> {
    return api.put<ApiResponse<unknown>>(`${id}/status`, { isActive })
  }

  async function updateRoles(id: number, dto: UpdateUserRolesDto): Promise<ApiResponse<unknown>> {
    return api.put<ApiResponse<unknown>>(`${id}/roles`, dto)
  }

  return { getPaged, getById, create, setStatus, updateRoles }
}
