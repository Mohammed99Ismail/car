/** Matches backend RoleSummaryDto. */
export interface RoleDto {
  id: number
  nameAr: string
  nameEn: string
  isSystem: boolean
  permissionsCount: number
  usersCount: number
}

/** Matches backend RoleDetailDto. */
export interface RoleDetailDto {
  id: number
  nameAr: string
  nameEn: string
  isSystem: boolean
  permissionIds: number[]
}

/** Matches backend CreateRoleRequest / UpdateRoleRequest. */
export interface SaveRoleDto {
  nameAr: string
  nameEn: string
  permissionIds: number[]
}

/** Matches backend PermissionDto. */
export interface PermissionDto {
  id: number
  key: string
  module: string
  nameAr: string
  nameEn: string
}

/** Matches backend ModulePermissionsDto. */
export interface ModulePermissionsDto {
  module: string
  permissions: PermissionDto[]
}
