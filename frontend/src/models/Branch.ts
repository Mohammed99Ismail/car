export interface BranchDto {
  id: string
  name: string
  address: string
  mobile: string
  createdAt: string
  updatedAt?: string | null
}

export interface CreateBranchDto {
  name: string
  address: string
  mobile: string
}

export interface UpdateBranchDto extends CreateBranchDto {
  id: string
}
