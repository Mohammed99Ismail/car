import { useApi } from '~/composables/useApi'
import type { ApiResponse, BilingualLookupDto, LookupColorDto, LookupItemDto } from '~/models/ApiResponse'

// In-memory cache for lookups to speed up page transitions and avoid redundant network calls
const lookupCache = new Map<string, { data: any; timestamp: number }>()
const CACHE_TTL_MS = 60 * 1000 // 60 seconds TTL

async function withLookupCache<T>(key: string, fetcher: () => Promise<ApiResponse<T>>): Promise<ApiResponse<T>> {
  const cached = lookupCache.get(key)
  const now = Date.now()
  if (cached && now - cached.timestamp < CACHE_TTL_MS) {
    return cached.data
  }
  const res = await fetcher()
  if (res && (res.isSuccess || res.data)) {
    lookupCache.set(key, { data: res, timestamp: now })
  }
  return res
}

export function clearLookupCache() {
  lookupCache.clear()
}

export const useLookupService = () => {
  const api = useApi('lookups')

  async function getMakes(search?: string): Promise<ApiResponse<LookupItemDto[]>> {
    const key = `makes_${search || ''}`
    return withLookupCache(key, () => api.get<ApiResponse<LookupItemDto[]>>('makes', search ? { search } : undefined))
  }

  async function getModels(makeId: number, search?: string): Promise<ApiResponse<LookupItemDto[]>> {
    const key = `models_${makeId}_${search || ''}`
    return withLookupCache(key, () => api.get<ApiResponse<LookupItemDto[]>>(`makes/${makeId}/models`, search ? { search } : undefined))
  }

  async function getYears(): Promise<ApiResponse<number[]>> {
    return withLookupCache('years', () => api.get<ApiResponse<number[]>>('years'))
  }

  async function getBranches(search?: string): Promise<ApiResponse<LookupItemDto[]>> {
    const key = `branches_${search || ''}`
    return withLookupCache(key, () => api.get<ApiResponse<LookupItemDto[]>>('branches', search ? { search } : undefined))
  }

  async function getColors(search?: string): Promise<ApiResponse<LookupColorDto[]>> {
    const key = `colors_${search || ''}`
    return withLookupCache(key, () => api.get<ApiResponse<LookupColorDto[]>>('colors', search ? { search } : undefined))
  }

  async function getRoles(): Promise<ApiResponse<BilingualLookupDto[]>> {
    return withLookupCache('roles', () => api.get<ApiResponse<BilingualLookupDto[]>>('roles'))
  }

  return { getMakes, getModels, getYears, getBranches, getColors, getRoles, clearCache: clearLookupCache }
}

