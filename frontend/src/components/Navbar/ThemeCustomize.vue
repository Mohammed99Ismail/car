<script setup lang='ts'>
import { PaintBrush24Regular as CustomizeIcon } from '@vicons/fluent'

const { t } = useI18n()
</script>

<template>
  <div v-bind="$attrs">
    <n-tooltip placement="top" trigger="hover">
      <template #trigger>
        <n-popover trigger="click" placement="bottom" :show-arrow="false" style="width: 320px;">
          <template #trigger>
            <n-button quaternary circle>
              <template #icon>
                <NIcon size="1.4rem">
                  <CustomizeIcon />
                </NIcon>
              </template>
            </n-button>
          </template>
          <div class="p-2">
            <customize-dialog />
          </div>
        </n-popover>
      </template>
      <span>{{ t('button.toggle_customize') }}</span>
    </n-tooltip>
  </div>
</template>