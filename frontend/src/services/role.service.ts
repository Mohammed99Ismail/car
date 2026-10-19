import { useApi } from '~/composables/useApi'
import type { ApiResponse } from '~/models/ApiResponse'
import type {
  CreateRoleDto,
  PermissionDto,
  RoleDto,
  UpdateRoleDto,
} from '~/models/Role'

export const useRoleService = () => {
  const rolesApi = useApi('roles')
  const permissionsApi = useApi('permissions')

  async function getAll(): Promise<ApiResponse<RoleDto[]>> {
    return rolesApi.get<ApiResponse<RoleDto[]>>('')
  }

  async function getById(id: string): Promise<ApiResponse<RoleDto>> {
    return rolesApi.get<ApiResponse<RoleDto>>(id)
  }

  async function create(dto: CreateRoleDto): Promise<ApiResponse<RoleDto>> {
    return rolesApi.post<ApiResponse<RoleDto>>('', dto)
  }

  async function update(
    id: string,
    dto: UpdateRoleDto,
  ): Promise<ApiResponse<RoleDto>> {
    return rolesApi.put<ApiResponse<RoleDto>>(id, dto)
  }

  async function remove(id: string): Promise<ApiResponse<boolean>> {
    return rolesApi.delete<ApiResponse<boolean>>(id)
  }

  async function getPermissions(): Promise<ApiResponse<PermissionDto[]>> {
    return permissionsApi.get<ApiResponse<PermissionDto[]>>('')
  }

  return {
    getAll,
    getById,
    create,
    update,
    remove,
    getPermissions,
  }
}
