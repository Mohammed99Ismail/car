export interface RoleDto {
  id: string
  name: string
  description?: string | null
  isSystemRole: boolean
  permissions: string[]
}

export interface CreateRoleDto {
  name: string
  description?: string | null
  permissions: string[]
}

export interface UpdateRoleDto extends CreateRoleDto {
  id: string
}

export interface PermissionDto {
  id: string
  name: string
  code: string
  module: string
  description?: string | null
}
