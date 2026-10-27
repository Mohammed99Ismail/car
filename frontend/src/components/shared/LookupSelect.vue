<script setup lang="ts">
import { h } from 'vue'
import type { SelectOption } from 'naive-ui'
import { useDebouncedLookup } from '~/composables/useDebouncedLookup'

interface Props {
  value?: number | string | Array<number | string> | null
  lookupType?: 'makes' | 'models' | 'branches' | 'years' | 'roles' | 'contractTypes' | 'colors'
  makeId?: number | null
  fetchFn?: (search?: string) => Promise<SelectOption[]>
  debounceDelay?: number
  placeholder?: string
  clearable?: boolean
  disabled?: boolean
  multiple?: boolean
  size?: 'small' | 'medium' | 'large'
  status?: 'warning' | 'error'
}

const props = withDefaults(defineProps<Props>(), {
  value: null,
  lookupType: 'makes',
  makeId: null,
  debounceDelay: 300,
  clearable: true,
  disabled: false,
  multiple: false,
  size: 'medium',
})

const emit = defineEmits<{
  (e: 'update:value', val: any): void
  (e: 'select-item', item: SelectOption | null): void
  (e: 'clear'): void
}>()

const makeIdRef = toRef(props, 'makeId')

const {
  loading,
  options: fetchedOptions,
  search: debouncedSearch,
  searchNow,
  reload,
} = useDebouncedLookup({
  type: props.lookupType,
  makeId: makeIdRef,
  fetchFn: props.fetchFn,
  debounceDelay: props.debounceDelay,
  immediate: true,
})

// Keep selected option so label doesn't get cleared during remote searching
const selectedOptionCache = ref<SelectOption | null>(null)

const mergedOptions = computed<SelectOption[]>(() => {
  const list = [...fetchedOptions.value]
  if (
    selectedOptionCache.value &&
    !list.some((o) => o.value === selectedOptionCache.value?.value)
  ) {
    list.unshift(selectedOptionCache.value)
  }
  return list
})

function renderLabel(option: SelectOption) {
  if (props.lookupType === 'colors' && (option as any).hexCode) {
    return h('div', { class: 'flex items-center gap-2' }, [
      h('span', {
        class: 'w-3.5 h-3.5 rounded-full border border-slate-300 dark:border-slate-600 inline-block shrink-0 shadow-xs',
        style: { backgroundColor: (option as any).hexCode }
      }),
      h('span', null, option.label as string)
    ])
  }
  return option.label as string
}

function handleSearch(query: string) {
  debouncedSearch(query)
}

function handleUpdateValue(val: any, option: any) {
  if (option && !Array.isArray(option)) {
    selectedOptionCache.value = option
    emit('select-item', option)
  } else if (!val) {
    selectedOptionCache.value = null
    emit('select-item', null)
  }
  emit('update:value', val)
}

function handleClear() {
  selectedOptionCache.value = null
  emit('clear')
  emit('update:value', null)
  searchNow('')
}

defineExpose({
  reload,
  searchNow,
})
</script>

<template>
  <n-select
    :value="value"
    filterable
    remote
    :clearable="clearable"
    :disabled="disabled"
    :multiple="multiple"
    :loading="loading"
    :options="mergedOptions"
    :placeholder="placeholder"
    :size="size"
    :status="status"
    :render-label="renderLabel"
    class="rounded-xl"
    @search="handleSearch"
    @update:value="handleUpdateValue"
    @clear="handleClear"
  />
</template>
