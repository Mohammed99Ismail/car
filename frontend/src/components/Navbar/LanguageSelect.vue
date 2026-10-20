<script setup lang="ts">
import { Globe20Regular as TranslateIcon } from '@vicons/fluent'
import { NIcon } from 'naive-ui/es/icon'
import { storeToRefs } from 'pinia'
import EnglandIcon from '../CustomIcons/EnglandIcon.vue'

defineProps<{ showTitle?: boolean }>()
const { t } = useI18n()
const layoutStore = useLayoutStore()
const { activeLanguage } = storeToRefs(layoutStore)
const language = ref(activeLanguage.value)

const languages = [
  {
    label: () =>
      h('div', { class: 'flex items-center gap-2' }, [
        h('span', { class: 'text-base font-bold' }, '🇸🇦'),
        h('span', null, 'العربية (Arabic)'),
      ]),
    value: 'ar',
  },
  {
    label: () =>
      h('div', { class: 'flex items-center gap-2' }, [
        h(EnglandIcon),
        h('span', null, 'English'),
      ]),
    value: 'en',
  },
]

function changeLanguage(lang: string) {
  language.value = lang
  layoutStore.changeLanguage(lang)
}

watch(
  () => layoutStore.activeLanguage,
  (val) => {
    if (val) language.value = val
  },
  { immediate: true },
)
</script>

<template>
  <div v-bind="$attrs">
    <n-tooltip placement="top" trigger="hover">
      <template #trigger>
        <n-popselect
          v-model:value="language"
          trigger="click"
          :options="languages"
          @update:value="changeLanguage"
        >
          <n-button quaternary :circle="!showTitle">
            <template #icon>
              <NIcon size="1.3rem">
                <TranslateIcon />
              </NIcon>
            </template>
            <span v-if="showTitle" class="mx-1 font-semibold">
              {{ activeLanguage === 'ar' ? 'العربية' : 'English' }}
            </span>
          </n-button>
        </n-popselect>
      </template>
      <span>{{ t('button.toggle_langs') }}</span>
    </n-tooltip>
  </div>
</template>

<style lang="scss" scoped></style>
