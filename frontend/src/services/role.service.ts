import { useApi } from '~/composables/useApi'
import type { ApiResponse, BackendPagedList, PagedList, PagedQuery } from '~/models/ApiResponse'
import { toBackendQuery, toPagedResponse } from '~/models/ApiResponse'
import type { ModulePermissionsDto, RoleDetailDto, RoleDto, SaveRoleDto } from '~/models/Role'

export const useRoleService = () => {
  const rolesApi = useApi('roles')
  const permissionsApi = useApi('permissions')

  async function getPaged(params: PagedQuery = { pageIndex: 1, pageSize: 100 }): Promise<ApiResponse<PagedList<RoleDto>>> {
    const res = await rolesApi.get<ApiResponse<BackendPagedList<RoleDto>>>('', toBackendQuery(params))
    return toPagedResponse(res)
  }

  async function getById(id: number): Promise<ApiResponse<RoleDetailDto>> {
    return rolesApi.get<ApiResponse<RoleDetailDto>>(String(id))
  }

  async function create(dto: SaveRoleDto): Promise<ApiResponse<RoleDetailDto>> {
    return rolesApi.post<ApiResponse<RoleDetailDto>>('', dto)
  }

  async function update(id: number, dto: SaveRoleDto): Promise<ApiResponse<RoleDetailDto>> {
    return rolesApi.put<ApiResponse<RoleDetailDto>>(String(id), dto)
  }

  async function remove(id: number): Promise<ApiResponse<unknown>> {
    return rolesApi.delete<ApiResponse<unknown>>(String(id))
  }

  async function getPermissions(): Promise<ApiResponse<ModulePermissionsDto[]>> {
    return permissionsApi.get<ApiResponse<ModulePermissionsDto[]>>('')
  }

  return { getPaged, getById, create, update, remove, getPermissions }
}
