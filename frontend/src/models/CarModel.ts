export interface CarModelDto {
  id: number
  makeId: number
  makeName: string
  name: string
}

export interface CreateCarModelDto {
  makeId: number
  name: string
}

export interface UpdateCarModelDto {
  makeId: number
  name: string
}

export interface CarModelQuery {
  page?: number
  limit?: number
  search?: string
  makeId?: number
  orderName?: string
  orderDirection?: string
}
