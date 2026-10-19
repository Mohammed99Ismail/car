<script setup lang="ts">
import { useDebounceFn } from '@vueuse/core'
import type { SelectOption } from 'naive-ui'

interface Props {
  modelValue?: string | number | null
  placeholder?: string
  fetchOptions: (search?: string) => Promise<Array<{ label: string; value: string | number }>>
  cacheKey?: string
  disabled?: boolean
  clearable?: boolean
  depKey?: any
}

const props = withDefaults(defineProps<Props>(), {
  modelValue: null,
  placeholder: 'اختر...',
  cacheKey: '',
  disabled: false,
  clearable: true,
  depKey: null,
})

const emit = defineEmits<{
  (e: 'update:modelValue', value: string | number | null): void
  (e: 'change', value: string | number | null, option?: SelectOption): void
}>()

const options = ref<SelectOption[]>([])
const loading = ref(false)
const searchQuery = ref('')

// In-memory cache map: cacheKey + depKey + search -> SelectOption[]
const cache = new Map<string, SelectOption[]>()

function getCacheKey(search: string): string {
  return `${props.cacheKey}_${String(props.depKey ?? '')}_${search.trim().toLowerCase()}`
}

async function loadData(search = '') {
  if (props.disabled) return

  const key = getCacheKey(search)
  if (cache.has(key)) {
    options.value = cache.get(key)!
    return
  }

  loading.value = true
  try {
    const data = await props.fetchOptions(search)
    const mapped: SelectOption[] = data.map((item) => ({
      label: item.label,
      value: item.value,
    }))
    cache.set(key, mapped)
    options.value = mapped
  } catch (err) {
    console.error('LookupSelect fetch error:', err)
  } finally {
    loading.value = false
  }
}

const debouncedSearch = useDebounceFn((query: string) => {
  searchQuery.value = query
  loadData(query)
}, 300)

function handleSearch(query: string) {
  debouncedSearch(query)
}

function handleUpdateValue(value: string | number | null, option: SelectOption) {
  emit('update:modelValue', value)
  emit('change', value, option)
}

// Re-fetch when depKey changes (e.g. make changed -> reload models)
watch(
  () => props.depKey,
  () => {
    cache.clear()
    searchQuery.value = ''
    loadData('')
  },
)

watch(
  () => props.disabled,
  (val) => {
    if (!val && options.value.length === 0) {
      loadData('')
    }
  },
)

onMounted(() => {
  if (!props.disabled) {
    loadData('')
  }
})
</script>

<template>
  <n-select
    :value="modelValue"
    :options="options"
    :loading="loading"
    :disabled="disabled"
    :clearable="clearable"
    :placeholder="placeholder"
    filterable
    remote
    @search="handleSearch"
    @update:value="handleUpdateValue"
  />
</template>
