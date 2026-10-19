export enum ResultStatusEnum {
  Success = 0,
  Failed = 1,
  NotFound = 2,
  ValidationError = 3,
  Unauthorized = 4,
  Forbidden = 5,
}

export interface ApiResponse<T> {
  status?: ResultStatusEnum | number
  resultStatus?: number
  isSuccess?: boolean
  message?: string | null
  data: T
  errors?: string[] | null
}

export interface PagedList<T> {
  items: T[]
  pageIndex: number
  totalPages: number
  totalCount: number
  hasPreviousPage: boolean
  hasNextPage: boolean
}

export interface LookupItemDto {
  id: string
  name: string
}

export function isApiSuccess<T>(res?: ApiResponse<T> | null): boolean {
  if (!res) return false
  if (res.isSuccess !== undefined) return res.isSuccess
  if (res.resultStatus !== undefined) return res.resultStatus === 200
  if (res.status !== undefined) return res.status === ResultStatusEnum.Success || res.status === 0 || res.status === 200
  return res.data !== undefined && res.data !== null
}
