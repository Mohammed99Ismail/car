/** Matches backend CarDto. */
export interface CarDto {
  id: number
  makeId: number
  makeName: string
  modelId: number
  modelName: string
  manufactureYear: number
  color: string
  plateNumber: string
  branchId: number
  branchName: string
  purchaseDate: string
  purchasePrice: number
  imagePath: string | null
  chassisNumber: string
  engineNumber: string
  notes: string | null
  createdDate: string
}

/** Matches backend CreateCarRequest / UpdateCarRequest. */
export interface SaveCarDto {
  makeId: number
  modelId: number
  manufactureYear: number
  color: string
  plateNumber: string
  branchId: number
  purchaseDate: string
  purchasePrice: number
  chassisNumber: string
  engineNumber: string
  notes?: string | null
}

export interface CarQueryParameters {
  pageIndex?: number
  pageSize?: number
  search?: string
  sortBy?: string
  isDescending?: boolean
  makeId?: number | null
  modelId?: number | null
  branchId?: number | null
  manufactureYear?: number | null
}
