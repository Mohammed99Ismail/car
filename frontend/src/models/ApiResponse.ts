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

/** Normalized paged list used by the pages (1-based pageIndex). */
export interface PagedList<T> {
  items: T[]
  pageIndex: number
  totalPages: number
  totalCount: number
  hasPreviousPage: boolean
  hasNextPage: boolean
}

/** Raw backend shape: PagedListModel<T> { data, length, page (0-based), limit, pages } */
export interface BackendPagedList<T> {
  data: T[]
  length: number
  page: number
  limit: number
  pages: number
}

/** Common UI query parameters (1-based pageIndex). */
export interface PagedQuery {
  pageIndex?: number
  pageSize?: number
  search?: string
  sortBy?: string
  isDescending?: boolean
}

export interface LookupItemDto {
  id: number
  name: string
}

/** Bilingual lookup (backend LookupDto). */
export interface BilingualLookupDto {
  id: number
  nameAr: string
  nameEn: string
}

export function isApiSuccess<T>(res?: ApiResponse<T> | null): boolean {
  if (!res) return false
  if (res.isSuccess !== undefined) return res.isSuccess
  if (res.resultStatus !== undefined) return res.resultStatus >= 200 && res.resultStatus < 300
  if (res.status !== undefined) return res.status === ResultStatusEnum.Success || res.status === 200
  return res.data !== undefined && res.data !== null
}

/** Converts UI paging params to backend QueryParameters (page is 0-based, limit). */
export function toBackendQuery(params: PagedQuery & Record<string, any> = {}): Record<string, any> {
  const { pageIndex, pageSize, sortBy, isDescending, ...rest } = params
  const query: Record<string, any> = {
    page: Math.max((pageIndex ?? 1) - 1, 0),
    limit: pageSize ?? 10,
  }
  if (sortBy) {
    query.orderName = sortBy
    query.orderDirection = isDescending ? 'desc' : 'asc'
  }
  Object.entries(rest).forEach(([k, v]) => {
    if (v !== undefined && v !== null && v !== '') query[k] = v
  })
  return query
}

/** Converts backend PagedListModel response to the normalized PagedList shape. */
export function toPagedResponse<TIn, TOut = TIn>(
  res: ApiResponse<BackendPagedList<TIn>>,
  map?: (item: TIn) => TOut,
): ApiResponse<PagedList<TOut>> {
  const raw = res?.data
  const items = (raw?.data ?? []).map(i => (map ? map(i) : (i as unknown as TOut)))
  const pageIndex = (raw?.page ?? 0) + 1
  const totalPages = raw?.pages ?? 0
  return {
    ...res,
    data: {
      items,
      pageIndex,
      totalPages,
      totalCount: raw?.length ?? 0,
      hasPreviousPage: pageIndex > 1,
      hasNextPage: pageIndex < totalPages,
    },
  }
}

/** Picks the localized name from a bilingual lookup. */
export function localizedName(item: { nameAr: string; nameEn: string }, locale: string): string {
  return locale?.startsWith('en') ? item.nameEn || item.nameAr : item.nameAr || item.nameEn
}
