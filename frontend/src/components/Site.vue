<script setup lang="ts">
const loadingBar = useLoadingBar()
const router = useRouter()

let isLoading = false
let safetyTimer: ReturnType<typeof setTimeout> | null = null

function safeStart() {
  if (safetyTimer) clearTimeout(safetyTimer)
  if (!isLoading) {
    isLoading = true
    loadingBar.start()
  }
  // Safety timeout: automatically finish after 3.5s so bar NEVER hangs stuck on screen
  safetyTimer = setTimeout(() => {
    safeFinish()
  }, 3500)
}

function safeFinish() {
  if (safetyTimer) {
    clearTimeout(safetyTimer)
    safetyTimer = null
  }
  if (isLoading) {
    isLoading = false
    loadingBar.finish()
  }
}

function safeError() {
  if (safetyTimer) {
    clearTimeout(safetyTimer)
    safetyTimer = null
  }
  if (isLoading) {
    isLoading = false
    loadingBar.error()
  }
}

const unhookBefore = router.beforeEach((to: any, from: any) => {
  if (to.path !== from.path) {
    safeStart()
  }
})

const unhookAfter = router.afterEach((_to: any, _from: any, failure: any) => {
  if (failure) {
    safeError()
  } else {
    nextTick(() => {
      safeFinish()
    })
  }
})

const unhookError = router.onError(() => {
  safeError()
})

onUnmounted(() => {
  unhookBefore()
  unhookAfter()
  unhookError()
  safeFinish()
})
</script>

<template>
    <DarkModeContainer class="z-1" />
    <RouterView />
</template>