<script setup lang="ts">
const layout = useLayoutStore()

const toggleButtonPosition = ref<{ left: string; right: string; top: string }>({
  left: 'auto',
  right: 'auto',
  top: '0px',
})
const isAnimating = ref(false)
const circleActive = ref(false)
let isInitial = true

function getToggleElementPosition() {
  const element = document.querySelector('#theme-toggle') as HTMLElement
  if (!element) {
    toggleButtonPosition.value = {
      left: layout.isRtl ? 'auto' : '20px',
      right: layout.isRtl ? '20px' : 'auto',
      top: '16px',
    }
    return
  }
  const rect = element.getBoundingClientRect()
  toggleButtonPosition.value = {
    left: layout.isRtl ? 'auto' : `${rect.left + rect.width / 2}px`,
    right: layout.isRtl ? `${window.innerWidth - (rect.right - rect.width / 2)}px` : 'auto',
    top: `${rect.top + rect.height / 2}px`,
  }
}

watch(
  () => layout.isDark,
  (isDark: boolean) => {
    // On initial cold start / page mount, apply class immediately without any overlay/circle
    if (isInitial) {
      if (isDark) {
        document.documentElement.classList.add('dark')
      } else {
        document.documentElement.classList.remove('dark')
      }
      isInitial = false
      return
    }

    // User toggled theme: trigger contained ripple transition
    getToggleElementPosition()
    isAnimating.value = true
    circleActive.value = false

    requestAnimationFrame(() => {
      circleActive.value = true
    })

    // At 350ms, switch theme class on documentElement
    setTimeout(() => {
      if (isDark) {
        document.documentElement.classList.add('dark')
      } else {
        document.documentElement.classList.remove('dark')
      }
    }, 350)

    // At 500ms, completely destroy overlay so it leaves zero residual DOM footprint
    setTimeout(() => {
      isAnimating.value = false
      circleActive.value = false
    }, 500)
  },
  { immediate: true },
)
</script>

<template>
  <div v-if="isAnimating" class="dark-mode-overlay">
    <div
      class="dark-mode-container"
      :style="{ left: toggleButtonPosition.left, right: toggleButtonPosition.right, top: toggleButtonPosition.top }"
    >
      <div
        class="dark-mode"
        :class="{ active: circleActive }"
        :style="{ backgroundColor: layout.isDark ? '#283046' : '#EEE' }"
      />
    </div>
  </div>
</template>

<style lang="scss" scoped>
.dark-mode-overlay {
  position: fixed;
  inset: 0;
  width: 100vw;
  height: 100vh;
  overflow: hidden !important;
  pointer-events: none !important;
  z-index: 99999;
}

.dark-mode-container {
  position: absolute;
  display: flex;
  justify-content: center;
  align-items: center;
  width: 0;
  height: 0;
  pointer-events: none;

  .dark-mode {
    position: absolute;
    width: 250vmax;
    height: 250vmax;
    border-radius: 50%;
    transform: scale(0);
    transition: transform 400ms cubic-bezier(0.4, 0, 0.2, 1);
    pointer-events: none;
    flex: 0 0 auto;

    &.active {
      transform: scale(1);
    }
  }
}
</style>