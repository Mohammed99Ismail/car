import type { SelectOption } from 'naive-ui'
import { isApiSuccess } from '~/models/ApiResponse'
import { useLookupService } from '~/services/lookup.service'

export interface UseDebouncedLookupOptions {
  /** The lookup type or a custom fetch function */
  type?: 'makes' | 'models' | 'branches' | 'years' | 'roles' | 'contractTypes' | 'colors'
  /** For models: the parent makeId */
  makeId?: MaybeRef<number | null | undefined>
  /** Custom fetch function if type is not a predefined one */
  fetchFn?: (search?: string) => Promise<SelectOption[]>
  /** Debounce delay in milliseconds (default: 300) */
  debounceDelay?: number
  /** Whether to load initial data immediately on creation (default: true) */
  immediate?: boolean
}

export function useDebouncedLookup(options: UseDebouncedLookupOptions = {}) {
  const {
    type = 'makes',
    makeId,
    fetchFn,
    debounceDelay = 300,
    immediate = true,
  } = options

  const lookupService = useLookupService()
  const loading = ref(false)
  const rawOptions = ref<SelectOption[]>([])
  const cache = new Map<string, SelectOption[]>()
  let querySeq = 0

  async function fetchOptions(search?: string): Promise<SelectOption[]> {
    if (fetchFn) {
      return fetchFn(search)
    }

    const trimmed = search?.trim() || ''
    const currentMakeId = unref(makeId)

    // Cache key based on type, parent makeId, and search
    const cacheKey = `${type}:${currentMakeId || 0}:${trimmed.toLowerCase()}`
    if (cache.has(cacheKey)) {
      return cache.get(cacheKey)!
    }

    let results: SelectOption[] = []

    switch (type) {
      case 'makes': {
        const res = await lookupService.getMakes(trimmed || undefined)
        if (isApiSuccess(res) && res.data) {
          results = res.data.map(m => ({ label: m.name, value: m.id }))
        }
        break
      }
      case 'models': {
        if (!currentMakeId) {
          results = []
          break
        }
        const res = await lookupService.getModels(currentMakeId, trimmed || undefined)
        if (isApiSuccess(res) && res.data) {
          results = res.data.map(m => ({ label: m.name, value: m.id }))
        }
        break
      }
      case 'branches': {
        const res = await lookupService.getBranches(trimmed || undefined)
        if (isApiSuccess(res) && res.data) {
          results = res.data.map(b => ({ label: b.name, value: b.id }))
        }
        break
      }
      case 'years': {
        const res = await lookupService.getYears()
        if (isApiSuccess(res) && res.data) {
          results = res.data.map(y => ({ label: String(y), value: y }))
        }
        break
      }
      case 'colors': {
        const res = await lookupService.getColors(trimmed || undefined)
        if (isApiSuccess(res) && res.data) {
          results = res.data.map(c => ({
            label: c.name,
            value: c.name,
            id: c.id,
            hexCode: c.hexCode,
          }))
        }
        break
      }
      case 'roles': {
        const res = await lookupService.getRoles()
        if (isApiSuccess(res) && res.data) {
          results = res.data.map(r => ({ label: r.nameAr || r.nameEn, value: r.id }))
        }
        break
      }
    }

    cache.set(cacheKey, results)
    return results
  }

  // Raw search function with race-condition protection
  async function searchNow(query?: string) {
    const thisQueryId = ++querySeq
    loading.value = true
    try {
      const items = await fetchOptions(query)
      if (thisQueryId === querySeq) {
        rawOptions.value = items
      }
    } catch (err) {
      console.error(`Failed to load lookup [${type}]:`, err)
    } finally {
      if (thisQueryId === querySeq) {
        loading.value = false
      }
    }
  }

  // Debounced search for keystrokes
  const debouncedSearch = useDebounceFn((query: string) => {
    searchNow(query)
  }, debounceDelay)

  function clearCache() {
    cache.clear()
  }

  function reload() {
    clearCache()
    return searchNow()
  }

  if (makeId) {
    watch(
      () => unref(makeId),
      () => {
        reload()
      },
    )
  }

  if (immediate) {
    searchNow()
  }

  return {
    loading,
    options: rawOptions,
    search: debouncedSearch,
    searchNow,
    reload,
    clearCache,
  }
}
