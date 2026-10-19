export interface CarDto {
  id: string
  makeId: string
  makeName: string
  modelId: string
  modelName: string
  year: number
  color: string
  plateNumber: string
  branchId: string
  branchName: string
  purchaseDate: string
  purchasePrice: number
  imagePath: string | null
  chassisNumber: string
  engineNumber: string
  notes: string | null
  createdAt: string
  updatedAt?: string | null
}

export interface CreateCarDto {
  makeId: string
  modelId: string
  year: number
  color: string
  plateNumber: string
  branchId: string
  purchaseDate: string
  purchasePrice: number
  imagePath?: string | null
  chassisNumber: string
  engineNumber: string
  notes?: string | null
}

export interface UpdateCarDto extends CreateCarDto {
  id: string
}

export interface CarQueryParameters {
  pageIndex?: number
  pageSize?: number
  search?: string
  sortBy?: string
  isDescending?: boolean
  makeId?: string
  modelId?: string
  branchId?: string
  year?: number
}
