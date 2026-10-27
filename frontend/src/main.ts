import { setupLayouts } from 'virtual:generated-layouts'
import generatedRoutes from 'virtual:generated-pages'
import { createRouter, createWebHistory } from 'vue-router'
import i18n from '~/modules/i18n'
import App from './App.vue'
import type { AppModule } from './types'
import { appConfig } from '~/config/app.config'

import '@unocss/reset/tailwind-compat.css'
import 'uno.css'
import './styles/main.scss'

async function initializeMocking() {
  const { worker } = await import('~/mocks/browser')
  return worker
    .start({
      onUnhandledRequest: 'bypass',
      serviceWorker: {
        url: '/mockServiceWorker.js',
        options: {
          scope: '/',
        },
      },
      quiet: true,
      waitUntilReady: false,
    })
    .catch((error) => {
      console.warn('MSW initialization failed:', error)
      return Promise.resolve()
    })
}

const routes = setupLayouts(generatedRoutes)
routes.push(
  {
    path: '/reset-password',
    redirect: (to) => ({ path: '/account/resetpassword', query: to.query }),
  },
  {
    path: '/account/reset-password',
    redirect: (to) => ({ path: '/account/resetpassword', query: to.query }),
  },
)
const router = createRouter({
  history: createWebHistory(),
  routes,
})

const app = createApp(App)
app.use(router)
Object.values(
  import.meta.glob<{ install: AppModule }>('./modules/*.ts', { eager: true }),
).forEach((i: any) => {
  i.install?.(app, router)
})

// register filters
app.config.globalProperties.$filters = {}
Object.values(
  import.meta.glob<any>('./common/filters/*.filter.ts', {
    eager: true,
    import: 'default',
  }),
).forEach((filters: any) => {
  Object.keys(filters).forEach((func) => {
    app.config.globalProperties.$filters[func] = filters[func]
  })
})

// Register v-can directive for permission-based rendering
app.directive('can', (el, binding) => {
  const accountStore = useAccountStore()
  const permission = binding.value
  if (!permission) return
  const hasPerm = Array.isArray(permission)
    ? permission.some((p: string) => accountStore.hasPermission(p))
    : accountStore.hasPermission(permission)

  if (!hasPerm) {
    el.style.display = 'none'
  }
})

router.beforeEach((to, _, next) => {
  const accountStore = useAccountStore()
  const anonymousPaths = [
    '/account/login',
    '/account/register',
    '/account/forgotpassword',
    '/account/resetpassword',
    '/account/reset-password',
    '/reset-password',
    '/404',
    '/error',
    '/maintenance',
  ]

  const path = to.path.toLowerCase()
  const isAnonymous =
    anonymousPaths.some((p) => path.startsWith(p)) ||
    Boolean(to.meta?.allowAnonymous)

  if (!accountStore.isAuthenticated() && !isAnonymous) {
    next('/account/login')
    return
  }

  if (
    accountStore.isAuthenticated() &&
    (path === '/account/login' || path === '/account/register')
  ) {
    next('/')
    return
  }

  const { t } = i18n.global
  const baseTitle = appConfig.appName || t('title')
  let title = baseTitle
  if (to.meta?.title) title = `${t(`menu.${to.meta.title}`)} - ${baseTitle}`

  document.title = title
  next()
})

async function startApp() {
  if (import.meta.env.VITE_API_MOCKING_ENABLED === 'true') {
    try {
      await initializeMocking()
    } catch (error) {
      console.warn('Mock service initialization failed:', error)
    }
  }

  try {
    app.mount('#app')
  } catch (error) {
    console.error('App mounting failed:', error)
  }
}

startApp()
