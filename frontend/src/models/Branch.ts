/** Matches backend BranchDto. */
export interface BranchDto {
  id: number
  name: string
  address: string
  mobile: string
  createdDate: string
}

export interface CreateBranchDto {
  name: string
  address: string
  mobile: string
}

export interface UpdateBranchDto extends CreateBranchDto {
  id: number
}
