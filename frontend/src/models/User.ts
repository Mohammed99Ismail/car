export interface UserDto {
  id: string
  fullName: string
  email: string
  mobile?: string | null
  isActive: boolean
  createdAt: string
  roles: string[]
}

export interface UpdateUserRolesDto {
  roles: string[]
}
