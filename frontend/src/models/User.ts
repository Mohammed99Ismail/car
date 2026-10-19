/** Matches backend UserSummaryDto (roles = localized role names). */
export interface UserDto {
  id: number
  fullName: string
  email: string
  isActive: boolean
  createdDate: string
  roles: string[]
}

/** Matches backend UserDetailDto. */
export interface UserDetailDto {
  id: number
  fullName: string
  email: string
  isActive: boolean
  createdDate: string
  roleIds: number[]
}

/** Matches backend CreateUserRequest. */
export interface CreateUserDto {
  fullName: string
  email: string
  password: string
  roleIds: number[]
}

/** Matches backend UpdateUserRolesRequest. */
export interface UpdateUserRolesDto {
  roleIds: number[]
}
