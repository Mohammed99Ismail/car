import { acceptHMRUpdate, defineStore } from 'pinia'
import i18n from '~/modules/i18n'

export const useLayoutStore = defineStore(
  'layout',
  () => {
    const collapsed = ref(false)
    const forceCollapsed = ref(false)
    const mobileMenuClosed = ref(true)
    const mobileMode = ref(false)
    const activeLanguage = ref('ar')
    const isRtl = ref(true)
    const themeColor = ref('#2563eb')
    const isDark = ref(false)
    const isWelcomeShown = ref(false)
    const isFluid = ref(false)
    const flatDesign = ref(true)
    const supportEnabled = ref(false)

    const dialogPlacement = computed(() => (isRtl.value ? 'left' : 'right'))

    watch(
      () => useWindowSize().width.value,
      (newValue: number) => {
        forceCollapsed.value = newValue <= 1024
        mobileMode.value = newValue < 600
      },
      { immediate: true },
    )

    function toggleSidebar() {
      if (mobileMode.value) mobileMenuClosed.value = false
      else collapsed.value = !collapsed.value
    }

    function closeSidebar() {
      mobileMenuClosed.value = true
    }

    function setDarkTheme(state: boolean) {
      isDark.value = state
    }

    function toggleTheme() {
      isDark.value = !isDark.value
    }

    function t(key: string, ...args: any[]): string {
      try {
        return (i18n.global.t as any)(key, ...args)
      } catch {
        return key
      }
    }

    function applyLanguageToDomAndI18n(lang: string) {
      if (!lang) return
      if (i18n?.global?.locale) {
        if (typeof i18n.global.locale === 'object' && 'value' in i18n.global.locale) {
          i18n.global.locale.value = lang
        } else {
          (i18n.global.locale as any) = lang
        }
      }

      isRtl.value = lang === 'ar' || lang === 'fa'

      if (typeof document !== 'undefined') {
        const dir = isRtl.value ? 'rtl' : 'ltr'
        document.documentElement.setAttribute('dir', dir)
        document.documentElement.setAttribute('lang', lang)
        const body = document.querySelector('body') as HTMLElement | null
        if (body) {
          if (isRtl.value) body.classList.add('rtl')
          else body.classList.remove('rtl')
        }
      }
    }

    function changeLanguage(lang: string) {
      if (!lang) return
      activeLanguage.value = lang
      applyLanguageToDomAndI18n(lang)
    }

    function setThemeColor(color: string) {
      themeColor.value = color
    }

    function showWelcome() {
      // Promo welcome toast disabled
      isWelcomeShown.value = true
    }

    function resetWelcomeState() {
      isWelcomeShown.value = false
    }

    function $reset() {
      mobileMode.value = false
    }

    function setSupportEnabled() {
      supportEnabled.value = true
    }

    return {
      collapsed,
      forceCollapsed,
      mobileMode,
      toggleSidebar,
      toggleTheme,
      isRtl,
      activeLanguage,
      changeLanguage,
      isDark,
      setThemeColor,
      themeColor,
      dialogPlacement,
      isWelcomeShown,
      showWelcome,
      resetWelcomeState,
      closeSidebar,
      $reset,
      mobileMenuClosed,
      isFluid,
      flatDesign,
      setDarkTheme,
      supportEnabled,
      setSupportEnabled,
    }
  },
  {
    persist: {
      omit: ['mobileMode', 'forceCollapsed', 'supportEnabled'],
    },
  },
)

if (import.meta.hot)
  import.meta.hot.accept(acceptHMRUpdate(useLayoutStore, import.meta.hot))
